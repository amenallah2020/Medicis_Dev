using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using RHEVENT.Models;

namespace RHEVENT.Controllers
{
    public class E_ListFormationDiffusController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: E_ListFormationDiffus
        public ActionResult Index()
        {

            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
             
            string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection con = new SqlConnection(constr);
            con.Open();

            DataTable dt = new DataTable();


            SqlDataAdapter da;
            da = new SqlDataAdapter(" SELECT  distinct   [Code_formt] ,Objet,null,null,null,null FROM [RH_MEDICIS].[dbo].[E_ListFormationDiffus] where MatFormateur =  '" + user.matricule + "'", con);
            da.Fill(dt);



            List<E_ListFormationDiffus> list = new List<E_ListFormationDiffus>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                E_ListFormationDiffus e = new E_ListFormationDiffus();

                e.Code_formt = dt.Rows[i]["Code_formt"].ToString();

                e.Objet = dt.Rows[i]["Objet"].ToString();

                e.MatFormateur = e.Mat_usr = e.Nom_usr  = e.Code_grp = null;

                e.DateDiffus = Convert.ToDateTime("0001/01/01");

                list.Add(e);

            }


                return View(list);
        }

        // GET: E_ListFormationDiffus/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var list = from m in db.e_ListFormationDiffus
                       where m.Code_formt == id
                       select m;

            //E_ListFormationDiffus e_ListFormationDiffus = db.e_ListFormationDiffus.Find(id);
            //if (e_ListFormationDiffus == null)
            //{
            //    return HttpNotFound();
            //}

            return View(list.ToList());
        }

        // GET: E_ListFormationDiffus/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: E_ListFormationDiffus/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Mat_usr,Nom_usr,Code_grp,Code_formt")] E_ListFormationDiffus e_ListFormationDiffus)
        {
            if (ModelState.IsValid)
            {
                db.e_ListFormationDiffus.Add(e_ListFormationDiffus);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(e_ListFormationDiffus);
        }

        // GET: E_ListFormationDiffus/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            E_ListFormationDiffus e_ListFormationDiffus = db.e_ListFormationDiffus.Find(id);
            if (e_ListFormationDiffus == null)
            {
                return HttpNotFound();
            }
            return View(e_ListFormationDiffus);
        }

        // POST: E_ListFormationDiffus/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Mat_usr,Nom_usr,Code_grp,Code_formt")] E_ListFormationDiffus e_ListFormationDiffus)
        {
            if (ModelState.IsValid)
            {
                db.Entry(e_ListFormationDiffus).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(e_ListFormationDiffus);
        }

        // GET: E_ListFormationDiffus/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            E_ListFormationDiffus e_ListFormationDiffus = db.e_ListFormationDiffus.Find(id);
            if (e_ListFormationDiffus == null)
            {
                return HttpNotFound();
            }
            return View(e_ListFormationDiffus);
        }

        // POST: E_ListFormationDiffus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            E_ListFormationDiffus e_ListFormationDiffus = db.e_ListFormationDiffus.Find(id);
            db.e_ListFormationDiffus.Remove(e_ListFormationDiffus);
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
