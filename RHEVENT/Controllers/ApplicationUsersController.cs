using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RHEVENT.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using System.Data.Entity.Validation;
using System.Collections.Generic;
using System.Web.Security;
using RHEVENT.ViewModels;

namespace RHEVENT.Controllers
{
    public class ApplicationUsersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationSignInManager _signInManager;
       private ApplicationUserManager _userManager;

        public  IdentityResult result ;

        //private ApplicationUserManager _userManagerrr;


        private ApplicationRoleManager _roleManager;

        UserManager<IdentityUser> userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>());


        UserManager<IdentityUser> userManager2 = new UserManager<IdentityUser>(new UserStore<IdentityUser>());


        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        //public ApplicationUserManager UserManagerr
        //{
        //    get
        //    {
        //        return _userManagerrr ?? HttpContext.GetOwinContext().Get<ApplicationUserManager>();
        //    }
        //    private set
        //    {
        //        _userManagerrr = value;
        //    }
        //}

        // GET: ApplicationUsers

        //[Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
           /* ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.nom_prenom = user.nom + " " + user.prenom;
            ViewBag.email = user.Email;*/
            return View(db.Users.ToList());
 
        }

        // GET: ApplicationUsers/Details/5
        //[Authorize(Roles = "Admin")]
        public ActionResult Details(string id)
        {
           // ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
          //  ViewBag.nom_prenom = user.nom + " " + user.prenom;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = db.Users.Find(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            return View(applicationUser);
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
        // GET: ApplicationUsers/Create
      // [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
         //   ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
         //   ViewBag.nom_prenom = user.nom + " " + user.prenom;
            List<SelectListItem> list = new List<SelectListItem>();
            var signataires = db.Users.ToList().Select(x => new SelectListItem
            {
                Value = x.UserName,
                Text = x.nom + " " + x.prenom + "  " + x.service
            }).Distinct().OrderBy(sign=>sign.Text);

            List<SelectListItem> list11 = new List<SelectListItem>();
            var fonctions = db.FonctionsUsers.ToList().Select(x => new SelectListItem
            {
                Value = x.Fonction,
                Text = x.Fonction
            }).Distinct().OrderBy(sign => sign.Text);

            /* if (signataires.Count() == 0)
             { signataires.Add(new SelectListItem() { Text = "  ", Value = " " }); }*/
            RegisterViewModel app = new RegisterViewModel();
            app.signataires = signataires;
            app.fonctions = fonctions;
            if (signataires.Count() == 0)
            {
                List<SelectListItem> listevide = new List<SelectListItem>();
                listevide.Add(new SelectListItem { Text = "admin", Value = "admin" });
                app.signataires = listevide;
                return View(app);
            }
            if (fonctions.Count() == 0)
            {
                List<SelectListItem> listevide1 = new List<SelectListItem>();
                listevide1.Add(new SelectListItem { Text = "Admin", Value = "Admin" });
                app.fonctions = listevide1;
                return View(app);
            }
            return View(app);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
      //  [Authorize(Roles="Admin")]
        public async Task<ActionResult> Create(RegisterViewModel model, string EtatUsr , string StatutUsr)
        {
            string error = "";
            try
            {
                ApplicationDbContext context = new ApplicationDbContext();

                    string NomUsr = model.nom + " " + model.prenom;

                    var user = new ApplicationUser { UserName =  model.matricule.ToString() , NomPrenom = NomUsr ,  Statut = StatutUsr ,  Etat = EtatUsr , Email = model.Email, nom = model.nom, prenom = model.prenom, matricule = model.matricule, telephone = model.telephone, fonction = model.fonction, date_naissance = model.date_naissance, date_recrutement = model.date_recrutement, site = model.site, signataire = model.signataire , service = model.service,RoleName=model.RoleName }; 
              
                    //if (user.signataires == null) { user.signataire = "admin"; }

                    var result = await UserManager.CreateAsync(user, model.Password);

                    //if (result.Succeeded)
                    //{
                    //UserManager.AddToRole(user.Id,model.RoleName.ToString());
                    return RedirectToAction("Index", "ApplicationUsers");
                    //}
                    //else
                    //{
                    //// return RedirectToAction(result.Errors.ToString());
                    //return View("Error");
                    //}
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



            //     return View(model);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }





        // GET: ApplicationUsers/Edit/5
           //[Authorize(Roles = "Admin")]
        public ActionResult Edit(string id)
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.nom_prenom = user.nom + " " + user.prenom;
            ViewBag.email = user.Email;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = db.Users.Find(id);

            if (applicationUser == null)
            {
                return HttpNotFound();
            }

            var signataires = db.Users.ToList().Select(x => new SelectListItem
            {
                Value = x.UserName,
                Text = x.nom+" "+x.prenom+" "+x.service
            }).Distinct().OrderBy(sign => sign.Text);

            List<SelectListItem> list11 = new List<SelectListItem>();
            var fonctions = db.FonctionsUsers.ToList().Select(x => new SelectListItem
            {
                Value = x.Fonction,
                Text = x.Fonction
            }).Distinct().OrderBy(sign => sign.Text);


            applicationUser.signataires = signataires;
            applicationUser.fonctions = fonctions;

            ViewBag.list_signataires = signataires;
            return View(applicationUser);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin")]
        public ActionResult Edit(ApplicationUser applicationUser , string EtatUsr , string StatutUsr)
        {
            string error = "";
            if (ModelState.IsValid)
            {
                try
                {

                    //  Update Role for user
                    //IdentityResult delete = UserManager.RemoveFromRole(applicationUser.Id,  "Admin");
                    //IdentityResult delete2 = UserManager.RemoveFromRole(applicationUser.Id, "Chargé_RH");
                    //IdentityResult delete3 = UserManager.RemoveFromRole(applicationUser.Id, "Employer");
                    //IdentityResult delete4 = UserManager.RemoveFromRole(applicationUser.Id, "Superieur_Hiéarchique");
                    //IdentityResult delete5 = UserManager.RemoveFromRole(applicationUser.Id, "Directeur");
                    //IdentityResult delete6 = UserManager.RemoveFromRole(applicationUser.Id, "Chargé_personnel_RH");
                    //UserManager.AddToRole(applicationUser.Id, applicationUser.RoleName.ToString());

                    string NomUsr = applicationUser.nom + " " + applicationUser.prenom;


                    ApplicationUser appuser = db.Users.First(u => u.Id == applicationUser.Id);
                    appuser.Statut = StatutUsr;
                    appuser.NomPrenom = NomUsr;
                    appuser.Etat = EtatUsr;
                    appuser.UserName = applicationUser.matricule;
                    appuser.nom = applicationUser.nom;
                    appuser.prenom = applicationUser.prenom;
                    appuser.matricule = applicationUser.matricule;
                    appuser.telephone = applicationUser.telephone;
                    appuser.fonction = applicationUser.fonction;
                 // appuser.date_recrutement = applicationUser.date_recrutement;
                 // appuser.date_naissance = applicationUser.date_naissance;
                    appuser.Email = applicationUser.Email;
                    appuser.signataire = applicationUser.signataire;
                    appuser.service = applicationUser.service;
                    appuser.site = applicationUser.site;
                    appuser.RoleName = applicationUser.RoleName;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }catch(DbEntityValidationException e) {
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
        
            return View(applicationUser);
        }

        // GET: ApplicationUsers/Delete/5
        //[Authorize(Roles = "Admin")]
        public ActionResult Delete(string id)
        {
       //     ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
        //    ViewBag.nom_prenom = user.nom + " " + user.prenom;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = db.Users.Find(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            return View(applicationUser);
        }

        // POST: ApplicationUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
 
        public ActionResult DeleteConfirmed(string id)
        {
            ApplicationUser applicationUser = db.Users.Find(id);
            db.Users.Remove(applicationUser);
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
         //[Authorize(Roles = "Admin")]
        public ActionResult ChangePassword(string id)
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.nom_prenom = user.nom + " " + user.prenom;
            ViewBag.email = user.Email;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin")]
        public ActionResult ChangePassword(ChangePasswordViewModelbyAdmin model)
        {
            if (ModelState.IsValid)
            {
                UserManager<IdentityUser> userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>());
                userManager.RemovePassword(model.Id);
                userManager.AddPassword(model.Id, model.Password);
                return View("OK");
            }
            else
            {
                return View("~/Views/Shared/Error.cshtml");
            }
        }
     //   [Authorize]
        public ActionResult ChangePasswordByUser()
        {
            return View();
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public  ActionResult ChangePasswordByUser(ChangePasswordViewModelByUser cpuser)
        {

            if (ModelState.IsValid)
            {

                if (cpuser.Password == cpuser.ConfirmPassword)
                {

                    IdentityResult result = UserManager.ChangePassword(User.Identity.GetUserId(), cpuser.AncienPassword, cpuser.Password);
                    if (result.Succeeded)
                    {
                        ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
                        user.EmailConfirmed = true;
                        db.SaveChanges();
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        return RedirectToAction("erreur", "erruer");
                    }

                }
                else
                {
                    return RedirectToAction("erreur", "erruer");
                }
            }
            else
            {
                return View();
            }
        }
        private ApplicationUserManager __userManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }

       public ActionResult RoleIndex(string id)
        {


            IEnumerable<string> roles = UserManager.GetRoles(id);
            UserRolesViewModel uroles = new UserRolesViewModel(roles,id);           
            return View(uroles);
        }


        [HttpGet]
        public async Task <ActionResult> ManageUserRoles(string id)
        {

            ViewBag.userId = id;

            //ApplicationUser user = db.Users.Find(User.Identity.GetUserId());

            //ApplicationUser applicationUser = db.Users.Find(id);

            //var user = applicationUser;
            //ApplicationUser applicationUser = db.Users.Find(id);

            var user = await userManager.FindByIdAsync(id);

            //var user = applicationUser;

            //if (user == null)
            //{
            //    ViewBag.ErrorMessage = $"L'utilisateur avec Id = {id} est introuvable";

            //    return View("NotFound");
            //}

            var model = new List<RoleUsersViewModel>();

            foreach (var role in RoleManager.Roles)
            {
                var roleUsersViewModel = new RoleUsersViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name
                };

                //List<string> roles = userManager.GetRoles(user.Id).ToList();


                //foreach ( string a in roles)
                //{
                //    if (a == role.Name)
                //    {
                //        roleUsersViewModel.IsSelected = true;

                //    }
                //    else
                //    {
                //        roleUsersViewModel.IsSelected = false;
                //    }

                //}

                if (await userManager.IsInRoleAsync(user.Id, role.Name))
                { 
                    roleUsersViewModel.IsSelected = true;
                }
                else
                {
                    roleUsersViewModel.IsSelected = false;
                }

                model.Add(roleUsersViewModel);
            }

            return View(model);
        }

        [HttpPost]
        public async Task <ActionResult> ManageUserRoles(List<RoleUsersViewModel> model, string id)
        {
            var user = userManager.FindByIdAsync(id);

            //if (user == null)
            //{
            //    ViewBag.ErrorMessage = $"L'utilisateur avec Id = {userId} est introuvable";
            //    return View("NotFound");
            //}

            var IntervQuery = from m in model
                              where m.IsSelected
                              select m;

            //int a = IntervQuery.Count();

            //if (a > 1)
            //{
            //    ViewBag.ErrorMessage = $"Il faut choisir un seul rôle";
            //    return View("InterditRoles");

            //}

            var roles = await userManager2.GetRolesAsync(id);
           
            if (roles.Count != 0)
            {
                foreach (string r in roles)
                {

                    result = await userManager.RemoveFromRolesAsync(id, r);
                }
            
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", "Ne peut pas supprimer les rôles existants de l'utilisateur");
                    return View(model);
                }
            }

            var rt = from m in model
                    where m.IsSelected
                    select m;

            foreach (RoleUsersViewModel u in rt)
            { 
                result = await userManager.AddToRolesAsync(id, u.RoleName);

            }

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Impossible d'ajouter des rôles sélectionnés à l'utilisateur");
            }

            //, new { Id = userId }
            return RedirectToAction("Index");

        }

    }
}
