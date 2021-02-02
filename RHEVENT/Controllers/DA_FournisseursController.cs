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
    public class DA_FournisseursController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: DA_Fournisseurs
        public ActionResult Index()
        {
            return View(db.DA_Fournisseurs.ToList());
        }

        // GET: DA_Fournisseurs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DA_Fournisseurs dA_Fournisseurs = db.DA_Fournisseurs.Find(id);
            if (dA_Fournisseurs == null)
            {
                return HttpNotFound();
            }
            return View(dA_Fournisseurs);
        }

        // GET: DA_Fournisseurs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DA_Fournisseurs/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Code,Raison,Adresse,Tel,Mobile")] DA_Fournisseurs dA_Fournisseurs)
        {
            if (ModelState.IsValid)
            {
                db.DA_Fournisseurs.Add(dA_Fournisseurs);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(dA_Fournisseurs);
        }

        // GET: DA_Fournisseurs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DA_Fournisseurs dA_Fournisseurs = db.DA_Fournisseurs.Find(id);
            if (dA_Fournisseurs == null)
            {
                return HttpNotFound();
            }
            return View(dA_Fournisseurs);
        }

        // POST: DA_Fournisseurs/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Code,Raison,Adresse,Tel,Mobile")] DA_Fournisseurs dA_Fournisseurs)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dA_Fournisseurs).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(dA_Fournisseurs);
        }

        // GET: DA_Fournisseurs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DA_Fournisseurs dA_Fournisseurs = db.DA_Fournisseurs.Find(id);
            if (dA_Fournisseurs == null)
            {
                return HttpNotFound();
            }
            return View(dA_Fournisseurs);
        }

        // POST: DA_Fournisseurs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DA_Fournisseurs dA_Fournisseurs = db.DA_Fournisseurs.Find(id);
            db.DA_Fournisseurs.Remove(dA_Fournisseurs);
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
