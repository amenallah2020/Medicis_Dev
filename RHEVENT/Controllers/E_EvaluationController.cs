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
    public class E_EvaluationController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public static string searchTermCodeE = string.Empty;

        public static string ValTermCodeE = string.Empty;

        public static string searchTermUsr = string.Empty;

        public static string ValTermUsr = string.Empty;

        public static string searchTermObjet = string.Empty;

        public static string ValTermObjet = string.Empty;

        public IQueryable<E_Evaluation> list;

        // GET: E_Evaluation
        public ActionResult Index(string searchStringCodeE, string searchStringObjet)
        {

            ViewData["CurrentFilterCodeE"] = searchStringCodeE;
            ViewData["CurrentFilterObjet"] = searchStringObjet;

            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());

              list = from m in db.E_Evaluation
                       where m.Etat_Eval== "Active" && m.Matricule_Formateur == user.matricule
                       orderby m.Date_Creation descending
                       select m;

            if (!String.IsNullOrEmpty(searchStringCodeE) || !String.IsNullOrEmpty(searchStringObjet))
            {
                 
                E_EvaluationController.searchTermCodeE = "searchStringCodeE";
                E_EvaluationController.ValTermCodeE = searchStringCodeE;


                E_EvaluationController.searchTermObjet = "searchStringObjet";
                E_EvaluationController.ValTermObjet = searchStringObjet;

                if (!String.IsNullOrEmpty(searchStringObjet))
                    list = list.Where(s => s.Objet_Eval.ToLower().Contains(searchStringObjet.ToLower()));

                if (!String.IsNullOrEmpty(searchStringCodeE))
                    list = list.Where(s => s.Code_Eval.ToLower().Contains(searchStringCodeE.ToLower()));

            }

                return View(list.ToList());
        }


        public ActionResult Exporter(string id, string searchStringUsr, string Result)
        {
            ViewData["CurrentFilterUsr"] = searchStringUsr;
            ViewData["CurrentFilterResult"] = Result;

            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());

            string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection con = new SqlConnection(constr);
            con.Open();


            List<E_ResultQCM> list = new List<E_ResultQCM>();

            if (!String.IsNullOrEmpty(searchStringUsr) || (Result != null))
            {
                var r = ((from m in db.E_ResultQCM
                          join nn in db.E_Evaluation on m.Code_EvalByQCM equals nn.Code_Eval
                          where nn.Matricule_Formateur == user.matricule && m.Code_EvalByQCM == id
                          select new { nn.Code_Eval, nn.Objet_Eval, m.CodeForm, m.ObjForm, m.Usr, nn.Date_Creation, m.DeadLine, m.Score, m.Resultat }).Distinct())
                         .Union(from m in db.E_ResultQCM_Historiq
                                join nn in db.E_Evaluation on m.Code_EvalByQCM equals nn.Code_Eval
                                where nn.Matricule_Formateur == user.matricule && m.Code_EvalByQCM == id
                                select new { nn.Code_Eval, nn.Objet_Eval, m.CodeForm, m.ObjForm, m.Usr, nn.Date_Creation, m.DeadLine, m.Score, m.Resultat }).Distinct().ToList();



                E_EvaluationController.searchTermUsr = "searchStringUsr";
                E_EvaluationController.ValTermUsr = searchStringUsr;

                if (!String.IsNullOrEmpty(searchStringUsr))
                    r = r.Where(s => s.Usr.ToLower().Contains(searchStringUsr.ToLower())).ToList();

                if (!String.IsNullOrEmpty(Result))
                    r = r.Where(s => s.Resultat.Equals(Result)).ToList();

                foreach (var ee in r)
                {
                    E_ResultQCM e_ResultQCM = new E_ResultQCM();

                    e_ResultQCM.Code_EvalByQCM = ee.Code_Eval;

                    e_ResultQCM.ObjEval = ee.Objet_Eval;

                    e_ResultQCM.CodeForm = ee.CodeForm;

                    e_ResultQCM.ObjForm = ee.ObjForm;

                    e_ResultQCM.Usr = ee.Usr;

                    e_ResultQCM.DateEval = Convert.ToDateTime(ee.Date_Creation);

                    e_ResultQCM.DeadLine = Convert.ToDateTime(ee.DeadLine);

                    e_ResultQCM.Score = Convert.ToInt32(ee.Score);

                    e_ResultQCM.Resultat = ee.Resultat;

                    list.Add(e_ResultQCM);
                }

                ExcelPackage pck2 = new ExcelPackage();

                ExcelWorksheet ws2 = pck2.Workbook.Worksheets.Add("Statistique d'évaluation");
                ws2.Cells["A1"].Value = "Evaluation";
                ws2.Cells["B1"].Value = "Objet évaluation";
                ws2.Cells["C1"].Value = "Formation";
                ws2.Cells["D1"].Value = "Objet formation";
                ws2.Cells["E1"].Value = "Utilisateur";
                ws2.Cells["F1"].Value = "Date évaluation";
                ws2.Cells["J1"].Value = "Date limite";
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
                    ws2.Cells[String.Format("J{0}", rowStart2)].Value = item.DeadLine.ToShortDateString();
                    ws2.Cells[String.Format("H{0}", rowStart2)].Value = item.Score+" %";
                    ws2.Cells[String.Format("I{0}", rowStart2)].Value = item.Resultat;
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

            DataTable dt = new DataTable();

            SqlDataAdapter da;
            da = new SqlDataAdapter("select distinct Code_Eval, E_Evaluation.Objet_Eval , E_ResultQCM.CodeForm,  ObjForm, Usr, Date_Creation, DeadLine , Score, Resultat   from E_ResultQCM  inner join E_Evaluation on E_Evaluation.Code_Eval = E_ResultQCM.Code_EvalByQCM where E_Evaluation.Matricule_Formateur = " + user.matricule + " and Code_Eval =  '" + id + "' union select distinct Code_Eval, E_Evaluation.Objet_Eval , [E_ResultQCM_Historiq].CodeForm,  ObjForm, Usr, Date_Creation, DeadLine , Score, Resultat   from[E_ResultQCM_Historiq] inner join E_Evaluation on E_Evaluation.Code_Eval = [E_ResultQCM_Historiq].Code_EvalByQCM  where E_Evaluation.Matricule_Formateur = " + user.matricule + " and Code_Eval = '" + id + "'   ", con);
            da.Fill(dt);



            for (int i = 0; i < dt.Rows.Count; i++)
            {
                E_ResultQCM e_ResultQCM = new E_ResultQCM();

                e_ResultQCM.Code_EvalByQCM = dt.Rows[i]["Code_Eval"].ToString();

                e_ResultQCM.ObjEval = dt.Rows[i]["Objet_Eval"].ToString();

                e_ResultQCM.CodeForm = dt.Rows[i]["CodeForm"].ToString();

                e_ResultQCM.ObjForm = dt.Rows[i]["ObjForm"].ToString();

                e_ResultQCM.Usr = dt.Rows[i]["Usr"].ToString();

                e_ResultQCM.DateEval = Convert.ToDateTime(dt.Rows[i]["Date_Creation"].ToString());

                e_ResultQCM.DeadLine = Convert.ToDateTime(dt.Rows[i]["DeadLine"].ToString());

                e_ResultQCM.Score = Convert.ToInt32(dt.Rows[i]["Score"].ToString());

                e_ResultQCM.Resultat = dt.Rows[i]["Resultat"].ToString();

                list.Add(e_ResultQCM);

            }


            ExcelPackage pck = new ExcelPackage();

            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Statistique d'évaluation");
            ws.Cells["A1"].Value = "Evaluation";
            ws.Cells["B1"].Value = "Objet évaluation";
            ws.Cells["C1"].Value = "Formation";
            ws.Cells["D1"].Value = "Objet formation";
            ws.Cells["E1"].Value = "Utilisateur";
            ws.Cells["F1"].Value = "Date évaluation";
            ws.Cells["J1"].Value = "Date limite";
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
                ws.Cells[String.Format("J{0}", rowStart)].Value = item.DeadLine.ToShortDateString();
                ws.Cells[String.Format("H{0}", rowStart)].Value = item.Score + " %"; ;
                ws.Cells[String.Format("I{0}", rowStart)].Value = item.Resultat;
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


        public ActionResult Statistique(string id,string searchStringUsr, string Result)
        {
            ViewData["CurrentFilterUsr"] = searchStringUsr;
            ViewData["CurrentFilterResult"] = Result;

            Session["Cde"] = id;

            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());

            string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection con = new SqlConnection(constr);
            con.Open();


            List<E_ResultQCM> list = new List<E_ResultQCM>();

            if (!String.IsNullOrEmpty(searchStringUsr) || (Result != null))
            {
                var r = ((from m in db.E_ResultQCM
                          join nn in db.E_Evaluation on m.Code_EvalByQCM equals nn.Code_Eval
                          where nn.Matricule_Formateur == user.matricule && m.Code_EvalByQCM == id
                          select new { nn.Code_Eval, nn.Objet_Eval, m.CodeForm, m.ObjForm, m.Usr, nn.Date_Creation, m.DeadLine, m.Score, m.Resultat }).Distinct())
                         .Union(from m in db.E_ResultQCM_Historiq
                                join nn in db.E_Evaluation on m.Code_EvalByQCM equals nn.Code_Eval
                                where nn.Matricule_Formateur == user.matricule && m.Code_EvalByQCM == id
                                select new { nn.Code_Eval, nn.Objet_Eval, m.CodeForm, m.ObjForm, m.Usr, nn.Date_Creation, m.DeadLine, m.Score, m.Resultat }).Distinct().ToList();



                E_EvaluationController.searchTermUsr = "searchStringUsr";
                E_EvaluationController.ValTermUsr = searchStringUsr;

                if (!String.IsNullOrEmpty(searchStringUsr))
                    r = r.Where(s => s.Usr.ToLower().Contains(searchStringUsr.ToLower())).ToList();

                if (!String.IsNullOrEmpty(Result))
                    r = r.Where(s => s.Resultat.Equals(Result)).ToList();

                foreach (var ee in r)
                {
                    E_ResultQCM e_ResultQCM = new E_ResultQCM();

                    e_ResultQCM.Code_EvalByQCM = ee.Code_Eval;

                    e_ResultQCM.ObjEval = ee.Objet_Eval;

                    e_ResultQCM.CodeForm = ee.CodeForm;

                    e_ResultQCM.ObjForm = ee.ObjForm;

                    e_ResultQCM.Usr = ee.Usr;

                    e_ResultQCM.DateEval = Convert.ToDateTime(ee.Date_Creation);

                    e_ResultQCM.DeadLine = Convert.ToDateTime(ee.DeadLine);

                    e_ResultQCM.Score = Convert.ToInt32(ee.Score);

                    e_ResultQCM.Resultat = ee.Resultat;

                    list.Add(e_ResultQCM);
                }

                return View(list);
            }

            DataTable dt = new DataTable();

            SqlDataAdapter da;
            da = new SqlDataAdapter("select distinct Code_Eval, E_Evaluation.Objet_Eval , E_ResultQCM.CodeForm,  ObjForm, Usr, Date_Creation, DeadLine , Score, Resultat   from E_ResultQCM  inner join E_Evaluation on E_Evaluation.Code_Eval = E_ResultQCM.Code_EvalByQCM where E_Evaluation.Matricule_Formateur = "+user.matricule+" and Code_Eval =  '" + id + "' union select distinct Code_Eval, E_Evaluation.Objet_Eval , [E_ResultQCM_Historiq].CodeForm,  ObjForm, Usr, Date_Creation, DeadLine , Score, Resultat   from[E_ResultQCM_Historiq] inner join E_Evaluation on E_Evaluation.Code_Eval = [E_ResultQCM_Historiq].Code_EvalByQCM  where E_Evaluation.Matricule_Formateur = "+user.matricule+" and Code_Eval = '" + id + "'   ", con);
            da.Fill(dt);

            

            for (int i=0; i< dt.Rows.Count; i++)
            {
                E_ResultQCM e_ResultQCM = new E_ResultQCM();

                e_ResultQCM.Code_EvalByQCM = dt.Rows[i]["Code_Eval"].ToString();

                e_ResultQCM.ObjEval = dt.Rows[i]["Objet_Eval"].ToString();

                e_ResultQCM.CodeForm = dt.Rows[i]["CodeForm"].ToString();

                e_ResultQCM.ObjForm = dt.Rows[i]["ObjForm"].ToString();

                e_ResultQCM.Usr = dt.Rows[i]["Usr"].ToString();

                e_ResultQCM.DateEval = Convert.ToDateTime( dt.Rows[i]["Date_Creation"].ToString());

                e_ResultQCM.DeadLine = Convert.ToDateTime(dt.Rows[i]["DeadLine"].ToString());

                e_ResultQCM.Score = Convert.ToInt32 ( dt.Rows[i]["Score"].ToString());

                e_ResultQCM.Resultat = dt.Rows[i]["Resultat"].ToString();

                list.Add(e_ResultQCM);

            }


            return View(list);
        }





        // GET: E_Evaluation/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            E_Evaluation e_Evaluation = db.E_Evaluation.Find(id);
            if (e_Evaluation == null)
            {
                return HttpNotFound();
            }
            return View(e_Evaluation);
        }

        // GET: E_Evaluation/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: E_Evaluation/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Code_Eval,Etat_Eval,Date_Creation,Matricule_Formateur,Objet_Eval,Pourc_Valid,Duree_Eval")] E_Evaluation e_Evaluation)
        {
            if (ModelState.IsValid)
            {

                ApplicationUser user = db.Users.Find(User.Identity.GetUserId());

                e_Evaluation.Matricule_Formateur = user.matricule;

                e_Evaluation.Etat_Eval = "Active";

                e_Evaluation.Date_Creation = System.DateTime.Now;

                //e_Evaluation.Date_Modif = Convert.ToDateTime("01-01-0001");

                   
                string OldChaine = System.DateTime.Now.ToShortDateString();

                string NewChaine = OldChaine.Replace("/", "");

                string date = System.DateTime.Now.ToShortDateString();

                string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
                SqlConnection con = new SqlConnection(constr);
                con.Open();

                DataTable dt = new DataTable();

                SqlDataAdapter da;
                da = new SqlDataAdapter("SELECT  * FROM e_Evaluation where CONVERT(VARCHAR(10), Date_Creation, 103)='" + date + "'", con);
                da.Fill(dt);

                int a = dt.Rows.Count;

                int p = a + 1;

                if (p <= 9)
                    e_Evaluation.Code_Eval = "Eval_" + NewChaine + "_0" + (a + 1);
                else
                    e_Evaluation.Code_Eval = "Eval_" + NewChaine + "_" + (a + 1);

                //e_Evaluation.Code_Eval = "Eval_" + NewChaine + "_1" + (a + 1);

                SqlCommand cmd = new SqlCommand("Insert into E_Evaluation (Code_Eval,Etat_Eval,Date_Creation,Matricule_Formateur,Objet_Eval,Pourc_Valid,Duree_Eval) values(@Code_Eval,@Etat_Eval,@Date_Creation,@Matricule_Formateur,@Objet_Eval,@Pourc_Valid,@Duree_Eval)", con);

                cmd.Parameters.AddWithValue("@Code_Eval", e_Evaluation.Code_Eval);
                cmd.Parameters.AddWithValue("@Etat_Eval", e_Evaluation.Etat_Eval);
                cmd.Parameters.AddWithValue("@Date_Creation", e_Evaluation.Date_Creation);
                cmd.Parameters.AddWithValue("@Matricule_Formateur", e_Evaluation.Matricule_Formateur);
                cmd.Parameters.AddWithValue("@Objet_Eval", e_Evaluation.Objet_Eval);
                cmd.Parameters.AddWithValue("@Pourc_Valid", e_Evaluation.Pourc_Valid);
                cmd.Parameters.AddWithValue("@Duree_Eval", e_Evaluation.Duree_Eval);
                 
                cmd.ExecuteNonQuery();


                //db.E_Evaluation.Add(e_Evaluation);
                //db.SaveChanges();
                return RedirectToAction("Index");
 
            }

            return View(e_Evaluation);
        }

        // GET: E_Evaluation/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            E_Evaluation e_Evaluation = db.E_Evaluation.Find(id);
            if (e_Evaluation == null)
            {
                return HttpNotFound();
            }
            return View(e_Evaluation);
        }

        // POST: E_Evaluation/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Code_Eval,Etat_Eval,Date_Creation,Matricule_Formateur,Objet_Eval,Pourc_Valid,Duree_Eval")] E_Evaluation e_Evaluation)
        {
            if (ModelState.IsValid)
            {
                var list = from m in db.E_Evaluation
                           where m.Id == e_Evaluation.Id
                           select m;


                foreach (E_Evaluation e in list)
                {
                    e_Evaluation.Matricule_Formateur = e.Matricule_Formateur;

                    e_Evaluation.Date_Creation = e.Date_Creation;

                    e_Evaluation.Etat_Eval = e.Etat_Eval;

                    e_Evaluation.Code_Eval = e.Code_Eval;


                }

                string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
                SqlConnection con = new SqlConnection(constr);
                con.Open();


                SqlCommand cmd = new SqlCommand("UPDATE E_Evaluation set Date_Modif= '"+System.DateTime.Now+"', Duree_Eval =  '" + e_Evaluation.Duree_Eval + "' , Pourc_Valid = " + e_Evaluation.Pourc_Valid + " ,  Objet_Eval = '" + e_Evaluation.Objet_Eval + "' where  Code_Eval= '" + e_Evaluation.Code_Eval+"'  ", con);
                cmd.ExecuteNonQuery();


                db.SaveChanges();


                return RedirectToAction("Index");
            }
            return View(e_Evaluation);
        }

        // GET: E_Evaluation/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            E_Evaluation e_Evaluation = db.E_Evaluation.Find(id);
            if (e_Evaluation == null)
            {
                return HttpNotFound();
            }
            return View(e_Evaluation);
        }

        // POST: E_Evaluation/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            E_Evaluation e_Evaluation = db.E_Evaluation.Find(id);
            db.E_Evaluation.Remove(e_Evaluation);
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
