using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using OfficeOpenXml;
using RHEVENT.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RHEVENT.Controllers
{
    [Authorize(Roles = "Admin_Medicis,RH_Admin")]
    public class ExportEtatController : Controller
    {
        // GET: ExportEtat
        private ApplicationDbContext db = new ApplicationDbContext();
        UserManager<IdentityUser> userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>());
        [Authorize(Roles = "Admin_Medicis,RH_Admin")]
        public ActionResult Export_Autorisation()
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.nom_prenom = user.nom + " " + user.prenom;
            ViewBag.email = user.Email;
            return View();
        }
        [Authorize(Roles = "Admin_Medicis,RH_Admin")]
        public ActionResult Export_Conges()
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.nom_prenom = user.nom + " " + user.prenom;
            ViewBag.email = user.Email;
            return View();
        }
        [Authorize(Roles = "Admin_Medicis,RH_Admin")]
        [HttpPost]
        public ActionResult Export_Conges(string date_debut,string date_fin)
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            string date_string_debut = date_debut.Replace(".", "");
            string date_string_fin = date_fin.Replace(".", "");

            DateTime date;
            DateTime date2;

            if (DateTime.TryParseExact(date_string_debut, "ddMMyyyy",System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None,out date)) ;              
            if (DateTime.TryParseExact(date_string_fin, "ddMMyyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out date2)) ;

            if (date > date2)
            {
                return View("Error");
            }
            else
            { 
                var query = from con in db.Conges where ((con.jour_debut >= date) & (con.jour_fin <= date2) & (con.acceptation_superieur == acceptation_superieur_hierarchique.Demande_validée)&(con.site==user.site))
                            select new {
                                matricule = con.matricule,
                                nom_prenom = con.nom_prenom,
                                service = con.service,
                                jour_debut = con.jour_debut,
                                heure_sortie = con.heure_sortie,
                                jour_fin = con.jour_fin,
                                heure_entree = con.heure_entree
                            };

                var c = query.ToList().Select(r => new Conge
                {
                    matricule = r.matricule,
                    nom_prenom = r.nom_prenom,
                    service = r.service,
                    jour_debut = r.jour_debut,
                    heure_sortie=r.heure_sortie,
                    jour_fin = r.jour_fin,
                    heure_entree=r.heure_entree
                }).ToList();

                 
                ExcelPackage pck = new ExcelPackage();
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Etat des congés");
                ws.Cells["A1"].Value = "Matricule";
                ws.Cells["B1"].Value = "Nom & Prénom";
                ws.Cells["C1"].Value = "Service";
                ws.Cells["D1"].Value = "Jour début";
                ws.Cells["E1"].Value = "Heure sortie";
                ws.Cells["F1"].Value = "Jour fin";
                ws.Cells["G1"].Value = "Heure entrée";
                int rowStart = 2;
                int jour = 0;
                foreach (var item in c)
                {
                 //   jour++;
                    ws.Cells[String.Format("A{0}", rowStart)].Value = item.matricule;
                    ws.Cells[String.Format("B{0}", rowStart)].Value = item.nom_prenom;
                    ws.Cells[String.Format("C{0}", rowStart)].Value = item.service;
                    ws.Cells[String.Format("D{0}", rowStart)].Value = item.jour_debut+"";
                    ws.Cells[String.Format("E{0}", rowStart)].Value = item.heure_sortie;
                    ws.Cells[String.Format("F{0}", rowStart)].Value = item.jour_fin+"";
                    ws.Cells[String.Format("G{0}", rowStart)].Value = item.heure_entree;
                    rowStart++;
                }
              //  return RedirectToAction(jour + "");
                ws.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("Content-disposition", "attachment:filename=" + "EtatCongés.xlsx");
                Response.BinaryWrite(pck.GetAsByteArray());
                Response.End();
                return View();
            }
        }
        [Authorize(Roles = "Admin_Medicis,RH_Admin")]
        [HttpPost]
        public ActionResult Export_Autorisation(string date_debut, string date_fin)
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            string date_string_debut = date_debut.Replace(".", "");
            string date_string_fin = date_fin.Replace(".", "");

            DateTime date;
            DateTime date2;

            if (DateTime.TryParseExact(date_string_debut, "ddMMyyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out date)) ;
            if (DateTime.TryParseExact(date_string_fin, "ddMMyyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out date2)) ;

            if (date > date2)
            {
                return View("Error");
            }
            else
            {
                var query = from aut in db.Autorisations
                            where ((aut.jour_autorisation >= date) & (aut.jour_autorisation <= date2) & (aut.acceptation_superieur == acceptation_superieur_hierarchique.Demande_validée) &(aut.site==user.site))
                            select new
                            {
                                matricule = aut.matricule,
                                nom_prenom = aut.nom_prenom,
                                service = aut.service,
                                jour_autorisation = aut.jour_autorisation,
                                heure_sortie = aut.heure_sortie,
                                heure_enree = aut.heure_entree,                              
                            };

                var a = query.ToList().Select(r => new Autorisation
                {
                    matricule = r.matricule,
                    nom_prenom = r.nom_prenom,
                    service = r.service,
                    jour_autorisation = r.jour_autorisation,
                    heure_sortie = r.heure_sortie,
                    heure_entree = r.heure_enree,
                }).ToList();

                ExcelPackage pck = new ExcelPackage();
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Etat des autorisations");
                ws.Cells["A1"].Value = "Matricule";
                ws.Cells["B1"].Value = "Nom & Prénom";
                ws.Cells["C1"].Value = "Service";
                ws.Cells["D1"].Value = "Jour autorisation";
                ws.Cells["E1"].Value = "Heure sortie";        
                ws.Cells["F1"].Value = "Heure entrée";
                int rowStart = 2;
                int jour = 0;
                foreach (var item in a)
                {
                    //   jour++;
                    ws.Cells[String.Format("A{0}", rowStart)].Value = item.matricule;
                    ws.Cells[String.Format("B{0}", rowStart)].Value = item.nom_prenom;
                    ws.Cells[String.Format("C{0}", rowStart)].Value = item.service;
                    ws.Cells[String.Format("D{0}", rowStart)].Value = item.jour_autorisation + "";
                    ws.Cells[String.Format("E{0}", rowStart)].Value = item.heure_sortie;       
                    ws.Cells[String.Format("F{0}", rowStart)].Value = item.heure_entree;
                    rowStart++;
                }
                //  return RedirectToAction(jour + "");
                ws.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("Content-disposition", "attachment:filename=" + "EtatCongés.xlsx");
                Response.BinaryWrite(pck.GetAsByteArray());
                Response.End();
                return View();
            }
        }
        [Authorize(Roles = "Admin_Medicis,RH_Admin")]
        [Authorize]
        public ActionResult Export_Attestation()
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.nom_prenom = user.nom + " " + user.prenom;
            ViewBag.email = user.Email;
            return View();
        }



        [Authorize(Roles = "Admin_Medicis,RH_Admin")]
        [HttpPost]
        public ActionResult Export_Attestation(string date_debut, string date_fin)
        {
            string date_string_debut = date_debut.Replace(".", "");
            string date_string_fin = date_fin.Replace(".", "");

            DateTime date;
            DateTime date2;

            if (DateTime.TryParseExact(date_string_debut, "ddMMyyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out date)) ;
            if (DateTime.TryParseExact(date_string_fin, "ddMMyyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out date2)) ;

            if (date > date2)
            {
                return View("Error");
            }
            else
            { 
                var query = from att in db.Attestations
                            where ((att.Datetime >= date) & (att.Datetime <= date2))
                            select new
                            {
                                UserName = att.UserName,
                                nom_prenom = att.nom_prenom,
                                service = att.service,
                                titre_attestation =  att.titre_attestation,
                                Datetime = att.Datetime,
                                etat_demande =att.etat_demande
            };

                var a = query.ToList().Select(r => new Attestation
                {
                    UserName = r.UserName,
                    nom_prenom = r.nom_prenom,
                    service = r.service,
                    titre_attestation = r.titre_attestation,
                    Datetime = r.Datetime,
                    etat_demande = r.etat_demande
                }).ToList();


                ExcelPackage pck = new ExcelPackage();
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Etat des attestations");
                ws.Cells["A1"].Value = "Matricule";
                ws.Cells["B1"].Value = "Nom & Prénom";
                ws.Cells["C1"].Value = "Service";
                ws.Cells["D1"].Value = "Titre attestation";
                ws.Cells["E1"].Value = "Date attestation";
                ws.Cells["F1"].Value = "Etat demande";
                int rowStart = 2;
                int jour = 0;
                foreach (var item in a)
                {
                    //   jour++;
                    ws.Cells[String.Format("A{0}", rowStart)].Value = item.UserName;
                    ws.Cells[String.Format("B{0}", rowStart)].Value = item.nom_prenom;
                    ws.Cells[String.Format("C{0}", rowStart)].Value = item.service;
                    ws.Cells[String.Format("D{0}", rowStart)].Value = (AttestationDemandee) Int32.Parse( item.titre_attestation);
                    ws.Cells[String.Format("E{0}", rowStart)].Value = item.Datetime.ToString();
                    ws.Cells[String.Format("F{0}", rowStart)].Value = item.etat_demande;
                    rowStart++;
                }
                //  return RedirectToAction(jour + "");
                ws.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("Content-disposition", "attachment:filename=" + "Etatattestations.xlsx");
                Response.BinaryWrite(pck.GetAsByteArray());
                Response.End();
                return View();
            }
        }

        public ActionResult Export_Heures_supplementaires()
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.nom_prenom = user.nom + " " + user.prenom;
            ViewBag.email = user.Email;
            return View();
        }


        [Authorize(Roles = "Admin_Medicis,RH_Admin")]
        [HttpPost]
        public ActionResult Export_Heures_supplementaires(string date_debut, string date_fin)
        {
            string date_string_debut = date_debut.Replace(".", "");
            string date_string_fin = date_fin.Replace(".", "");

            DateTime date;
            DateTime date2;

            if (DateTime.TryParseExact(date_string_debut, "ddMMyyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out date)) ;
            if (DateTime.TryParseExact(date_string_fin, "ddMMyyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out date2)) ;

            if (date > date2)
            {
                return View("Error");
            }
            else
            {

                var query = from hsup in db.Heure_Superieur
                            where ((hsup.date_creation >= date) & (hsup.date_creation <= date2) & (hsup.Approbation_Heure_Sup == Approbation_Heures_Superieurs.Demande_validée))
                            select new
                            {
                                nom_prenom_superieur = hsup.nom_prenom_superieur,
                                nom_prenom_employer = hsup.nom_prenom_employer,
                                matricule_employer = hsup.matricule_employer,
                                service = hsup.Service,
                                
                                jour_planing = hsup.jour_planing,
                              
                                commentaire = hsup.Commentaire
                            };

                var a = query.ToList().Select(r => new Heure_Superieur
                {
                    nom_prenom_superieur = r.nom_prenom_superieur,
                    nom_prenom_employer = r.nom_prenom_employer,
                    matricule_employer = r.matricule_employer,
                    Service = r.service,
                 
                    jour_planing = r.jour_planing,
                    Commentaire = r.commentaire
                }).ToList();


                ExcelPackage pck = new ExcelPackage();
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Etat des heures supplémentaires");
                ws.Cells["A1"].Value = "Supérieur hiérarchique";
                ws.Cells["B1"].Value = "Employer";
                ws.Cells["C1"].Value = "Matricule employer";
                ws.Cells["D1"].Value = "Service";
                ws.Cells["E1"].Value = "Titre";
                ws.Cells["F1"].Value = "Jour début";
                ws.Cells["G1"].Value = "Jour fin";
                ws.Cells["H1"].Value = "Heure début";
                ws.Cells["I1"].Value = "Heure fin";
                ws.Cells["J1"].Value = "Commentaire";
                int rowStart = 2;
                int jour = 0;
                foreach (var item in a)
                {
                    //   jour++;
                    ws.Cells[String.Format("A{0}", rowStart)].Value = item.nom_prenom_superieur;
                    ws.Cells[String.Format("B{0}", rowStart)].Value = item.nom_prenom_employer;
                    ws.Cells[String.Format("C{0}", rowStart)].Value = item.matricule_employer;
                    ws.Cells[String.Format("D{0}", rowStart)].Value = item.Service;
           
                    ws.Cells[String.Format("E{0}", rowStart)].Value = item.jour_planing.ToString();
           
                    ws.Cells[String.Format("F{0}", rowStart)].Value = item.Commentaire;
                    rowStart++;
                }
                //  return RedirectToAction(jour + "");
                ws.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("Content-disposition", "attachment:filename=" + "Etat_heures_supplémentaires.xlsx");
                Response.BinaryWrite(pck.GetAsByteArray());
                Response.End();
                return View();
            }
        }











        public ActionResult Export_List_Repas()
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.nom_prenom = user.nom + " " + user.prenom;
            ViewBag.email = user.Email;
            return View();
        }


      


    }
}