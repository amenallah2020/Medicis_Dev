using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using OfficeOpenXml;
using RHEVENT.Models;

namespace RHEVENT.Controllers
{
    public class E_FormationController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

         
        public static string searchTermCodeF = string.Empty;

        public static string ValTermCodeF = string.Empty;

        public static string searchTermUsr = string.Empty;

        public static string ValTermUsr = string.Empty;

        public static string searchTermCodeE = string.Empty;

        public static string ValTermCodeE = string.Empty;

        public static string searchTermObjet = string.Empty;

        public static string ValTermObjet = string.Empty;

        public IQueryable<string> recherche;



        public ActionResult AddEval(string CodeF)
        {
            Session["CodeForm"] = CodeF;

            var list = from m in db.E_Formation
                       where m.Code == CodeF
                       select m;

            foreach (E_Formation e in list)
            {
                Session["objetForm"] = e.Objet;

            }

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddEval([Bind(Include = "Id,Code_Eval,Etat_Eval,Date_Creation,Matricule_Formateur,Objet_Eval,Pourc_Valid,Duree_Eval")] E_Evaluation e_Evaluation, string CodeForm)
        {
            if (ModelState.IsValid)
            {

                ApplicationUser user = db.Users.Find(User.Identity.GetUserId());

                e_Evaluation.Matricule_Formateur = user.matricule;

                e_Evaluation.Etat_Eval = "Active";

                e_Evaluation.Date_Creation = System.DateTime.Now;

                e_Evaluation.Date_Modif = System.DateTime.Now;


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


                SqlCommand cmd = new SqlCommand("Insert into E_Evaluation (Code_Eval,CodeForm,Etat_Eval,Date_Creation,Date_Modif,Matricule_Formateur,Objet_Eval,Pourc_Valid,Duree_Eval) values(@Code_Eval,@CodeForm,@Etat_Eval,@Date_Creation,@Date_Modif,@Matricule_Formateur,@Objet_Eval,@Pourc_Valid,@Duree_Eval)", con);

                cmd.Parameters.AddWithValue("@Code_Eval", e_Evaluation.Code_Eval);
                cmd.Parameters.AddWithValue("@Etat_Eval", e_Evaluation.Etat_Eval);
                cmd.Parameters.AddWithValue("@Date_Creation", e_Evaluation.Date_Creation);
                cmd.Parameters.AddWithValue("@Date_Modif", e_Evaluation.Date_Modif);
                cmd.Parameters.AddWithValue("@Matricule_Formateur", e_Evaluation.Matricule_Formateur);
                cmd.Parameters.AddWithValue("@Objet_Eval", e_Evaluation.Objet_Eval);
                cmd.Parameters.AddWithValue("@Pourc_Valid", e_Evaluation.Pourc_Valid);
                cmd.Parameters.AddWithValue("@Duree_Eval", e_Evaluation.Duree_Eval);
                cmd.Parameters.AddWithValue("@CodeForm", CodeForm);

                cmd.ExecuteNonQuery();

                SqlCommand cmd2 = new SqlCommand("update E_Formation set CodeEval = '" + e_Evaluation.Code_Eval + "' where Code = '" + CodeForm + "' ", con);
                cmd2.ExecuteNonQuery();

                //db.E_Evaluation.Add(e_Evaluation);
                //db.SaveChanges();

            }

            return RedirectToAction("ListQCM", "E_Formation", new { codeEval = e_Evaluation.Code_Eval, codeF = CodeForm, EtatDiff = e_Evaluation.EtatDiff });

            //return View(e_Evaluation);
        }



        public ActionResult ListQCM(string codeEval, string codeF, string EtatDiff)
        {
            var list = from m in db.E_Evaluation
                       where m.Code_Eval == codeEval
                       select m;

            foreach (E_Evaluation r in list)
            {
                ViewBag.codeEval = codeEval;

                ViewBag.objetEval = r.Objet_Eval;

            }


            var lf = from m in db.E_Formation
                     where m.Code == codeF
                     select m;

            foreach (E_Formation e in lf)
            {
                ViewBag.codeFor = e.Code;

                ViewBag.objetFor = e.Objet;

            }

            ViewBag.EtatD = EtatDiff;

            var l = from m in db.E_QCM
                    where m.Code_EvalByQCM == codeEval
                    orderby m.Date_Creation descending
                    select m;

            return View(l.ToList());
        }

        public ActionResult AddQCM(string codeEval, string codeF)
        {
            ViewBag.codeEval = codeEval;

            ViewBag.codeForm = codeF;

            return View();
        }


        public ActionResult EditQCM(int? id, string codeEval, string codef)
        {
            ViewBag.codeEval = codeEval;

            ViewBag.codeFor = codef;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            E_QCM e_QCM = db.E_QCM.Find(id);
            if (e_QCM == null)
            {
                return HttpNotFound();
            }
            return View(e_QCM);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditQCM([Bind(Include = "Id,Code_QCM,Code_EvalByQCM,Date_Creation,Question,Reponse1,Reponse2,Reponse3,Reponse4,Coeff")] E_QCM e_QCM, string EtatRep1, string EtatRep2, string EtatRep3, string EtatRep4, string codeForm)
        {
            if (ModelState.IsValid)
            {

                //if (EtatRep1 == "Faux" && EtatRep2 == "Faux" && EtatRep3 == "Faux" && EtatRep4 == "Faux")
                //{
                //    ModelState.AddModelError("", "Il faut une réponse vrai.");
                //    ViewBag.codeEval = e_QCM.Code_EvalByQCM;
                //    ViewBag.codeFor = codeForm;

                //    return View(e_QCM);
                //}

                //int countA = 0;

                //List<string> aaa = new List<string>();

                //aaa.Add(EtatRep1);

                //aaa.Add(EtatRep2);

                //aaa.Add(EtatRep3);

                //aaa.Add(EtatRep4);

                //for (int i = 0; i < aaa.Count; i++)
                //{
                //    if (aaa[i].ToString() == "Vrai")
                //    {
                //        countA += 1;
                //    }
                //}

                //if (countA > 1)
                //{
                //    ModelState.AddModelError("", "Il faut une seule réponse vrai.");
                //    ViewBag.codeEval = e_QCM.Code_EvalByQCM;
                //    ViewBag.codeFor = codeForm;

                //    return View(e_QCM);
                //}



                //e_QCM.Date_Modif = System.DateTime.Now;


                string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
                SqlConnection con = new SqlConnection(constr);
                con.Open();


                SqlCommand cmd = new SqlCommand("UPDATE e_QCM set Date_Modif= '" + System.DateTime.Now + "', Question =  '" + e_QCM.Question + "' , Reponse1 = '" + e_QCM.Reponse1 + "' , EtatRep1 = '" + EtatRep1 + "' ,   Reponse2 = '" + e_QCM.Reponse2 + "', EtatRep2 = '" + EtatRep2 + "' , Reponse3 = '" + e_QCM.Reponse3 + "', EtatRep3 = '" + EtatRep3 + "' , Reponse4 = '" + e_QCM.Reponse4 + "', EtatRep4 = '" + EtatRep4 + "' ,  Coeff = " + e_QCM.Coeff + " where  Id= '" + e_QCM.Id + "'  ", con);
                cmd.ExecuteNonQuery();

                //db.Entry(e_QCM).State = EntityState.Detached;

                //db.Entry(e_QCM).State = EntityState.Modified;

                db.SaveChanges();


                var list = from m in db.E_QCM
                           where m.Id == e_QCM.Id
                           select m;


                foreach (E_QCM e in list)
                {
                    e_QCM.Code_EvalByQCM = e.Code_EvalByQCM;

                    e_QCM.Code_QCM = e.Code_QCM;

                    e_QCM.Date_Creation = e.Date_Creation;

                    e_QCM.EtatRep1 = e.EtatRep1;

                    e_QCM.EtatRep2 = e.EtatRep2;

                    e_QCM.EtatRep3 = e.EtatRep3;

                    e_QCM.EtatRep4 = e.EtatRep4;


                }

                return RedirectToAction("ListQCM", new { codeEval = e_QCM.Code_EvalByQCM, codeF = codeForm });
            }

            return View(e_QCM);
        }


        public ActionResult ListEval(string CodeEval, string CodeF)
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());

            var list = from m in db.E_Evaluation
                       where m.Code_Eval == CodeEval
                       select m;

            ViewBag.CodeF = CodeF;


            var fo = from m in db.E_Formation
                     where m.Code == CodeF
                     select m;


            foreach (E_Formation ee in fo)
            {
                ViewBag.objetFor = ee.Objet;
            }

            return View(list.ToList());
        }

        public ActionResult EditEval(int? id, string CodeF)
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

            ViewBag.CodeF = CodeF;

            ViewBag.CodeEval = e_Evaluation.Code_Eval;

