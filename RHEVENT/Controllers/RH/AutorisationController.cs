using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using PagedList;
using RHEVENT.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace RHEVENT.Controllers
{
    public class AutorisationController : Controller
    {
        // GET: Autorisation
        private ApplicationDbContext db = new ApplicationDbContext();
        UserManager<IdentityUser> userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>());

        private ApplicationUserManager _userManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }


        [Authorize]

        public ActionResult Demander_autorisation()
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.nom_prenom = user.nom+" "+user.prenom;
            ViewBag.email = user.Email;

            ViewBag.Role = "";

            List<string> roles = userManager.GetRoles(user.Id).ToList();
            if (roles.Contains("Admin") || roles.Contains ("Superieur_Hiéarchique") || roles.Contains("Directeur"))
            {
                ViewBag.Role = "show_attribute_menu";
            }



        /*   if ( Roles.IsUserInRole("Admin") || Roles.IsUserInRole("Superieur_Hiéarchique") || Roles.IsUserInRole("Directeur"))
            {
               
            }*/
            
           // ViewBag.Role = _userManager.GetRoles(user.Id).FirstOrDefault();
            return View();
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task <ActionResult> Demander_autorisation(Autorisation autorisation)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
                autorisation.UserId = User.Identity.GetUserId();
                autorisation.UserName = User.Identity.GetUserName();
                autorisation.matricule = user.matricule;
                autorisation.Fonction = user.fonction;
                autorisation.service = user.service;
                autorisation.site = user.site;
                autorisation.superieur_hierarchique = user.signataire;
                autorisation.Date_emission_demande = System.DateTime.Now;
                autorisation.jour_autorisation = autorisation.jour_autorisation;
                autorisation.heure_sortie = autorisation.heure_sortie;
                autorisation.heure_entree = autorisation.heure_entree;
                autorisation.acceptation_superieur = acceptation_superieur_hierarchique.Demande_en_cours;
                autorisation.acceptation_ressource = acceptation_ressource_humaine.Demande_en_cours;
                autorisation.nom_prenom = user.nom + " " + user.prenom;
                autorisation.Approbateur_RH = "";
                autorisation.Date_validation_superieur = System.DateTime.Now;
                autorisation.Solde_Conge = user.Solde_Conge;
                var user_signataire = userManager.FindByName(user.signataire);
                if (user_signataire.Id == user.Id)
                {
                    autorisation.acceptation_superieur = acceptation_superieur_hierarchique.Demande_validée;
                }
                db.Autorisations.Add(autorisation);
                if (user_signataire.Email != user.Email)
                {
                    Email email = new Email();
                    email.Destinataire = user_signataire.Email;
                    email.Email_Destinataire = user_signataire.Email;
                    email.Sujet = "Demande d'autorisation";
                    email.Message = "Demande d'autorisation de la part de " + user.nom + " " + user.prenom;
                    email.Current_User_Event = user.nom + " " + user.prenom + " " + user.service;
                    email.Date_email = System.DateTime.Now;
                    email.Etat_Envoi = "0";
                    db.Emails.Add(email);
                }
                await db.SaveChangesAsync();
                return View("OK");
            }
            else
            {
                return View("Error");
            }
        }
        [Authorize]
        public ActionResult Mes_autorisations(int? page)
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.nom_prenom = user.nom + " " + user.prenom;
            ViewBag.email = user.Email;
           
                int pageSize = 20;
                int pageNumber = (page ?? 1);
                // return View(db.Attestations.ToList().Select(att=>att).Where(att=>att.UserId == User.Identity.GetUserId()));
                return View(db.Autorisations.ToList().Select(aut => aut).Where(aut => aut.UserId == User.Identity.GetUserId()).OrderByDescending(aut => aut.Date_emission_demande).ToPagedList(pageNumber, pageSize));
           
        }

        //[Authorize(Roles = "Admin,Directeur,Superieur_Hiéarchique")]
        public ActionResult Approbation_superieur_hiéarchique( int? page)
        {


            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.nom_prenom = user.nom + " " + user.prenom;
            ViewBag.email = user.Email;

            int pageSize = 20;
            int pageNumber = (page ?? 1);

            return View(db.Autorisations.ToList().Select(aut => aut).Where((aut => aut.superieur_hierarchique == User.Identity.GetUserName())).OrderBy(aut => aut.acceptation_superieur).OrderByDescending(aut=>aut.Date_emission_demande).ToPagedList(pageNumber,pageSize));           
        }
        [Authorize(Roles = "Admin_Medicis,RH_Admin,RH_Directeur,RH_Superieur_Hiéarchique")]
        public ActionResult UpdateAutorisationSuperieur(int id)
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.nom_prenom = user.nom + " " + user.prenom;
            ViewBag.email = user.Email;
            Autorisation autorisation = db.Autorisations.Find(id);           
            return View(autorisation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin_Medicis,RH_Admin,RH_Directeur,RH_Superieur_Hiéarchique")]
        public ActionResult UpdateAutorisationSuperieur(Autorisation autorisation)
        {

         
                string error = "";
               
                    try
                    {
                        Autorisation a = db.Autorisations.FirstOrDefault(x => x.Id == autorisation.Id);
                        a.commentaire_superieur_hiearchique = autorisation.commentaire_superieur_hiearchique;
                        a.acceptation_superieur = autorisation.acceptation_superieur;
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
        public ActionResult Approbation_RH(int ?page)
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.nom_prenom = user.nom + " " + user.prenom;
            ViewBag.email = user.Email;
            int pageSize = 20;
            int pageNumber = (page ?? 1);
            return View(db.Autorisations.ToList().Select(aut => aut).Where(aut=>aut.acceptation_superieur == acceptation_superieur_hierarchique.Demande_validée).Where(aut=>aut.site == user.site).OrderBy(aut => aut.acceptation_ressource).ToPagedList(pageNumber, pageSize));
        }

        [Authorize(Roles = "Admin_Medicis,RH_Admin")]
        public ActionResult UpdateAutorisationRessourcesHumaines(int id)
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.nom_prenom = user.nom + " " + user.prenom;
            ViewBag.email = user.Email;
            Autorisation autorisation = db.Autorisations.Find(id);
            return View(autorisation);
        }


        [Authorize(Roles = "Admin_Medicis,RH_Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateAutorisationRessourcesHumaines(Autorisation autorisation)
        {
            string error = "";

            try
            {
                ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
                Autorisation a = db.Autorisations.FirstOrDefault(x => x.Id == autorisation.Id);
                a.commentaire_superieur_hiearchique = autorisation.commentaire_rh;
                a.acceptation_ressource = autorisation.acceptation_ressource;
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
            // return View("Error");
        }
        [Authorize(Roles = "Admin_Medicis,RH_Admin,RH_Directeur,RH_Superieur_Hiéarchique")]
        public ActionResult AttribuerAutorisation()
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

            AutorisationViewModels AutVM = new AutorisationViewModels();
            AutVM.employers = employers;

            return View(AutVM);
        }


        [Authorize(Roles = "Admin_Medicis,RH_Admin,RH_Directeur,RH_Superieur_Hiéarchique")]
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<ActionResult> AttribuerAutorisation(AutorisationViewModels AutVM)
        {
            string error = "";
            if (ModelState.IsValid)
            {
                    try
                    {
                        ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
                        ApplicationUser appuser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindByName(AutVM.nom_prenom.Substring(AutVM.nom_prenom.Length - 4, AutVM.nom_prenom.Length));

                        //    return RedirectToAction(appuser.matricule);
                        Autorisation aut = new Autorisation();
                    aut.UserId = appuser.Id;
                    aut.UserName = appuser.UserName;
                    aut.matricule = appuser.matricule;
                    aut.Fonction = appuser.fonction;
                    aut.service = appuser.service;
                    aut.site = appuser.site;
                    aut.superieur_hierarchique = appuser.signataire;
                    aut.Date_emission_demande = System.DateTime.Now;
                    aut.jour_autorisation = AutVM.jour_autorisation;                  
                    aut.heure_sortie = AutVM.heure_sortie;
                    aut.heure_entree = AutVM.heure_entree;
                    aut.acceptation_superieur = acceptation_superieur_hierarchique.Demande_validée;
                    aut.acceptation_ressource = acceptation_ressource_humaine.Demande_en_cours;
                    aut.nom_prenom = appuser.nom + " " + appuser.prenom;
                    aut.Approbateur_RH = "";
                    aut.Date_validation_superieur = System.DateTime.Now;
                    
                    db.Autorisations.Add(aut);
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
            else
            {
                return View("Error");
            }
        }


        public ViewResult UpdateAutorisationByUser(int id)
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.nom_prenom = user.nom + " " + user.prenom;
            ViewBag.email = user.Email;
            Autorisation autorisation = db.Autorisations.Find(id);
            return View(autorisation);


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateAutorisationByUser(Autorisation autorisation)
        {

            if (autorisation.acceptation_superieur != acceptation_superieur_hierarchique.Demande_en_cours)
            {
                return RedirectToAction("Error");
            }
            if (ModelState.IsValid)
            {
                ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
                Autorisation aut = db.Autorisations.FirstOrDefault(x => x.Id == autorisation.Id);
                aut.jour_autorisation = autorisation.jour_autorisation;
 
                aut.heure_entree = autorisation.heure_entree;
                aut.heure_sortie = aut.heure_sortie;
                db.SaveChanges();
                return View("OK");
            }
            else
            {
                return View("Error");
            }

        }

        public ViewResult DeleteAutorisationByUser(int id)
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.nom_prenom = user.nom + " " + user.prenom;
            ViewBag.email = user.Email;
            Autorisation aut = db.Autorisations.Find(id);
            return View(aut);
        }
        [HttpPost]
        public ActionResult DeleteAutorisationByUser(Autorisation autorisation)
        {
            Autorisation a = db.Autorisations.Find(autorisation.Id);
            db.Autorisations.Remove(a);
            db.SaveChanges();
            return View("OK");

        }


    }
}