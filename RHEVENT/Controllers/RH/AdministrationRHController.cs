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
    public class AdministrationRHController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        UserManager<IdentityUser> userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>());
        // GET: AdministrationRH
        [Authorize(Roles = "Admin_Medicis,RH_Admin")]
        public ActionResult Index()
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.nom_prenom = user.nom + " " + user.prenom;
            ViewBag.email = user.Email;
            return View();
        }

        public ActionResult SupervisionRH()
        {
            return View();
        }

    }
}