using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PagedList;
using RHEVENT.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace RHEVENT.Controllers
{
    public class DemandeController : Controller
    {
        // GET: Demande
        private ApplicationDbContext db = new ApplicationDbContext();
        UserManager<IdentityUser> userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>());

        [Authorize]
        public ActionResult Demander_attestation()
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.nom_prenom = user.nom + " " + user.prenom;
            ViewBag.email = user.Email;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Demander_attestation(Attestation attestation)
        {

            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            var users_rh = from u in db.Users  where u.Roles.Any(r => r.RoleId == "7281c516-fab8-4f13-8bbd-1e26d5634a93")
            select u;

            string error = "";
                try
                {
                    attestation.Datetime = System.DateTime.Now;
                    attestation.UserId = User.Identity.GetUserId();
                    attestation.UserName = User.Identity.GetUserName();
                    attestation.Approbateur_demande = "PERSONNE";
                    attestation.etat_demande = attestation.etat_demande;
                    attestation.titre_attestation = ((int)attestation.Intitule).ToString();
                    attestation.nom_prenom = user.nom + " " + user.prenom;
                    attestation.service = user.service;
                    attestation.Approbateur_RH = "";
                    db.Attestations.Add(attestation);
                
                    foreach (var rh_u in users_rh)
                     {
                    Email email = new Email();
                    email.Destinataire = rh_u.nom + " " + rh_u.prenom;
                    email.Email_Destinataire = rh_u.Email;
                    email.Sujet = "Demande Attestation";
                    email.Message = "Attestation de la part de " + attestation.nom_prenom;
                    email.Current_User_Event = attestation.nom_prenom + " " + attestation.service;
                    email.Date_email = System.DateTime.Now;
                    email.Etat_Envoi = "0";
                    db.Emails.Add(email);                  
                     }
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
                return View("Error");
               // return RedirectToAction(error);
                 }
               
/*
            }
            else
            {
                return View("Error");

            }*/

           
        }

        [Authorize]
        public ActionResult Mes_attestations(int? page)
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.nom_prenom = user.nom + " " + user.prenom;
            ViewBag.email = user.Email;
            try
            {
                int pageSize = 10;
                int pageNumber = (page ?? 1);
                // return View(db.Attestations.ToList().Select(att=>att).Where(att=>att.UserId == User.Identity.GetUserId()));

                return View(db.Attestations.ToList().Select(att => att).Where(att => att.UserId == User.Identity.GetUserId()).OrderByDescending(att=>att.Datetime).ToPagedList(pageNumber, pageSize));
            }catch(Exception e)
            {
                return RedirectToAction("Mes_attestations");
            } 
        }

        [Authorize(Roles = "Admin_Medicis,RH_Admin")] 
        public ActionResult Attestations(string sortOrder,int? page)
        {

            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.nom_prenom = user.nom + " " + user.prenom;
            ViewBag.email = user.Email;

            ViewBag.CurrentSort = sortOrder;
            ViewBag.MatriculeSortParam = String.IsNullOrEmpty(sortOrder) ? "matricule_desc":"matricule_asc";
            ViewBag.DateDemande = sortOrder == "DateDemande" ? "date_desc" : "date";
            ViewBag.TitreDemande = sortOrder == "TitreDemande" ? "titre_desc" : "";
            ViewBag.EtatDemande = sortOrder == "EtatDemande" ? "etat_desc" : "etat_asc";

            var attes = from a in db.Attestations select a;

            switch(sortOrder)
            {
                case "matricule_desc":
                    attes = attes.OrderByDescending(a => a.UserId);
                    break;
                case "matricule_asc":
                    attes = attes.OrderBy(a => a.UserId);
                    break;

                case "date_desc":
                    attes = attes.OrderByDescending(a => a.Datetime);
                    break;

                case "date":
                    attes = attes.OrderBy(a => a.Datetime);
                    break;

                case "titre_desc":
                    attes = attes.OrderByDescending(a => a.titre_attestation);
                    break;

                case "etat_desc":
                    attes = attes.OrderByDescending(a => a.etat_demande);
                    break;

                case "etat_asc":
                    attes = attes.OrderBy(a => a.etat_demande);
                    break;

                default:
                    attes = attes.OrderBy(a => a.etat_demande);
                    break;

            }

            int pageSize = 20;
            int pageNumber = (page ?? 1);
            return View(attes.ToPagedList(pageNumber,pageSize));     
           
        }


        [Authorize(Roles = "Admin_Medicis,RH_Admin")]
        public ActionResult UpdateAttestation(int id)
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.nom_prenom = user.nom + " " + user.prenom;
            ViewBag.email = user.Email;
            Attestation attestattion = db.Attestations.Find(id);
            return View(attestattion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin_Medicis,RH_Admin")]
        public ActionResult UpdateAttestation(Attestation attestation)
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            string error = "";
            if (ModelState.IsValid)
            {
                try
                {
                 Attestation a =    db.Attestations.FirstOrDefault(x => x.Id == attestation.Id);
                    a.commentaire = attestation.commentaire;
                    a.Approbateur_demande = User.Identity.Name.ToString().Replace("@teriak.com","");
                    a.etat_demande = attestation.etat_demande;
                    a.Approbateur_RH = user.prenom + " " + user.matricule;
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
            }

            return View(attestation);
        }
         
    }
}