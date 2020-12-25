using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using PagedList;
using RHEVENT.Models;
using static RHEVENT.Models.Enumeration;

namespace RHEVENT.Controllers
{
    public class Heure_SuperieurController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        UserManager<IdentityUser> userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>());

        // GET: Heure_Superieur
        [Authorize(Roles = "Admin_Medicis,RH_Admin,RH_Directeur,RH_Superieur_Hiéarchique")]
        public ActionResult Index(string sortOrder,string search_nom_prenom_matricule,string Search_jour_planing,string currentFilter, int? page)
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.nom_prenom = user.nom + " " + user.prenom;
            ViewBag.email = user.Email;

            ApplicationUser appuser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(User.Identity.GetUserId());
            ViewBag.Sorting_nom_prenom = String.IsNullOrEmpty(sortOrder) ? "nom_prenom" : "";
            ViewBag.Sortig_jour_planing = sortOrder == "Date" ? "date_desc" : "Date";




            if (search_nom_prenom_matricule != null)
            {
                page = 1;
            }
            else
            {
                search_nom_prenom_matricule = currentFilter;
            }

            DateTime date=System.DateTime.Now;
            DateTime date2=System.DateTime.Now;

 

            if (!String.IsNullOrEmpty(Search_jour_planing) )
            {
                string date_string_planing = Search_jour_planing.Replace(".", "");             
                if (DateTime.TryParseExact(date_string_planing, "ddMMyyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out date)) ;        
            }

            var heure_sups = from s in db.Heure_Superieur.Where(x=>x.matricule_superieur == appuser.matricule)
                            select s;

            if (!String.IsNullOrEmpty(search_nom_prenom_matricule))
            {        
                if (String.IsNullOrEmpty(Search_jour_planing) )
                {
                    heure_sups = heure_sups.Where(s => s.nom_prenom_employer.Contains(search_nom_prenom_matricule)
                                           || s.matricule_employer.Contains(search_nom_prenom_matricule) );
                }

                else if (!String.IsNullOrEmpty(Search_jour_planing) )
                {
                    heure_sups = heure_sups.Where(s => s.nom_prenom_employer.Contains(search_nom_prenom_matricule)
                                           || s.matricule_employer.Contains(search_nom_prenom_matricule)
                                          & s.jour_planing == date

                                           );
                }
             
            }

            else if (String.IsNullOrEmpty(search_nom_prenom_matricule))
            {
                if (!String.IsNullOrEmpty(Search_jour_planing))

                {
                    heure_sups = heure_sups.Where(s => s.jour_planing == date);
                }
              
            }
 

            switch (sortOrder)
             {
                 case "nom_prenom":
                    heure_sups = heure_sups.OrderByDescending(h => h.nom_prenom_employer);
                     break;
                 case "Date":
                    heure_sups = heure_sups.OrderBy(h => h.jour_planing);
                     break;
                 case "date_desc":
                    heure_sups = heure_sups.OrderByDescending(s => s.jour_planing);
                     break;
                 default:
                     heure_sups = heure_sups.OrderBy(h => h.nom_prenom_employer);
                     break;
             }
            // return View(heure_sups.ToList());

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(heure_sups.ToPagedList(pageNumber, pageSize));

        }

        [Authorize]
        // GET: Heure_Superieur/Details/5
        public ActionResult Details(int? id)
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.nom_prenom = user.nom + " " + user.prenom;
            ViewBag.email = user.Email;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Heure_Superieur heure_Superieur = db.Heure_Superieur.Find(id);
            if (heure_Superieur == null) 
            {
                return HttpNotFound();
            }
            return View(heure_Superieur);
        }

        // GET: Heure_Superieur/Create
        [Authorize(Roles = "Admin_Medicis,RH_Admin,RH_Directeur,RH_Superieur_Hiéarchique")]

        public ActionResult Create()
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.nom_prenom = user.nom + " " + user.prenom;
            ViewBag.email = user.Email;
           
            List<SelectListItem> items = PopulateEmployers(user,db);
            Heure_Superieur hsup = new Heure_Superieur();
            hsup.employers = items;
            hsup.matricule_employer ="9999";
            return View(hsup);
        }
        private static List<SelectListItem> PopulateEmployers(ApplicationUser user, ApplicationDbContext db)
        {

            
            List<SelectListItem> items = new List<SelectListItem>();

            var employers = db.Users.Where(x => x.signataire == user.UserName).ToList().Select(x => new SelectListItem
            {
                Value = x.matricule,
                Text = x.nom + " " + x.prenom + " " + x.matricule
            });

            foreach (var emp in employers)
            {
                items.Add(new SelectListItem
                {
                    Text = emp.Text ,
                    Value = emp.Value
                });
            }
            return items;
        }

        public string get_nom_prenom_employer_by_matricule(string matricule)
        {
            var  query = from st in db.Users where st.matricule == matricule select st;
            var user = query.FirstOrDefault<ApplicationUser>();
            return user.nom + " " + user.prenom;
        }


        // POST: Heure_Superieur/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin_Medicis,RH_Admin,RH_Directeur,RH_Superieur_Hiéarchique")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task <ActionResult> Create(Heure_Superieur hsup)
        {
           // return RedirectToAction(hsup.list_employers.Length+"");

            

            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            int start_index = 0;
            int nbr_horraires = hsup.list_employers.Length / 11;
            

            int i = 0;
            if (nbr_horraires >0 )
            {
                while (i< nbr_horraires)
                { 
                    string chaine = hsup.list_employers.Substring(start_index, 11);
                    string matricule = chaine.Substring(0, 4);
                    string heure_debut  = chaine.Substring(4, 2)+":00";
                    string heure_fin = chaine.Substring(9, 2)+":00";

                    if ((heure_debut.Equals("00:00")) & (heure_fin.Equals("00:00")))
                    {
                    }
                    else { 
                        Heure_Superieur heure = new Heure_Superieur();
                        heure.matricule_superieur = user.matricule;
                        heure.matricule_employer = matricule;
                        heure.nom_prenom_superieur = user.prenom + " " + user.nom;
                        heure.nom_prenom_employer = get_nom_prenom_employer_by_matricule(matricule);
                        heure.Service = user.service;
                        heure.jour_planing = hsup.jour_planing;
                        heure.date_creation = System.DateTime.Now;
                        heure.date_debut_prevu = heure_debut;
                        heure.date_fin_prevu = heure_fin;
                        db.Heure_Superieur.Add(heure);
                        await db.SaveChangesAsync();
                    }
                    i++;
                    start_index = start_index + 11;
         
                }
                return View("OK");
            }

            return View("Error");
          
    
        }
        [Authorize(Roles = "Admin_Medicis,RH_Admin,RH_Directeur,RH_Superieur_Hiéarchique")]

        // GET: Heure_Superieur/Edit/5
        public ActionResult Edit(int? id)
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.nom_prenom = user.nom + " " + user.prenom;
            ViewBag.email = user.Email; 
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Heure_Superieur heure_Superieur = db.Heure_Superieur.Find(id);
           

            if (heure_Superieur == null)
            {
                return HttpNotFound();
            }
             
                return View(heure_Superieur);
        }
        // POST: Heure_Superieur/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin_Medicis,RH_Admin,RH_Directeur,RH_Superieur_Hiéarchique")]

        public ActionResult Edit(Heure_Superieur heure_Superieur)
        {
            if (ModelState.IsValid)
            {
                Heure_Superieur h = db.Heure_Superieur.FirstOrDefault(x => x.Id == heure_Superieur.Id);
                h.matricule_superieur = heure_Superieur.matricule_superieur;
                h.nom_prenom_superieur = heure_Superieur.nom_prenom_superieur;
                h.nom_prenom_employer = heure_Superieur.nom_prenom_employer;
                h.Service = heure_Superieur.Service;
          
                h.jour_planing = heure_Superieur.jour_planing;
 
                h.Commentaire = heure_Superieur.Commentaire;
                h.date_creation = heure_Superieur.date_creation;
                h.matricule_employer = heure_Superieur.matricule_employer;
                

              //  db.Entry(heure_Superieur).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(heure_Superieur);
        }
        [Authorize(Roles = "Admin_Medicis,RH_Admin,RH_Directeur,RH_Superieur_Hiéarchique")]

        // GET: Heure_Superieur/Delete/5
        public ActionResult Delete(int? id)
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.nom_prenom = user.nom + " " + user.prenom;
            ViewBag.email = user.Email;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Heure_Superieur heure_Superieur = db.Heure_Superieur.Find(id);
            if (heure_Superieur == null)
            {
                return HttpNotFound();
            }
           
            return View(heure_Superieur);
        }

        // POST: Heure_Superieur/Delete/5
        [Authorize(Roles = "Admin_Medicis,RH_Admin,RH_Directeur,RH_Superieur_Hiéarchique")]

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {

            Heure_Superieur heure_Superieur = db.Heure_Superieur.Find(id);
            db.Heure_Superieur.Remove(heure_Superieur);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        [Authorize(Roles = "Admin_Medicis,RH_Admin")]
        public ActionResult List_Heures_Superieurs(string sortOrder, string search_nom_prenom_matricule, string Search_jour_planing, string currentFilter, int? page)
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.nom_prenom = user.nom + " " + user.prenom;
            ViewBag.email = user.Email;
            ApplicationUser appuser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(User.Identity.GetUserId());
            ViewBag.Sorting_nom_prenom = String.IsNullOrEmpty(sortOrder) ? "nom_prenom" : "";
            ViewBag.Sortig_jour_planing = sortOrder == "Date" ? "date_desc" : "Date";
            if (search_nom_prenom_matricule != null)
            {
                page = 1;
            }
            else
            {
                search_nom_prenom_matricule = currentFilter;
            }

            DateTime date = System.DateTime.Now;
            DateTime date2 = System.DateTime.Now;



            if (!String.IsNullOrEmpty(Search_jour_planing))
            {
                string date_string_planing = Search_jour_planing.Replace(".", "");
                if (DateTime.TryParseExact(date_string_planing, "ddMMyyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out date)) ;
            }

            var heure_sups = from s in db.Heure_Superieur.Where(x => x.matricule_superieur == appuser.matricule)
                             select s;

            if (!String.IsNullOrEmpty(search_nom_prenom_matricule))
            {
                if (String.IsNullOrEmpty(Search_jour_planing))
                {
                    heure_sups = heure_sups.Where(s => s.nom_prenom_employer.Contains(search_nom_prenom_matricule)
                                           || s.matricule_employer.Contains(search_nom_prenom_matricule));
                }

                else if (!String.IsNullOrEmpty(Search_jour_planing))
                {
                    heure_sups = heure_sups.Where(s => s.nom_prenom_employer.Contains(search_nom_prenom_matricule)
                                           || s.matricule_employer.Contains(search_nom_prenom_matricule)
                                          & s.jour_planing == date

                                           );
                }

            }

            else if (String.IsNullOrEmpty(search_nom_prenom_matricule))
            {
                if (!String.IsNullOrEmpty(Search_jour_planing))

                {
                    heure_sups = heure_sups.Where(s => s.jour_planing == date);
                }

            }

            switch (sortOrder)
            {
                case "nom_prenom":
                    heure_sups = heure_sups.OrderByDescending(h => h.nom_prenom_employer);
                    break;
                case "Date":
                    heure_sups = heure_sups.OrderBy(h => h.jour_planing);
                    break;
                case "date_desc":
                    heure_sups = heure_sups.OrderByDescending(s => s.jour_planing);
                    break;
                default:
                    heure_sups = heure_sups.OrderBy(h => h.nom_prenom_employer);
                    break;
            }
            // return View(heure_sups.ToList());

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(heure_sups.ToPagedList(pageNumber, pageSize));

        }
        [Authorize]
       // public ActionResult Mes_Heures_Superieurs(string sortOrder, string search_nom_prenom_matricule, string Search_jour_planing, string currentFilter, int? page)

            public ActionResult Mes_Heures_Superieurs()
            {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.nom_prenom = user.nom + " " + user.prenom;
            ViewBag.email = user.Email;
             
            return View (db.MesHeuresSup.Where(x => x.matricule == user.matricule).ToList());
       /*     ApplicationUser appuser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(User.Identity.GetUserId());
            ViewBag.Sorting_nom_prenom = String.IsNullOrEmpty(sortOrder) ? "nom_prenom" : "";
            ViewBag.Sortig_jour_debut = sortOrder == "Date" ? "date_desc" : "Date";



            if (search_nom_prenom_matricule != null)
            {
                page = 1;
            }
            else
            {
                search_nom_prenom_matricule = currentFilter;
            }

            DateTime date = System.DateTime.Now;
            DateTime date2 = System.DateTime.Now;



            if (!String.IsNullOrEmpty(Search_jour_planing))
            {
                string date_string_planing = Search_jour_planing.Replace(".", "");
                if (DateTime.TryParseExact(date_string_planing, "ddMMyyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out date)) ;
            }

            var heure_sups = from s in db.Heure_Superieur.Where(x => x.matricule_employer == appuser.matricule)
                             select s;
             heure_sups = heure_sups.Where(x => x.date_approbation != null);

            if (!String.IsNullOrEmpty(search_nom_prenom_matricule))
            {
                if (String.IsNullOrEmpty(Search_jour_planing))
                {
                    heure_sups = heure_sups.Where(s => s.nom_prenom_employer.Contains(search_nom_prenom_matricule)
                                           || s.matricule_employer.Contains(search_nom_prenom_matricule));
                }

                else if (!String.IsNullOrEmpty(Search_jour_planing))
                {
                    heure_sups = heure_sups.Where(s => s.nom_prenom_employer.Contains(search_nom_prenom_matricule)
                                           || s.matricule_employer.Contains(search_nom_prenom_matricule)
                                          & s.jour_planing == date

                                           );
                }

            }

            else if (String.IsNullOrEmpty(search_nom_prenom_matricule))
            {
                if (!String.IsNullOrEmpty(Search_jour_planing))

                {
                    heure_sups = heure_sups.Where(s => s.jour_planing == date);
                }

            }


            switch (sortOrder)
            {
                case "nom_prenom":
                    heure_sups = heure_sups.OrderByDescending(h => h.nom_prenom_employer);
                    break;
                case "Date":
                    heure_sups = heure_sups.OrderBy(h => h.jour_planing);
                    break;
                case "date_desc":
                    heure_sups = heure_sups.OrderByDescending(s => s.jour_planing);
                    break;
                default:
                    heure_sups = heure_sups.OrderBy(h => h.nom_prenom_employer);
                    break;
            }
            // return View(heure_sups.ToList());

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(heure_sups.ToPagedList(pageNumber, pageSize));

    */

        }
        [Authorize(Roles = "Admin_Medicis,RH_Admin")]
        public ActionResult Approbation_RH(int? id)
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.nom_prenom = user.nom + " " + user.prenom;
            ViewBag.email = user.Email;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Heure_Superieur heure_Superieur = db.Heure_Superieur.Find(id);


            if (heure_Superieur == null)
            {
                return HttpNotFound();
            }
            return View(heure_Superieur);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin_Medicis,RH_Admin")]
        public ActionResult Approbation_RH(Heure_Superieur heure_Superieur)
        {
           ApplicationUser appuser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(User.Identity.GetUserId());
             
 
            if (ModelState.IsValid)
              {
                Heure_Superieur h = db.Heure_Superieur.FirstOrDefault(x => x.Id == heure_Superieur.Id);
                h.Approbation_Heure_Sup = heure_Superieur.Approbation_Heure_Sup;
                h.date_debut_pointage = heure_Superieur.date_debut_pointage;
                h.date_fin_pointage = heure_Superieur.date_fin_pointage;
                h.approbateur = appuser.nom + " " + appuser.prenom;
                h.date_approbation = System.DateTime.Now;
                db.SaveChanges();
                return View ("OK");        
               }
              else
               {
                  return View("Error");
               }
        }



        [Authorize(Roles = "Admin_Medicis,RH_Admin")]
        public ActionResult Import_Etat_HeuresSupp()
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.nom_prenom = user.nom + " " + user.prenom;
            ViewBag.email = user.Email;
            return View();
        }

        private ApplicationUserManager _userManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }

        [Authorize(Roles = "Admin_Medicis,RH_Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Import_Etat_HeuresSupp(HttpPostedFileBase file)
        {

            int c = 0;

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
                                if (matricule.Length == 2) { matricule = "00" + matricule; }
                                if (matricule.Length == 3) { matricule = "0" + matricule; }
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
                                        solde = solde.Replace(" ", "");
                                        solde = solde.Replace(",", ".");
                                        float s = float.Parse(solde, CultureInfo.InvariantCulture.NumberFormat);
                                        appuser.Solde_Conge = s;
                                        appuser.Dernier_maj_solde_conge = date_dernier;
                                        var user = _userManager.FindById(appuser.Id);
                                        _userManager.Update(user);
                                        db.SaveChanges();


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










    }
}
