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

        public static string searchTermCodeF = string.Empty;

        public static string ValTermCodeF = string.Empty;

        public static string searchTermGroupe = string.Empty;

        public static string ValTermGroupe = string.Empty;

        public static string searchTermMatUsr = string.Empty;

        public static string ValTermMatUsr = string.Empty;

        public static string searchTermUsr = string.Empty;

        public static string ValTermUsr = string.Empty;

        public static string searchTermCodeE = string.Empty;

        public static string ValTermCodeE = string.Empty;

        public static string searchTermObjet = string.Empty;

        public static string ValTermObjet = string.Empty;

        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: E_ListFormationDiffus
        public ActionResult Index(string searchStringCodeF, string searchStringObjet)
        {

            ViewData["CurrentFilterCodeF"] = searchStringCodeF; 
            ViewData["CurrentFilterObjet"] = searchStringObjet;

            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
             
            string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection con = new SqlConnection(constr);
            con.Open();

            DataTable dt = new DataTable();


            //SqlDataAdapter da;
            //da = new SqlDataAdapter(" SELECT  distinct   [Code_formt] ,Objet,null,null,null,null FROM [dbo].[E_ListFormationDiffus] where MatFormateur =  '" + user.matricule + "'", con);
            //da.Fill(dt);

            if (!String.IsNullOrEmpty(searchStringCodeF)   || !String.IsNullOrEmpty(searchStringObjet))
            {
                var r = (from m in db.e_ListFormationDiffus
                         where m.MatFormateur == user.matricule  
                         select new { m.Code_formt,  m.Objet  }).Distinct();



                E_ListFormationDiffusController.searchTermCodeF = "searchStringCodeF";
                E_ListFormationDiffusController.ValTermCodeF = searchStringCodeF;


                E_ListFormationDiffusController.searchTermObjet = "searchStringObjet";
                E_ListFormationDiffusController.ValTermObjet = searchStringObjet;

                if (!String.IsNullOrEmpty(searchStringCodeF))
                    r = r.Where(s => s.Code_formt.ToLower().Contains(searchStringCodeF.ToLower()));

                if (!String.IsNullOrEmpty(searchStringObjet))
                    r = r.Where(s => s.Objet.ToLower().Contains(searchStringObjet.ToLower()));


                List<E_ListFormationDiffus> list2 = new List<E_ListFormationDiffus>();

                foreach (var ff in r)
                {
                    E_ListFormationDiffus e = new E_ListFormationDiffus();
                     
                    e.Code_formt = ff.Code_formt;

                    e.Objet = ff.Objet;

                     
                    e.MatFormateur = e.Mat_usr = e.Nom_usr = e.Code_grp = null;

                    e.DateDiffus = Convert.ToDateTime("0001/01/01");


                    list2.Add(e);
                }

                
                return View(list2);


            }


                SqlDataAdapter da;
            da = new SqlDataAdapter(" SELECT  distinct   [Code_formt] ,[E_ListFormationDiffus].Objet , f.Date_Creation, CONVERT(nvarchar, DateDiffus, 103) DateDiffus, null, null, null FROM[dbo].[E_ListFormationDiffus] inner join  dbo.E_Formation f on f.Code = [E_ListFormationDiffus].Code_formt  where E_ListFormationDiffus.MatFormateur =  '" + user.matricule + "' order  by f.Date_Creation desc", con);
            da.Fill(dt);


            List<E_ListFormationDiffus> list = new List<E_ListFormationDiffus>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                E_ListFormationDiffus e = new E_ListFormationDiffus();

                e.Code_formt = dt.Rows[i]["Code_formt"].ToString();

                e.Objet = dt.Rows[i]["Objet"].ToString();

                //e.deadline = Convert.ToDateTime( dt.Rows[i]["deadline"].ToString());

                e.MatFormateur = e.Mat_usr = e.Nom_usr  = e.Code_grp = null;

                e.DateDiffus = Convert.ToDateTime("0001/01/01");

                list.Add(e);

            }


                return View(list);
        }

        // GET: E_ListFormationDiffus/Details/5
        public ActionResult Details(string id , string searchStringGroupe, string searchStringMatUsr, string searchStringUsr)
        {

            ViewData["CurrentFilterGroupe"] = searchStringGroupe;
            ViewData["CurrentFilterMatUsr"] = searchStringMatUsr;
            ViewData["CurrentFilterUsr"] = searchStringUsr;

            var list = from m in db.e_ListFormationDiffus
                       where m.Code_formt == id
                       select m;


            if (!String.IsNullOrEmpty(searchStringGroupe) || !String.IsNullOrEmpty(searchStringMatUsr) || !String.IsNullOrEmpty(searchStringUsr))
            {
                

                E_ListFormationDiffusController.searchTermGroupe = "searchStringGroupe";
                E_ListFormationDiffusController.ValTermGroupe = searchStringGroupe;


                E_ListFormationDiffusController.searchTermMatUsr = "searchStringMatUsr";
                E_ListFormationDiffusController.ValTermMatUsr = searchStringMatUsr;

                E_ListFormationDiffusController.searchTermUsr = "searchStringUsr";
                E_ListFormationDiffusController.ValTermUsr = searchStringUsr;

                if (!String.IsNullOrEmpty(searchStringGroupe))
                    list = list.Where(s => s.Code_grp.ToLower().Contains(searchStringGroupe.ToLower()));

                if (!String.IsNullOrEmpty(searchStringMatUsr))
                    list = list.Where(s => s.Mat_usr.ToLower().Contains(searchStringMatUsr.ToLower()));


                if (!String.IsNullOrEmpty(searchStringUsr))
                    list = list.Where(s => s.Nom_usr.ToLower().Contains(searchStringUsr.ToLower()));


                
                return View(list.ToList());


            }



            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
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
