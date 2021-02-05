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
    public class DA_Materiels_DemController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: DA_Materiels_Dem
        public ActionResult Index()
        {
            return View(db.DA_Materiels_Dem.ToList());
        }
        public ActionResult Materiels_Dem(string id)
        {
            Session["reff"] = id;
            return RedirectToAction("Create");
        }
        // GET: DA_Materiels_Dem/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DA_Materiels_Dem dA_Materiels_Dem = db.DA_Materiels_Dem.Find(id);
            if (dA_Materiels_Dem == null)
            {
                return HttpNotFound();
            }
            return View(dA_Materiels_Dem);
        }

        // GET: DA_Materiels_Dem/Create
        public ActionResult Create()
        {
            string reference = Session["reff"].ToString();
            if (reference != null)
            {
                var list = (from m in db.DA_Materiels_Dem
                            where m.Réference == reference
                            orderby m.Désignation
                            select m);

                DA_Materiels_Dem matdem = new DA_Materiels_Dem();
                
                matdem.listesfournisseurs = db.DA_Fournisseurs.OrderBy(obj => obj.Raison).ToList<DA_Fournisseurs>();
                matdem.listesmaterielsdem = list.ToList();

                return View(matdem);
            }
            else
            {
                return RedirectToAction("MesDemandes", "DA_Demande");
            }
        }

        // POST: DA_Materiels_Dem/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Réference,Code,Désignation,Version,Fournisseur,Date_Recp_Souh")] DA_Materiels_Dem dA_Materiels_Dem)
        {
            if (ModelState.IsValid)
            {
                dA_Materiels_Dem.Réference = Session["reff"].ToString();
                db.DA_Materiels_Dem.Add(dA_Materiels_Dem);
                db.SaveChanges();
                Session["reff"] = dA_Materiels_Dem.Réference;
                //return RedirectToAction("Index");
                return RedirectToAction("Create", "DA_Materiels_Dem");
            }

            return View(dA_Materiels_Dem);
        }

        // GET: DA_Materiels_Dem/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DA_Materiels_Dem dA_Materiels_Dem = db.DA_Materiels_Dem.Find(id);
            if (dA_Materiels_Dem == null)
            {
                return HttpNotFound();
            }
            return View(dA_Materiels_Dem);
        }

        // POST: DA_Materiels_Dem/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Réference,Code,Désignation,Version,Fournisseur,Date_Recp_Souh")] DA_Materiels_Dem dA_Materiels_Dem)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dA_Materiels_Dem).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(dA_Materiels_Dem);
        }

        // GET: DA_Materiels_Dem/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DA_Materiels_Dem dA_Materiels_Dem = db.DA_Materiels_Dem.Find(id);
            if (dA_Materiels_Dem == null)
            {
                return HttpNotFound();
            }
            return View(dA_Materiels_Dem);
        }

        // POST: DA_Materiels_Dem/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DA_Materiels_Dem dA_Materiels_Dem = db.DA_Materiels_Dem.Find(id);
            db.DA_Materiels_Dem.Remove(dA_Materiels_Dem);
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