            return View(e_Evaluation);
        }


        [HttpPost]
        public ActionResult SuppEval(string CodeF, string CodeEval)
        {
            string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection con = new SqlConnection(constr);
            con.Open();



            SqlCommand cmd = new SqlCommand("update E_Formation set CodeEval = null where CodeEval  = '" + CodeEval + "'", con);
            cmd.ExecuteNonQuery();

            SqlCommand cmd2 = new SqlCommand("delete from E_QCM where Code_EvalByQCM = '" + CodeEval + "'", con);
            cmd2.ExecuteNonQuery();

            SqlCommand cmd3 = new SqlCommand("delete from E_Evaluation  where Code_Eval  = '" + CodeEval + "'", con);
            cmd3.ExecuteNonQuery();


            return RedirectToAction("Index", "E_Formation");
        }

        // POST: E_Evaluation/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditEval([Bind(Include = "Id,Code_Eval,Etat_Eval,Date_Creation,Matricule_Formateur,Objet_Eval,Pourc_Valid,Duree_Eval")] E_Evaluation e_Evaluation, string CodeF)
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


                SqlCommand cmd = new SqlCommand("UPDATE E_Evaluation set Date_Modif= '" + System.DateTime.Now + "', Duree_Eval =  '" + e_Evaluation.Duree_Eval + "' , Pourc_Valid = " + e_Evaluation.Pourc_Valid + " ,  Objet_Eval = '" + e_Evaluation.Objet_Eval + "' where  Code_Eval= '" + e_Evaluation.Code_Eval + "'  ", con);
                cmd.ExecuteNonQuery();


                db.SaveChanges();


                return RedirectToAction("ListEval", "E_Formation", new { CodeF = CodeF, CodeEval = e_Evaluation.Code_Eval });
            }
            return View(e_Evaluation);
        }


        public ActionResult DeleteQCM(int? id, string codeEval, string codeF)
        {
            ViewBag.codeEval = codeEval;

            ViewBag.codeFor = codeF;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            E_QCM e_QCM = db.E_QCM.Find(id);
            if (e_QCM == null)
            {
                return HttpNotFound();
            }
            return View(e_QCM);
        }

        // POST: E_QCM/Delete/5
        [HttpPost, ActionName("DeleteQCM")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, string codeForm)
        {
            E_QCM e_QCM = db.E_QCM.Find(id);
            db.E_QCM.Remove(e_QCM);
            db.SaveChanges();
            return RedirectToAction("ListQCM", new { codeEval = e_QCM.Code_EvalByQCM, CodeF = codeForm });
        }




        // POST: E_QCM/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddQCM([Bind(Include = "Id,Code_QCM,Code_EvalByQCM,Date_Creation,Question,Reponse1,Reponse2,Reponse3, Reponse4,Coeff")] E_QCM e_QCM, string codeEvl, string codeF, string EtatRep1, string EtatRep2, string EtatRep3, string EtatRep4)
        {
            if (ModelState.IsValid)
            {
                if (EtatRep1 == "Faux" && EtatRep2 == "Faux" && EtatRep3 == "Faux" && EtatRep4 == "Faux")
                {
                    ModelState.AddModelError("", "Il faut une réponse vrai.");
                    ViewBag.codeEval = codeEvl;
                    ViewBag.codeForm = codeF;

                    return View(e_QCM);
                }

                int countA = 0;

                List<string> aaa = new List<string>();

                aaa.Add(EtatRep1);

                aaa.Add(EtatRep2);

                aaa.Add(EtatRep3);

                aaa.Add(EtatRep4);

                for (int i = 0; i < aaa.Count; i++)
                {
                    if (aaa[i].ToString() == "Vrai")
                    {
                        countA += 1;
                    }
                }

                //if (countA > 1)
                //{
                //    ModelState.AddModelError("", "Il faut une seule réponse vrai.");
                //    ViewBag.codeEval = codeEvl;
                //    ViewBag.codeForm = codeF;
                //    return View(e_QCM);
                //}


                e_QCM.Code_EvalByQCM = codeEvl;

                e_QCM.Date_Creation = System.DateTime.Now;

                //, string coeff, string Reponse1, string Reponse2, string Reponse3,string Reponse4, string Question
                //e_QCM.Question = Question;

                //e_QCM.Reponse1 = Reponse1;

                //e_QCM.Reponse2 = Reponse2;

                //e_QCM.Reponse3 = Reponse3;

                //e_QCM.Reponse4 = Reponse4;

                //e_QCM.Coeff = Convert.ToInt32(coeff);

                var qcm = from m in db.E_QCM
                          where m.Code_EvalByQCM == e_QCM.Code_EvalByQCM
                          select m;

                int o = qcm.Count();

                if (o != 0)
                {  
                    e_QCM.NumQ = o + 1; 
                }
                else
                {
                    e_QCM.NumQ = 1;
                }

                string OldChaine = System.DateTime.Now.ToShortDateString();

                string NewChaine = OldChaine.Replace("/", "");

                string date = System.DateTime.Now.ToShortDateString();

                string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
                SqlConnection con = new SqlConnection(constr);
                con.Open();

                DataTable dt = new DataTable();

                SqlDataAdapter da;
                da = new SqlDataAdapter("SELECT  * FROM E_QCM where CONVERT(VARCHAR(10), Date_Creation, 103)='" + date + "'", con);
                da.Fill(dt);

                int a = dt.Rows.Count;

                int p = a + 1;

                if (p <= 9)
                    e_QCM.Code_QCM = "QCM_" + NewChaine + "_0" + (a + 1);
                else
                    e_QCM.Code_QCM = "QCM_" + NewChaine + "_1" + (a + 1);


                e_QCM.Date_Modif = Convert.ToDateTime("01-01-1800");

                e_QCM.EtatRep1 = EtatRep1;

                e_QCM.EtatRep2 = EtatRep2;

                e_QCM.EtatRep3 = EtatRep3;

                e_QCM.EtatRep4 = EtatRep4;

                if (e_QCM.Reponse3 != null && e_QCM.Reponse4 != null)
                {
                    SqlCommand cmd = new SqlCommand("Insert into E_QCM (Code_QCM,NumQ, Code_EvalByQCM,Date_Creation,Question,Coeff,Reponse1,Reponse2,Reponse3,Reponse4,Date_Modif,EtatRep1,EtatRep2,EtatRep3,EtatRep4) values(@Code_QCM, @NumQ, @Code_EvalByQCM,@Date_Creation,@Question,@Coeff,@Reponse1,@Reponse2,@Reponse3,@Reponse4,@Date_Modif,@EtatRep1,@EtatRep2,@EtatRep3,@EtatRep4)", con);

                    cmd.Parameters.AddWithValue("@Code_QCM", e_QCM.Code_QCM);
                    cmd.Parameters.AddWithValue("@NumQ", e_QCM.NumQ);
                    cmd.Parameters.AddWithValue("@Code_EvalByQCM", e_QCM.Code_EvalByQCM);
                    cmd.Parameters.AddWithValue("@Date_Creation", e_QCM.Date_Creation);
                    cmd.Parameters.AddWithValue("@Question", e_QCM.Question);
                    cmd.Parameters.AddWithValue("@Reponse1", e_QCM.Reponse1);
                    cmd.Parameters.AddWithValue("@Reponse2", e_QCM.Reponse2);
                    cmd.Parameters.AddWithValue("@Reponse3", e_QCM.Reponse3);
                    cmd.Parameters.AddWithValue("@Reponse4", e_QCM.Reponse4);
                    cmd.Parameters.AddWithValue("@Date_Modif", e_QCM.Date_Modif);
                    cmd.Parameters.AddWithValue("@EtatRep1", e_QCM.EtatRep1);
                    cmd.Parameters.AddWithValue("@EtatRep2", e_QCM.EtatRep2);
                    cmd.Parameters.AddWithValue("@EtatRep3", e_QCM.EtatRep3);
                    cmd.Parameters.AddWithValue("@EtatRep4", e_QCM.EtatRep4);
                    cmd.Parameters.AddWithValue("@Coeff", e_QCM.Coeff);

                    cmd.ExecuteNonQuery();
                }

                else if (e_QCM.Reponse3 == null && e_QCM.Reponse4 == null)
                {
                    SqlCommand cmd = new SqlCommand("Insert into E_QCM (Code_QCM, NumQ , Code_EvalByQCM,Date_Creation,Question,Coeff,Reponse1,Reponse2,Date_Modif,EtatRep1,EtatRep2,EtatRep3,EtatRep4) values(@Code_QCM, @NumQ, @Code_EvalByQCM,@Date_Creation,@Question,@Coeff,@Reponse1,@Reponse2,@Date_Modif,@EtatRep1,@EtatRep2,@EtatRep3,@EtatRep4)", con);

                    cmd.Parameters.AddWithValue("@Code_QCM", e_QCM.Code_QCM);
                    cmd.Parameters.AddWithValue("@NumQ", e_QCM.NumQ);
                    cmd.Parameters.AddWithValue("@Code_EvalByQCM", e_QCM.Code_EvalByQCM);
                    cmd.Parameters.AddWithValue("@Date_Creation", e_QCM.Date_Creation);
                    cmd.Parameters.AddWithValue("@Question", e_QCM.Question);
                    cmd.Parameters.AddWithValue("@Reponse1", e_QCM.Reponse1);
                    cmd.Parameters.AddWithValue("@Reponse2", e_QCM.Reponse2);
                    //cmd.Parameters.AddWithValue("@Reponse3", e_QCM.Reponse3);
                    //cmd.Parameters.AddWithValue("@Reponse4", e_QCM.Reponse4);
                    cmd.Parameters.AddWithValue("@Date_Modif", e_QCM.Date_Modif);
                    cmd.Parameters.AddWithValue("@EtatRep1", e_QCM.EtatRep1);
                    cmd.Parameters.AddWithValue("@EtatRep2", e_QCM.EtatRep2);
                    cmd.Parameters.AddWithValue("@EtatRep3", e_QCM.EtatRep3);
                    cmd.Parameters.AddWithValue("@EtatRep4", e_QCM.EtatRep4);
                    cmd.Parameters.AddWithValue("@Coeff", e_QCM.Coeff);

                    cmd.ExecuteNonQuery();
                }

                else if (e_QCM.Reponse3 != null)
                {
                    SqlCommand cmd = new SqlCommand("Insert into E_QCM (Code_QCM, NumQ, Code_EvalByQCM,Date_Creation,Question,Coeff,Reponse1,Reponse2,Reponse3,Date_Modif,EtatRep1,EtatRep2,EtatRep3,EtatRep4) values(@Code_QCM, @NumQ, @Code_EvalByQCM,@Date_Creation,@Question,@Coeff,@Reponse1,@Reponse2,@Reponse3,@Date_Modif,@EtatRep1,@EtatRep2,@EtatRep3,@EtatRep4)", con);

                    cmd.Parameters.AddWithValue("@Code_QCM", e_QCM.Code_QCM);
                    cmd.Parameters.AddWithValue("@NumQ", e_QCM.NumQ);
                    cmd.Parameters.AddWithValue("@Code_EvalByQCM", e_QCM.Code_EvalByQCM);
                    cmd.Parameters.AddWithValue("@Date_Creation", e_QCM.Date_Creation);
                    cmd.Parameters.AddWithValue("@Question", e_QCM.Question);
                    cmd.Parameters.AddWithValue("@Reponse1", e_QCM.Reponse1);
                    cmd.Parameters.AddWithValue("@Reponse2", e_QCM.Reponse2);
                    cmd.Parameters.AddWithValue("@Reponse3", e_QCM.Reponse3);
                    //cmd.Parameters.AddWithValue("@Reponse4", e_QCM.Reponse4);
                    cmd.Parameters.AddWithValue("@Date_Modif", e_QCM.Date_Modif);
                    cmd.Parameters.AddWithValue("@EtatRep1", e_QCM.EtatRep1);
                    cmd.Parameters.AddWithValue("@EtatRep2", e_QCM.EtatRep2);
                    cmd.Parameters.AddWithValue("@EtatRep3", e_QCM.EtatRep3);
                    cmd.Parameters.AddWithValue("@EtatRep4", e_QCM.EtatRep4);
                    cmd.Parameters.AddWithValue("@Coeff", e_QCM.Coeff);

                    cmd.ExecuteNonQuery();
                }

                else if (e_QCM.Reponse4 != null)
                {
                    SqlCommand cmd = new SqlCommand("Insert into E_QCM (Code_QCM, NumQ, Code_EvalByQCM,Date_Creation,Question,Coeff,Reponse1,Reponse2,Reponse3,Date_Modif,EtatRep1,EtatRep2,EtatRep3,EtatRep4) values(@Code_QCM, @NumQ, @Code_EvalByQCM,@Date_Creation,@Question,@Coeff,@Reponse1,@Reponse2,@Reponse4,@Date_Modif,@EtatRep1,@EtatRep2,@EtatRep3,@EtatRep4)", con);

                    cmd.Parameters.AddWithValue("@Code_QCM", e_QCM.Code_QCM);
                    cmd.Parameters.AddWithValue("@NumQ", e_QCM.NumQ);
                    cmd.Parameters.AddWithValue("@Code_EvalByQCM", e_QCM.Code_EvalByQCM);
                    cmd.Parameters.AddWithValue("@Date_Creation", e_QCM.Date_Creation);
                    cmd.Parameters.AddWithValue("@Question", e_QCM.Question);
                    cmd.Parameters.AddWithValue("@Reponse1", e_QCM.Reponse1);
                    cmd.Parameters.AddWithValue("@Reponse2", e_QCM.Reponse2);
                    //cmd.Parameters.AddWithValue("@Reponse3", e_QCM.Reponse3);
                    cmd.Parameters.AddWithValue("@Reponse4", e_QCM.Reponse4);
                    cmd.Parameters.AddWithValue("@Date_Modif", e_QCM.Date_Modif);
                    cmd.Parameters.AddWithValue("@EtatRep1", e_QCM.EtatRep1);
                    cmd.Parameters.AddWithValue("@EtatRep2", e_QCM.EtatRep2);
                    cmd.Parameters.AddWithValue("@EtatRep3", e_QCM.EtatRep3);
                    cmd.Parameters.AddWithValue("@EtatRep4", e_QCM.EtatRep4);
                    cmd.Parameters.AddWithValue("@Coeff", e_QCM.Coeff);

                    cmd.ExecuteNonQuery();
                }

                //db.E_QCM.Add(e_QCM);
                db.SaveChanges();
                return RedirectToAction("ListQCM", new { codeEval = e_QCM.Code_EvalByQCM, codeF = codeF });
            }

            return View(e_QCM);
        }

        public ActionResult Exporter(string id, string searchStringCodeF, string searchStringObjet, string searchStringUsr, string Etat)
        {

            //string pp = Session["Cdf"].ToString();

            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());

            string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection con = new SqlConnection(constr);
            con.Open();

            

            ViewData["CurrentFilterCodeF"] = searchStringCodeF;
            ViewData["CurrentFilterUsr"] = searchStringUsr;
            ViewData["CurrentFilterObjet"] = searchStringObjet;
            ViewData["CurrentFilterEtat"] = Etat;

            List<E_ResultFormation> list = new List<E_ResultFormation>();


            if (!String.IsNullOrEmpty(searchStringCodeF) || !String.IsNullOrEmpty(searchStringUsr) || !String.IsNullOrEmpty(searchStringObjet) || (Etat != null))
            {
                //var r = (from m in db.e_ListFormationDiffus
                //         join nn in db.E_ResultFormation on m.Code_formt equals nn.Code_Formation
                //         join p in db.E_Formation on m.Code_formt equals p.Code
                //         where m.MatFormateur == user.matricule && m.Code_formt == id
                //         select new { m.Code_formt, m.Objet, m.Mat_usr, m.Nom_usr, m.deadline, m.DateDiffus, Etat = (nn.Resultat == "Complete" ? "Complete" : "Incomplete") }).Distinct();


                DataTable dtz = new DataTable();

                if (!String.IsNullOrEmpty(searchStringUsr))
                {
                    SqlDataAdapter daz;

                    daz = new SqlDataAdapter("select distinct  E_ListFormationDiffus.Code_formt, E_ListFormationDiffus.Objet ObjForm, E_ListFormationDiffus.Mat_usr, E_ListFormationDiffus.Nom_usr , 'Incomplete' etat , convert(date, '') DateTerm , E_ListFormationDiffus.deadline , E_ListFormationDiffus.DateDiffus from E_ListFormationDiffus inner join E_Formation on E_Formation.Code = E_ListFormationDiffus.Code_formt left outer join E_ResultFormation on E_ResultFormation.Code_Formation =  E_ListFormationDiffus.Code_formt where E_Formation.Matricule_Formateur = " + user.matricule + " and E_ListFormationDiffus.Code_formt = '" + id + "'   and E_ListFormationDiffus.Mat_usr not in ( select distinct E_ResultFormation.MatUser from E_ResultFormation inner join   E_Formation on E_Formation.Code = E_ResultFormation.Code_Formation where code_formation ='" + id + "' and  E_Formation.Matricule_Formateur = " + user.matricule + ") and E_ListFormationDiffus.Nom_usr like '%" + searchStringUsr + "%'   union  select distinct E_Formation.Code Code_formt, E_Formation.Objet ObjForm, E_ResultFormation.MatUser Mat_usr, E_ResultFormation.Usr Nom_usr, E_ResultFormation.Resultat etat , E_ResultFormation.DateTerm ,E_ResultFormation.DeadLine , convert(date,'') DateDiffus  from E_ResultFormation inner join   E_Formation on E_Formation.Code = E_ResultFormation.Code_Formation where code_formation ='" + id + "' and  E_Formation.Matricule_Formateur = " + user.matricule + "  and E_ResultFormation.Usr  like '%" + searchStringUsr + "%' ", con);
                    daz.Fill(dtz);
                }
                else if (!String.IsNullOrEmpty(Etat))
                {
                    if (Etat == "Incomplete")
                    {
                        SqlDataAdapter daz;

                        daz = new SqlDataAdapter("select distinct  E_ListFormationDiffus.Code_formt, E_ListFormationDiffus.Objet ObjForm, E_ListFormationDiffus.Mat_usr, E_ListFormationDiffus.Nom_usr , 'Incomplete' etat , convert(date, '') DateTerm , E_ListFormationDiffus.deadline , E_ListFormationDiffus.DateDiffus from E_ListFormationDiffus inner join E_Formation on E_Formation.Code = E_ListFormationDiffus.Code_formt left outer join E_ResultFormation on E_ResultFormation.Code_Formation =  E_ListFormationDiffus.Code_formt where E_Formation.Matricule_Formateur = " + user.matricule + " and E_ListFormationDiffus.Code_formt = '" + id + "'   and E_ListFormationDiffus.Mat_usr not in ( select distinct E_ResultFormation.MatUser from E_ResultFormation inner join   E_Formation on E_Formation.Code = E_ResultFormation.Code_Formation where code_formation ='" + id + "' and  E_Formation.Matricule_Formateur = " + user.matricule + ") and    DateTerm is  null   union  select distinct E_Formation.Code Code_formt, E_Formation.Objet ObjForm, E_ResultFormation.MatUser Mat_usr, E_ResultFormation.Usr Nom_usr, E_ResultFormation.Resultat etat , E_ResultFormation.DateTerm ,E_ResultFormation.DeadLine , convert(date,'') DateDiffus  from E_ResultFormation inner join   E_Formation on E_Formation.Code = E_ResultFormation.Code_Formation where code_formation ='" + id + "' and  E_Formation.Matricule_Formateur = " + user.matricule + "   and DateTerm is  null ", con);
                        daz.Fill(dtz);

                    }
                    else
                    {
                        SqlDataAdapter daz;

                        daz = new SqlDataAdapter("select distinct  E_ListFormationDiffus.Code_formt, E_ListFormationDiffus.Objet ObjForm, E_ListFormationDiffus.Mat_usr, E_ListFormationDiffus.Nom_usr , 'Incomplete' etat , convert(date, '') DateTerm , E_ListFormationDiffus.deadline , E_ListFormationDiffus.DateDiffus from E_ListFormationDiffus inner join E_Formation on E_Formation.Code = E_ListFormationDiffus.Code_formt left outer join E_ResultFormation on E_ResultFormation.Code_Formation =  E_ListFormationDiffus.Code_formt where E_Formation.Matricule_Formateur = " + user.matricule + " and E_ListFormationDiffus.Code_formt = '" + id + "'   and E_ListFormationDiffus.Mat_usr not in ( select distinct E_ResultFormation.MatUser from E_ResultFormation inner join   E_Formation on E_Formation.Code = E_ResultFormation.Code_Formation where code_formation ='" + id + "' and  E_Formation.Matricule_Formateur = " + user.matricule + ") and    DateTerm is not null   union  select distinct E_Formation.Code Code_formt, E_Formation.Objet ObjForm, E_ResultFormation.MatUser Mat_usr, E_ResultFormation.Usr Nom_usr, E_ResultFormation.Resultat etat , E_ResultFormation.DateTerm ,E_ResultFormation.DeadLine , convert(date,'') DateDiffus  from E_ResultFormation inner join   E_Formation on E_Formation.Code = E_ResultFormation.Code_Formation where code_formation ='" + id + "' and  E_Formation.Matricule_Formateur = " + user.matricule + "   and DateTerm is not null ", con);
                        daz.Fill(dtz);

                    }

                }
                else if (!String.IsNullOrEmpty(searchStringUsr) && (!String.IsNullOrEmpty(Etat)))
                {
                    if (Etat == "Incomplete")
                    {
                        SqlDataAdapter daz;

                        daz = new SqlDataAdapter("select distinct  E_ListFormationDiffus.Code_formt, E_ListFormationDiffus.Objet ObjForm, E_ListFormationDiffus.Mat_usr, E_ListFormationDiffus.Nom_usr , 'Incomplete' etat , convert(date, '') DateTerm , E_ListFormationDiffus.deadline , E_ListFormationDiffus.DateDiffus from E_ListFormationDiffus inner join E_Formation on E_Formation.Code = E_ListFormationDiffus.Code_formt left outer join E_ResultFormation on E_ResultFormation.Code_Formation =  E_ListFormationDiffus.Code_formt where E_Formation.Matricule_Formateur = " + user.matricule + " and E_ListFormationDiffus.Code_formt = '" + id + "'   and E_ListFormationDiffus.Mat_usr not in ( select distinct E_ResultFormation.MatUser from E_ResultFormation inner join   E_Formation on E_Formation.Code = E_ResultFormation.Code_Formation where code_formation ='" + id + "' and  E_Formation.Matricule_Formateur = " + user.matricule + ") and    DateTerm is  null and E_ListFormationDiffus.Nom_usr like '%" + searchStringUsr + "%'   union  select distinct E_Formation.Code Code_formt, E_Formation.Objet ObjForm, E_ResultFormation.MatUser Mat_usr, E_ResultFormation.Usr Nom_usr, E_ResultFormation.Resultat etat , E_ResultFormation.DateTerm ,E_ResultFormation.DeadLine , convert(date,'') DateDiffus  from E_ResultFormation inner join   E_Formation on E_Formation.Code = E_ResultFormation.Code_Formation where code_formation ='" + id + "' and  E_Formation.Matricule_Formateur = " + user.matricule + "   and DateTerm is  null and E_ResultFormation.Usr like '%" + searchStringUsr + "%' ", con);
                        daz.Fill(dtz);

                    }
                    else
                    {
                        SqlDataAdapter daz;

                        daz = new SqlDataAdapter("select distinct  E_ListFormationDiffus.Code_formt, E_ListFormationDiffus.Objet ObjForm, E_ListFormationDiffus.Mat_usr, E_ListFormationDiffus.Nom_usr , 'Incomplete' etat , convert(date, '') DateTerm , E_ListFormationDiffus.deadline , E_ListFormationDiffus.DateDiffus from E_ListFormationDiffus inner join E_Formation on E_Formation.Code = E_ListFormationDiffus.Code_formt left outer join E_ResultFormation on E_ResultFormation.Code_Formation =  E_ListFormationDiffus.Code_formt where E_Formation.Matricule_Formateur = " + user.matricule + " and E_ListFormationDiffus.Code_formt = '" + id + "'   and E_ListFormationDiffus.Mat_usr not in ( select distinct E_ResultFormation.MatUser from E_ResultFormation inner join   E_Formation on E_Formation.Code = E_ResultFormation.Code_Formation where code_formation ='" + id + "' and  E_Formation.Matricule_Formateur = " + user.matricule + ") and    DateTerm is not  null and E_ListFormationDiffus.Nom_usr like '%" + searchStringUsr + "%'   union  select distinct E_Formation.Code Code_formt, E_Formation.Objet ObjForm, E_ResultFormation.MatUser Mat_usr, E_ResultFormation.Usr Nom_usr, E_ResultFormation.Resultat etat , E_ResultFormation.DateTerm ,E_ResultFormation.DeadLine , convert(date,'') DateDiffus  from E_ResultFormation inner join   E_Formation on E_Formation.Code = E_ResultFormation.Code_Formation where code_formation ='" + id + "' and  E_Formation.Matricule_Formateur = " + user.matricule + "   and DateTerm is not null and E_ResultFormation.Usr like '%" + searchStringUsr + "%' ", con);
                        daz.Fill(dtz);


                    }
                }
                else
                {
                    SqlDataAdapter daz;

                    daz = new SqlDataAdapter("select distinct  E_ListFormationDiffus.Code_formt, E_ListFormationDiffus.Objet ObjForm, E_ListFormationDiffus.Mat_usr, E_ListFormationDiffus.Nom_usr , 'Incomplete' etat , convert(date, '') DateTerm , E_ListFormationDiffus.deadline , E_ListFormationDiffus.DateDiffus from E_ListFormationDiffus inner join E_Formation on E_Formation.Code = E_ListFormationDiffus.Code_formt left outer join E_ResultFormation on E_ResultFormation.Code_Formation =  E_ListFormationDiffus.Code_formt where E_Formation.Matricule_Formateur = " + user.matricule + " and E_ListFormationDiffus.Code_formt = '" + id + "'   and E_ListFormationDiffus.Mat_usr not in ( select distinct E_ResultFormation.MatUser from E_ResultFormation inner join   E_Formation on E_Formation.Code = E_ResultFormation.Code_Formation where code_formation ='" + id + "' and  E_Formation.Matricule_Formateur = " + user.matricule + ")  union  select distinct E_Formation.Code Code_formt, E_Formation.Objet ObjForm, E_ResultFormation.MatUser Mat_usr, E_ResultFormation.Usr Nom_usr, E_ResultFormation.Resultat etat , E_ResultFormation.DateTerm ,E_ResultFormation.DeadLine , convert(date,'') DateDiffus  from E_ResultFormation inner join   E_Formation on E_Formation.Code = E_ResultFormation.Code_Formation where code_formation ='" + id + "' and  E_Formation.Matricule_Formateur = " + user.matricule + "   ", con);
                    daz.Fill(dtz);
                }

                E_FormationController.searchTermCodeF = "searchStringCodeF";
                E_FormationController.ValTermCodeF = searchStringCodeF;

                E_FormationController.searchTermUsr = "searchStringUsr";
                E_FormationController.ValTermUsr = searchStringUsr;

                E_FormationController.searchTermObjet = "searchStringObjet";
                E_FormationController.ValTermObjet = searchStringObjet;

                //if (!String.IsNullOrEmpty(searchStringCodeF))
                //    r = r.Where(s => s.Code_formt.ToLower().Contains(searchStringCodeF.ToLower()));

                //if (!String.IsNullOrEmpty(searchStringObjet))
                //    r = r.Where(s => s.Objet.ToLower().Contains(searchStringCodeF.ToLower()));

                //if (!String.IsNullOrEmpty(searchStringUsr))
                //    r = r.Where(s => s.Nom_usr.ToLower().Contains(searchStringUsr.ToLower()));

                //if (!String.IsNullOrEmpty(Etat))
                //    r = r.Where(s => s.Etat == Etat);


                for(int q=0; q<dtz.Rows.Count; q++)
                {
                    E_ResultFormation e_ResultFormation = new E_ResultFormation();

                    e_ResultFormation.Code_Formation = dtz.Rows[q]["Code_formt"].ToString()  ;

                    e_ResultFormation.ObjForm = dtz.Rows[q]["ObjForm"].ToString() ;

                    e_ResultFormation.MatUser = dtz.Rows[q]["Mat_usr"].ToString() ;

                    e_ResultFormation.Usr = dtz.Rows[q]["Nom_usr"].ToString()   ;

                    if (dtz.Rows[q]["DateDiffus"].ToString()  != "")
                    {
                        e_ResultFormation.DateTerm = Convert.ToDateTime(dtz.Rows[q]["DateDiffus"].ToString());
                    }
                    e_ResultFormation.DeadLine = Convert.ToDateTime(dtz.Rows[q]["DateDiffus"].ToString());

                    e_ResultFormation.Etat = dtz.Rows[q]["Etat"].ToString() ;

                    list.Add(e_ResultFormation);

                }

                ExcelPackage pck2 = new ExcelPackage();

                ExcelWorksheet ws2 = pck2.Workbook.Worksheets.Add("Statistique de formation");
                ws2.Cells["A1"].Value = "Formation";
                ws2.Cells["B1"].Value = "Objet formation";
                ws2.Cells["C1"].Value = "Utilisateur";
                ws2.Cells["D1"].Value = "Date formation";
                ws2.Cells["E1"].Value = "Date limite";
                ws2.Cells["F1"].Value = "Etat";

                int rowStart2 = 2;
                int jour2 = 0;

                foreach (var item in list)
                {
                    //   jour++;
                    ws2.Cells[String.Format("A{0}", rowStart2)].Value = item.Code_Formation;
                    ws2.Cells[String.Format("B{0}", rowStart2)].Value = item.ObjForm;
                    ws2.Cells[String.Format("C{0}", rowStart2)].Value = item.Usr;
                    ws2.Cells[String.Format("D{0}", rowStart2)].Value = item.DateTerm.ToShortDateString();
                    ws2.Cells[String.Format("E{0}", rowStart2)].Value = item.DeadLine.ToShortDateString();
                    ws2.Cells[String.Format("F{0}", rowStart2)].Value = item.Etat;
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

            //SqlDataAdapter da;
            //da = new SqlDataAdapter(" select distinct  E_ListFormationDiffus.Code_formt, E_ListFormationDiffus.Objet ObjForm, E_ListFormationDiffus.Mat_usr, E_ListFormationDiffus.Nom_usr, E_ListFormationDiffus.deadline, E_ResultFormation.DateTerm , case when(E_ResultFormation.Resultat = 'Complete') then 'Complete' else 'Incomplete' end as Etat from E_ListFormationDiffus left join E_ResultFormation on E_ResultFormation.Code_Formation = E_ListFormationDiffus.Code_formt inner join E_Formation on E_Formation.Code = E_ListFormationDiffus.Code_formt where E_Formation.Matricule_Formateur = " + user.matricule + " and E_ListFormationDiffus.Code_formt = '" + id + "' ", con);
            //da.Fill(dt);

            SqlDataAdapter da;
            da = new SqlDataAdapter("select distinct  E_ListFormationDiffus.Code_formt, E_ListFormationDiffus.Objet ObjForm, E_ListFormationDiffus.Mat_usr, E_ListFormationDiffus.Nom_usr , 'Incomplete' etat , convert(date, '') DateTerm , E_ListFormationDiffus.deadline from E_ListFormationDiffus inner join E_Formation on E_Formation.Code = E_ListFormationDiffus.Code_formt left outer join E_ResultFormation on E_ResultFormation.Code_Formation =  E_ListFormationDiffus.Code_formt where E_Formation.Matricule_Formateur = " + user.matricule + " and E_ListFormationDiffus.Code_formt = '" + id + "'   and E_ListFormationDiffus.Mat_usr not in ( select distinct E_ResultFormation.MatUser from E_ResultFormation inner join   E_Formation on E_Formation.Code = E_ResultFormation.Code_Formation where code_formation ='" + id + "' and  E_Formation.Matricule_Formateur = " + user.matricule + ")  union  select distinct E_Formation.Code Code_formt, E_Formation.Objet ObjForm, E_ResultFormation.MatUser Mat_usr, E_ResultFormation.Usr Nom_usr, E_ResultFormation.Resultat etat , E_ResultFormation.DateTerm ,E_ResultFormation.DeadLine  from E_ResultFormation inner join   E_Formation on E_Formation.Code = E_ResultFormation.Code_Formation where code_formation ='" + id + "' and  E_Formation.Matricule_Formateur = " + user.matricule + "   ", con);
            da.Fill(dt);


            for (int i = 0; i < dt.Rows.Count; i++)
            {
                E_ResultFormation e_ResultFormation = new E_ResultFormation();

                e_ResultFormation.Code_Formation = dt.Rows[i]["Code_formt"].ToString();

                e_ResultFormation.ObjForm = dt.Rows[i]["ObjForm"].ToString();

                e_ResultFormation.MatUser = dt.Rows[i]["Mat_usr"].ToString();

                e_ResultFormation.Usr = dt.Rows[i]["Nom_usr"].ToString();

                if (dt.Rows[i]["DateTerm"].ToString() != "")
                {
                    e_ResultFormation.DateTerm = Convert.ToDateTime(dt.Rows[i]["DateTerm"].ToString());
                }
                e_ResultFormation.DeadLine = Convert.ToDateTime(dt.Rows[i]["DeadLine"].ToString());

                e_ResultFormation.Etat = dt.Rows[i]["Etat"].ToString();

                list.Add(e_ResultFormation);

            }


            ExcelPackage pck = new ExcelPackage();

            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Statistique de formation");
            ws.Cells["A1"].Value = "Formation";
            ws.Cells["B1"].Value = "Objet formation";
            ws.Cells["C1"].Value = "Utilisateur";
            ws.Cells["D1"].Value = "Date formation";
            ws.Cells["E1"].Value = "Date limite";
            ws.Cells["F1"].Value = "Etat";

            int rowStart = 2;
            int jour = 0;

            foreach (var item in list)
            {
                //   jour++;
                ws.Cells[String.Format("A{0}", rowStart)].Value = item.Code_Formation;
                ws.Cells[String.Format("B{0}", rowStart)].Value = item.ObjForm;
                ws.Cells[String.Format("C{0}", rowStart)].Value = item.Usr;
                ws.Cells[String.Format("D{0}", rowStart)].Value = item.DateTerm.ToShortDateString();
                ws.Cells[String.Format("E{0}", rowStart)].Value = item.DeadLine.ToShortDateString();
                ws.Cells[String.Format("F{0}", rowStart)].Value = item.Etat;
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


        // GET: E_Formation
        public ActionResult Index(string searchStringCodeF , string searchStringCodeE , string searchStringObjet)
        {
            //, string searchStringDateC
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());

            string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection con = new SqlConnection(constr);
            con.Open();

            ViewData["CurrentFilterCodeF"] = searchStringCodeF;
            ViewData["CurrentFilterCodeE"] = searchStringCodeE;
            ViewData["CurrentFilterObjet"] = searchStringObjet;

            //if (searchStringDateC != null)
            //{
            //    if (searchStringDateC.ToString() != "")
            //        searchStringDateC = Convert.ToDateTime(searchStringDateC).ToShortDateString();
            //}

            List<E_Formation> list = new List<E_Formation>();

             //|| searchStringDateC != null
            if (!String.IsNullOrEmpty(searchStringCodeF) || !String.IsNullOrEmpty(searchStringCodeE) || !String.IsNullOrEmpty(searchStringObjet))
            {
                var r = (from m in db.E_Formation
                         where m.Matricule_Formateur == user.matricule && m.Etat_Formation == "Active"
                         select new { m.Code, m.CodeEval , m.Objet ,   m.Date_Creation , m.EtatDiff }).Distinct();

             
               
                E_FormationController.searchTermCodeF = "searchStringCodeF";
                E_FormationController.ValTermCodeF = searchStringCodeF;

                E_FormationController.searchTermCodeE = "searchStringCodeE";
                E_FormationController.ValTermCodeE = searchStringCodeE;

                E_FormationController.searchTermObjet = "searchStringObjet";
                E_FormationController.ValTermObjet = searchStringObjet;

                if (!String.IsNullOrEmpty(searchStringCodeF))
                    r = r.Where(s => s.Code.ToLower(). Contains(searchStringCodeF.ToLower()));


                if (!String.IsNullOrEmpty(searchStringCodeE))
                    r = r.Where(s => s.CodeEval.ToLower().Contains(searchStringCodeE.ToLower()));

                if (!String.IsNullOrEmpty(searchStringObjet))
                    r = r.Where(s => s.Objet.ToLower().Contains(searchStringObjet.ToLower()));



                //if (!String.IsNullOrEmpty(searchStringDateC))
                //    r = r.Where(s => s.Date_Creation. == searchStringDateC);



                foreach (var ff in r)
                {
                    E_Formation e = new E_Formation();

                    e.Code = ff.Code;

                    e.CodeEval = ff.CodeEval;

                    e.Matricule_Formateur = e.ImageName = e.Chemin = e.Etat_Formation = null;

                    e.NumDiapo = 0;

                    e.Date_Creation = ff.Date_Creation ;

                    e.Objet = ff.Objet;

                    //e.Deadline = Convert.ToDateTime(dt2.Rows[i]["Deadline"]);

                    //e.Eval = dt2.Rows[i]["Eval"].ToString();

                    e.Id = 0;

                    e.EtatDiff = ff.EtatDiff ;

                    list.Add(e);
                }

                return View(list);
            }

            DataTable dt = new DataTable();

            DataTable dt2 = new DataTable();


            SqlDataAdapter da;
            da = new SqlDataAdapter(" SELECT  distinct   [code] ,Date_Creation ,EtatDiff , CodeEval FROM [E_Formation] where Etat_Formation ='Active' and  [Matricule_Formateur] =  '" + user.matricule + "' order by Date_Creation desc", con);
            da.Fill(dt);



          

            for (int i = 0; i < dt.Rows.Count; i++)
            {

                E_Formation e = new E_Formation();

                string dd = dt.Rows[i]["code"].ToString();


                  
                SqlDataAdapter da2;
                da2 = new SqlDataAdapter(" SELECT  TOP (1) Date_Creation, Objet , Id  FROM [E_Formation] where   Code =  '" + dd + "'", con);
                da2.Fill(dt2);


                e.Code = dt.Rows[i]["code"].ToString();

                e.CodeEval = dt.Rows[i]["CodeEval"].ToString();

                e.Matricule_Formateur = e.ImageName = e.Chemin = e.Etat_Formation = null;

                e.NumDiapo = 0;

                e.Date_Creation = Convert.ToDateTime(dt2.Rows[i]["Date_Creation"]);

                e.Objet = dt2.Rows[i]["Objet"].ToString();

                //e.Deadline = Convert.ToDateTime(dt2.Rows[i]["Deadline"]);

                //e.Eval = dt2.Rows[i]["Eval"].ToString();

                e.Id = Convert.ToInt32(dt2.Rows[i]["Id"]);

                e.EtatDiff = dt.Rows[i]["EtatDiff"].ToString();

                list.Add(e);

            }



        
            return View(list);

            //ApplicationUser user = db.Users.Find(User.Identity.GetUserId());

            //var FormQuery = from m in db.E_Formation
            //                where m.Etat_Formation == "Active" &&   m.Matricule_Formateur == user.matricule
            //                select m;


            //return View(FormQuery);
        }


        [HttpPost]
        public ActionResult Dupliquer(System.Collections.Generic.IEnumerable<HttpPostedFileBase> files, E_Formation e_Formation, string Eval, string codeFormation, string id)
        {
            var list  = from m in db.E_Formation
                        where m.Code == codeFormation
                         select m;



            string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection con = new SqlConnection(constr);
            con.Open();

            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());

            e_Formation.Matricule_Formateur = user.matricule;

            e_Formation.Date_Creation = System.DateTime.Now;
  
            e_Formation.EtatF = "A_realiser";

            e_Formation.Etat_Formation = "Active";


            string OldChaine = System.DateTime.Now.ToShortDateString();

            string NewChaine = OldChaine.Replace("/", "");

            string date = System.DateTime.Now.ToShortDateString();

            DataTable dt = new DataTable();

            SqlDataAdapter da;
            da = new SqlDataAdapter("SELECT  * FROM E_Formation where CONVERT(VARCHAR(10), Date_Creation, 103)='" + date + "'", con);
            da.Fill(dt);

            int a = dt.Rows.Count;

            
            int p = a + 1;

            if (p <= 9)
                e_Formation.Code = "F_" + NewChaine + "_0" + (a + 1);
            else
                e_Formation.Code = "F" + NewChaine + "_" + (a + 1);


            string nom_formation = e_Formation.Code;


            //string fileName = "test.txt";
            //string sourcePath = @"../../SlideImages/"+codeFormation+"";
            //string targetPath = @"../../SlideImages/"+nom_formation+"";

            //string sourceFile = System.IO.Path.Combine(sourcePath);
            //string destFile = System.IO.Path.Combine(targetPath);


           
            string folderName = Server.MapPath("\\SlideImages\\");
            string pathString = System.IO.Path.Combine(folderName, nom_formation);
            System.IO.Directory.CreateDirectory(pathString);

            string pathOrig = System.IO.Path.Combine(folderName, codeFormation);

            System.IO.File.Copy(pathOrig, pathString, true);


            int i = 0;

            //foreach (var file in files)
            foreach (var f in list)
            {
                //if (file != null && file.ContentLength > 0)
                //{
                    //string ext = Path.GetExtension(file.FileName);

                    //if (ext.Equals(".jpg") || ext.Equals(".png") || ext.Equals(".gif") || ext.Equals(".jpeg") || ext.Equals(".JPEG") || ext.Equals(".JPG") || ext.Equals(".PNG") || ext.Equals(".GIF"))
                    //{
                        //i++;
                        //string filename = Path.GetFileName(file.FileName);
                        //string fichier = "~/SlideImages/" + nom_formation + "/" + filename;

                        //System.Drawing.Bitmap bmpPostedImage = new System.Drawing.Bitmap(file.InputStream);
                        //System.Drawing.Image image = (System.Drawing.Image)bmpPostedImage;
                        //bmpPostedImage = new Bitmap(700, 350);
                        //Graphics graphic = Graphics.FromImage(bmpPostedImage);
                        //graphic.DrawImage(image, 0, 0, 700, 350);
                        //graphic.DrawImage(image, new Rectangle(0, 0, 700, 350), 0, 0, 0, 0, GraphicsUnit.Pixel);
                        //graphic.Dispose();
                        //bmpPostedImage.Save(Server.MapPath(fichier));

                        //file.SaveAs(Server.MapPath(fichier));
                         
                        //string chemin = "../../SlideImages/" + nom_formation + "/" + filename;
                        SqlCommand cmd = new SqlCommand("Insert into E_Formation (Code,Objet,Etat_Formation,Date_Creation,Matricule_Formateur,ImageName,Numdiapo,Chemin, EtatF   ) values(@Code,@Objet,@Etat_Formation,@Date_Creation,@Matricule_Formateur,@ImageName,@Numdiapo,@Chemin, @EtatF )", con);

                        cmd.Parameters.AddWithValue("@Code", e_Formation.Code);
                        cmd.Parameters.AddWithValue("@Objet", e_Formation.Objet);
                        cmd.Parameters.AddWithValue("@Etat_Formation", e_Formation.Etat_Formation);
                        cmd.Parameters.AddWithValue("@Date_Creation", e_Formation.Date_Creation);
                        cmd.Parameters.AddWithValue("@Matricule_Formateur", e_Formation.Matricule_Formateur);
                        cmd.Parameters.AddWithValue("@ImageName", f.ImageName);
                        cmd.Parameters.AddWithValue("@Numdiapo", f.NumDiapo);
                        //cmd.Parameters.AddWithValue("@NumFormation", nom_formation);
                        cmd.Parameters.AddWithValue("@Chemin", f.Chemin);

                        //cmd.Parameters.AddWithValue("@Eval", e_Formation.Eval);
                        cmd.Parameters.AddWithValue("@EtatF", e_Formation.EtatF);
                        //cmd.Parameters.AddWithValue("@Deadline", e_Formation.Deadline);
                        cmd.ExecuteNonQuery();


                    //}
                    //else
                    //{
                    //    return Content("<script language='javascript' type='text/javascript'>alert('Type de fichier invalide!');</script>");
                    //}
                //}
                //else
                //{

                //}
            }


            con.Close();
            return RedirectToAction("Index", "E_Formation");
        }


        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(System.Collections.Generic.IEnumerable<HttpPostedFileBase> files, E_Formation e_Formation, string Eval)
        {
            string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection con = new SqlConnection(constr);
            con.Open();

            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());

            e_Formation.Matricule_Formateur = user.matricule;

            e_Formation.Date_Creation = System.DateTime.Now;

            //e_Formation.Eval = Eval;

            e_Formation.EtatF = "A_realiser";

            e_Formation.Etat_Formation = "Active";


            string OldChaine = System.DateTime.Now.ToShortDateString();

            string NewChaine = OldChaine.Replace("/", "");

            string date = System.DateTime.Now.ToShortDateString();

            DataTable dt = new DataTable();

            SqlDataAdapter da;
            da = new SqlDataAdapter("SELECT  * FROM E_Formation where CONVERT(VARCHAR(10), Date_Creation, 103)='" + date + "'", con);
            da.Fill(dt);

            int a = dt.Rows.Count;

            //var FormQuery = from m in db.E_Formation
            //                  where m.Date_Creation.ToShortDateString() == System.DateTime.Now.ToShortDateString()
            //                  orderby m.Date_Creation descending
            //                  select m;

            //int a = FormQuery.Count();



            int p = a + 1;

            if (p <= 9)
                e_Formation.Code = "F_" + NewChaine + "_0" + (a + 1);
            else
                e_Formation.Code = "F" + NewChaine + "_" + (a + 1);


            string nom_formation = e_Formation.Code;

            string folderName = Server.MapPath("\\SlideImages\\");
            string pathString = System.IO.Path.Combine(folderName, nom_formation);
            System.IO.Directory.CreateDirectory(pathString);

            int i = 0;

            foreach (var file in files)
            {
                if (file != null && file.ContentLength > 0)
                {
                    string ext = Path.GetExtension(file.FileName);

                    if (ext.Equals(".jpg") || ext.Equals(".png") || ext.Equals(".gif") || ext.Equals(".jpeg") || ext.Equals(".JPEG") || ext.Equals(".JPG") || ext.Equals(".PNG") || ext.Equals(".GIF"))
                    {
                        i++;
                        string filename = Path.GetFileName(file.FileName);
                        string fichier = "~/SlideImages/" + nom_formation + "/" + filename;

                        System.Drawing.Bitmap bmpPostedImage = new System.Drawing.Bitmap(file.InputStream);
                        System.Drawing.Image image = (System.Drawing.Image)bmpPostedImage;
                        bmpPostedImage = new Bitmap(700, 350);
                        Graphics graphic = Graphics.FromImage(bmpPostedImage);
                        graphic.DrawImage(image, 0, 0, 700, 350);
                        graphic.DrawImage(image, new Rectangle(0, 0, 700, 350), 0, 0, 0, 0, GraphicsUnit.Pixel);
                        graphic.Dispose();
                        bmpPostedImage.Save(Server.MapPath(fichier));

                         file.SaveAs(Server.MapPath(fichier));



                        string chemin = "../../SlideImages/" + nom_formation + "/" + filename;
                        SqlCommand cmd = new SqlCommand("Insert into E_Formation (Code,Objet,Etat_Formation,Date_Creation,Matricule_Formateur,ImageName,Numdiapo,Chemin, EtatF   ) values(@Code,@Objet,@Etat_Formation,@Date_Creation,@Matricule_Formateur,@ImageName,@Numdiapo,@Chemin, @EtatF )", con);

                        cmd.Parameters.AddWithValue("@Code", e_Formation.Code);
                        cmd.Parameters.AddWithValue("@Objet", e_Formation.Objet);
                        cmd.Parameters.AddWithValue("@Etat_Formation", e_Formation.Etat_Formation);
                        cmd.Parameters.AddWithValue("@Date_Creation", e_Formation.Date_Creation);
                        cmd.Parameters.AddWithValue("@Matricule_Formateur", e_Formation.Matricule_Formateur);
                        cmd.Parameters.AddWithValue("@ImageName", filename);
                        cmd.Parameters.AddWithValue("@Numdiapo", i);
                        //cmd.Parameters.AddWithValue("@NumFormation", nom_formation);
                        cmd.Parameters.AddWithValue("@Chemin", chemin);

                        //cmd.Parameters.AddWithValue("@Eval", e_Formation.Eval);
                        cmd.Parameters.AddWithValue("@EtatF", e_Formation.EtatF);
                        //cmd.Parameters.AddWithValue("@Deadline", e_Formation.Deadline);
                        cmd.ExecuteNonQuery();


                    }
                    else
                    {
                        return Content("<script language='javascript' type='text/javascript'>alert('Type de fichier invalide!');</script>");
                    }
                }
                else
                {

                }
            }


            con.Close();
            return RedirectToAction("Index", "E_Formation");
        }


        static public int numdiapo = -1;
        static public int nbdiapo = -1;
        static public string source = "0";

        [HttpGet]
        public ActionResult ConsultUsr(string id)
        {

            BindDataList1(id);

            return View();

        }

        [HttpGet]
        public ActionResult Consult(string id)
        {
            //E_Formation e = db.E_Formation.Find(id); 

            BindDataList1(id);

            return View();

            //string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            //SqlConnection con = new SqlConnection(constr);
            //con.Open();

            //var s = from m in db.E_Formation
            //        where m.Code == id
            //        select m;



            //SqlCommand cmd2 = new SqlCommand("delete from [e_ListImg]  ", con);
            //cmd2.ExecuteNonQuery();

            //foreach (E_Formation dd in s)
            //{
            //    SqlCommand cmd = new SqlCommand("INSERT INTO e_ListImg  ([Code]  ,[chemin]) values ('"+dd.Code+"', '"+dd.Chemin+"') ", con);
            //    cmd.ExecuteNonQuery();

            //}

            //return View(db.E_listImg.ToList());

        }
        public void BindDataList1(string nomform)
        {

            string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection con = new SqlConnection(constr);
            con.Open();


            SqlCommand command = new SqlCommand("SELECT Chemin from E_Formation where Code='" + nomform + "' order by  NumDiapo ", con);
            SqlDataAdapter da = new SqlDataAdapter(command);
            DataTable dt = new DataTable();
            dt.Clear();
            da.Fill(dt);

            SqlCommand command1 = new SqlCommand("SELECT max(Numdiapo) from E_Formation where Code='" + nomform + "'", con);
            SqlDataAdapter da1 = new SqlDataAdapter(command1);
            DataTable dt1 = new DataTable();
            da1.Fill(dt1);

            string nbdiapo1 = Convert.ToString(dt1.Rows[0][0]);
            if (dt.Rows.Count >= 1)
            {
                nbdiapo = Convert.ToInt16(nbdiapo1);
            }
            Session["nbdiap"] = nbdiapo;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                TempData[i.ToString()] = Convert.ToString(dt.Rows[i][0]);
            }
            con.Close();

        }

        // GET: E_Formation/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            E_Formation e_Formation = db.E_Formation.Find(id);
            if (e_Formation == null)
            {
                return HttpNotFound();
            }
            return View(e_Formation);
        }


        // GET: E_Formation/Edit/5
        public ActionResult Edit(string id)
        {
            //if (id == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}

            //E_Formation e_Formation = db.E_Formation.Find(id);

            var list = (from m in db.E_Formation
                        where m.Code == id
                        select m).Take(1);

            E_Formation e_Formation = new E_Formation();

            foreach (E_Formation ss in list)
            {
                e_Formation = db.E_Formation.Find(ss.Id);
            }

            if (e_Formation == null)
            {
                return HttpNotFound();
            }
            return View(e_Formation);
        }

        //[HttpPost]
        //public ActionResult SlideUsr(string Slide)
        //{


        //    //return View();

        //}

        //public ActionResult Terminer(string Codef)
        //{
        //    ApplicationUser user = db.Users.Find(User.Identity.GetUserId());

        //    E_ResultFormation e_ResultFormation = new E_ResultFormation();

        //    e_ResultFormation.Code_Formation = Codef;

        //    E_Formation f = ((from m in db.E_Formation
        //                      where m.Code == Codef
        //                      select m).Take(1)).Single();

        //    e_ResultFormation.ObjForm = f.Objet;

        //    e_ResultFormation.MatUser = user.matricule;

        //    e_ResultFormation.Usr = user.NomPrenom;

        //    e_ResultFormation.Resultat = "Complete";

        //    e_ResultFormation.DateTerm = System.DateTime.Now;

        //    db.E_ResultFormation.Add(e_ResultFormation);

        //    db.SaveChanges();


        //    return RedirectToAction("Index", "EMenuFormUser");
        //}

        //public ActionResult PassEval(string Codef)
        //{
        //    ApplicationUser user = db.Users.Find(User.Identity.GetUserId());

        //    E_ResultFormation e_ResultFormation = new E_ResultFormation();

        //    e_ResultFormation.Code_Formation = Codef;

        //    E_Formation f = (from m in db.E_Formation
        //                     where m.Code == Codef
        //                     select m).Single();

        //    e_ResultFormation.ObjForm = f.Objet;

        //    e_ResultFormation.MatUser = user.matricule;

        //    e_ResultFormation.Usr = user.NomPrenom;

        //    e_ResultFormation.Resultat = "Complete";

        //    e_ResultFormation.DateTerm = System.DateTime.Now;

        //    db.E_ResultFormation.Add(e_ResultFormation);

        //    db.SaveChanges();

        //    return RedirectToAction("Consult", "E_QCM", new { id = f.CodeEval });
        //}


        public ActionResult ResultatFormation()
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());

            var list = from m in db.E_ResultFormation
                       where m.MatUser == user.matricule
                       select m;

            return View(list.ToList());

        }

        //public static int i = 0;
        [HttpGet]
        public ActionResult SlideUsr(string sld, string codeF, string next, string Previous, string codeform)
        {

            if (codeF != null)
            codeF = codeF.Substring(0, 13);

            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());


            var verifForm = from m in db.E_ResultFormation
                        where m.Code_Formation == codeF && m.MatUser == user.matricule
                        select m;

            int vv = verifForm.Count();

            if (vv != 0)
            {
                return RedirectToAction("ConsultUsr", "E_Formation", new { id = codeF });
            }




            if (next != null)
            {
                if (next == "Suivant")
                {
                    var ff = from m in db.E_Formation
                             where m.Code == codeform
                             select m;

                    int Total = ff.Count();

                    ViewBag.count = Total;

                    int dd = Convert.ToInt32(sld);

                    int ss = Convert.ToInt32(sld) + 1;

                    var verif = from m in db.E_SlideUsr
                                where m.MatUsr == user.matricule && m.Code_Formation == codeform && m.numSlide == ss
                                select m;

                    var name = from m in db.E_Formation
                               where m.Code == codeform && m.NumDiapo == ss
                               select m;


                    int v = verif.Count();


                    if (v == 0)
                    {
                        E_SlideUsr e_SlideUsr = new E_SlideUsr();

                        e_SlideUsr.Code_Formation = codeform;

                        e_SlideUsr.MatUsr = user.matricule;

                        e_SlideUsr.numSlide = Convert.ToInt32(ss);

                        e_SlideUsr.TotalSlide = Total;

                        e_SlideUsr.Date_Creation = System.DateTime.Now;

                        foreach (E_Formation o in name)
                        {
                            e_SlideUsr.NameSlide = o.ImageName;
                        }

                        db.E_SlideUsr.Add(e_SlideUsr);


                    }

                    if (ss == Total)
                    {

                        var rr = from m in db.E_ResultFormation
                                 where m.MatUser == user.matricule && m.Code_Formation == codeform
                                 select m;

                        int cc = rr.Count();

                        E_ListFormationDiffus ef = ((from m in db.e_ListFormationDiffus
                                                    where m.Code_formt == codeform
                                                   select m).Take(1)).Single();

                        if (cc == 0)
                        {
                            E_ResultFormation e_ResultFormation = new E_ResultFormation();

                            e_ResultFormation.Code_Formation = codeform;

                            E_Formation f = ((from m in db.E_Formation
                                              where m.Code == codeform
                                              select m).Take(1)).Single();

                            e_ResultFormation.ObjForm = f.Objet;

                            e_ResultFormation.MatUser = user.matricule;

                            e_ResultFormation.Usr = user.NomPrenom;

                            e_ResultFormation.Resultat = "Complete";

                            e_ResultFormation.DateTerm = System.DateTime.Now;

                            e_ResultFormation.DeadLine = ef.deadline;

                            db.E_ResultFormation.Add(e_ResultFormation);
                        }

                    }

                    db.SaveChanges();

                    E_Formation ee = (from m in db.E_Formation
                                      where m.Code == codeform && m.NumDiapo == ss
                                      select m).Single();

                    return View(ee);

                }
            }
            if (Previous != null)
            {
                if (Previous.ToString() == "Précédent")
                {

                    var ff = from m in db.E_Formation
                             where m.Code == codeform
                             select m;

                    int Total = ff.Count();

                    ViewBag.count = Total;

                    //int dd = Convert.ToInt32(sld);

                    //var verif = from m in db.E_SlideUsr
                    //            where m.MatUsr == user.matricule && m.Code_Formation == codeform && m.numSlide == dd
                    //            select m;

                    //var name = from m in db.E_Formation
                    //           where m.Code == codeform && m.NumDiapo == dd
                    //           select m;


                    //int v = verif.Count();

                    //if (v == 0)
                    //{
                    //    E_SlideUsr e_SlideUsr = new E_SlideUsr();

                    //    e_SlideUsr.Code_Formation = codeform;

                    //    e_SlideUsr.MatUsr = user.matricule;

                    //    e_SlideUsr.numSlide = Convert.ToInt32(sld);

                    //    e_SlideUsr.TotalSlide = Total;

                    //    e_SlideUsr.Date_Creation = System.DateTime.Now;

                    //    foreach (E_Formation o in name)
                    //    {
                    //        e_SlideUsr.NameSlide = o.ImageName;
                    //    }

                    //    db.E_SlideUsr.Add(e_SlideUsr);

                    //    db.SaveChanges();
                    //}

                    int ss = Convert.ToInt32(sld) - 1;

                    if (ss != 0)
                    {
                        E_Formation ee = (from m in db.E_Formation
                                          where m.Code == codeform && m.NumDiapo == ss
                                          select m).Single();

                        return View(ee);
                    }
                    else if (ss == 0)
                    {
                        var zz = from m in db.E_Formation
                                 where m.Code == codeform
                                 select m;

                        int count = zz.Count();

                        E_Formation ee = (from m in db.E_Formation
                                          where m.Code == codeform && m.NumDiapo == count
                                          select m).Single();

                        return View(ee);

                    }

                }

            }

            int i = 0;

            string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection con = new SqlConnection(constr);
            con.Open();


            SqlCommand command = new SqlCommand("SELECT *   FROM  [E_Formation] where Code = '" + codeF + "'    ", con);
            SqlDataAdapter da = new SqlDataAdapter(command);
            DataTable dt = new DataTable();
            dt.Clear();
            da.Fill(dt);

            int tt = 0;
            Session["nbdiap"] = tt = dt.Rows.Count;

            E_Formation e = new E_Formation();

            //var ll = from m in db.E_SlideUsr
            //         where m.MatUser == user.matricule && m.CodeF == codeF
            //         select m;

            //int nbr = ll.Count(); 


            //if (nbr < dt.Rows.Count)
            //{
            if (i < dt.Rows.Count)
            {
                e.Code = Convert.ToString(dt.Rows[i]["Code"]);

                e.Objet = Convert.ToString(dt.Rows[i]["Objet"]);

                e.Etat_Formation = Convert.ToString(dt.Rows[i]["Etat_Formation"]);

                e.Date_Creation = Convert.ToDateTime(dt.Rows[i]["Date_Creation"]);

                e.Matricule_Formateur = Convert.ToString(dt.Rows[i]["Matricule_Formateur"]);

                e.ImageName = Convert.ToString(dt.Rows[i]["ImageName"]);

                e.NumDiapo = Convert.ToInt32(dt.Rows[i]["NumDiapo"]);

                e.Chemin = Convert.ToString(dt.Rows[i]["Chemin"]);

                e.EtatDiff = Convert.ToString(dt.Rows[i]["EtatDiff"]);

                e.CodeEval = Convert.ToString(dt.Rows[i]["CodeEval"]);

                ViewBag.count = dt.Rows.Count;


                E_SlideUsr e_SlideUsr = new E_SlideUsr();

                e_SlideUsr.Code_Formation = Convert.ToString(dt.Rows[i]["Code"]);

                e_SlideUsr.MatUsr = user.matricule;

                e_SlideUsr.numSlide = Convert.ToInt32(dt.Rows[i]["NumDiapo"]);

                e_SlideUsr.TotalSlide = tt;

                e_SlideUsr.Date_Creation = System.DateTime.Now;

                e_SlideUsr.NameSlide = Convert.ToString(dt.Rows[i]["ImageName"]);


                db.E_SlideUsr.Add(e_SlideUsr);

                db.SaveChanges();

                i++;

                return View(e);
            }
            //}

            //else if (nbr == dt.Rows.Count)
            //{
            //    return RedirectToAction("ResultQCM", "E_QCM", new { codeEval = id });
            //}

            return View();
        }


        public ActionResult Statistique(string id , string searchStringCodeF, string searchStringObjet, string searchStringUsr, string Etat)
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());

            string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection con = new SqlConnection(constr);
            con.Open();

            ViewData["CurrentFilterCodeF"] = searchStringCodeF;
            ViewData["CurrentFilterUsr"] = searchStringUsr;
            ViewData["CurrentFilterObjet"] = searchStringObjet;

            ViewData["CurrentFilterEtat"] = Etat;

            Session["Cdf"] = id;

            List<E_ResultFormation> list = new List<E_ResultFormation>();


            if (!String.IsNullOrEmpty(searchStringCodeF) || !String.IsNullOrEmpty(searchStringUsr) || !String.IsNullOrEmpty(searchStringObjet) || (Etat!=null))
            {
                //var r = (from m in db.e_ListFormationDiffus
                //         join nn in db.E_ResultFormation on m.Code_formt equals nn.Code_Formation
                //         join p in db.E_Formation on m.Code_formt equals p.Code
                //         where m.MatFormateur == user.matricule && m.Code_formt == id
                //         select new { m.Code_formt, m.Objet, m.Mat_usr, m.Nom_usr, m.deadline, m.DateDiffus, Etat = (nn.Resultat == "Complete" ? "Complete" : "Incomplete") }).Distinct();


                //var r = ((from m in db.e_ListFormationDiffus
                //           join p in db.E_Formation on m.Code_formt equals p.Code
                //           join n in db.E_ResultFormation on m.Code_formt equals n.Code_Formation
                //           where p.Code == id && m.MatFormateur == user.matricule
                //           select new { Code_formt = m.Code_formt, ObjForm = m.Objet, m.Mat_usr, m.Nom_usr, etat = "Incomplete", DateTerm = Convert.ToDateTime("1900-01-01 00:00:00.000"), m.deadline, m.DateDiffus }).Distinct())

                //         .Union(from m in db.E_ResultFormation
                //                join nn in db.E_Formation on m.Code_Formation equals nn.Code
                //                where nn.Code == id && nn.Matricule_Formateur == user.matricule
                //                select new { Code_formt = nn.Code, ObjForm = nn.Objet, Mat_usr = m.MatUser, Nom_usr = m.Usr, etat = m.Resultat, m.DateTerm, deadline = m.DeadLine, DateDiffus = Convert.ToDateTime("1900-01-01 00:00:00.000") });

                DataTable dtz = new DataTable();

                if (!String.IsNullOrEmpty(searchStringUsr))
                {
                    SqlDataAdapter daz;
                    
                    daz = new SqlDataAdapter("select distinct  E_ListFormationDiffus.Code_formt, E_ListFormationDiffus.Objet ObjForm, E_ListFormationDiffus.Mat_usr, E_ListFormationDiffus.Nom_usr , 'Incomplete' etat , convert(date, '') DateTerm , E_ListFormationDiffus.deadline , E_ListFormationDiffus.DateDiffus from E_ListFormationDiffus inner join E_Formation on E_Formation.Code = E_ListFormationDiffus.Code_formt left outer join E_ResultFormation on E_ResultFormation.Code_Formation =  E_ListFormationDiffus.Code_formt where E_Formation.Matricule_Formateur = " + user.matricule + " and E_ListFormationDiffus.Code_formt = '" + id + "'   and E_ListFormationDiffus.Mat_usr not in ( select distinct E_ResultFormation.MatUser from E_ResultFormation inner join   E_Formation on E_Formation.Code = E_ResultFormation.Code_Formation where code_formation ='" + id + "' and  E_Formation.Matricule_Formateur = " + user.matricule + ") and E_ListFormationDiffus.Nom_usr like '%"+searchStringUsr+"%'   union  select distinct E_Formation.Code Code_formt, E_Formation.Objet ObjForm, E_ResultFormation.MatUser Mat_usr, E_ResultFormation.Usr Nom_usr, E_ResultFormation.Resultat etat , E_ResultFormation.DateTerm ,E_ResultFormation.DeadLine , convert(date,'') DateDiffus  from E_ResultFormation inner join   E_Formation on E_Formation.Code = E_ResultFormation.Code_Formation where code_formation ='" + id + "' and  E_Formation.Matricule_Formateur = " + user.matricule + "  and E_ResultFormation.Usr  like '%" + searchStringUsr + "%' ", con);
                    daz.Fill(dtz);
                }
                else if (!String.IsNullOrEmpty(Etat))
                {
                    if (Etat == "Incomplete")
                    {
                        SqlDataAdapter daz;
                       
                        daz = new SqlDataAdapter("select distinct  E_ListFormationDiffus.Code_formt, E_ListFormationDiffus.Objet ObjForm, E_ListFormationDiffus.Mat_usr, E_ListFormationDiffus.Nom_usr , 'Incomplete' etat , convert(date, '') DateTerm , E_ListFormationDiffus.deadline , E_ListFormationDiffus.DateDiffus from E_ListFormationDiffus inner join E_Formation on E_Formation.Code = E_ListFormationDiffus.Code_formt left outer join E_ResultFormation on E_ResultFormation.Code_Formation =  E_ListFormationDiffus.Code_formt where E_Formation.Matricule_Formateur = " + user.matricule + " and E_ListFormationDiffus.Code_formt = '" + id + "'   and E_ListFormationDiffus.Mat_usr not in ( select distinct E_ResultFormation.MatUser from E_ResultFormation inner join   E_Formation on E_Formation.Code = E_ResultFormation.Code_Formation where code_formation ='" + id + "' and  E_Formation.Matricule_Formateur = " + user.matricule + ") and    DateTerm is  null   union  select distinct E_Formation.Code Code_formt, E_Formation.Objet ObjForm, E_ResultFormation.MatUser Mat_usr, E_ResultFormation.Usr Nom_usr, E_ResultFormation.Resultat etat , E_ResultFormation.DateTerm ,E_ResultFormation.DeadLine , convert(date,'') DateDiffus  from E_ResultFormation inner join   E_Formation on E_Formation.Code = E_ResultFormation.Code_Formation where code_formation ='" + id + "' and  E_Formation.Matricule_Formateur = " + user.matricule + "   and DateTerm is  null ", con);
                        daz.Fill(dtz);

                    }
                    else
                    {
                        SqlDataAdapter daz;
                       
                        daz = new SqlDataAdapter("select distinct  E_ListFormationDiffus.Code_formt, E_ListFormationDiffus.Objet ObjForm, E_ListFormationDiffus.Mat_usr, E_ListFormationDiffus.Nom_usr , 'Incomplete' etat , convert(date, '') DateTerm , E_ListFormationDiffus.deadline , E_ListFormationDiffus.DateDiffus from E_ListFormationDiffus inner join E_Formation on E_Formation.Code = E_ListFormationDiffus.Code_formt left outer join E_ResultFormation on E_ResultFormation.Code_Formation =  E_ListFormationDiffus.Code_formt where E_Formation.Matricule_Formateur = " + user.matricule + " and E_ListFormationDiffus.Code_formt = '" + id + "'   and E_ListFormationDiffus.Mat_usr not in ( select distinct E_ResultFormation.MatUser from E_ResultFormation inner join   E_Formation on E_Formation.Code = E_ResultFormation.Code_Formation where code_formation ='" + id + "' and  E_Formation.Matricule_Formateur = " + user.matricule + ") and    DateTerm is not null   union  select distinct E_Formation.Code Code_formt, E_Formation.Objet ObjForm, E_ResultFormation.MatUser Mat_usr, E_ResultFormation.Usr Nom_usr, E_ResultFormation.Resultat etat , E_ResultFormation.DateTerm ,E_ResultFormation.DeadLine , convert(date,'') DateDiffus  from E_ResultFormation inner join   E_Formation on E_Formation.Code = E_ResultFormation.Code_Formation where code_formation ='" + id + "' and  E_Formation.Matricule_Formateur = " + user.matricule + "   and DateTerm is not null ", con);
                        daz.Fill(dtz);

                    }

                }
                else if (!String.IsNullOrEmpty(searchStringUsr) && (!String.IsNullOrEmpty(Etat)))
                {
                    if (Etat == "Incomplete")
                    {
                        SqlDataAdapter daz;
                     
                        daz = new SqlDataAdapter("select distinct  E_ListFormationDiffus.Code_formt, E_ListFormationDiffus.Objet ObjForm, E_ListFormationDiffus.Mat_usr, E_ListFormationDiffus.Nom_usr , 'Incomplete' etat , convert(date, '') DateTerm , E_ListFormationDiffus.deadline , E_ListFormationDiffus.DateDiffus from E_ListFormationDiffus inner join E_Formation on E_Formation.Code = E_ListFormationDiffus.Code_formt left outer join E_ResultFormation on E_ResultFormation.Code_Formation =  E_ListFormationDiffus.Code_formt where E_Formation.Matricule_Formateur = " + user.matricule + " and E_ListFormationDiffus.Code_formt = '" + id + "'   and E_ListFormationDiffus.Mat_usr not in ( select distinct E_ResultFormation.MatUser from E_ResultFormation inner join   E_Formation on E_Formation.Code = E_ResultFormation.Code_Formation where code_formation ='" + id + "' and  E_Formation.Matricule_Formateur = " + user.matricule + ") and    DateTerm is  null and E_ListFormationDiffus.Nom_usr like '%" + searchStringUsr + "%'   union  select distinct E_Formation.Code Code_formt, E_Formation.Objet ObjForm, E_ResultFormation.MatUser Mat_usr, E_ResultFormation.Usr Nom_usr, E_ResultFormation.Resultat etat , E_ResultFormation.DateTerm ,E_ResultFormation.DeadLine , convert(date,'') DateDiffus  from E_ResultFormation inner join   E_Formation on E_Formation.Code = E_ResultFormation.Code_Formation where code_formation ='" + id + "' and  E_Formation.Matricule_Formateur = " + user.matricule + "   and DateTerm is  null and E_ResultFormation.Usr like '%" + searchStringUsr + "%' ", con);
                        daz.Fill(dtz);

                    }
                    else
                    {
                        SqlDataAdapter daz;
                         
                        daz = new SqlDataAdapter("select distinct  E_ListFormationDiffus.Code_formt, E_ListFormationDiffus.Objet ObjForm, E_ListFormationDiffus.Mat_usr, E_ListFormationDiffus.Nom_usr , 'Incomplete' etat , convert(date, '') DateTerm , E_ListFormationDiffus.deadline , E_ListFormationDiffus.DateDiffus from E_ListFormationDiffus inner join E_Formation on E_Formation.Code = E_ListFormationDiffus.Code_formt left outer join E_ResultFormation on E_ResultFormation.Code_Formation =  E_ListFormationDiffus.Code_formt where E_Formation.Matricule_Formateur = " + user.matricule + " and E_ListFormationDiffus.Code_formt = '" + id + "'   and E_ListFormationDiffus.Mat_usr not in ( select distinct E_ResultFormation.MatUser from E_ResultFormation inner join   E_Formation on E_Formation.Code = E_ResultFormation.Code_Formation where code_formation ='" + id + "' and  E_Formation.Matricule_Formateur = " + user.matricule + ") and    DateTerm is not  null and E_ListFormationDiffus.Nom_usr like '%" + searchStringUsr + "%'   union  select distinct E_Formation.Code Code_formt, E_Formation.Objet ObjForm, E_ResultFormation.MatUser Mat_usr, E_ResultFormation.Usr Nom_usr, E_ResultFormation.Resultat etat , E_ResultFormation.DateTerm ,E_ResultFormation.DeadLine , convert(date,'') DateDiffus  from E_ResultFormation inner join   E_Formation on E_Formation.Code = E_ResultFormation.Code_Formation where code_formation ='" + id + "' and  E_Formation.Matricule_Formateur = " + user.matricule + "   and DateTerm is not null and E_ResultFormation.Usr like '%" + searchStringUsr + "%' ", con);
                        daz.Fill(dtz);


                    }
                }
                else
                {
                    SqlDataAdapter daz;
                   
                    daz = new SqlDataAdapter("select distinct  E_ListFormationDiffus.Code_formt, E_ListFormationDiffus.Objet ObjForm, E_ListFormationDiffus.Mat_usr, E_ListFormationDiffus.Nom_usr , 'Incomplete' etat , convert(date, '') DateTerm , E_ListFormationDiffus.deadline , E_ListFormationDiffus.DateDiffus from E_ListFormationDiffus inner join E_Formation on E_Formation.Code = E_ListFormationDiffus.Code_formt left outer join E_ResultFormation on E_ResultFormation.Code_Formation =  E_ListFormationDiffus.Code_formt where E_Formation.Matricule_Formateur = " + user.matricule + " and E_ListFormationDiffus.Code_formt = '" + id + "'   and E_ListFormationDiffus.Mat_usr not in ( select distinct E_ResultFormation.MatUser from E_ResultFormation inner join   E_Formation on E_Formation.Code = E_ResultFormation.Code_Formation where code_formation ='" + id + "' and  E_Formation.Matricule_Formateur = " + user.matricule + ")  union  select distinct E_Formation.Code Code_formt, E_Formation.Objet ObjForm, E_ResultFormation.MatUser Mat_usr, E_ResultFormation.Usr Nom_usr, E_ResultFormation.Resultat etat , E_ResultFormation.DateTerm ,E_ResultFormation.DeadLine , convert(date,'') DateDiffus  from E_ResultFormation inner join   E_Formation on E_Formation.Code = E_ResultFormation.Code_Formation where code_formation ='" + id + "' and  E_Formation.Matricule_Formateur = " + user.matricule + "   ", con);
                    daz.Fill(dtz);
                }



                //--select distinct  E_ListFormationDiffus.Code_formt, E_ListFormationDiffus.Objet ObjForm, E_ListFormationDiffus.Mat_usr,
                //                --E_ListFormationDiffus.Nom_usr , 'Incomplete' etat , convert(date, '') DateTerm , E_ListFormationDiffus.deadline

                //               -- from E_ListFormationDiffus inner join E_Formation on E_Formation.Code = E_ListFormationDiffus.Code_formt
                //               --  left outer join E_ResultFormation on E_ResultFormation.Code_Formation = E_ListFormationDiffus.Code_formt

                //               --  where E_Formation.Matricule_Formateur = 0001 and E_ListFormationDiffus.Code_formt = 'F_24022021_01'

                //               --  and E_ListFormationDiffus.Mat_usr not in (select distinct E_ResultFormation.MatUser from E_ResultFormation inner join E_Formation on E_Formation.Code = E_ResultFormation.Code_Formation where code_formation = 'F_24022021_01' and E_Formation.Matricule_Formateur = 0001)

                //--union  select distinct E_Formation.Code Code_formt, E_Formation.Objet ObjForm, E_ResultFormation.MatUser Mat_usr, E_ResultFormation.Usr Nom_usr, E_ResultFormation.Resultat etat, E_ResultFormation.DateTerm ,E_ResultFormation.DeadLine from E_ResultFormation inner join E_Formation on E_Formation.Code = E_ResultFormation.Code_Formation where code_formation = 'F_24022021_01' and E_Formation.Matricule_Formateur = 0001


                 
                E_FormationController.searchTermCodeF = "searchStringCodeF";
                E_FormationController.ValTermCodeF = searchStringCodeF;

                E_FormationController.searchTermUsr = "searchStringUsr";
                E_FormationController.ValTermUsr = searchStringUsr;

                E_FormationController.searchTermObjet = "searchStringObjet";
                E_FormationController.ValTermObjet = searchStringObjet;

                //if (!String.IsNullOrEmpty(searchStringCodeF))
                //    r = r.Where(s => s.Code_formt.ToLower().Contains(searchStringCodeF.ToLower()));

                //if (!String.IsNullOrEmpty(searchStringObjet))
                //    r = r.Where(s => s.ObjForm.ToLower().Contains(searchStringCodeF.ToLower()));

                //if (!String.IsNullOrEmpty(searchStringUsr))
                //    r = r.Where(s => s.Nom_usr.ToLower().Contains(searchStringUsr.ToLower()));

                //if (!String.IsNullOrEmpty(Etat))
                //    r = r.Where(s => s.etat == Etat);


               for (int o =0; o < dtz.Rows.Count; o++)
                {
                    E_ResultFormation e_ResultFormation = new E_ResultFormation();

                    e_ResultFormation.Code_Formation = dtz.Rows[o]["Code_formt"].ToString();

                    e_ResultFormation.ObjForm = dtz.Rows[o]["ObjForm"].ToString() ;

                    e_ResultFormation.MatUser = dtz.Rows[o]["Mat_usr"].ToString()  ;

                    e_ResultFormation.Usr = dtz.Rows[o]["Nom_usr"].ToString()   ;

                    if (dtz.Rows[o]["DateDiffus"].ToString()    != "")
                    {
                        e_ResultFormation.DateTerm = Convert.ToDateTime(dtz.Rows[o]["DateDiffus"].ToString());
                    }
                    e_ResultFormation.DeadLine = Convert.ToDateTime(dtz.Rows[o]["DateDiffus"].ToString());

                    e_ResultFormation.Etat = dtz.Rows[o]["etat"].ToString()  ;

                    list.Add(e_ResultFormation);

                }

                return View(list);
            }

            DataTable dt = new DataTable();

            //SqlDataAdapter da;
            //da = new SqlDataAdapter(" select distinct  E_ListFormationDiffus.Code_formt, E_ListFormationDiffus.Objet ObjForm, E_ListFormationDiffus.Mat_usr, E_ListFormationDiffus.Nom_usr, E_ListFormationDiffus.deadline, E_ResultFormation.DateTerm , case when(E_ResultFormation.Resultat = 'Complete') then 'Complete' else 'Incomplete' end as Etat from E_ListFormationDiffus left join E_ResultFormation on E_ResultFormation.Code_Formation = E_ListFormationDiffus.Code_formt inner join E_Formation on E_Formation.Code = E_ListFormationDiffus.Code_formt where E_Formation.Matricule_Formateur = "+user.matricule+" and E_ListFormationDiffus.Code_formt = '" + id + "' ", con);
            //da.Fill(dt);

            SqlDataAdapter da;
            da = new SqlDataAdapter("select distinct  E_ListFormationDiffus.Code_formt, E_ListFormationDiffus.Objet ObjForm, E_ListFormationDiffus.Mat_usr, E_ListFormationDiffus.Nom_usr , 'Incomplete' etat , convert(date, '') DateTerm , E_ListFormationDiffus.deadline from E_ListFormationDiffus inner join E_Formation on E_Formation.Code = E_ListFormationDiffus.Code_formt left outer join E_ResultFormation on E_ResultFormation.Code_Formation =  E_ListFormationDiffus.Code_formt where E_Formation.Matricule_Formateur = "+ user.matricule+" and E_ListFormationDiffus.Code_formt = '"+id+"'   and E_ListFormationDiffus.Mat_usr not in ( select distinct E_ResultFormation.MatUser from E_ResultFormation inner join   E_Formation on E_Formation.Code = E_ResultFormation.Code_Formation where code_formation ='"+id+"' and  E_Formation.Matricule_Formateur = "+user.matricule+ ")  union  select distinct E_Formation.Code Code_formt, E_Formation.Objet ObjForm, E_ResultFormation.MatUser Mat_usr, E_ResultFormation.Usr Nom_usr, E_ResultFormation.Resultat etat , E_ResultFormation.DateTerm ,E_ResultFormation.DeadLine  from E_ResultFormation inner join   E_Formation on E_Formation.Code = E_ResultFormation.Code_Formation where code_formation ='"+id+"' and  E_Formation.Matricule_Formateur = "+user.matricule+"   ", con);
            da.Fill(dt);


            for (int i = 0; i < dt.Rows.Count; i++)
            {
                E_ResultFormation e_ResultFormation = new E_ResultFormation();

                e_ResultFormation.Code_Formation = dt.Rows[i]["Code_formt"].ToString();

                e_ResultFormation.ObjForm = dt.Rows[i]["ObjForm"].ToString();

                e_ResultFormation.MatUser = dt.Rows[i]["Mat_usr"].ToString();

                e_ResultFormation.Usr = dt.Rows[i]["Nom_usr"].ToString();

                if (dt.Rows[i]["DateTerm"].ToString() != "")
                { 
                 e_ResultFormation.DateTerm = Convert.ToDateTime(dt.Rows[i]["DateTerm"].ToString());
                }
                e_ResultFormation.DeadLine = Convert.ToDateTime(dt.Rows[i]["DeadLine"].ToString());

                e_ResultFormation.Etat = dt.Rows[i]["Etat"].ToString();

                list.Add(e_ResultFormation);

            }


            return View(list);
        }
     

        //POST: E_Formation/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier.Pour
        // plus de détails, voir https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Bind(Include = "Id,Code,Objet,Etat_Formation,Date_Creation,Etat_Diffusion,Matricule_Formateur")]E_Formation e_Formation,
        public ActionResult Edit( System.Collections.Generic.IEnumerable<HttpPostedFileBase> files   )
        {
             
            if (ModelState.IsValid)
            {
                string code = Session["code"].ToString();

                string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
                SqlConnection con = new SqlConnection(constr);
                con.Open();

                var listf = from m in db.E_Formation
                            where m.Code == code 
                            select m;

                E_Formation e = new E_Formation();

                foreach (E_Formation eee in listf)
                { 
                    e = db.E_Formation.Find(eee.Id);
                }

                string nom_formation = e.Code;


                string folderName = Server.MapPath("\\SlideImages\\");

                //string DelImg = Server.MapPath("\\DeleteImg\\");

                //string pathDelImg = System.IO.Path.Combine(DelImg, nom_formation);

                string pathString = System.IO.Path.Combine(folderName, nom_formation);


                string path = Server.MapPath("~/SlideImages/" + nom_formation);
                string[] filess = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
                foreach (string fil in filess)
                {
                    System.IO.File.Delete(fil);
                }
            //then delete folder
            Directory.Delete(path);



                //System.IO.Directory.Move(pathString, pathDelImg);

                System.IO.Directory.CreateDirectory(pathString);

                 

                SqlCommand cmd2 = new SqlCommand("delete FROM E_Formation where Code='" + e.Code + "' ", con);
                cmd2.ExecuteNonQuery();


                int i = 0;

                foreach (var file in files)
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        string ext = Path.GetExtension(file.FileName);

                        if (ext.Equals(".jpg") || ext.Equals(".png") || ext.Equals(".gif") || ext.Equals(".jpeg") || ext.Equals(".JPEG") || ext.Equals(".JPG") || ext.Equals(".PNG") || ext.Equals(".GIF"))
                        {
                            i++;
                            string filename = Path.GetFileName(file.FileName);

                            
                            string fichier = "~/SlideImages/" + nom_formation + "/" + filename;

                            System.Drawing.Bitmap bmpPostedImage = new System.Drawing.Bitmap(file.InputStream);
                            System.Drawing.Image image = (System.Drawing.Image)bmpPostedImage;
                            bmpPostedImage = new Bitmap(700, 350);
                            Graphics graphic = Graphics.FromImage(bmpPostedImage);
                            graphic.DrawImage(image, 0, 0, 700, 350);
                            graphic.DrawImage(image, new Rectangle(0, 0, 700, 350), 0, 0, 0, 0, GraphicsUnit.Pixel);
                            graphic.Dispose();
                            bmpPostedImage.Save(Server.MapPath(fichier));

                             
                            //file.SaveAs(Server.MapPath(fichier));

                           

                            string chemin = "../../SlideImages/" + nom_formation + "/" + filename;
                            SqlCommand cmd = new SqlCommand("Insert into E_Formation (Code,Objet,Etat_Formation,Date_Creation,Matricule_Formateur,ImageName,Numdiapo,Chemin) values(@Code,@Objet,@Etat_Formation,@Date_Creation,@Matricule_Formateur,@ImageName,@Numdiapo,@Chemin)", con);

                            cmd.Parameters.AddWithValue("@Code", e.Code);
                            cmd.Parameters.AddWithValue("@Objet", e.Objet);
                            cmd.Parameters.AddWithValue("@Etat_Formation", e.Etat_Formation);
                            cmd.Parameters.AddWithValue("@Date_Creation", e.Date_Creation);
                            cmd.Parameters.AddWithValue("@Matricule_Formateur", e.Matricule_Formateur);
                            cmd.Parameters.AddWithValue("@ImageName", filename);
                            cmd.Parameters.AddWithValue("@Numdiapo", i);
                            //cmd.Parameters.AddWithValue("@NumFormation", nom_formation);
                            cmd.Parameters.AddWithValue("@Chemin", chemin);
                            cmd.ExecuteNonQuery();


                         


                        }
                        else
                        {
                            return Content("<script language='javascript' type='text/javascript'>alert('Type de fichier invalide!');</script>");
                        }
                    }
                }
                    

                //db.Entry(e_Formation).State = EntityState.Modified;
                //db.SaveChanges();
               
            }
            return RedirectToAction("Index", "E_Formation");
            //return View(e_Formation);
        }

        // GET: E_Formation/Delete/5
        public ActionResult Delete(string id)
        {
            //if (id == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}
            //E_Formation e_Formation = db.E_Formation.Find(id);
            //if (e_Formation == null)
            //{
            //    return HttpNotFound();
            //}


            var list = (from m in db.E_Formation
                        where m.Code == id
                        select m).Take(1);

            E_Formation e_Formation = new E_Formation();

            foreach (E_Formation ss in list)
            {
                e_Formation = db.E_Formation.Find(ss.Id);
            }

            return View(e_Formation);
        }

        // POST: E_Formation/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed()
        {
            string code = Session["code"].ToString();

            string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection con = new SqlConnection(constr);
            con.Open();

            SqlCommand cmd = new SqlCommand("delete FROM E_Formation where Code='" + code + "' ", con);
            cmd.ExecuteNonQuery();


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
