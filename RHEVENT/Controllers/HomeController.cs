using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using RHEVENT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RHEVENT.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        UserManager<IdentityUser> userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>());
        [Authorize]
      
        public ActionResult Index()
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.nom_prenom = user.nom + " " + user.prenom;
            ViewBag.email = user.Email;

            string demandeurr = user.nom + " " + user.prenom;
            Session["userconnecté"] = demandeurr;

            Session["auth"] = "authenticated";

            if (user.EmailConfirmed == false )
            {
                return RedirectToAction("ChangePasswordByUser","ApplicationUsers");
            }
            else {

                return View(db.Images.ToList().Select(img => img).OrderByDescending(img => img.date_upload));


            }

            
        }
    
        [Authorize]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [Authorize]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}