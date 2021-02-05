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
    public class DA_ProduitsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: DA_Produits
        public ActionResult Index()
        {
            return View(db.DA_Produits.ToList());
        }

        // GET: DA_Produits/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DA_Produits dA_Produits = db.DA_Produits.Find(id);
            if (dA_Produits == null)
            {
                return HttpNotFound();
            }
            return View(dA_Produits);
        }

        // GET: DA_Produits/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DA_Produits/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Code,Désignation")] DA_Produits dA_Produits)
        {
            if (ModelState.IsValid)
            {
                db.DA_Produits.Add(dA_Produits);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(dA_Produits);
        }

        // GET: DA_Produits/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DA_Produits dA_Produits = db.DA_Produits.Find(id);
            if (dA_Produits == null)
            {
                return HttpNotFound();
            }
            return View(dA_Produits);
        }

        // POST: DA_Produits/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Code,Désignation")] DA_Produits dA_Produits)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dA_Produits).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(dA_Produits);
        }

        // GET: DA_Produits/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DA_Produits dA_Produits = db.DA_Produits.Find(id);
            if (dA_Produits == null)
            {
                return HttpNotFound();
            }
            return View(dA_Produits);
        }

        // POST: DA_Produits/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DA_Produits dA_Produits = db.DA_Produits.Find(id);
            db.DA_Produits.Remove(dA_Produits);
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
