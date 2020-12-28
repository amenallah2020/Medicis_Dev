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
    public class DA_LaboController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        public JsonResult IsLaboNameExist(string LaboName, int? Id)
        {
            var validateName = db.DA_Labo.FirstOrDefault
                                (x => x.Laboratoire == LaboName && x.Id != Id);
            if (validateName != null)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: DA_Labo
        public ActionResult Index()
        {
            return View(db.DA_Labo.ToList());
        }

        // GET: DA_Labo/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DA_Labo dA_Labo = db.DA_Labo.Find(id);
            if (dA_Labo == null)
            {
                return HttpNotFound();
            }
            return View(dA_Labo);
        }

        // GET: DA_Labo/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DA_Labo/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Laboratoire,Adresse,Tel,Mobile")] DA_Labo dA_Labo)
        {
            bool IsLaboNameExist = db.DA_Labo.Any
        (x => x.Laboratoire == dA_Labo.Laboratoire && x.Id != dA_Labo.Id);
            if (IsLaboNameExist == true)
            {
                ModelState.AddModelError("Laboratoire", "Ce laboratoire existe déja");
            }

            if (ModelState.IsValid)
            {
                db.DA_Labo.Add(dA_Labo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(dA_Labo);
        }

        // GET: DA_Labo/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DA_Labo dA_Labo = db.DA_Labo.Find(id);
            if (dA_Labo == null)
            {
                return HttpNotFound();
            }
            return View(dA_Labo);
        }

        // POST: DA_Labo/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Laboratoire,Adresse,Tel,Mobile")] DA_Labo dA_Labo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dA_Labo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(dA_Labo);
        }

        // GET: DA_Labo/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DA_Labo dA_Labo = db.DA_Labo.Find(id);
            if (dA_Labo == null)
            {
                return HttpNotFound();
            }
            return View(dA_Labo);
        }

        // POST: DA_Labo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DA_Labo dA_Labo = db.DA_Labo.Find(id);
            db.DA_Labo.Remove(dA_Labo);
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
