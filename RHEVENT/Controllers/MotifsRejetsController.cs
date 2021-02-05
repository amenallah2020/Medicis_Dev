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
    public class MotifsRejetsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: MotifsRejets
        public ActionResult Index()
        {
            return View(db.MotifsRejets.ToList());
        }

        // GET: MotifsRejets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MotifsRejet motifsRejet = db.MotifsRejets.Find(id);
            if (motifsRejet == null)
            {
                return HttpNotFound();
            }
            return View(motifsRejet);
        }

        // GET: MotifsRejets/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MotifsRejets/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,MotifRejet,Conséquense")] MotifsRejet motifsRejet)
        {
            if (ModelState.IsValid)
            {
                db.MotifsRejets.Add(motifsRejet);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(motifsRejet);
        }

        // GET: MotifsRejets/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MotifsRejet motifsRejet = db.MotifsRejets.Find(id);
            if (motifsRejet == null)
            {
                return HttpNotFound();
            }
            return View(motifsRejet);
        }

        // POST: MotifsRejets/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,MotifRejet,Conséquense")] MotifsRejet motifsRejet)
        {
            if (ModelState.IsValid)
            {
                db.Entry(motifsRejet).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(motifsRejet);
        }

        // GET: MotifsRejets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MotifsRejet motifsRejet = db.MotifsRejets.Find(id);
            if (motifsRejet == null)
            {
                return HttpNotFound();
            }
            return View(motifsRejet);
        }

        // POST: MotifsRejets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MotifsRejet motifsRejet = db.MotifsRejets.Find(id);
            db.MotifsRejets.Remove(motifsRejet);
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
