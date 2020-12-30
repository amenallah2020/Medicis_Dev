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
    public class DA_TypesActionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: DA_TypesActions
        public ActionResult Index()
        {
            return View(db.DA_TypesActions.ToList());
        }

        // GET: DA_TypesActions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DA_TypesActions dA_TypesActions = db.DA_TypesActions.Find(id);
            if (dA_TypesActions == null)
            {
                return HttpNotFound();
            }
            return View(dA_TypesActions);
        }

        // GET: DA_TypesActions/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DA_TypesActions/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,TypeAction")] DA_TypesActions dA_TypesActions)
        {
            if (ModelState.IsValid)
            {
                db.DA_TypesActions.Add(dA_TypesActions);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(dA_TypesActions);
        }

        // GET: DA_TypesActions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DA_TypesActions dA_TypesActions = db.DA_TypesActions.Find(id);
            if (dA_TypesActions == null)
            {
                return HttpNotFound();
            }
            return View(dA_TypesActions);
        }

        // POST: DA_TypesActions/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,TypeAction")] DA_TypesActions dA_TypesActions)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dA_TypesActions).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(dA_TypesActions);
        }

        // GET: DA_TypesActions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DA_TypesActions dA_TypesActions = db.DA_TypesActions.Find(id);
            if (dA_TypesActions == null)
            {
                return HttpNotFound();
            }
            return View(dA_TypesActions);
        }

        // POST: DA_TypesActions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DA_TypesActions dA_TypesActions = db.DA_TypesActions.Find(id);
            db.DA_TypesActions.Remove(dA_TypesActions);
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
