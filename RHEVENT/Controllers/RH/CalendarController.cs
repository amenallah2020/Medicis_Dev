using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Reflection;
using RHEVENT.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System.Web.Script.Serialization;
using System.Globalization;
using Microsoft.AspNet.Identity.Owin;
using static RHEVENT.Models.Enumeration;

namespace RHEVENT.Controllers
{
    public class CalendarController : Controller
    {
        // GET: Calendar
        private ApplicationDbContext db = new ApplicationDbContext();
        UserManager<IdentityUser> userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>());
        [Authorize]
        public ActionResult Index()
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.nom_prenom = user.nom + " " + user.prenom;
            ViewBag.email = user.Email;
            ApplicationUser applicationUser = db.Users.Find(User.Identity.GetUserId());
            //Delete file if exist 
            string fullpath = Request.MapPath("~/Files/" + applicationUser.service.ToString() + ".json");
            if (System.IO.File.Exists(fullpath))
            {
                System.IO.File.Delete(fullpath);
            }

            List<calendar_event> mylist = new List<calendar_event>();


            foreach (Conge c in db.Conges.ToList().Select(cong=>cong).Where((cong=>cong.service == applicationUser.service)).Select(cong=>cong).Where(cong=>cong.acceptation_superieur == acceptation_superieur_hierarchique.Demande_validée))
            {
                calendar_event ce = new calendar_event();
                DateTime dt1 = new DateTime(c.jour_debut.Value.Year, c.jour_debut.Value.Month, c.jour_debut.Value.Day,Int32.Parse(c.heure_sortie.Substring(0,2)), Int32.Parse(c.heure_sortie.Substring(3, 2)), c.jour_debut.Value.Second, c.jour_debut.Value.Millisecond, DateTimeKind.Utc);
                string s1 = dt1.ToString("o", CultureInfo.InvariantCulture);
                DateTime dt2 = new DateTime(c.jour_fin.Value.Year, c.jour_fin.Value.Month, c.jour_fin.Value.Day, Int32.Parse(c.heure_entree.Substring(0, 2)), Int32.Parse(c.heure_entree.Substring(3, 2)), c.jour_fin.Value.Second, c.jour_fin.Value.Millisecond, DateTimeKind.Utc);
                string s2 = dt2.ToString("o", CultureInfo.InvariantCulture);
                ce.title ="Congé " + c.nom_prenom ;
                ce.start = s1;
                ce.end = s2;
                ce.color = "green";
                ce.textColor = "white";
                mylist.Add(ce);
            }
            foreach (Autorisation a in db.Autorisations.ToList().Select(aut => aut).Where(aut => aut.service == applicationUser.service).Select(aut=>aut).Where(aut=>aut.acceptation_superieur == acceptation_superieur_hierarchique.Demande_validée))
            {
                calendar_event ce = new calendar_event();

                DateTime dt1 = new DateTime(a.jour_autorisation.Value.Year, a.jour_autorisation.Value.Month, a.jour_autorisation.Value.Day, Int32.Parse(a.heure_sortie.Substring(0, 2)), Int32.Parse(a.heure_sortie.Substring(3, 2)), a.jour_autorisation.Value.Second, a.jour_autorisation.Value.Millisecond, DateTimeKind.Utc);
                string s1 = dt1.ToString("o", CultureInfo.InvariantCulture);

                DateTime dt2 = new DateTime(a.jour_autorisation.Value.Year, a.jour_autorisation.Value.Month, a.jour_autorisation.Value.Day, Int32.Parse(a.heure_entree.Substring(0, 2)), Int32.Parse(a.heure_entree.Substring(3, 2)), a.jour_autorisation.Value.Second, a.jour_autorisation.Value.Millisecond, DateTimeKind.Utc);
                string s2 = dt2.ToString("o", CultureInfo.InvariantCulture);

                ce.title = "Autorisation " + a.nom_prenom;
                ce.start = s1;
                ce.end = s2;
                ce.color = "blue";
                ce.textColor = "white";
                mylist.Add(ce);
            }
            string jsondata = new JavaScriptSerializer().Serialize(mylist);
            string path = Server.MapPath("~/Files/");
            System.IO.File.WriteAllText(path + applicationUser.service.ToString() + ".json", jsondata);

