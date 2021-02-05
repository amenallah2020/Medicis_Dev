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
    public class DA_CodesArticlesSageController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: DA_CodesArticlesSage
        public ActionResult Index()
        {
            return View(db.DA_CodesArticlesSage.ToList());
        }

        // GET: DA_CodesArticlesSage/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DA_CodesArticlesSage dA_CodesArticlesSage = db.DA_CodesArticlesSage.Find(id);
            if (dA_CodesArticlesSage == null)
            {
                return HttpNotFound();
            }
            return View(dA_CodesArticlesSage);
        }

        // GET: DA_CodesArticlesSage/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DA_CodesArticlesSage/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Code")] DA_CodesArticlesSage dA_CodesArticlesSage)
        {
            if (ModelState.IsValid)
            {
                db.DA_CodesArticlesSage.Add(dA_CodesArticlesSage);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(dA_CodesArticlesSage);
        }

        // GET: DA_CodesArticlesSage/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DA_CodesArticlesSage dA_CodesArticlesSage = db.DA_CodesArticlesSage.Find(id);
            if (dA_CodesArticlesSage == null)
            {
                return HttpNotFound();
            }
            return View(dA_CodesArticlesSage);
        }

        // POST: DA_CodesArticlesSage/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Code")] DA_CodesArticlesSage dA_CodesArticlesSage)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dA_CodesArticlesSage).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(dA_CodesArticlesSage);
        }

        // GET: DA_CodesArticlesSage/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DA_CodesArticlesSage dA_CodesArticlesSage = db.DA_CodesArticlesSage.Find(id);
            if (dA_CodesArticlesSage == null)
            {
                return HttpNotFound();
            }
            return View(dA_CodesArticlesSage);
        }

        // POST: DA_CodesArticlesSage/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DA_CodesArticlesSage dA_CodesArticlesSage = db.DA_CodesArticlesSage.Find(id);
            db.DA_CodesArticlesSage.Remove(dA_CodesArticlesSage);
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
