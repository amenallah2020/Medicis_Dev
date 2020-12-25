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
    public class Calendrier_DirectionController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Calendrier_Direction
        public ActionResult Index()
        {
            return View(db.Calendrier_Directions.ToList());
        }

        // GET: Calendrier_Direction/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Calendrier_Direction calendrier_Direction = db.Calendrier_Directions.Find(id);
            if (calendrier_Direction == null)
            {
                return HttpNotFound();
            }
            return View(calendrier_Direction);
        }

        // GET: Calendrier_Direction/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Calendrier_Direction/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,matricule,nom_prenom,service")] Calendrier_Direction calendrier_Direction)
        {
            if (ModelState.IsValid)
            {
                db.Calendrier_Directions.Add(calendrier_Direction);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(calendrier_Direction);
        }

        // GET: Calendrier_Direction/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Calendrier_Direction calendrier_Direction = db.Calendrier_Directions.Find(id);
            if (calendrier_Direction == null)
            {
                return HttpNotFound();
            }
            return View(calendrier_Direction);
        }

        // POST: Calendrier_Direction/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,matricule,nom_prenom,service")] Calendrier_Direction calendrier_Direction)
        {
            if (ModelState.IsValid)
            {
                db.Entry(calendrier_Direction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(calendrier_Direction);
        }

        // GET: Calendrier_Direction/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Calendrier_Direction calendrier_Direction = db.Calendrier_Directions.Find(id);
            if (calendrier_Direction == null)
            {
                return HttpNotFound();
            }
            return View(calendrier_Direction);
        }

        // POST: Calendrier_Direction/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Calendrier_Direction calendrier_Direction = db.Calendrier_Directions.Find(id);
            db.Calendrier_Directions.Remove(calendrier_Direction);
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
