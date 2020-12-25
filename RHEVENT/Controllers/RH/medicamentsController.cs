using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using OfficeOpenXml;
using PagedList;
using RHEVENT.Models;

namespace RHEVENT.Controllers
{
    public class medicamentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        UserManager<IdentityUser> userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>());



        // GET: medicaments
        public ActionResult Index()
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.nom_prenom = user.nom + " " + user.prenom;
            ViewBag.email = user.Email;
            return View(db.medicaments.ToList());
        }

        // GET: medicaments/Details/5
        public ActionResult Details(int? id)
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.nom_prenom = user.nom + " " + user.prenom;
            ViewBag.email = user.Email;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            medicament medicament = db.medicaments.Find(id);
            if (medicament == null)
            {
                return HttpNotFound();
            }
            return View(medicament);
        }

        // GET: medicaments/Create
        public ActionResult Create()
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.nom_prenom = user.nom + " " + user.prenom;
            ViewBag.email = user.Email;
            return View();
        }

        // POST: medicaments/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,code_medicament,designation_medicament,validation_prt,lien_image")] medicament medicament)
        {
            if (ModelState.IsValid)
            {
                db.medicaments.Add(medicament);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(medicament);
        }

        // GET: medicaments/Edit/5
        public ActionResult Edit(int? id)
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.nom_prenom = user.nom + " " + user.prenom;
            ViewBag.email = user.Email;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            medicament medicament = db.medicaments.Find(id);
            if (medicament == null)
            {
                return HttpNotFound();
            }
            return View(medicament);
        }

        // POST: medicaments/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,code_medicament,designation_medicament,validation_prt,lien_image")] medicament medicament)
        {
            if (ModelState.IsValid)
            {
                db.Entry(medicament).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(medicament);
        }

        // GET: medicaments/Delete/5
        public ActionResult Delete(int? id)
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.nom_prenom = user.nom + " " + user.prenom;
            ViewBag.email = user.Email;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            medicament medicament = db.medicaments.Find(id);
            if (medicament == null)
            {
                return HttpNotFound();
            }
            return View(medicament);
        }

        // POST: medicaments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            medicament medicament = db.medicaments.Find(id);
            db.medicaments.Remove(medicament);
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

        public ActionResult Commande()
        {

            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.nom_prenom = user.nom + " " + user.prenom;
            ViewBag.email = user.Email;
            return View(db.medicaments.ToList().OrderBy(x => x.designation_medicament));
        }

        public string get_medicament_by_code(string code)
        {

            medicament m = db.medicaments.Where(x => x.code_medicament == code).FirstOrDefault();

            return m.designation_medicament;
        }


        public ActionResult validercommande(string id)
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.nom_prenom = user.nom + " " + user.prenom;
            ViewBag.email = user.Email;

            List<CommandeL> list_pdts_a_commander = new List<CommandeL>();
            string cmd = id;

            int start_index = 0;
            int nbr_pdts = cmd.Length / 6;
            int i = 0;
            while (i < cmd.Length)
            {

                string produit = cmd.Substring(start_index, 6);
                CommandeL c = new CommandeL();



                c.Quantite_Commandee = Int32.Parse((produit.Substring(5, 1)));
                c.Designation_medicament = get_medicament_by_code(produit.Substring(0, 5));
                c.Code_medicament = produit.Substring(0, 5);
                list_pdts_a_commander.Add(c);
                start_index = start_index + 6;
                i = i + 6;
            }

            CommandeL cl = new CommandeL();
            cl.list_afficher = list_pdts_a_commander;
            return View(cl);
        }


        [HttpPost]
        public async Task<ActionResult> validercommande(CommandeL commandel)
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.nom_prenom = user.nom + " " + user.prenom;
            ViewBag.email = user.Email;

            List<CommandeL> list_pdts_a_commander = new List<CommandeL>();
            string cmd = commandel.list_lignes_valider;
     
            int start_index = 0;
            int nbr_pdts = cmd.Length / 6;
            int i = 0;
            string REF_CMD = "CMD-" + user.matricule + "-" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + "-" + DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second + "-" + DateTime.Now.Millisecond;

            Commande commande = new Commande();
            commande.Ref_cmd = REF_CMD;
            commande.Nom_prenom = user.prenom + " " + user.nom;
            commande.Matricule = user.matricule;
            commande.Service = user.service;
            commande.Etat_commande_medicament = Etat_commande_medicament.Demande_en_cours;
            commande.Date_commande = System.DateTime.Now;
            db.Commandes.Add(commande);

            while (i < cmd.Length)
            {
                string produit = cmd.Substring(start_index, 6);
                CommandeL c = new CommandeL();
                c.Quantite_Commandee = Int32.Parse((produit.Substring(5, 1)));
                c.Designation_medicament = get_medicament_by_code(produit.Substring(0, 5));
                c.Code_medicament = produit.Substring(0, 5);
                c.Nom_prenom = user.prenom + " " + user.nom;
                c.Matricule = user.matricule;
                c.Service = user.service;
                c.Validation_prt = require_prt_validation(produit.Substring(0, 5));
                c.Date_creation = System.DateTime.Now;
                c.Ref_cmd = REF_CMD;
                c.etat = Etat_ligne.en_cours;
                c.Date_validation = System.DateTime.Now;
                db.Commandels.Add(c);

                start_index = start_index + 6;
                i = i + 6;
            }
            await db.SaveChangesAsync();
            return View("OK");

        }

        public validation require_prt_validation(string pf)
        {
            medicament m = db.medicaments.Where(x => x.code_medicament == pf).FirstOrDefault();
            return m.validation_prt;
        }

        [HttpPost]
        public ActionResult Commande(medicament m)
        {
            if (m.list_medicaments_commander.Length > 42)
            {
                return RedirectToAction("Commande", "medicaments");
            }
            return RedirectToAction("validercommande", "medicaments", new { id = m.list_medicaments_commander });
        }

        public ActionResult Approbation_Primaire()
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.nom_prenom = user.nom + " " + user.prenom;
            ViewBag.email = user.Email;

            List<CommandeL> list= new List<CommandeL>();
            List<CommandeL> list2 = new List<CommandeL>();
            list = db.Commandels.ToList().Where(x => x.Decision_primaire == validation.RIEN & x.Validation_prt == validation.NON).ToList();
            list2 = db.Commandels.ToList().Where(y => y.Validation_prt == validation.OUI & y.Decision_prt == Decision_PRT.OUI & y.Decision_primaire == validation.RIEN).ToList();

            list.AddRange(list2);

            CommandeL c = new CommandeL();
            c.list_afficher = list;
            return View(c);
        }

        [HttpPost]
        public async Task <ActionResult> Approbation_Primaire(CommandeL cl)
        {

            try { 



            string chaine = cl.list_lignes_valider;
              

                int deb = 0;

                List<string> list = new List<string>();
                for (int i = 0; i < chaine.Length; i++)
                {
                    if (chaine.Substring(i, 1).Equals(";"))
                    {

                        if (deb == 0) { list.Add(chaine.Substring(deb, i - deb)); }
                        else { list.Add(chaine.Substring(deb + 1, i - deb - 1)); }
                        deb = i;

                    }
                }
                deb = 0;

                for (int i = 0; i < list.Count; i++)
                {
                    int reponse = 1;
                    string id = "";
                    string quantite = "";
                    string commentaire = "";
                    string ligne = list.ElementAt(i);
                    ligne = ligne.Replace(";", "");
                    int debl = 0;
                    for (int j = 0; j < ligne.Length; j++)
                    {

                        if (ligne.Substring(j, 1).Equals("-"))
                        {

                            id = ligne.Substring(0, j);
                            debl = j + 1;

                        }


                        if (ligne.Substring(j, 1).Equals("_"))
                        {
                            quantite = ligne.Substring(debl, j - debl);
                            
                            reponse =Int32.Parse( ligne.Substring(j + 1, 1));
                            debl = j + 2;
                            commentaire = ligne.Substring(debl, ligne.Length - debl);
                        }
                    }


                    ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
        
                    try
                    {
                        int int_id = Int32.Parse(id);
                        CommandeL cmdl = db.Commandels.FirstOrDefault(coml => coml.Id == int_id);

                        cmdl.Quantite_acceptee = Int32.Parse(quantite);
                        cmdl.Commentaire_primaire = commentaire;
                        cmdl.Approbateur_primaire = user.prenom + " " + user.prenom;
                        cmdl.Decision_primaire = (validation)reponse;

                        await db.SaveChangesAsync();
                    }
                    catch (DbEntityValidationException ee)
                    {
                        foreach (var error in ee.EntityValidationErrors)
                        {
                            foreach (var thisError in error.ValidationErrors)
                            {
                                var errorMessage = thisError.ErrorMessage;
                                return RedirectToAction(errorMessage);
                            }
                        }



                    }

                }
      
           }catch (Exception ex) {
              // return RedirectToAction(ex.ToString());
               return View("Error");

                 
          }
            return View("OK");
        }
        public ActionResult mescommandes()
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.nom_prenom = user.nom + " " + user.prenom;
            ViewBag.email = user.Email;
            return View(db.Commandes.Where(x=>x.Matricule == user.matricule).ToList());
        }


        [Authorize (Roles = "Admin_Medicis,RH_PRT,RH_Admin")]
        public ActionResult LignesCommande(string id)
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.nom_prenom = user.nom + " " + user.prenom;
            ViewBag.email = user.Email;
            ViewBag.refcommande = id;
            return View(db.Commandels.Where(x => x.Ref_cmd == id).ToList());
        }
        [Authorize(Roles = "Admin_Medicis,RH_PRT,RH_Admin")]
        public ActionResult ApprobationPRT()
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.nom_prenom = user.nom + " " + user.prenom;
            ViewBag.email = user.Email;

            List<CommandeL> list = new List<CommandeL>();
            list = db.Commandels.ToList().Where(x => x.Validation_prt == validation.NON).OrderBy(x=>x.Decision_prt).ToList();           
            CommandeL c = new CommandeL();
            c.list_afficher = list;
          
            return View(c);
        }
        [Authorize(Roles = "Admin_Medicis,RH_PRT,RH_Admin")]
        public ActionResult Update_demande_by_PRT(string id)
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.nom_prenom = user.nom + " " + user.prenom;
            ViewBag.email = user.Email;
            int int_id = Int32.Parse(id);
            CommandeL cmdl = db.Commandels.Find(int_id);
            return View(cmdl);
        }
        [HttpPost]
        [Authorize(Roles = "Admin_Medicis,RH_PRT,RH_Admin")]
        public async Task <ActionResult> Update_demande_by_PRT(CommandeL cmdl)
        {

            
            CommandeL c = db.Commandels.Find(cmdl.Id);
            c.Decision_prt = cmdl.Decision_prt;
            c.Designation_medicament = cmdl.Designation_medicament;
            c.Code_medicament = cmdl.Code_medicament;
            c.Quantite_Commandee = cmdl.Quantite_Commandee;
            c.Commentaire_prt = cmdl.Commentaire_prt;
            await db.SaveChangesAsync();
      
            return RedirectToAction("ApprobationPRT", "medicaments");
        }

        public ActionResult Commandes (int? page,string search_nom_prenom_matricule,string Search_jour_commande)
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.nom_prenom = user.nom + " " + user.prenom;
            ViewBag.email = user.Email;
            List <Commande> list_commandes = db.Commandes.ToList();
            DateTime date = System.DateTime.Now;
            if (!String.IsNullOrEmpty(Search_jour_commande))
            {
                string date_string_planing = Search_jour_commande.Replace(".", "");
                if (DateTime.TryParseExact(date_string_planing, "ddMMyyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out date)) ;
            }



            if (!String.IsNullOrEmpty(search_nom_prenom_matricule))
            {
                list_commandes = list_commandes.Where(x => x.Nom_prenom.Contains(search_nom_prenom_matricule) || x.Matricule.Contains(search_nom_prenom_matricule)).ToList(); 
            }

            if (!String.IsNullOrEmpty(Search_jour_commande))
            {

               
                list_commandes = list_commandes.Where(x => x.Date_commande.Date == date).ToList();
            }

         
            int pageSize = 10; 
            int pageNumber = (page ?? 1);
            return View (list_commandes.ToPagedList(pageNumber,pageSize));
        }


        public ActionResult Demande2()
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.nom_prenom = user.nom + " " + user.prenom;
            ViewBag.email = user.Email;
            return View();
        }
        [HttpPost]
        public JsonResult Demande2(string Prefix)
        {

            var medicaments_list = (from M in db.medicaments.ToList()
                            where (M.designation_medicament.StartsWith(Prefix.ToUpper()) || M.designation_medicament.StartsWith(Prefix.ToLower()) ) 
                            select new {M.designation_medicament});

            return Json(medicaments_list, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult AddMedicament(medicament m)
        {
            return View("Demande2");
        }

        public  ActionResult Commande_du_jour()
        {

            var query = from com in db.Commandels
                        where ((com.Decision_primaire == validation.OUI) & (com.etat == Etat_ligne.en_cours))
                        select new
                        {
                            Ref_cmd = com.Ref_cmd,
                            Designation_medicament = com.Designation_medicament,
                            Code_medicament = com.Code_medicament,
                            Nom_prenom = com.Nom_prenom,
                            Matricule = com.Matricule,
                            Quantite_acceptee = com.Quantite_acceptee 
                        };

            var c = query.ToList().Select(r => new CommandeL
            {
                Ref_cmd = r.Ref_cmd,
                Designation_medicament = r.Designation_medicament,
                Code_medicament = r.Code_medicament,
                Nom_prenom = r.Nom_prenom,
                Matricule=r.Matricule,
                Quantite_acceptee = r.Quantite_acceptee

            }).ToList().OrderBy(a=>a.Matricule);




            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Liste des médicaments pour le "+System.DateTime.Now.ToString());
            ws.Cells["A1"].Value = "Ref_cmd";
            ws.Cells["B1"].Value = "Designation_medicament";
            ws.Cells["C1"].Value = "Code_medicament";
            ws.Cells["D1"].Value = "Nom_prenom";
            ws.Cells["E1"].Value = "Matricule";
            ws.Cells["F1"].Value = "Quantite_acceptee";
         
            int rowStart = 2;
            int jour = 0;
            foreach (var item in c)
            {
                //   jour++;
                ws.Cells[String.Format("A{0}", rowStart)].Value = item.Ref_cmd;
                ws.Cells[String.Format("B{0}", rowStart)].Value = item.Designation_medicament;
                ws.Cells[String.Format("C{0}", rowStart)].Value = item.Code_medicament;
                ws.Cells[String.Format("D{0}", rowStart)].Value = item.Nom_prenom ;
                ws.Cells[String.Format("E{0}", rowStart)].Value = item.Matricule;
                ws.Cells[String.Format("F{0}", rowStart)].Value = item.Quantite_acceptee ;             
                rowStart++;
            }
            //  return RedirectToAction(jour + "");
            ws.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("Content-disposition", "attachment:filename=" + "liste_médicaments.xlsx");
            Response.BinaryWrite (pck.GetAsByteArray());
            Response.End();

            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
         

            var updates = db.Commandels.Where(u => u.etat == Etat_ligne.en_cours).ToList();
            updates.ForEach(a =>
                          {
                              a.etat = Etat_ligne.accepte;
                              a.Date_validation = System.DateTime.Now;
                              a.user_validation = user.nom + " " + user.prenom + " " + user.matricule;
                          });
            db.SaveChanges();
            return RedirectToAction("Commandes");
        }



    }
}
