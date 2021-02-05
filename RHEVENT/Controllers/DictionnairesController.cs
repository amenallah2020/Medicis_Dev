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
    public class DictionnairesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Dictionnaires
        public ActionResult Index()
        {
            return View(db.Dictionnaires.ToList());
        }

        // GET: Dictionnaires/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dictionnaire dictionnaire = db.Dictionnaires.Find(id);
            if (dictionnaire == null)
            {
                return HttpNotFound();
            }
            return View(dictionnaire);
        }

        // GET: Dictionnaires/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Dictionnaires/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,table,champ,valeur,signification")] Dictionnaire dictionnaire)
        {
            if (ModelState.IsValid)
            {
                db.Dictionnaires.Add(dictionnaire);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(dictionnaire);
        }

        // GET: Dictionnaires/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dictionnaire dictionnaire = db.Dictionnaires.Find(id);
            if (dictionnaire == null)
            {
                return HttpNotFound();
            }
            return View(dictionnaire);
        }

        // POST: Dictionnaires/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,table,champ,valeur,signification")] Dictionnaire dictionnaire)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dictionnaire).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(dictionnaire);
        }

        // GET: Dictionnaires/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dictionnaire dictionnaire = db.Dictionnaires.Find(id);
            if (dictionnaire == null)
            {
                return HttpNotFound();
            }
            return View(dictionnaire);
        }

        // POST: Dictionnaires/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Dictionnaire dictionnaire = db.Dictionnaires.Find(id);
            db.Dictionnaires.Remove(dictionnaire);
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
