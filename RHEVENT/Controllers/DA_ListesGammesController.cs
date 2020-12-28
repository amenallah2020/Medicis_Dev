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
    public class DA_ListesGammesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: DA_ListesGammes
        public ActionResult Index()
        {
            return View(db.DA_ListesGammes.ToList());
        }

        // GET: DA_ListesGammes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DA_ListesGammes dA_ListesGammes = db.DA_ListesGammes.Find(id);
            if (dA_ListesGammes == null)
            {
                return HttpNotFound();
            }
            return View(dA_ListesGammes);
        }

        // GET: DA_ListesGammes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DA_ListesGammes/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Gamme")] DA_ListesGammes dA_ListesGammes)
        {
            if (ModelState.IsValid)
            {
                db.DA_ListesGammes.Add(dA_ListesGammes);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(dA_ListesGammes);
        }

        // GET: DA_ListesGammes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DA_ListesGammes dA_ListesGammes = db.DA_ListesGammes.Find(id);
            if (dA_ListesGammes == null)
            {
                return HttpNotFound();
            }
            return View(dA_ListesGammes);
        }

        // POST: DA_ListesGammes/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Gamme")] DA_ListesGammes dA_ListesGammes)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dA_ListesGammes).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(dA_ListesGammes);
        }

        // GET: DA_ListesGammes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DA_ListesGammes dA_ListesGammes = db.DA_ListesGammes.Find(id);
            if (dA_ListesGammes == null)
            {
                return HttpNotFound();
            }
            return View(dA_ListesGammes);
        }

        // POST: DA_ListesGammes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DA_ListesGammes dA_ListesGammes = db.DA_ListesGammes.Find(id);
            db.DA_ListesGammes.Remove(dA_ListesGammes);
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
