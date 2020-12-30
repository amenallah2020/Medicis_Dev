using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using RHEVENT.Models;
using static RHEVENT.Models.Enumeration;



namespace RHEVENT.Controllers
{
    public class DA_DemandeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        UserManager<IdentityUser> userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>());
        

        public ActionResult aaaa()
        {
            return View();
        }
        // GET: DA_Demande
        public ActionResult DemandesAll()
        {
            return View(db.DA_Demande.ToList());
        }

        
        public ActionResult MesDemandes()
        {
            return View(db.DA_Demande.ToList());
        }

       

        public ActionResult DemendesEnAttente()
        {
            return View(db.DA_Demande.ToList());
        }

        // GET: DA_Demande/Details/5 
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DA_Demande dA_Demande = db.DA_Demande.Find(id);
            if (dA_Demande == null)
            {
                return HttpNotFound();
            }
            return View(dA_Demande);
        }

        // GET: DA_Demande/Create
        public ActionResult Create()
        {
            //var BOList = db
            //         .DA_ListesGammes
            //         .ToList()
            //         .Select(d => new SelectListItem
            //         {
            //             //Value = d.Id.ToString(),
            //             Value = d.Gamme,
            //             Text = d.Gamme
            //         });
            //ViewBag.BOData = new SelectList(BOList, "Value", "Text");


            //List<DA_ListesGammes> listgamme = db.DA_ListesGammes.ToList();

            //ViewBag.listgamme = listgamme;
           

            DA_Demande dem = new DA_Demande();

            dem.listesGammes=db.DA_ListesGammes.ToList<DA_ListesGammes>();
            dem.Date_demande = DateTime.Now;
            ViewData.Model = dem;
            return View(dem);
        }
        
        // POST: DA_Demande/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DA_Demande dA_Demande)
        {
            string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection con = new SqlConnection(constr);
            con.Open();
            SqlDataAdapter da = new SqlDataAdapter("SELECT count(*) FROM DA_Demande", con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            int a = Convert.ToUInt16(dt.Rows[0][0]);
            int p = a + 1;
            string increment = "";
            if (p <= 9) increment = "0" + p;
            else increment = p.ToString();


            if (ModelState.IsValid)
            {
                string gammee= dA_Demande.Cibles;
                string dayy = DateTime.Now.Day.ToString();
                string monthh = DateTime.Now.Month.ToString();
                string yearr = DateTime.Now.Year.ToString();
                if (dayy.Length == 1) { dayy = "0" + dayy; }
                if (monthh.Length == 1) { monthh = "0" + monthh; }
                ApplicationUser user = db.Users.Find(User.Identity.GetUserId());

                dA_Demande.Réference = "DA-" + yearr + "-" + monthh + "-" + dayy + "-" + increment;
                dA_Demande.Demandeur = user.nom + " " + user.prenom;
                dA_Demande.Objet = dA_Demande.Objet;
                dA_Demande.Cibles = dA_Demande.Cibles;
                dA_Demande.Gamme = gammee;
                dA_Demande.Argumentaires = dA_Demande.Argumentaires;
                dA_Demande.Budget = dA_Demande.Budget;
                dA_Demande.Date_demande = dA_Demande.Date_demande;
                dA_Demande.Date_reception = dA_Demande.Date_reception;
                dA_Demande.Date_action = dA_Demande.Date_action;
                dA_Demande.Etat = "En attente";

                db.DA_Demande.Add(dA_Demande);
                db.SaveChanges();
                return RedirectToAction("MesDemandes");
            }

            return View(dA_Demande);
        }

        // GET: DA_Demande/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DA_Demande dA_Demande = db.DA_Demande.Find(id);
            if (dA_Demande == null)
            {
                return HttpNotFound();
            }
            return View(dA_Demande);
        }

        // POST: DA_Demande/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Réference,Demandeur,Gamme,Objet,Cibles,Argumentaires,Budget,Date_demande,Date_reception,Date_action")] DA_Demande dA_Demande)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dA_Demande).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("MesDemandes");
            }
            return View(dA_Demande);
        }

        // GET: DA_Demande/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DA_Demande dA_Demande = db.DA_Demande.Find(id);
            if (dA_Demande == null)
            {
                return HttpNotFound();
            }
            return View(dA_Demande);
        }

        // POST: DA_Demande/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DA_Demande dA_Demande = db.DA_Demande.Find(id);
            db.DA_Demande.Remove(dA_Demande);
            db.SaveChanges();
            return RedirectToAction("MesDemandes");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
