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
    public class DA_BudgetController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: DA_Budget
        public ActionResult Index()
        {
            return View(db.DA_Budget.ToList());
        }

        // GET: DA_Budget/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DA_Budget dA_Budget = db.DA_Budget.Find(id);
            if (dA_Budget == null)
            {
                return HttpNotFound();
            }
            return View(dA_Budget);
        }

        // GET: DA_Budget/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DA_Budget/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Réference,BudgetElementaire,PrixUnitaire,Quantité,Total,Fournisseur,AL")] DA_Budget dA_Budget)
        {
            if (ModelState.IsValid)
            {
                db.DA_Budget.Add(dA_Budget);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(dA_Budget);
        }

        // GET: DA_Budget/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DA_Budget dA_Budget = db.DA_Budget.Find(id);
            if (dA_Budget == null)
            {
                return HttpNotFound();
            }
            return View(dA_Budget);
        }

        // POST: DA_Budget/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Réference,BudgetElementaire,PrixUnitaire,Quantité,Total,Fournisseur,AL")] DA_Budget dA_Budget)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dA_Budget).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(dA_Budget);
        }

        // GET: DA_Budget/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DA_Budget dA_Budget = db.DA_Budget.Find(id);
            if (dA_Budget == null)
            {
                return HttpNotFound();
            }
            return View(dA_Budget);
        }

        // POST: DA_Budget/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DA_Budget dA_Budget = db.DA_Budget.Find(id);
            db.DA_Budget.Remove(dA_Budget);
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
