using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RHEVENT.Models;

namespace RHEVENT.Controllers
{
    public class MesDemAjoutsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: MesDemAjouts
        public ActionResult Index()
        {
            return View(db.MesDemAjouts.ToList());
        }

        // GET: MesDemAjouts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MesDemAjout mesDemAjout = db.MesDemAjouts.Find(id);
            if (mesDemAjout == null)
            {
                return HttpNotFound();
            }
            return View(mesDemAjout);
        }

             



        // GET: MesDemAjouts/Create
        public ActionResult Create()
        {
            MesDemAjout dem = new MesDemAjout();

           
            return View(dem);
        }

        // POST: MesDemAjouts/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Réference,Demandeur,Gamme,Objet,Cibles,Argumentaires,Budget,Date_demande,Date_reception,Date_action,Etat")] MesDemAjout mesDemAjout)
        {
            if (ModelState.IsValid)
            {
                db.MesDemAjouts.Add(mesDemAjout);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(mesDemAjout);
        }

        // GET: MesDemAjouts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MesDemAjout mesDemAjout = db.MesDemAjouts.Find(id);
            if (mesDemAjout == null)
            {
                return HttpNotFound();
            }
            return View(mesDemAjout);
        }

        // POST: MesDemAjouts/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Réference,Demandeur,Gamme,Objet,Cibles,Argumentaires,Budget,Date_demande,Date_reception,Date_action,Etat")] MesDemAjout mesDemAjout)
        {
            if (ModelState.IsValid)
            {
                db.Entry(mesDemAjout).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(mesDemAjout);
        }

        // GET: MesDemAjouts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MesDemAjout mesDemAjout = db.MesDemAjouts.Find(id);
            if (mesDemAjout == null)
            {
                return HttpNotFound();
            }
            return View(mesDemAjout);
        }

        // POST: MesDemAjouts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MesDemAjout mesDemAjout = db.MesDemAjouts.Find(id);
            db.MesDemAjouts.Remove(mesDemAjout);
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
    }
}
