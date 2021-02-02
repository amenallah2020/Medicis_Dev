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
    public class DA_MaterielsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: DA_Materiels
        public ActionResult Index()
        {
            return View(db.DA_Materiels.ToList());
        }

        // GET: DA_Materiels/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DA_Materiels dA_Materiels = db.DA_Materiels.Find(id);
            if (dA_Materiels == null)
            {
                return HttpNotFound();
            }
            return View(dA_Materiels);
        }

        // GET: DA_Materiels/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DA_Materiels/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Code,Désignation,Version")] DA_Materiels dA_Materiels)
        {
            if (ModelState.IsValid)
            {
                db.DA_Materiels.Add(dA_Materiels);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(dA_Materiels);
        }

        // GET: DA_Materiels/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DA_Materiels dA_Materiels = db.DA_Materiels.Find(id);
            if (dA_Materiels == null)
            {
                return HttpNotFound();
            }
            return View(dA_Materiels);
        }

        // POST: DA_Materiels/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Code,Désignation,Version")] DA_Materiels dA_Materiels)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dA_Materiels).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(dA_Materiels);
        }

        // GET: DA_Materiels/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DA_Materiels dA_Materiels = db.DA_Materiels.Find(id);
            if (dA_Materiels == null)
            {
                return HttpNotFound();
            }
            return View(dA_Materiels);
        }

        // POST: DA_Materiels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DA_Materiels dA_Materiels = db.DA_Materiels.Find(id);
            db.DA_Materiels.Remove(dA_Materiels);
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
