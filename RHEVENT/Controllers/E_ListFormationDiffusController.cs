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
using OfficeOpenXml;
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


        public ActionResult Exporter(string searchStringCodeF, string searchStringObjet, string Usr   )
        {

            ViewData["CurrentFilterCodeF"] = searchStringCodeF;
            ViewData["CurrentFilterObjet"] = searchStringObjet;



            var dd = from m in db.Users
                     where m.matricule == Usr
                     select m;

            if (dd.Count() != 0)
            {
                ApplicationUser p = (from m in db.Users
                                     where m.matricule == Usr
                                     select m).Single();
                Session["Usrr"] = p.NomPrenom;

                Session["MatUsrr"] = Usr;
            }

            else
            {
                Session["Usrr"] = "Selectionnez un utilisateur";
                Session["MatUsrr"] = "";
            }



            var listu = from m in db.Users
                        orderby m.NomPrenom
                        select m;

            List<ApplicationUser> listUser = listu.ToList<ApplicationUser>();

            ViewBag.listUser = listUser;

            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());

            string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection con = new SqlConnection(constr);
            con.Open();

            DataTable dt = new DataTable();


            //SqlDataAdapter da;
            //da = new SqlDataAdapter(" SELECT  distinct   [Code_formt] ,Objet,null,null,null,null FROM [dbo].[E_ListFormationDiffus] where MatFormateur =  '" + user.matricule + "'", con);
            //da.Fill(dt);

            if (!String.IsNullOrEmpty(searchStringCodeF) || !String.IsNullOrEmpty(searchStringObjet) || !String.IsNullOrEmpty(Usr))
            {
                var r = (from m in db.e_ListFormationDiffus
                         join n in db.E_ResultFormation on m.Code_formt equals n.Code_Formation
                         where m.MatFormateur == user.matricule
                         select new { m.Code_formt, m.Mat_usr, m.Objet, m.Nom_usr , Etat = (n.Etat == "Complete"? "Complete" : "Incomplete") , n.DateTerm , n.DeadLine }).Distinct();



                E_ListFormationDiffusController.searchTermCodeF = "searchStringCodeF";
                E_ListFormationDiffusController.ValTermCodeF = searchStringCodeF;


                E_ListFormationDiffusController.searchTermObjet = "searchStringObjet";
                E_ListFormationDiffusController.ValTermObjet = searchStringObjet;

                if (!String.IsNullOrEmpty(searchStringCodeF))
                    r = r.Where(s => s.Code_formt.ToLower().Contains(searchStringCodeF.ToLower()));

                if (!String.IsNullOrEmpty(searchStringObjet))
                    r = r.Where(s => s.Objet.ToLower().Contains(searchStringObjet.ToLower()));

                if (!String.IsNullOrEmpty(Usr))
                {
                    if (Usr.Trim() != null)
                        r = r.Where(s => s.Mat_usr.Equals(Usr));
                }



                List<E_ListFormationDiffus> list2 = new List<E_ListFormationDiffus>();

                foreach (var ff in r)
                {
                    E_ListFormationDiffus e = new E_ListFormationDiffus();

                    e.Code_formt = ff.Code_formt;

                    e.Objet = ff.Objet;
                     

                    e.Mat_usr = ff.Mat_usr;

                    e.Nom_usr = ff.Nom_usr;

                    if (ff.DateTerm.ToString().Trim() != "")
                        e.DateDiffus = Convert.ToDateTime(ff.DateTerm.ToString());
                    else
                        e.DateDiffus = Convert.ToDateTime("01/01/0001");

                    if (ff.DeadLine.ToString().Trim() != "")
                        e.deadline = Convert.ToDateTime(ff.DeadLine.ToString());
                    else
                        e.deadline = Convert.ToDateTime("01/01/0001");

                    list2.Add(e);
                }



                ExcelPackage pck2 = new ExcelPackage();

                ExcelWorksheet ws2 = pck2.Workbook.Worksheets.Add("Liste des formations diffusées");
                ws2.Cells["A1"].Value = "Formation";
                ws2.Cells["B1"].Value = "Objet formation";
                ws2.Cells["C1"].Value = "Matricule Utilisateur";
                ws2.Cells["D1"].Value = "Utilisateur"; 
                ws2.Cells["E1"].Value = "Date formation";
                ws2.Cells["F1"].Value = "Date limite";

                int rowStart2 = 2;
                int jour2 = 0;

                foreach (var item in list2)
                {
                    //   jour++;
                    ws2.Cells[String.Format("A{0}", rowStart2)].Value = item.Code_formt;
                    ws2.Cells[String.Format("B{0}", rowStart2)].Value = item.Objet;
                    ws2.Cells[String.Format("C{0}", rowStart2)].Value = item.Mat_usr;
                    ws2.Cells[String.Format("D{0}", rowStart2)].Value = item.Nom_usr;
                    ws2.Cells[String.Format("E{0}", rowStart2)].Value = item.DateDiffus.ToShortDateString();
                    ws2.Cells[String.Format("F{0}", rowStart2)].Value = item.deadline.ToShortDateString();
                    rowStart2++;
                }

                //  return RedirectToAction(jour + "");
                ws2.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("Content-disposition", "attachment:filename=" + "Etatattestations.xlsx");
                Response.BinaryWrite(pck2.GetAsByteArray());
                Response.End();
                return View();
                 

            }


            SqlDataAdapter da;
            da = new SqlDataAdapter(" SELECT  distinct  [Code_formt] ,[E_ListFormationDiffus].Objet ,  Mat_usr , Nom_usr ,case E_ResultFormation.Etat when 'Complete' then 'Complete' else 'Incomplete' end Etat ,   E_ResultFormation.DateTerm ,  E_ResultFormation.DeadLine  FROM[dbo].[E_ListFormationDiffus]  inner join  dbo.E_Formation f on f.Code = [E_ListFormationDiffus].Code_formt   left outer join E_ResultFormation on E_ResultFormation.Code_Formation = [E_ListFormationDiffus].Code_formt where E_ListFormationDiffus.MatFormateur =  '" + user.matricule + "' ", con);
            da.Fill(dt);


            List<E_ListFormationDiffus> list = new List<E_ListFormationDiffus>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                E_ListFormationDiffus e = new E_ListFormationDiffus();

                e.Code_formt = dt.Rows[i]["Code_formt"].ToString();

                e.Objet = dt.Rows[i]["Objet"].ToString();

                //e.deadline = Convert.ToDateTime( dt.Rows[i]["deadline"].ToString());

                e.Mat_usr = dt.Rows[i]["Mat_usr"].ToString();

                e.Nom_usr = dt.Rows[i]["Nom_usr"].ToString();

                if ( dt.Rows[i]["DateTerm"].ToString().Trim() != "")
                    e.DateDiffus = Convert.ToDateTime(dt.Rows[i]["DateTerm"].ToString());
                else
                    e.DateDiffus = Convert.ToDateTime("01/01/0001");

                if (dt.Rows[i]["DeadLine"].ToString().Trim() != "")
                    e.deadline = Convert.ToDateTime(dt.Rows[i]["DeadLine"].ToString());
                else
                    e.deadline = Convert.ToDateTime("01/01/0001");

                list.Add(e);

            }


            ExcelPackage pck  = new ExcelPackage();

            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Liste des formations diffusées");
            ws.Cells["A1"].Value = "Formation";
            ws.Cells["B1"].Value = "Objet formation";
            ws.Cells["C1"].Value = "Matricule Utilisateur";
            ws.Cells["D1"].Value = "Utilisateur";
            ws.Cells["E1"].Value = "Date formation";
            ws.Cells["F1"].Value = "Date limite";

            int rowStart = 2;
            int jour = 0;

            foreach (var item in list)
            {
                //   jour++;
                ws.Cells[String.Format("A{0}", rowStart)].Value = item.Code_formt;
                ws.Cells[String.Format("B{0}", rowStart)].Value = item.Objet;
                ws.Cells[String.Format("C{0}", rowStart)].Value = item.Mat_usr;
                ws.Cells[String.Format("D{0}", rowStart)].Value = item.Nom_usr;
                ws.Cells[String.Format("E{0}", rowStart)].Value = item.DateDiffus.ToShortDateString();
                ws.Cells[String.Format("F{0}", rowStart)].Value = item.deadline.ToShortDateString();
                rowStart++;
            }

            //  return RedirectToAction(jour + "");
            ws.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("Content-disposition", "attachment:filename=" + "Etatattestations.xlsx");
            Response.BinaryWrite(pck.GetAsByteArray());
            Response.End();
            return View();
        }


        // GET: E_ListFormationDiffus
        public ActionResult Index(string searchStringCodeF, string searchStringObjet , string Usr)
        {
            
            ViewData["CurrentFilterCodeF"] = searchStringCodeF; 
            ViewData["CurrentFilterObjet"] = searchStringObjet;

           

            var dd = from m in db.Users
                     where m.matricule == Usr
                     select m;

            if (dd.Count() != 0)
            {
                ApplicationUser p = (from m in db.Users
                                     where m.matricule == Usr
                                     select m).Single();
                Session["Usrr"]  = p.NomPrenom;

                Session["MatUsrr"] = Usr;  
            }

            else
            {
                Session["Usrr"] =   "Selectionnez un utilisateur";
                Session["MatUsrr"] = "";
            }



                var listu = from m in db.Users
                        orderby m.NomPrenom
                     select m;

            List<ApplicationUser> listUser = listu.ToList<ApplicationUser>();

            ViewBag.listUser = listUser;

            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
             
            string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection con = new SqlConnection(constr);
            con.Open();

            DataTable dt = new DataTable();


            //SqlDataAdapter da;
            //da = new SqlDataAdapter(" SELECT  distinct   [Code_formt] ,Objet,null,null,null,null FROM [dbo].[E_ListFormationDiffus] where MatFormateur =  '" + user.matricule + "'", con);
            //da.Fill(dt);

            if (!String.IsNullOrEmpty(searchStringCodeF)   || !String.IsNullOrEmpty(searchStringObjet) || !String.IsNullOrEmpty(Usr))
            {
                var r = (from m in db.e_ListFormationDiffus
                         where m.MatFormateur == user.matricule  
                         select new { m.Code_formt,  m.Objet , m.Mat_usr  }).Distinct();

                 

                E_ListFormationDiffusController.searchTermCodeF = "searchStringCodeF";
                E_ListFormationDiffusController.ValTermCodeF = searchStringCodeF;


                E_ListFormationDiffusController.searchTermObjet = "searchStringObjet";
                E_ListFormationDiffusController.ValTermObjet = searchStringObjet;

                if (!String.IsNullOrEmpty(searchStringCodeF))
                    r = r.Where(s => s.Code_formt.ToLower().Contains(searchStringCodeF.ToLower()));

                if (!String.IsNullOrEmpty(searchStringObjet))
                    r = r.Where(s => s.Objet.ToLower().Contains(searchStringObjet.ToLower()));

                if (!String.IsNullOrEmpty(Usr))
                { 
                    if(Usr.Trim() != null)
                    r = r.Where(s => s.Mat_usr.Equals(Usr));
                }



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
            da = new SqlDataAdapter(" SELECT  distinct  top(20)  [Code_formt] ,[E_ListFormationDiffus].Objet , f.Date_Creation, CONVERT(nvarchar, DateDiffus, 103) DateDiffus,    null, null FROM[dbo].[E_ListFormationDiffus] inner join  dbo.E_Formation f on f.Code = [E_ListFormationDiffus].Code_formt  where E_ListFormationDiffus.MatFormateur =  '" + user.matricule + "' order  by f.Date_Creation desc", con);
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
