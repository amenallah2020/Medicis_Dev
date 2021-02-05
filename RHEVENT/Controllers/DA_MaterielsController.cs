using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
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
        public async Task <ActionResult> synchro()
        {
            string constr = "Data Source = 192.168.1.201\\SAGEX3; Initial Catalog = x3v6; user id = da; password = da$2021";
            SqlConnection con = new SqlConnection(constr);
            con.Open();
            SqlDataAdapter da1 = new SqlDataAdapter("select  ITMMASTER.ITMREF_0,ITMMASTER.ITMSTA_0  from X3MEDICIS.ITMMASTER ", con);
            DataTable dt1 = new DataTable();
            da1.Fill(dt1);
            con.Close();

            string constr11 = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection con11 = new SqlConnection(constr11);
            con11.Open();
            for (int i = 0; i < dt1.Rows.Count; i++)
            {
                string codefromsage = dt1.Rows[i][0].ToString();
                string statut = dt1.Rows[i][1].ToString();
                SqlDataAdapter da11 = new SqlDataAdapter("select Code FROM DA_CodesArticlesSage where Code='" + codefromsage + "'", con11);
                DataTable dt11 = new DataTable();
                da11.Fill(dt11);
                if (dt11.Rows.Count == 0)
                {
                    DA_CodesArticlesSage NewCode = new DA_CodesArticlesSage();
                    NewCode.Code = codefromsage;
                    db.DA_CodesArticlesSage.Add(NewCode);
                    db.SaveChanges();
                }
                if (statut != "1")
                {
                    SqlCommand cmd = new SqlCommand("delete FROM DA_CodesArticlesSage where Code='" + codefromsage + "' ", con11);
                    cmd.ExecuteNonQuery();
                }
            }
            con11.Close();
            return RedirectToAction("Create", "DA_Materiels");
        }


        // GET: DA_Materiels/Create
        public ActionResult Create()
        {
            var list1 = (from m in db.DA_CodesArticlesSage
                         orderby m.Code
                         select m);
            ViewBag.liste1 = list1.ToList();

            return View();
        }

        // POST: DA_Materiels/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Type,Désignation,PlafondBudget,Code")] DA_Materiels dA_Materiels)
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
            var list1 = (from m in db.DA_CodesArticlesSage
                         orderby m.Code
                         select m);
            ViewBag.liste1 = list1.ToList();

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
        public ActionResult Edit([Bind(Include = "Id,Type,Désignation,PlafondBudget,Code")] DA_Materiels dA_Materiels)
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
