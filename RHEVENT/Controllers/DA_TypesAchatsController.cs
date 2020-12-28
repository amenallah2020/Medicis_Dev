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
    public class DA_TypesAchatsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: DA_TypesAchats
        public ActionResult Index()
        {
            return View(db.DA_TypesAchats.ToList());
        }

        // GET: DA_TypesAchats/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DA_TypesAchats dA_TypesAchats = db.DA_TypesAchats.Find(id);
            if (dA_TypesAchats == null)
            {
                return HttpNotFound();
            }
            return View(dA_TypesAchats);
        }

        // GET: DA_TypesAchats/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DA_TypesAchats/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,TypeAchat")] DA_TypesAchats dA_TypesAchats)
        {
            if (ModelState.IsValid)
            {
                db.DA_TypesAchats.Add(dA_TypesAchats);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(dA_TypesAchats);
        }

        // GET: DA_TypesAchats/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DA_TypesAchats dA_TypesAchats = db.DA_TypesAchats.Find(id);
            if (dA_TypesAchats == null)
            {
                return HttpNotFound();
            }
            return View(dA_TypesAchats);
        }

        // POST: DA_TypesAchats/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,TypeAchat")] DA_TypesAchats dA_TypesAchats)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dA_TypesAchats).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(dA_TypesAchats);
        }

        // GET: DA_TypesAchats/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DA_TypesAchats dA_TypesAchats = db.DA_TypesAchats.Find(id);
            if (dA_TypesAchats == null)
            {
                return HttpNotFound();
            }
            return View(dA_TypesAchats);
        }

        // POST: DA_TypesAchats/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DA_TypesAchats dA_TypesAchats = db.DA_TypesAchats.Find(id);
            db.DA_TypesAchats.Remove(dA_TypesAchats);
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
