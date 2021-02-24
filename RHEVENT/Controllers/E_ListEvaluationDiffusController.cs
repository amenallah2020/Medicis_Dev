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



        public ActionResult Exporter(string searchStringCodeE, string searchStringObjet, string Usr)
        {
            ViewData["CurrentFilterCodeE"] = searchStringCodeE;
            ViewData["CurrentFilterObjet"] = searchStringObjet;

            var dd = from m in db.Users
                     where m.matricule == Usr
                     select m;

            if (dd.Count() != 0)
            {
                ApplicationUser p = (from m in db.Users
                                     where m.matricule == Usr
                                     select m).Single();
                Session["UsrrEv"] = p.NomPrenom;

                Session["MatUsrrEv"] = Usr;
            }

            else
            {
                Session["UsrrEv"] = "Selectionnez un utilisateur";
                Session["MatUsrrEv"] = "";
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

            List<E_ResultQCM> list = new List<E_ResultQCM>();


            if (!String.IsNullOrEmpty(searchStringCodeE) || !String.IsNullOrEmpty(searchStringObjet) || !String.IsNullOrEmpty(Usr))
            {

                var r = ((from m in db.e_ListEvaluationDiffus
                         join n in db.E_ResultQCM on m.Code_eval equals n.Code_EvalByQCM into Result
                         join   f in db.E_Formation on m.Code_Formation equals f.Code into Form
                         where m.MatFormateur == user.matricule
                         select new { Code_EvalByQCM = m.Code_eval, ObjEval = m.Objet, CodeForm = m.Code_Formation, ObjForm = (from b in Form select b.Objet).FirstOrDefault(), MatUser = m.Mat_usr, Usr = m.Nom_usr, DateEval = Result.Select(aa => aa.DateEval).FirstOrDefault(), DeadLine = Result.Select(zz => zz.DeadLine).FirstOrDefault(), Score = Result.Select(ee => ee.Score).FirstOrDefault(), Resultat = Result.Select(nz => nz.Resultat).FirstOrDefault() }) )
                         
                         .Union(from m in db.e_ListEvaluationDiffus
                                join n in db.E_ResultQCM_Historiq on m.Code_eval equals n.Code_EvalByQCM into Result
                                join f in db.E_Formation on m.Code_Formation equals f.Code into Form
                                where m.MatFormateur == user.matricule
                                select new { Code_EvalByQCM = m.Code_eval, ObjEval = m.Objet, CodeForm = m.Code_Formation, ObjForm = (from b in Form select b.Objet ).FirstOrDefault(), MatUser = m.Mat_usr, Usr = m.Nom_usr, DateEval = Result.Select(aa => aa.DateEval).FirstOrDefault(), DeadLine = Result.Select(zz => zz.DeadLine).FirstOrDefault(), Score = Result.Select(ee => ee.Score).FirstOrDefault(), Resultat = Result.Select(nz => nz.Resultat).FirstOrDefault() }) .ToList() ;



                //var r = ((from m in db.e_ListEvaluationDiffus
                //          join n in db.E_ResultQCM on m.Code_eval equals n.Code_EvalByQCM 
                //          join f in db.E_Formation on m.Code_Formation equals f.Code 
                //          where m.MatFormateur == user.matricule
                //          select new { Code_EvalByQCM = m.Code_eval, ObjEval = m.Objet, CodeForm = m.Code_Formation, ObjForm = f.Objet, MatUser = m.Mat_usr, Usr = m.Nom_usr, DateEval = n.DateEval, DeadLine = n.DeadLine, Score = n.Score, Resultat = n.Resultat }).Distinct())

                //        .Union(from m in db.e_ListEvaluationDiffus
                //               join n in db.E_ResultQCM_Historiq on m.Code_eval equals n.Code_EvalByQCM  
                //               join f in db.E_Formation on m.Code_Formation equals f.Code  
                //               where m.MatFormateur == user.matricule
                //               select new { Code_EvalByQCM = m.Code_eval, ObjEval = m.Objet, CodeForm = m.Code_Formation, ObjForm = f.Objet, MatUser = m.Mat_usr, Usr = m.Nom_usr, DateEval = n.DateEval, DeadLine = n.DeadLine, Score = n.Score, Resultat = n.Resultat }).Distinct().ToList();





                E_EvaluationController.searchTermCodeE = "searchStringCodeE";
                E_EvaluationController.ValTermCodeE = searchStringCodeE;


                E_EvaluationController.searchTermObjet = "searchStringObjet";
                E_EvaluationController.ValTermObjet = searchStringObjet;

                if (!String.IsNullOrEmpty(searchStringObjet))
                    r = r.Where(s => s.ObjEval.ToLower().Contains(searchStringObjet.ToLower())).ToList();

                if (!String.IsNullOrEmpty(searchStringCodeE))
                    r = r.Where(s => s.Code_EvalByQCM.ToLower().Contains(searchStringCodeE.ToLower())).ToList();


                if (!String.IsNullOrEmpty(Usr))
                {
                    if (Usr.Trim() != null)
                        r = r.Where(s => s.MatUser.Equals(Usr)).ToList();
                }

                foreach (var ee in r)
                {
                    E_ResultQCM e = new E_ResultQCM();

                    e.Code_EvalByQCM = ee.Code_EvalByQCM;

                    e.ObjEval = ee.ObjEval;

                    e.CodeForm = ee.CodeForm;

                    e.ObjForm = ee.ObjForm;

                    e.MatUser = ee.MatUser;

                    e.Usr = ee.Usr;

                    e.DateEval = Convert.ToDateTime( ee.DateEval);

                    e.DeadLine = Convert.ToDateTime(ee.DeadLine);

                    e.Score = Convert.ToInt32(ee.Score);

                    e.Resultat = ee.Resultat; 

                    list.Add(e);

                }

                ExcelPackage pck2 = new ExcelPackage();

                ExcelWorksheet ws2 = pck2.Workbook.Worksheets.Add("Liste des évaluations diffusées");
                ws2.Cells["A1"].Value = "Evaluation";
                ws2.Cells["B1"].Value = "Objet évaluation";
                ws2.Cells["C1"].Value = "Formation";
                ws2.Cells["D1"].Value = "Objet formation";
                ws2.Cells["E1"].Value = "Utilisateur";
                ws2.Cells["F1"].Value = "Date évaluation";
                ws2.Cells["G1"].Value = "Date limite";
                ws2.Cells["H1"].Value = "Score";
                ws2.Cells["I1"].Value = "Résultat";

                int rowStart2 = 2;
                int jour2 = 0;

                foreach (var item in list)
                {
                    //   jour++;
                    ws2.Cells[String.Format("A{0}", rowStart2)].Value = item.Code_EvalByQCM;
                    ws2.Cells[String.Format("B{0}", rowStart2)].Value = item.ObjEval;
                    ws2.Cells[String.Format("C{0}", rowStart2)].Value = item.CodeForm;
                    ws2.Cells[String.Format("D{0}", rowStart2)].Value = item.ObjForm;
                    ws2.Cells[String.Format("E{0}", rowStart2)].Value = item.Usr;
                    ws2.Cells[String.Format("F{0}", rowStart2)].Value = item.DateEval.ToShortDateString();
                    ws2.Cells[String.Format("G{0}", rowStart2)].Value = item.DeadLine.ToShortDateString();
                    ws2.Cells[String.Format("H{0}", rowStart2)].Value = item.Score + " %";
                    ws2.Cells[String.Format("I{0}", rowStart2)].Value = item.Resultat;
                    rowStart2++;
                }

                //  return RedirectToAction(jour + "");
                ws2.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("Content-disposition", "attachment:filename="+System.DateTime.Now.ToShortDateString() + "Etatattestations.xlsx");
                Response.BinaryWrite(pck2.GetAsByteArray());
                Response.End();
                return View();

            }


            //SqlDataAdapter da;
            //da = new SqlDataAdapter(" SELECT  distinct   [Code_formt] ,Objet,null,null,null,null FROM [dbo].[E_ListFormationDiffus] where MatFormateur =  '" + user.matricule + "'", con);
            //da.Fill(dt);

            SqlDataAdapter da;
            da = new SqlDataAdapter(" select distinct  E_ListEvaluationDiffus.Code_eval Code_EvalByQCM, E_ListEvaluationDiffus.Objet ObjEval, E_ListEvaluationDiffus.Code_Formation CodeForm, E_Formation.Objet ObjForm, E_ListEvaluationDiffus.Mat_usr MatUser,  E_ListEvaluationDiffus.Nom_usr Usr, E_ResultQCM.DateEval DateEval, E_ListEvaluationDiffus.DeadLine , E_ResultQCM.Score, E_ResultQCM.Resultat  from E_ListEvaluationDiffus  left outer join E_ResultQCM on E_ResultQCM.Code_EvalByQCM = E_ListEvaluationDiffus.Code_eval  left outer join E_Formation on E_Formation.Code = E_ListEvaluationDiffus.Code_Formation where E_ListEvaluationDiffus.MatFormateur = " + user.matricule + "  union   select distinct E_ListEvaluationDiffus.Code_eval Code_EvalByQCM, E_ListEvaluationDiffus.Objet ObjEval, E_ListEvaluationDiffus.Code_Formation CodeForm, E_Formation.Objet ObjForm, E_ListEvaluationDiffus.Mat_usr MatUser,  E_ListEvaluationDiffus.Nom_usr Usr, E_ResultQCM_Historiq.DateEval DateEval, E_ListEvaluationDiffus.DeadLine , E_ResultQCM_Historiq.Score, E_ResultQCM_Historiq.Resultat from E_ListEvaluationDiffus  left outer join E_ResultQCM_Historiq on E_ResultQCM_Historiq.Code_EvalByQCM = E_ListEvaluationDiffus.Code_eval  left outer join E_Formation on E_Formation.Code = E_ListEvaluationDiffus.Code_Formation where E_ListEvaluationDiffus.MatFormateur = " + user.matricule+"", con);
            da.Fill(dt);



            for (int i = 0; i < dt.Rows.Count; i++)
            {
                E_ResultQCM e = new E_ResultQCM();

                e.Code_EvalByQCM = dt.Rows[i]["Code_EvalByQCM"].ToString();

                e.ObjEval = dt.Rows[i]["ObjEval"].ToString();

                e.CodeForm = dt.Rows[i]["CodeForm"].ToString();

                e.ObjForm = dt.Rows[i]["ObjForm"].ToString();

                e.MatUser = dt.Rows[i]["MatUser"].ToString();

                e.Usr = dt.Rows[i]["Usr"].ToString();

                if (dt.Rows[i]["DateEval"].ToString() != "")
                    e.DateEval = Convert.ToDateTime(dt.Rows[i]["DateEval"].ToString());
                else
                    e.DateEval = Convert.ToDateTime( null);

                e.DeadLine = Convert.ToDateTime(dt.Rows[i]["DeadLine"].ToString());

                if (dt.Rows[i]["Score"].ToString() != "")
                    e.Score = Convert.ToInt32(dt.Rows[i]["Score"].ToString());
                else
                    e.Score = null;

                e.Resultat = dt.Rows[i]["Resultat"].ToString();


                list.Add(e);

            }


            ExcelPackage pck = new ExcelPackage();

            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Liste des évaluations diffusées");
            ws.Cells["A1"].Value = "Evaluation";
            ws.Cells["B1"].Value = "Objet évaluation";
            ws.Cells["C1"].Value = "Formation";
            ws.Cells["D1"].Value = "Objet formation";
            ws.Cells["E1"].Value = "Utilisateur";
            ws.Cells["F1"].Value = "Date évaluation";
            ws.Cells["G1"].Value = "Date limite";
            ws.Cells["H1"].Value = "Score";
            ws.Cells["I1"].Value = "Résultat";

            int rowStart = 2;
            int jour = 0;

            foreach (var item in list)
            {
                //   jour++;
                ws.Cells[String.Format("A{0}", rowStart)].Value = item.Code_EvalByQCM;
                ws.Cells[String.Format("B{0}", rowStart)].Value = item.ObjEval;
                ws.Cells[String.Format("C{0}", rowStart)].Value = item.CodeForm;
                ws.Cells[String.Format("D{0}", rowStart)].Value = item.ObjForm;
                ws.Cells[String.Format("E{0}", rowStart)].Value = item.Usr;
                ws.Cells[String.Format("F{0}", rowStart)].Value = item.DateEval.ToShortDateString();
                ws.Cells[String.Format("G{0}", rowStart)].Value = item.DeadLine.ToShortDateString();
                ws.Cells[String.Format("H{0}", rowStart)].Value = item.Score + " %";
                ws.Cells[String.Format("I{0}", rowStart)].Value = item.Resultat;
                rowStart++;
            }

            //  return RedirectToAction(jour + "");
            ws.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("Content-disposition", "attachment:filename = "  + "Etatattestations.xlsx");
            Response.BinaryWrite(pck.GetAsByteArray());
            Response.End();
            return View();
        }



        // GET: E_ListFormationDiffus
        public ActionResult Index(string searchStringCodeE, string searchStringObjet ,   string Usr , string Retour)
        {
            ViewData["CurrentFilterCodeE"] = searchStringCodeE;
            ViewData["CurrentFilterObjet"] = searchStringObjet;

            var dd = from m in db.Users
                     where m.matricule == Usr
                     select m;

            if (dd.Count() != 0)
            {
                ApplicationUser p = (from m in db.Users
                                     where m.matricule == Usr
                                     select m).Single();
                Session["UsrrEv"] = p.NomPrenom;

                Session["MatUsrrEv"] = Usr;
            }

            else
            {
                Session["UsrrEv"] = "Selectionnez un utilisateur";
                Session["MatUsrrEv"] = "";
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

            List<E_ListEvaluationDiffus> list = new List<E_ListEvaluationDiffus>();


            if (!String.IsNullOrEmpty(searchStringCodeE) || !String.IsNullOrEmpty(searchStringObjet) || !String.IsNullOrEmpty(Usr))
            {

                var r = (from m in db.e_ListEvaluationDiffus
                         where m.MatFormateur == user.matricule
                         select new { m.Code_eval, m.Objet , m.Mat_usr }).Distinct();



                E_EvaluationController.searchTermCodeE = "searchStringCodeE";
                E_EvaluationController.ValTermCodeE = searchStringCodeE;


                E_EvaluationController.searchTermObjet = "searchStringObjet";
                E_EvaluationController.ValTermObjet = searchStringObjet;

                if (!String.IsNullOrEmpty(searchStringObjet))
                    r = r.Where(s => s.Objet.ToLower().Contains(searchStringObjet.ToLower()));

                if (!String.IsNullOrEmpty(searchStringCodeE))
                    r = r.Where(s => s.Code_eval.ToLower().Contains(searchStringCodeE.ToLower()));


                if (!String.IsNullOrEmpty(Usr))
                {
                    if (Usr.Trim() != null)
                        r = r.Where(s => s.Mat_usr.Equals(Usr));
                }

                foreach (var ee in r)
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

            if (Retour == null)
            { 
                SqlDataAdapter da;
                da = new SqlDataAdapter(" SELECT  distinct top(20) dbo.[E_ListEvaluationDiffus].[Code_eval] ,f.Objet_Eval Objet , f.Date_Creation,  CONVERT(nvarchar, dbo.[E_ListEvaluationDiffus].DateDiffus, 103) DateDiffus, null, null, null FROM[dbo].[E_ListEvaluationDiffus] inner join  dbo.E_Evaluation f on f.Code_Eval =  [E_ListEvaluationDiffus].Code_eval where[E_ListEvaluationDiffus].MatFormateur = '" + user.matricule + "' order  by f.Date_Creation desc", con);
                da.Fill(dt);
            }
            if (Retour != null)
            {
                SqlDataAdapter da;
                da = new SqlDataAdapter(" SELECT  distinct   dbo.[E_ListEvaluationDiffus].[Code_eval] ,f.Objet_Eval Objet , f.Date_Creation,  CONVERT(nvarchar, dbo.[E_ListEvaluationDiffus].DateDiffus, 103) DateDiffus, null, null, null FROM[dbo].[E_ListEvaluationDiffus] inner join  dbo.E_Evaluation f on f.Code_Eval =  [E_ListEvaluationDiffus].Code_eval where[E_ListEvaluationDiffus].MatFormateur = '" + user.matricule + "' order  by f.Date_Creation desc", con);
                da.Fill(dt);
            }


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
