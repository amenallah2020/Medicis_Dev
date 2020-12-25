using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PagedList;
using RHEVENT.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNet.Identity.Owin;
using System.Data.Entity;
using System.Globalization;
using static RHEVENT.Models.Enumeration;

namespace RHEVENT.Controllers
{
    public class CongeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        UserManager<IdentityUser> userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>());
        /*    private ApplicationSignInManager _signInManager;
            private ApplicationUserManager _userManager;
            public ApplicationUserManager UserManager
            {
                get
                {
                    return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                }
                private set
                {
                    _userManager = value;
                }
            }

            public ApplicationSignInManager SignInManager
            {
                get
                {
                    return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
                }
                private set
                {
                    _signInManager = value;
                }
            }

        */

        private ApplicationUserManager _userManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }

        [Authorize]
        public ActionResult Demander_conge()
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.nom_prenom = user.nom + " " + user.prenom;
            ViewBag.email = user.Email;
            List<string> roles = userManager.GetRoles(user.Id).ToList();
            if ( (roles.Contains("RH_Admin") && roles.Contains("RH")) || (roles.Contains("RH_Superieur_Hiéarchique") && roles.Contains("RH")) || (roles.Contains("Directeur") && roles.Contains("RH")))
            {
                ViewBag.Role = "show_attribute_menu";
            }
             
            return View();
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<ActionResult> Demander_conge(Conge conge)
        {
            if (ModelState.IsValid)
            { 
                if (conge.jour_debut >  conge.jour_fin)
                {
                    return View("Error");
                }
                else
                { 
                    ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
                    conge.UserId = User.Identity.GetUserId();
                    conge.UserName = User.Identity.GetUserName();
                    conge.matricule = user.matricule;
                    conge.Fonction = user.fonction;
                    conge.service = user.service;
                    conge.superieur_hierarchique = user.signataire;
                    conge.Date_emission_demande = System.DateTime.Now;
                    conge.service = user.service;
                    conge.jour_debut = conge.jour_debut;
                    conge.jour_fin = conge.jour_fin;
                    conge.heure_sortie = conge.heure_sortie;
                    conge.heure_entree = conge.heure_entree;
                    conge.acceptation_superieur = acceptation_superieur_hierarchique.Demande_en_cours;
                    conge.acceptation_ressource = acceptation_ressource_humaine.Demande_en_cours;
                    conge.nom_prenom = user.nom + " " + user.prenom;
                    conge.Approbateur_RH = "";
                    conge.Date_validation_superieur = System.DateTime.Now;
                    conge.site = user.site;
                    conge.Solde_Conge = user.Solde_Conge;
                    var user_signataire = userManager.FindByName(user.signataire);
                    if (user_signataire.Id == user.Id)
                    {
                        conge.acceptation_superieur = acceptation_superieur_hierarchique.Demande_validée;
                    }
                    db.Conges.Add(conge);
                    if (user_signataire.Email != user.Email)
                    {
                        Email email = new Email();
                        email.Destinataire = user_signataire.Email;
                        email.Email_Destinataire = user_signataire.Email;
                        email.Sujet = "Demande de congé";
                        email.Message = "Demande de congé de la part de " + user.nom + " " + user.prenom;
                        email.Current_User_Event = user.nom + " " + user.prenom + " " + user.service;
                        email.Date_email = System.DateTime.Now;
                        email.Etat_Envoi = "0";
                        db.Emails.Add(email);
                    }
                   
                    await db.SaveChangesAsync();
                    return View("OK");
                }
                 
            }
            else
            {
                return View("Error");
            }
        }


        [Authorize]
        public ActionResult Mes_conges(int? page)
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.nom_prenom = user.nom + " " + user.prenom;
            ViewBag.email = user.Email;
            
                int pageSize = 20;
                int pageNumber = (page ?? 1);
                // return View(db.Attestations.ToList().Select(att=>att).Where(att=>att.UserId == User.Identity.GetUserId()));
                return View(db.Conges.ToList().Select(aut => aut).Where(aut => aut.UserId == User.Identity.GetUserId()).OrderByDescending(aut => aut.Date_emission_demande).ToPagedList(pageNumber, pageSize));
        
        }

        [Authorize(Roles = "Admin_Medicis,RH,RH_Admin,RH_Directeur,RH_Superieur_Hiéarchique")]
        public ActionResult Approbation_superieur_hiéarchique(string sortOrder, int? page)
        { 
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.nom_prenom = user.nom + " " + user.prenom;
            ViewBag.email = user.Email;

            int pageSize = 20;
            int pageNumber = (page ?? 1);

            return View(db.Conges.ToList().Select(aut => aut).Where((aut => aut.superieur_hierarchique == User.Identity.GetUserName())).OrderBy(aut => aut.acceptation_superieur).OrderByDescending(aut=>aut.Date_emission_demande).ToPagedList(pageNumber,pageSize));

        }

        [Authorize(Roles = "Admin_Medicis,RH_Admin,RH_Directeur,RH_Superieur_Hiéarchique")]
        public ActionResult UpdateCongeSuperieur(int id)
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.nom_prenom = user.nom + " " + user.prenom;
            ViewBag.email = user.Email;
            Conge conge = db.Conges.Find(id);
            return View(conge);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin_Medicis,RH_Admin,RH_Directeur,RH_Superieur_Hiéarchique")]
        public ActionResult UpdateCongeSuperieur(Conge conge)
        {
            string error = "";

            try
            {
                Conge a = db.Conges.FirstOrDefault(x => x.Id == conge.Id);
                a.commentaire_superieur_hiearchique = conge.commentaire_superieur_hiearchique;
                a.acceptation_superieur = conge.acceptation_superieur;
                a.Date_validation_superieur = System.DateTime.Now;
                db.SaveChanges();
                return View("OK");
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {

                    error = error + eve.Entry.Entity.GetType().Name + "  " + eve.Entry.State;
                    foreach (var ve in eve.ValidationErrors)
                    {

                        error += error + ve.PropertyName + "   " + ve.ErrorMessage;
                    }
                }
                return RedirectToAction(error);
            }
            // return View("Error");
        }

        [Authorize(Roles = "Admin_Medicis,RH_Admin")]
        public ActionResult Approbation_RH(int? page)
        { 
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.nom_prenom = user.nom + " " + user.prenom;
            ViewBag.email = user.Email;

            int pageSize = 20;
            int pageNumber = (page ?? 1);
            return View(db.Conges.ToList().Select(aut => aut).Where(aut => aut.acceptation_superieur == acceptation_superieur_hierarchique.Demande_validée).Where(aut=>aut.site == user.site)  .OrderBy(aut=>aut.acceptation_ressource).ToPagedList(pageNumber,pageSize));
        }

        [Authorize(Roles = "Admin_Medicis,RH_Admin")]
        public ActionResult UpdateCongeRessourcesHumaines(int id)
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.nom_prenom = user.nom + " " + user.prenom;
            ViewBag.email = user.Email;
            Conge conge = db.Conges.Find(id);
            return View(conge);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin_Medicis,RH_Admin")]
        public ActionResult UpdateCongeRessourcesHumaines(Conge conge)
        {
            string error = "";

            try
            {
                ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
                Conge c = db.Conges.FirstOrDefault(x => x.Id == conge.Id);
                c.commentaire_superieur_hiearchique = conge.commentaire_rh;
                c.acceptation_ressource = conge.acceptation_ressource;
                c.Approbateur_RH = user.prenom+ "  "+ user.matricule;
                db.SaveChanges();
                return View("OK");
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {

                    error = error + eve.Entry.Entity.GetType().Name + "  " + eve.Entry.State;
                    foreach (var ve in eve.ValidationErrors)
                    {

                        error += error + ve.PropertyName + "   " + ve.ErrorMessage;
                    }
                }
                return RedirectToAction(error);
            }
            // return View("Error");
        }
        [Authorize]
        public ActionResult SoldeConge(int? page)
        { 
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.nom_prenom = user.nom + " " + user.prenom;
            ViewBag.email = user.Email;
            ViewBag.soldeConge = user.Solde_Conge;
            ViewBag.role = user.RoleName;

            //string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(System.DateTime.Now.Month -1);

            List<string> roles = userManager.GetRoles(user.Id).ToList();

            if (roles.Contains("Admin_Medicis") || (roles.Contains("RH") && roles.Contains("RH_Admin")) || (roles.Contains("RH") && roles.Contains("RH_Superieur_Hiéarchique")) || (roles.Contains("RH") && roles.Contains("RH_Directeur")))
            {
                ViewBag.Role = "show_solde_menu";
            }

             
            int pageSize = 100;
            int pageNumber = (page ?? 1);

            ViewBag.dernier_date = user.Dernier_maj_solde_conge;


            if ((roles.Contains("RH_Admin") && roles.Contains("RH"))  || roles.Contains("Admin_Medicis"))
            {
                return View(db.Users.ToList().OrderBy(x => x.service).ToPagedList(pageNumber, pageSize));
            }

            else if ((roles.Contains("RH_SoldeCongésService") && roles.Contains("RH")) || roles.Contains("Admin_Medicis"))
            {
                return View(db.Users.ToList().Where(x => x.service == user.service).OrderBy(x => x.prenom).ToPagedList(pageNumber, pageSize));
            }

           else  if  ((roles.Contains("RH_Superieur_Hiéarchique") && roles.Contains("RH")) || roles.Contains("Admin_Medicis"))
            {
                return View(db.Users.ToList().Where(x => x.signataire == user.matricule).OrderBy(x=>x.prenom).ToPagedList(pageNumber, pageSize));
        
            }
            else if
               ((roles.Contains("RH_Directeur") && roles.Contains("RH")) || roles.Contains("Admin_Medicis"))
            {
                 
                 var list_directeur_par_service = db.Calendrier_Directions.ToList().Where(x => x.matricule == user.matricule);

                  List<ApplicationUser> list_user= new List<ApplicationUser>(); 
                foreach (var item in list_directeur_par_service)
                {
                    //return RedirectToAction(item.service.ToString());
                    list_user.AddRange(db.Users.ToList().Where(x=>x.service == item.service.ToString()));
                }
                  return View(list_user.OrderBy(x => x.service).ToPagedList(pageNumber, pageSize));                 

            }
        
            else
            {
                return View(db.Users.ToList().Where(x => x.matricule == user.matricule).ToPagedList(pageNumber, pageSize));
            }
           
        }
        [Authorize(Roles = "Admin_Medicis,RH_Admin")]
        public ActionResult Import_Etat_Conges()
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.nom_prenom = user.nom + " " + user.prenom;
            ViewBag.email = user.Email;
            return View();
        }


        [Authorize(Roles = "Admin_Medicis,RH_Admin,RH_Directeur,RH_Superieur_Hiéarchique")]
        public ActionResult AttribuerCongé()
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.nom_prenom = user.nom + " " + user.prenom;
            ViewBag.email = user.Email;


            List<SelectListItem> list = new List<SelectListItem>();
            var employers = db.Users.Where(x => x.signataire == user.UserName).ToList().Select(x => new SelectListItem
            {
                Value = x.matricule,
                Text = x.nom + " " + x.prenom + " " + x.matricule
            }).Distinct().OrderBy(sign => sign.Text);
            CongeViewModels ConVM = new CongeViewModels();
            ConVM.employers = employers;

            return View(ConVM);
        }

        [Authorize(Roles = "Admin_Medicis,RH_Admin,RH_Directeur,RH_Superieur_Hiéarchique")] 
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<ActionResult> AttribuerCongé(CongeViewModels CongVM)
        {
            string error = "";
            if (ModelState.IsValid)
            { 
                if (CongVM.jour_debut > CongVM.jour_fin)
                {
                    return View("Error");
                }
                else
                {  
                    try
                    {
                        ApplicationUser user = db.Users.Find(User.Identity.GetUserId());

                        ApplicationUser appuser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindByName(CongVM.nom_prenom.Substring(CongVM.nom_prenom.Length - 4, CongVM.nom_prenom.Length));

                        //    return RedirectToAction(appuser.matricule);
                        Conge conge = new Conge();
                        conge.UserId = appuser.Id;
                        conge.UserName = appuser.UserName;
                        conge.matricule = appuser.matricule;
                        conge.Fonction = appuser.fonction;
                        conge.service = appuser.service;
                        conge.superieur_hierarchique = appuser.signataire;
                        conge.Date_emission_demande = System.DateTime.Now;
                        conge.site = appuser.site;
                        conge.jour_debut = CongVM.jour_debut;
                        conge.jour_fin = CongVM.jour_fin;

                        conge.heure_sortie = CongVM.heure_sortie;
                        conge.heure_entree = CongVM.heure_entree;

                        conge.acceptation_superieur = acceptation_superieur_hierarchique.Demande_validée;
                        conge.acceptation_ressource = acceptation_ressource_humaine.Demande_en_cours;

                        conge.nom_prenom = appuser.nom + " " + appuser.prenom;
                        conge.Approbateur_RH = "";
                        conge.Date_validation_superieur = System.DateTime.Now;
                        db.Conges.Add(conge);
                        await db.SaveChangesAsync();
                    
                          }
            catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {

                        error = error + eve.Entry.Entity.GetType().Name + "  " + eve.Entry.State;
                        foreach (var ve in eve.ValidationErrors)
                        {

                            error += error + ve.PropertyName + "   " + ve.ErrorMessage;
                        }
                    }
                    return RedirectToAction(error);
                }
                return View("OK");
                }
                 

            }
            else
            {
                return View("Error");
            }
        }

         

        [Authorize(Roles = "Admin_Medicis,RH_Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Import_Etat_Conges(HttpPostedFileBase file)
        { 
            System.IO.DirectoryInfo di = new DirectoryInfo(Server.MapPath("~/Files/"));

            foreach (FileInfo f in di.GetFiles())
            {
                if (f.Extension.Equals(".xlsx") || f.Extension.Equals(".xls"))
                {
                    f.Delete();
                }

            }
            string fichier = Server.MapPath("~/Files/" + file.FileName);
            file.SaveAs(fichier);

   
            string ext = "";
            if (file != null)
            {
                ext = Path.GetExtension(file.FileName);
                if (ext.Equals(".xlsx") || ext.Equals(".xls"))
                {
                    string sheet = "Feuil1";
                    var results = GetAllWorksheets(fichier);
                    foreach (Sheet item in results)
                    {
                        sheet = item.Name;
                    }
                    using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(fichier, false))
                    {

                        WorkbookPart workbookPart = spreadsheetDocument.WorkbookPart;
                        WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
                        SheetData sheetData = worksheetPart.Worksheet.Elements<SheetData>().Last();
                        int i = 1;

                         
                        foreach (Row r in sheetData.Elements<Row>())
                        {

                            if (r != null)
                            {

                                string matricule = GetCellValue(fichier, sheet, "D" + i);
                                matricule.Split();
                                if (matricule.Length == 1) { matricule = "000" + matricule; }
                                if (matricule.Length == 2) { matricule = "00" + matricule;  }
                                if (matricule.Length == 3) { matricule = "0" + matricule;   }
                                //return RedirectToAction(int_matricule+"");
                                ApplicationUser appuser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindByName(matricule);
                                if (appuser != null)
                                {                              
                                    float float_solde;
                                    string solde = GetCellValue(fichier, sheet, "J" + i);
                                    string date_dernier = GetCellValue(fichier, sheet, "C" + i);
                                     
                                        
                                        if (float.TryParse(solde, out float_solde))
                                    {
                                         appuser.Solde_Conge = float_solde;
                                        appuser.Dernier_maj_solde_conge = date_dernier;
                                          var user = _userManager.FindById(appuser.Id);
                                          _userManager.Update(user);
                                         db.SaveChanges();
                                        
                                    }
                                    else
                                    {
                                        try { 
                                        solde = solde.Replace(" ", "");
                                        solde = solde.Replace(",", ".");
                                        float s = float.Parse(solde, CultureInfo.InvariantCulture.NumberFormat);
                                        appuser.Solde_Conge = s;
                                        appuser.Dernier_maj_solde_conge = date_dernier;
                                        var user = _userManager.FindById(appuser.Id);
                                        _userManager.Update(user);
                                        db.SaveChanges();
                                        }
                                        catch(Exception e) { return RedirectToAction(matricule + ""); }

                                    }
 
                               
                                }
                             

                                i++;
                            }
                            else
                            {

                            }
                        }
                         

                    }
                }
            }
  
            return View();
            
        }

        public static Sheets GetAllWorksheets(string fileName)
        {
            Sheets theSheets = null;

            using (SpreadsheetDocument document =
                SpreadsheetDocument.Open(fileName, false))
            {
                WorkbookPart wbPart = document.WorkbookPart;
                theSheets = wbPart.Workbook.Sheets;
            }
            return theSheets;
        }


        public static string GetCellValue(string fileName,
       string sheetName,
       string addressName)
        {
            string value = null;

            // Open the spreadsheet document for read-only access.
            using (SpreadsheetDocument document =
                SpreadsheetDocument.Open(fileName, false))
            {
                // Retrieve a reference to the workbook part.
                WorkbookPart wbPart = document.WorkbookPart;

                // Find the sheet with the supplied name, and then use that 
                // Sheet object to retrieve a reference to the first worksheet.
                Sheet theSheet = wbPart.Workbook.Descendants<Sheet>().
                  Where(s => s.Name == sheetName).FirstOrDefault();

                // Throw an exception if there is no sheet.
                if (theSheet == null)
                {
                    throw new ArgumentException("sheetName");
                }

                // Retrieve a reference to the worksheet part.
                WorksheetPart wsPart =
                    (WorksheetPart)(wbPart.GetPartById(theSheet.Id));

                // Use its Worksheet property to get a reference to the cell 
                // whose address matches the address you supplied.
                Cell theCell = wsPart.Worksheet.Descendants<Cell>().
                  Where(c => c.CellReference == addressName).FirstOrDefault();

                // If the cell does not exist, return an empty string.
                if (theCell != null)
                {
                    value = theCell.InnerText;

                    // If the cell represents an integer number, you are done. 
                    // For dates, this code returns the serialized value that 
                    // represents the date. The code handles strings and 
                    // Booleans individually. For shared strings, the code 
                    // looks up the corresponding value in the shared string 
                    // table. For Booleans, the code converts the value into 
                    // the words TRUE or FALSE.
                    if (theCell.DataType != null)
                    {
                        switch (theCell.DataType.Value)
                        {
                            case CellValues.SharedString:

                                // For shared strings, look up the value in the
                                // shared strings table.
                                var stringTable =
                                    wbPart.GetPartsOfType<SharedStringTablePart>()
                                    .FirstOrDefault();

                                // If the shared string table is missing, something 
                                // is wrong. Return the index that is in
                                // the cell. Otherwise, look up the correct text in 
                                // the table.
                                if (stringTable != null)
                                {
                                    value =
                                        stringTable.SharedStringTable
                                        .ElementAt(int.Parse(value)).InnerText;
                                }
                                break;

                            case CellValues.Boolean:
                                switch (value)
                                {
                                    case "0":
                                        value = "FALSE";
                                        break;
                                    default:
                                        value = "TRUE";
                                        break;
                                }
                                break;
                        }
                    }
                }
            }
            return value;
        }

        public ViewResult UpdateCongeByUser(int id)
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.nom_prenom = user.nom + " " + user.prenom;
            ViewBag.email = user.Email;
            Conge conge = db.Conges.Find(id);
            return View(conge); 

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateCongeByUser (Conge conge)
        {

            if (conge.acceptation_superieur != acceptation_superieur_hierarchique.Demande_en_cours)
            {
                return RedirectToAction("Error");
            }
            if (ModelState.IsValid)
            {
                ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
                Conge c = db.Conges.FirstOrDefault(x => x.Id == conge.Id);
                c.jour_debut = conge.jour_debut;
                c.jour_fin = conge.jour_fin;
                c.heure_entree = conge.heure_entree;
                c.heure_sortie = conge.heure_sortie;
                db.SaveChanges();
                return View("OK");
            }
            else
            {
                return View("Error");
            }
          
        }


        public ViewResult DeleteCongeByUser(int id)
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.nom_prenom = user.nom + " " + user.prenom;
            ViewBag.email = user.Email;
            Conge conge = db.Conges.Find(id);
            return View(conge);
        }
        [HttpPost]
        public ActionResult  DeleteCongeByUser(Conge conge)
        {
            Conge c = db.Conges.Find(conge.Id);
            db.Conges.Remove(c);
            db.SaveChanges();
            return View("OK");

        }

    }
}