            ViewData["service"] = applicationUser.service;
            return View();
        }
        private ApplicationUserManager _userManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }
        [Authorize(Roles = "Admin_Medicis,RH_Admin,RH_Directeur,RH_Superieur_Hiéarchique")]
        public ActionResult CalendrierDirection()
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.nom_prenom = user.nom + " " + user.prenom;
            ViewBag.email = user.Email;
            ViewBag.Role = _userManager.GetRoles(user.Id).FirstOrDefault();
            Calendrier_Direction cd = new Calendrier_Direction();
            List<SelectListItem> list = new List<SelectListItem>();
            var list_serivces = db.Calendrier_Directions.ToList().Where(m => m.matricule == user.matricule).Select(x => new SelectListItem
            {
                Value = x.service.ToString(),
                Text = x.service.ToString()
            });

            cd.list_service = list_serivces;
            return View(cd);
        }

        [Authorize(Roles = "Admin_Medicis,RH_Admin,RH_Directeur,RH_Superieur_Hiéarchique")]
        [HttpPost]
        public ActionResult CalendrierDirection(string service)
        {
            return RedirectToAction("GetCalendar_by_Service","calendar",new {  id = service });
         
            return RedirectToAction("GetCalendar_by_Service","Calendar",service); 
        }
        [Authorize]
        public ActionResult GetCalendar_by_Service(string id)
        {
  
          ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.nom_prenom = user.nom + " " + user.prenom;
            ViewBag.email = user.Email;

 
            string fullpath = Request.MapPath("~/Files/" + id + ".json");
            if (System.IO.File.Exists(fullpath))
            {
                System.IO.File.Delete(fullpath);
            }

            List<calendar_event> mylist = new List<calendar_event>();


            foreach (Conge c in db.Conges.ToList().Select(cong => cong).Where((cong => cong.service == id)).Select(cong => cong).Where(cong => cong.acceptation_superieur == acceptation_superieur_hierarchique.Demande_validée))
            {
                calendar_event ce = new calendar_event();
                DateTime dt1 = new DateTime(c.jour_debut.Value.Year, c.jour_debut.Value.Month, c.jour_debut.Value.Day, Int32.Parse(c.heure_sortie.Substring(0, 2)), Int32.Parse(c.heure_sortie.Substring(3, 2)), c.jour_debut.Value.Second, c.jour_debut.Value.Millisecond, DateTimeKind.Utc);
                string s1 = dt1.ToString("o", CultureInfo.InvariantCulture);
                DateTime dt2 = new DateTime(c.jour_fin.Value.Year, c.jour_fin.Value.Month, c.jour_fin.Value.Day, Int32.Parse(c.heure_entree.Substring(0, 2)), Int32.Parse(c.heure_entree.Substring(3, 2)), c.jour_fin.Value.Second, c.jour_fin.Value.Millisecond, DateTimeKind.Utc);
                string s2 = dt2.ToString("o", CultureInfo.InvariantCulture);
                ce.title = "Congé " + c.nom_prenom;
                ce.start = s1;
                ce.end = s2;
                ce.color = "green";
                ce.textColor = "white";
                mylist.Add(ce);
            }
            foreach (Autorisation a in db.Autorisations.ToList().Select(aut => aut).Where(aut => aut.service == id).Select(aut => aut).Where(aut => aut.acceptation_superieur == acceptation_superieur_hierarchique.Demande_validée))
            {
                    calendar_event ce = new calendar_event();
                    DateTime dt1 = new DateTime(a.jour_autorisation.Value.Year,
                    a.jour_autorisation.Value.Month, a.jour_autorisation.Value.Day,
                    Int32.Parse(a.heure_sortie.Substring(0, 2)),
                    Int32.Parse(a.heure_sortie.Substring(3, 2)),
                    a.jour_autorisation.Value.Second, a.jour_autorisation.Value.Millisecond, DateTimeKind.Utc);

                string s1 = dt1.ToString("o", CultureInfo.InvariantCulture);

                DateTime dt2 = new DateTime(a.jour_autorisation.Value.Year, a.jour_autorisation.Value.Month, a.jour_autorisation.Value.Day, Int32.Parse(a.heure_entree.Substring(0, 2)), Int32.Parse(a.heure_entree.Substring(3, 2)), a.jour_autorisation.Value.Second, a.jour_autorisation.Value.Millisecond, DateTimeKind.Utc);
                string s2 = dt2.ToString("o", CultureInfo.InvariantCulture);

                ce.title = "Autorisation" + a.nom_prenom;
                ce.start = s1;
                ce.end = s2;
                ce.color = "blue";
                ce.textColor = "white";
                mylist.Add(ce);
            }
            string jsondata = new JavaScriptSerializer().Serialize(mylist);
            string path = Server.MapPath("~/Files/");
            System.IO.File.WriteAllText(path + id + ".json", jsondata);
            ViewData["service"] = id;
            return View();


   
        }
        [Authorize(Roles = "Admin_Medicis")]
        public ActionResult Calendar_Rights()
        {
            return View(db.Calendrier_Directions.ToList().OrderByDescending(x=>x.matricule));

        }
        
    }
    public class calendar_event
    {
        public string title     { get; set;}
        public string start     { get; set;}
        public string end       { get; set;}
        public string color     { get; set;}
        public string textColor { get; set;}
    }
  
}