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
    public class E_ListEvaluationDiffusController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public static string searchTermCodeE = string.Empty;

        public static string ValTermCodeE = string.Empty;

        public static string searchTermObjet = string.Empty;
         
        public static string ValTermObjet = string.Empty;

        public static string searchTermGroupe = string.Empty;

        public static string ValTermGroupe = string.Empty;

        public static string searchTermMatUsr = string.Empty;

        public static string ValTermMatUsr = string.Empty;

        public static string searchTermUsr = string.Empty;

        public static string ValTermUsr = string.Empty;


        // GET: E_ListFormationDiffus
        public ActionResult Index(string searchStringCodeE, string searchStringObjet)
        {
            ViewData["CurrentFilterCodeE"] = searchStringCodeE;
            ViewData["CurrentFilterObjet"] = searchStringObjet;

            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
             
            string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection con = new SqlConnection(constr);
            con.Open();

            DataTable dt = new DataTable();

            List<E_ListEvaluationDiffus> list = new List<E_ListEvaluationDiffus>();


            if (!String.IsNullOrEmpty(searchStringCodeE) || !String.IsNullOrEmpty(searchStringObjet))
            {

                var r = (from m in db.e_ListEvaluationDiffus
                         where m.MatFormateur == user.matricule
                         select new { m.Code_eval, m.Objet }).Distinct();



                E_EvaluationController.searchTermCodeE = "searchStringCodeE";
                E_EvaluationController.ValTermCodeE = searchStringCodeE;


                E_EvaluationController.searchTermObjet = "searchStringObjet";
                E_EvaluationController.ValTermObjet = searchStringObjet;

                if (!String.IsNullOrEmpty(searchStringObjet))
                    r = r.Where(s => s.Objet.ToLower().Contains(searchStringObjet.ToLower()));

                if (!String.IsNullOrEmpty(searchStringCodeE))
                    r = r.Where(s => s.Code_eval.ToLower().Contains(searchStringCodeE.ToLower()));

               foreach(var ee in r)
                {
                    E_ListEvaluationDiffus e = new E_ListEvaluationDiffus();

                    e.Code_eval = ee.Code_eval;

                    e.Objet =ee.Objet;

                    //e.deadline = Convert.ToDateTime( dt.Rows[i]["deadline"].ToString());

                    e.MatFormateur = e.Mat_usr = e.Nom_usr = e.Code_grp = null;

                    e.DateDiffus = Convert.ToDateTime("0001/01/01");

                    list.Add(e);

                }

                return View(list);

            }


            //SqlDataAdapter da;
            //da = new SqlDataAdapter(" SELECT  distinct   [Code_formt] ,Objet,null,null,null,null FROM [dbo].[E_ListFormationDiffus] where MatFormateur =  '" + user.matricule + "'", con);
            //da.Fill(dt);

            SqlDataAdapter da;
            da = new SqlDataAdapter(" SELECT  distinct   dbo.[E_ListEvaluationDiffus].[Code_eval] ,f.Objet_Eval Objet , f.Date_Creation,  CONVERT(nvarchar, dbo.[E_ListEvaluationDiffus].DateDiffus, 103) DateDiffus, null, null, null FROM[dbo].[E_ListEvaluationDiffus] inner join  dbo.E_Evaluation f on f.Code_Eval =  [E_ListEvaluationDiffus].Code_eval where[E_ListEvaluationDiffus].MatFormateur = '" + user.matricule + "' order  by f.Date_Creation desc", con);
            da.Fill(dt);


         
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                E_ListEvaluationDiffus e = new E_ListEvaluationDiffus();

                e.Code_eval = dt.Rows[i]["Code_eval"].ToString();

                e.Objet = dt.Rows[i]["Objet"].ToString();

                //e.deadline = Convert.ToDateTime( dt.Rows[i]["deadline"].ToString());

                e.MatFormateur = e.Mat_usr = e.Nom_usr  = e.Code_grp = null;

                e.DateDiffus = Convert.ToDateTime("0001/01/01");

                list.Add(e);

            }


                return View(list);
        }

        // GET: E_ListFormationDiffus/Details/5
        public ActionResult Details(string id , string searchStringGroupe, string searchStringMatUsr , string searchStringUsr)
        {
            ViewData["CurrentFilterGroupe"] = searchStringGroupe;
            ViewData["CurrentFilterMatUsr"] = searchStringMatUsr;
            ViewData["CurrentFilterUsr"] = searchStringUsr;


            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var list = from m in db.e_ListEvaluationDiffus
                       where m.Code_eval == id
                       select m;

            if (!String.IsNullOrEmpty(searchStringGroupe) || !String.IsNullOrEmpty(searchStringMatUsr) || !String.IsNullOrEmpty(searchStringUsr))
            {

                
                E_ListEvaluationDiffusController.searchTermGroupe = "searchStringGroupe";
                E_ListEvaluationDiffusController.ValTermGroupe = searchStringGroupe;


                E_ListEvaluationDiffusController.searchTermMatUsr = "searchStringMatUsr";
                E_ListEvaluationDiffusController.ValTermMatUsr = searchStringMatUsr;

                E_ListEvaluationDiffusController.searchTermUsr = "searchStringUsr";
                E_ListEvaluationDiffusController.ValTermUsr = searchStringUsr;

                if (!String.IsNullOrEmpty(searchStringGroupe))
                    list = list.Where(s => s.Objet.ToLower().Contains(searchStringGroupe.ToLower()));

                if (!String.IsNullOrEmpty(searchStringMatUsr))
                    list = list.Where(s => s.Mat_usr.ToLower().Contains(searchStringMatUsr.ToLower()));

                if (!String.IsNullOrEmpty(searchStringUsr))
                    list = list.Where(s => s.Nom_usr.ToLower().Contains(searchStringUsr.ToLower()));

          

                return View(list);

            }


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
