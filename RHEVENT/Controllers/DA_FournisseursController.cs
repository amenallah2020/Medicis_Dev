using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RHEVENT.Models;

namespace RHEVENT.Controllers
{
    public class DA_FournisseursController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: DA_Fournisseurs
        public ActionResult Index()
        {
            return View(db.DA_Fournisseurs.OrderBy(x=>x.Code).ToList());
        }

        // GET: DA_Fournisseurs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DA_Fournisseurs dA_Fournisseurs = db.DA_Fournisseurs.Find(id);
            if (dA_Fournisseurs == null)
            {
                return HttpNotFound();
            }
            return View(dA_Fournisseurs);
        }

        public ActionResult synchro()
        {
            string constr = "Data Source = 192.168.1.201\\SAGEX3; Initial Catalog = x3v6; user id = da; password = da$2021";
            SqlConnection con = new SqlConnection(constr);
            con.Open();
            SqlDataAdapter da1 = new SqlDataAdapter("select BPSUPPLIER.BPSNUM_0 , BPSUPPLIER.BPSNAM_0,BPSUPPLIER.ENAFLG_0 from X3MEDICIS.BPSUPPLIER ", con);
            DataTable dt1 = new DataTable();
            da1.Fill(dt1);
            con.Close();

            string constr11 = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection con11 = new SqlConnection(constr11);
            con11.Open();
            for (int i = 0; i < dt1.Rows.Count; i++)
            {
                string codefournisseurfromsage = dt1.Rows[i][0].ToString();
                string raisonfournisseurfromsage = dt1.Rows[i][1].ToString();
                string statut = dt1.Rows[i][2].ToString();
                SqlDataAdapter da11 = new SqlDataAdapter("select Code FROM DA_Fournisseurs where Code='" + codefournisseurfromsage + "'", con11);
                DataTable dt11 = new DataTable();
                da11.Fill(dt11);
                if (dt11.Rows.Count == 0)
                {
                    DA_Fournisseurs NewFournisseur = new DA_Fournisseurs();
                    NewFournisseur.Code = codefournisseurfromsage;
                    NewFournisseur.Raison = raisonfournisseurfromsage;
                    try
                    {
                        db.DA_Fournisseurs.Add(NewFournisseur);
                        db.SaveChanges();
                    }
                    catch (DbEntityValidationException ex)
                    {
                        foreach (var entityValidationErrors in ex.EntityValidationErrors)
                        {
                            foreach (var validationError in entityValidationErrors.ValidationErrors)
                            {
                                Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                                //Response.Redirect(validationError.ErrorMessage);

                            }
                        }
                    }
                    
                }
                if (statut != "2")
                {
                    SqlCommand cmd = new SqlCommand("delete FROM DA_Fournisseurs where Code='" + codefournisseurfromsage + "' ", con11);
                    cmd.ExecuteNonQuery();
                }
            }
            con11.Close();
            return RedirectToAction("Index", "DA_Fournisseurs");
        }


        // GET: DA_Fournisseurs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DA_Fournisseurs/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Code,Raison/*,Adresse,Tel,Mobile*/")] DA_Fournisseurs dA_Fournisseurs)
        {
            if (ModelState.IsValid)
            {
                db.DA_Fournisseurs.Add(dA_Fournisseurs);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(dA_Fournisseurs);
        }

        // GET: DA_Fournisseurs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DA_Fournisseurs dA_Fournisseurs = db.DA_Fournisseurs.Find(id);
            if (dA_Fournisseurs == null)
            {
                return HttpNotFound();
            }
            return View(dA_Fournisseurs);
        }

        // POST: DA_Fournisseurs/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Code,Raison/*,Adresse,Tel,Mobile*/")] DA_Fournisseurs dA_Fournisseurs)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dA_Fournisseurs).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(dA_Fournisseurs);
        }

        // GET: DA_Fournisseurs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DA_Fournisseurs dA_Fournisseurs = db.DA_Fournisseurs.Find(id);
            if (dA_Fournisseurs == null)
            {
                return HttpNotFound();
            }
            return View(dA_Fournisseurs);
        }

        // POST: DA_Fournisseurs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DA_Fournisseurs dA_Fournisseurs = db.DA_Fournisseurs.Find(id);
            db.DA_Fournisseurs.Remove(dA_Fournisseurs);
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
