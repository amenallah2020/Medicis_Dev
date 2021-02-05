using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RHEVENT.Models;
using RHEVENT.Models.RH;

namespace RHEVENT.Controllers.RH
{
    public class FonctionsUsersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: FonctionsUsers
        public ActionResult Index()
        {
            return View(db.FonctionsUsers.ToList());
        }

        // GET: FonctionsUsers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FonctionsUsers fonctionsUsers = db.FonctionsUsers.Find(id);
            if (fonctionsUsers == null)
            {
                return HttpNotFound();
            }
            return View(fonctionsUsers);
        }

        // GET: FonctionsUsers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: FonctionsUsers/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Fonction")] FonctionsUsers fonctionsUsers)
        {
            if (ModelState.IsValid)
            {
                db.FonctionsUsers.Add(fonctionsUsers);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(fonctionsUsers);
        }

        // GET: FonctionsUsers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FonctionsUsers fonctionsUsers = db.FonctionsUsers.Find(id);
            if (fonctionsUsers == null)
            {
                return HttpNotFound();
            }
            return View(fonctionsUsers);
        }

        // POST: FonctionsUsers/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Fonction")] FonctionsUsers fonctionsUsers)
        {
            if (ModelState.IsValid)
            {
                db.Entry(fonctionsUsers).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(fonctionsUsers);
        }

        // GET: FonctionsUsers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FonctionsUsers fonctionsUsers = db.FonctionsUsers.Find(id);
            if (fonctionsUsers == null)
            {
                return HttpNotFound();
            }
            return View(fonctionsUsers);
        }

        // POST: FonctionsUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            FonctionsUsers fonctionsUsers = db.FonctionsUsers.Find(id);
            db.FonctionsUsers.Remove(fonctionsUsers);
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
