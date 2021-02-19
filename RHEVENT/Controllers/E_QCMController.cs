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
    public class E_QCMController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        //public static   int i = 0;

        // GET: E_QCM
        public ActionResult Index(string codeEval, string EtatDiff)
        {
            var list = from m in db.E_Evaluation
                       where m.Code_Eval == codeEval
                       select m;

            foreach (E_Evaluation r in list)
            {
                ViewBag.codeEval = codeEval;

                ViewBag.objetEval = r.Objet_Eval;

            }

            ViewBag.EtatD = EtatDiff;

            var l = from m in db.E_QCM
                    where m.Code_EvalByQCM == codeEval
                    orderby m.Date_Creation descending
                    select m;

            return View(l.ToList());
        }


        [HttpPost]
        public ActionResult RepUser(string Reponse)
        {
            string Question = TempData["Question"].ToString();

            string CodeQcm = TempData["CodeQcm"].ToString();

            string Eval = Session["CodeEval"].ToString();

            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());


            E_RepUser e_RepUser = new E_RepUser();

            e_RepUser.MatUser = user.matricule;

            e_RepUser.Code_eval = Eval;

            e_RepUser.CodeQcm = CodeQcm;

            e_RepUser.Question = Question;

            e_RepUser.Date_Creation = System.DateTime.Now;

            e_RepUser.Reponse = Reponse;

            string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection con = new SqlConnection(constr);
            con.Open();

            SqlCommand cmd = new SqlCommand("Insert into E_RepUser (Code_eval,CodeQcm,Question,Reponse,Date_Creation,MatUser) values(@Code_eval, @CodeQcm, @Question,@Reponse,@Date_Creation,@MatUser)", con);

            cmd.Parameters.AddWithValue("@Code_eval", e_RepUser.Code_eval);
            cmd.Parameters.AddWithValue("@CodeQcm", e_RepUser.CodeQcm);
            cmd.Parameters.AddWithValue("@Question", e_RepUser.Question);
            cmd.Parameters.AddWithValue("@Reponse", e_RepUser.Reponse);
            cmd.Parameters.AddWithValue("@Date_Creation", e_RepUser.Date_Creation);
            cmd.Parameters.AddWithValue("@MatUser", e_RepUser.MatUser);

            cmd.ExecuteNonQuery();

            db.E_RepUser.Add(e_RepUser);


            return RedirectToAction("Consult", "E_QCM", new { id = Eval });


        }

        [HttpGet]
        public ActionResult ConsultParSlide(string id,string counter, string Reponse1, string Reponse2, string Reponse3, string Reponse4, string next, string Previous, string sld, string Terminer, string SNQ)
        {
            string ooid = id.Substring(0, 16);

            Session["Evl"] = ooid;

            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());

            var verifEval = from m in db.E_ResultQCM
                            where m.Code_EvalByQCM == ooid && m.MatUser == user.matricule
                            select m;

            int vv = verifEval.Count();

            if (vv != 0)
            {
                return RedirectToAction("Index", "E_EvalRealiseeUser");
            }

            int i = 0;
            int slid = 0;
            Session["CodeEval"] = ooid;

            string oo = null;

            string aa = null;





            if (TempData["Q"] != null)
            {
                if (TempData["Q"].ToString() != "")
                    slid = Convert.ToInt32(TempData["Q"].ToString());
            }

            string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection con = new SqlConnection(constr);
            con.Open();


            SqlCommand command = new SqlCommand("SELECT Code_QCM, Question, Reponse1, Reponse2,Reponse3, Reponse4 , NumQ from E_QCM where Code_EvalByQCM='" + ooid + "'  and NumQ = 1  ", con);
            SqlDataAdapter da = new SqlDataAdapter(command);
            DataTable dt = new DataTable();
            dt.Clear();
            da.Fill(dt);

            E_Evaluation dd = (from m in db.E_Evaluation
                               where m.Code_Eval == ooid
                               select m).Single();

            string duree = dd.Duree_Eval.ToString();

            //string MS = duree.Substring(8);

            string dr = "\"Feb 12, 2021 " + duree + "\"";

            //string ff = dr.Replace("\"","");

            //DateTime tt = Convert.ToDateTime(dr);

            Session["Duree"] = dr;

            ViewBag.Duree = dr;

            SqlCommand command4 = new SqlCommand("select  DATEDIFF(MINUTE, '0:00:00', Duree_Eval ) min ,  DATEDIFF(SECOND, '0:00:00', CONVERT(VARCHAR(8),GETDATE(),108) )- DATEDIFF(SECOND, '0:00:00', Duree_Eval ) diff,  DATEDIFF(SECOND, '0:00:00', CONVERT(VARCHAR(8),GETDATE(),108) ) today ,    DATEDIFF(SECOND, '0:00:00', Duree_Eval ) sec from E_Evaluation where Code_Eval='" + ooid + "'    ", con);
            SqlDataAdapter da4 = new SqlDataAdapter(command4);
            DataTable dt4 = new DataTable();
            dt4.Clear();
            da4.Fill(dt4);

            for (int h = 0; h < dt4.Rows.Count; h++)
            {
                Session["distance"] = Convert.ToInt32(dt4.Rows[i]["diff"].ToString());

                Session["MinF"] = Convert.ToInt32(dt4.Rows[i]["sec"].ToString());
            }


            E_Evaluation ddd = (from m in db.E_Evaluation
                                where m.Code_Eval == ooid
                               select m).Single();


            Session["MM"] = dd.Duree_Eval;

            SqlCommand command3 = new SqlCommand("select DATEDIFF(SECOND, '0:00:00', Duree_Eval ) sec from E_Evaluation where Code_Eval='" + ooid + "'    ", con);
            SqlDataAdapter da3 = new SqlDataAdapter(command3);
            DataTable dt3 = new DataTable();
            dt3.Clear();
            da3.Fill(dt3);

            //if (Session["min"] == null)
            //{
            //    if (i < dt.Rows.Count)
            //    {
            //        Session["min"] = dt3.Rows[i]["sec"].ToString();

            //        ViewBag.timeexpire = Session["Min"].ToString();
            //        Session["Evl"] = id;


            //        int SecEval = Convert.ToInt32(dt3.Rows[i]["sec"].ToString()); ;

            //    }

            //}
            //else
            //{
            //    double dddd = Convert.ToDouble(Session["Min"].ToString());
            //    ViewBag.timeexpire = dddd;
            //}
            //Session["min"] = 10;


            //SqlCommand command2 = new SqlCommand("SELECT Code_QCM, Question, Reponse1, Reponse2,Reponse3, Reponse4 from E_QCM where Code_EvalByQCM='" + id + "' and NumQ = 1   ", con);
            //SqlDataAdapter da2 = new SqlDataAdapter(command2);
            //DataTable dt2 = new DataTable();
            //dt2.Clear();
            //da2.Fill(dt2);

            //int pp = Convert.ToInt32(ViewBag.pp) + 1;

            Session["nbdiap"] = dt.Rows.Count;


            var ss = from m in db.E_QCM
                     where m.Code_EvalByQCM == ooid
                     select m;

            ViewBag.count = ss.Count();




            E_QCM e_QCM = new E_QCM();

            if (Terminer != null)
            {
                if (Terminer == "Terminer")
                {

                    var rp = from m in db.E_RepUser
                             where m.Code_eval == ooid && m.NumQ == (slid)
                             select m;

                    foreach (E_RepUser z in rp)
                    {
                        oo = z.Reponse;
                    }

                    //if (rp.Count() != 0)
                    //{
                    //    SqlCommand cmd2 = new SqlCommand("delete from  E_RepUser where Code_eval = '" + ooid + "' and NumQ = " + slid + "", con);

                    //    cmd2.ExecuteNonQuery();
                    //}

                    string Question = TempData["Question"].ToString();
                    string CodeQcm = TempData["CodeQcm"].ToString();
                    string Eval = Session["CodeEval"].ToString();


                    if (Reponse1.ToString() != "")
                    {
                        E_RepUser e_RepUser = new E_RepUser();

                        e_RepUser.MatUser = user.matricule;

                        e_RepUser.Code_eval = Eval;

                        e_RepUser.NumQ = slid;

                        e_RepUser.Ordre = 1;

                        e_RepUser.CodeQcm = CodeQcm;

                        e_RepUser.Question = Question;

                        e_RepUser.Date_Creation = System.DateTime.Now;

                        e_RepUser.Reponse = Reponse1;
                        //else
                        //{
                        //    e_RepUser.Reponse = oo;

                        //}

                        SqlCommand cmd = new SqlCommand("Insert into E_RepUser (Code_eval,NumQ, Ordre, CodeQcm,Question,Reponse,Date_Creation,MatUser) values(@Code_eval, @NumQ, @Ordre, @CodeQcm, @Question,@Reponse,@Date_Creation,@MatUser)", con);

                        cmd.Parameters.AddWithValue("@Code_eval", e_RepUser.Code_eval);
                        cmd.Parameters.AddWithValue("@NumQ", e_RepUser.NumQ);
                        cmd.Parameters.AddWithValue("@Ordre", e_RepUser.Ordre);
                        cmd.Parameters.AddWithValue("@CodeQcm", e_RepUser.CodeQcm);
                        cmd.Parameters.AddWithValue("@Question", e_RepUser.Question);
                        cmd.Parameters.AddWithValue("@Reponse", e_RepUser.Reponse);
                        cmd.Parameters.AddWithValue("@Date_Creation", e_RepUser.Date_Creation);
                        cmd.Parameters.AddWithValue("@MatUser", e_RepUser.MatUser);

                        cmd.ExecuteNonQuery();

                        db.E_RepUser.Add(e_RepUser);

                    }
                   
                    if (Reponse2.ToString() != "")
                    {
                        E_RepUser e_RepUser = new E_RepUser();

                        e_RepUser.MatUser = user.matricule;

                        e_RepUser.Code_eval = Eval;

                        e_RepUser.NumQ = slid;

                        e_RepUser.Ordre = 2;

                        e_RepUser.CodeQcm = CodeQcm;

                        e_RepUser.Question = Question;

                        e_RepUser.Date_Creation = System.DateTime.Now;


                        e_RepUser.Reponse = Reponse2;


                        SqlCommand cmd = new SqlCommand("Insert into E_RepUser (Code_eval,NumQ, Ordre,  CodeQcm,Question,Reponse,Date_Creation,MatUser) values(@Code_eval, @NumQ, @Ordre, @CodeQcm, @Question,@Reponse,@Date_Creation,@MatUser)", con);

                        cmd.Parameters.AddWithValue("@Code_eval", e_RepUser.Code_eval);
                        cmd.Parameters.AddWithValue("@NumQ", e_RepUser.NumQ);
                        cmd.Parameters.AddWithValue("@Ordre", e_RepUser.Ordre);
                        cmd.Parameters.AddWithValue("@CodeQcm", e_RepUser.CodeQcm);
                        cmd.Parameters.AddWithValue("@Question", e_RepUser.Question);
                        cmd.Parameters.AddWithValue("@Reponse", e_RepUser.Reponse);
                        cmd.Parameters.AddWithValue("@Date_Creation", e_RepUser.Date_Creation);
                        cmd.Parameters.AddWithValue("@MatUser", e_RepUser.MatUser);

                        cmd.ExecuteNonQuery();

                        db.E_RepUser.Add(e_RepUser);

                    }
                  

                    if (Reponse3.ToString() != "")
                    {
                        E_RepUser e_RepUser = new E_RepUser();

                        e_RepUser.MatUser = user.matricule;

                        e_RepUser.Code_eval = Eval;

                        e_RepUser.NumQ = slid;

                        e_RepUser.Ordre = 3;

                        e_RepUser.CodeQcm = CodeQcm;

                        e_RepUser.Question = Question;

                        e_RepUser.Date_Creation = System.DateTime.Now;

                        e_RepUser.Reponse = Reponse3;

                        SqlCommand cmd = new SqlCommand("Insert into E_RepUser (Code_eval,NumQ, Ordre,  CodeQcm,Question,Reponse,Date_Creation,MatUser) values(@Code_eval, @NumQ, @Ordre,  @CodeQcm, @Question,@Reponse,@Date_Creation,@MatUser)", con);

                        cmd.Parameters.AddWithValue("@Code_eval", e_RepUser.Code_eval);
                        cmd.Parameters.AddWithValue("@NumQ", e_RepUser.NumQ);
                        cmd.Parameters.AddWithValue("@Ordre", e_RepUser.Ordre);
                        cmd.Parameters.AddWithValue("@CodeQcm", e_RepUser.CodeQcm);
                        cmd.Parameters.AddWithValue("@Question", e_RepUser.Question);
                        cmd.Parameters.AddWithValue("@Reponse", e_RepUser.Reponse);
                        cmd.Parameters.AddWithValue("@Date_Creation", e_RepUser.Date_Creation);
                        cmd.Parameters.AddWithValue("@MatUser", e_RepUser.MatUser);

                        cmd.ExecuteNonQuery();

                        db.E_RepUser.Add(e_RepUser);

                    }
                    

                    if (Reponse4.ToString() != "")
                    {
                        E_RepUser e_RepUser = new E_RepUser();

                        e_RepUser.MatUser = user.matricule;

                        e_RepUser.Code_eval = Eval;

                        e_RepUser.NumQ = slid;

                        e_RepUser.Ordre = 4;

                        e_RepUser.CodeQcm = CodeQcm;

                        e_RepUser.Question = Question;

                        e_RepUser.Date_Creation = System.DateTime.Now;


                        e_RepUser.Reponse = Reponse4;


                        SqlCommand cmd = new SqlCommand("Insert into E_RepUser (Code_eval,NumQ, Ordre,  CodeQcm,Question,Reponse,Date_Creation,MatUser) values(@Code_eval, @NumQ, @Ordre,  @CodeQcm, @Question,@Reponse,@Date_Creation,@MatUser)", con);

                        cmd.Parameters.AddWithValue("@Code_eval", e_RepUser.Code_eval);
                        cmd.Parameters.AddWithValue("@NumQ", e_RepUser.NumQ);
                        cmd.Parameters.AddWithValue("@Ordre", e_RepUser.Ordre);
                        cmd.Parameters.AddWithValue("@CodeQcm", e_RepUser.CodeQcm);
                        cmd.Parameters.AddWithValue("@Question", e_RepUser.Question);
                        cmd.Parameters.AddWithValue("@Reponse", e_RepUser.Reponse);
                        cmd.Parameters.AddWithValue("@Date_Creation", e_RepUser.Date_Creation);
                        cmd.Parameters.AddWithValue("@MatUser", e_RepUser.MatUser);

                        cmd.ExecuteNonQuery();

                        db.E_RepUser.Add(e_RepUser);

                    }

                  
                    //return RedirectToAction("ResultQCM", "E_QCM", new { codeEval = Eval });
                    return RedirectToAction("ResultQCMParSlide", "E_QCM", new { codeEval = Eval , Slide = slid ,  Terminer ="Terminer" });


                }
            }

            if (next != null)
            {
                if (next == "Suivant")
                {
                    E_DeadLlineEvalUsr zz = (from m in db.E_DeadLlineEvalUsr
                                            where m.Code_eval == ooid && m.MatUser == user.matricule
                                            select m).Single();

                     

                    TimeSpan diff =   (zz.Deadline - DateTime.Now);

                    int   sssa = (int)( diff.TotalSeconds);

                    Session["MinF"] = sssa;

                    var verif = from m in db.E_RepUser
                                where m.Code_eval == ooid && m.NumQ == (slid + 1)
                                select m;

                    foreach (E_RepUser r in verif)
                    {
                        TempData["RS"] = r.Reponse;
                        //TempData["Question"] = r.Question;
                        //TempData["CodeQcm"] = r.CodeQcm;
                        //Session["CodeEval"] = r.Code_eval;

                    }

                    var rp = from m in db.E_RepUser
                             where m.Code_eval == ooid && m.NumQ == (slid)
                             select m;


                    foreach (E_RepUser z in rp)
                    {
                        oo = z.Reponse;
                    }

                    //if (verif.Count() != 0)
                    //{
                    //    SqlCommand cmd2 = new SqlCommand("delete from  E_RepUser where Code_eval = '" + ooid + "' and NumQ = " + slid + "", con);

                    //    cmd2.ExecuteNonQuery();
                    //}

                    string Question = TempData["Question"].ToString();
                    string CodeQcm = TempData["CodeQcm"].ToString();
                    string Eval = Session["CodeEval"].ToString();

                    if (Reponse1.ToString() != "")
                    {
                        E_RepUser e_RepUser = new E_RepUser();

                        e_RepUser.MatUser = user.matricule;

                        e_RepUser.Code_eval = Eval;

                        e_RepUser.NumQ = slid;

                        e_RepUser.Ordre = 1;

                        e_RepUser.CodeQcm = CodeQcm;

                        e_RepUser.Question = Question;

                        e_RepUser.Date_Creation = System.DateTime.Now;

                        e_RepUser.Reponse = Reponse1;
                        //else
                        //{
                        //    e_RepUser.Reponse = oo;

                        //}

                        SqlCommand cmd = new SqlCommand("Insert into E_RepUser (Code_eval,NumQ, Ordre,  CodeQcm,Question,Reponse,Date_Creation,MatUser) values(@Code_eval, @NumQ, @Ordre,  @CodeQcm, @Question,@Reponse,@Date_Creation,@MatUser)", con);

                        cmd.Parameters.AddWithValue("@Code_eval", e_RepUser.Code_eval);
                        cmd.Parameters.AddWithValue("@NumQ", e_RepUser.NumQ);
                        cmd.Parameters.AddWithValue("@Ordre", e_RepUser.Ordre);
                        cmd.Parameters.AddWithValue("@CodeQcm", e_RepUser.CodeQcm);
                        cmd.Parameters.AddWithValue("@Question", e_RepUser.Question);
                        cmd.Parameters.AddWithValue("@Reponse", e_RepUser.Reponse);
                        cmd.Parameters.AddWithValue("@Date_Creation", e_RepUser.Date_Creation);
                        cmd.Parameters.AddWithValue("@MatUser", e_RepUser.MatUser);

                        cmd.ExecuteNonQuery();

                        db.E_RepUser.Add(e_RepUser);

                    }
                

                    if (Reponse2.ToString() != "")
                    {
                        E_RepUser e_RepUser = new E_RepUser();

                        e_RepUser.MatUser = user.matricule;

                        e_RepUser.Code_eval = Eval;

                        e_RepUser.NumQ = slid;

                        e_RepUser.Ordre = 2;

                        e_RepUser.CodeQcm = CodeQcm;

                        e_RepUser.Question = Question;

                        e_RepUser.Date_Creation = System.DateTime.Now;


                        e_RepUser.Reponse = Reponse2;


                        SqlCommand cmd = new SqlCommand("Insert into E_RepUser (Code_eval,NumQ, Ordre,  CodeQcm,Question,Reponse,Date_Creation,MatUser) values(@Code_eval, @NumQ, @Ordre, @CodeQcm, @Question,@Reponse,@Date_Creation,@MatUser)", con);

                        cmd.Parameters.AddWithValue("@Code_eval", e_RepUser.Code_eval);
                        cmd.Parameters.AddWithValue("@NumQ", e_RepUser.NumQ);
                        cmd.Parameters.AddWithValue("@Ordre", e_RepUser.Ordre);
                        cmd.Parameters.AddWithValue("@CodeQcm", e_RepUser.CodeQcm);
                        cmd.Parameters.AddWithValue("@Question", e_RepUser.Question);
                        cmd.Parameters.AddWithValue("@Reponse", e_RepUser.Reponse);
                        cmd.Parameters.AddWithValue("@Date_Creation", e_RepUser.Date_Creation);
                        cmd.Parameters.AddWithValue("@MatUser", e_RepUser.MatUser);

                        cmd.ExecuteNonQuery();

                        db.E_RepUser.Add(e_RepUser);

                    }
                
                    if (Reponse3.ToString() != "")
                    {
                        E_RepUser e_RepUser = new E_RepUser();

                        e_RepUser.MatUser = user.matricule;

                        e_RepUser.Code_eval = Eval;

                        e_RepUser.NumQ = slid;

                        e_RepUser.Ordre = 3;

                        e_RepUser.CodeQcm = CodeQcm;

                        e_RepUser.Question = Question;

                        e_RepUser.Date_Creation = System.DateTime.Now;

                        e_RepUser.Reponse = Reponse3;

                        SqlCommand cmd = new SqlCommand("Insert into E_RepUser (Code_eval,NumQ, Ordre,  CodeQcm,Question,Reponse,Date_Creation,MatUser) values(@Code_eval, @NumQ, @Ordre, @CodeQcm, @Question,@Reponse,@Date_Creation,@MatUser)", con);

                        cmd.Parameters.AddWithValue("@Code_eval", e_RepUser.Code_eval);
                        cmd.Parameters.AddWithValue("@NumQ", e_RepUser.NumQ);
                        cmd.Parameters.AddWithValue("@Ordre", e_RepUser.Ordre);
                        cmd.Parameters.AddWithValue("@CodeQcm", e_RepUser.CodeQcm);
                        cmd.Parameters.AddWithValue("@Question", e_RepUser.Question);
                        cmd.Parameters.AddWithValue("@Reponse", e_RepUser.Reponse);
                        cmd.Parameters.AddWithValue("@Date_Creation", e_RepUser.Date_Creation);
                        cmd.Parameters.AddWithValue("@MatUser", e_RepUser.MatUser);

                        cmd.ExecuteNonQuery();

                        db.E_RepUser.Add(e_RepUser);

                    }
                    
                    if (Reponse4.ToString() != "")
                    {
                        E_RepUser e_RepUser = new E_RepUser();

                        e_RepUser.MatUser = user.matricule;

                        e_RepUser.Code_eval = Eval;

                        e_RepUser.NumQ = slid;

                        e_RepUser.Ordre = 4;

                        e_RepUser.CodeQcm = CodeQcm;

                        e_RepUser.Question = Question;

                        e_RepUser.Date_Creation = System.DateTime.Now;


                        e_RepUser.Reponse = Reponse4;


                        SqlCommand cmd = new SqlCommand("Insert into E_RepUser (Code_eval,NumQ, Ordre, CodeQcm,Question,Reponse,Date_Creation,MatUser) values(@Code_eval, @NumQ, @Ordre,  @CodeQcm, @Question,@Reponse,@Date_Creation,@MatUser)", con);

                        cmd.Parameters.AddWithValue("@Code_eval", e_RepUser.Code_eval);
                        cmd.Parameters.AddWithValue("@NumQ", e_RepUser.NumQ);
                        cmd.Parameters.AddWithValue("@Ordre", e_RepUser.Ordre);
                        cmd.Parameters.AddWithValue("@CodeQcm", e_RepUser.CodeQcm);
                        cmd.Parameters.AddWithValue("@Question", e_RepUser.Question);
                        cmd.Parameters.AddWithValue("@Reponse", e_RepUser.Reponse);
                        cmd.Parameters.AddWithValue("@Date_Creation", e_RepUser.Date_Creation);
                        cmd.Parameters.AddWithValue("@MatUser", e_RepUser.MatUser);

                        cmd.ExecuteNonQuery();

                        db.E_RepUser.Add(e_RepUser);

                    }

                    if (Reponse4.ToString() == "" && Reponse3.ToString() == "" && Reponse2.ToString() == "" && Reponse1.ToString() == "")
                    {
                        E_RepUser e_RepUser = new E_RepUser();

                        e_RepUser.MatUser = user.matricule;

                        e_RepUser.Code_eval = Eval;

                        e_RepUser.NumQ = slid;

                        //e_RepUser.Ordre = 4;

                        e_RepUser.CodeQcm = CodeQcm;

                        e_RepUser.Question = Question;

                        e_RepUser.Date_Creation = System.DateTime.Now;


                        //e_RepUser.Reponse = "";


                        SqlCommand cmd = new SqlCommand("Insert into E_RepUser (Code_eval,NumQ,    CodeQcm,Question,  Date_Creation,MatUser) values(@Code_eval, @NumQ,    @CodeQcm, @Question ,@Date_Creation,@MatUser)", con);

                        cmd.Parameters.AddWithValue("@Code_eval", e_RepUser.Code_eval);
                        cmd.Parameters.AddWithValue("@NumQ", e_RepUser.NumQ);
                        //cmd.Parameters.AddWithValue("@Ordre", e_RepUser.Ordre);
                        cmd.Parameters.AddWithValue("@CodeQcm", e_RepUser.CodeQcm);
                        cmd.Parameters.AddWithValue("@Question", e_RepUser.Question);
                        //cmd.Parameters.AddWithValue("@Reponse", e_RepUser.Reponse);
                        cmd.Parameters.AddWithValue("@Date_Creation", e_RepUser.Date_Creation);
                        cmd.Parameters.AddWithValue("@MatUser", e_RepUser.MatUser);

                        cmd.ExecuteNonQuery();

                        db.E_RepUser.Add(e_RepUser);

                    }


                    return RedirectToAction("ResultQCMParSlide", "E_QCM", new { codeEval = Eval, Slide = slid , sessionminf = Session["MinF"].ToString() });

                }
            }


            //var ll = from m in db.E_RepUser
            //         where m.MatUser == user.matricule && m.Code_eval == ooid
            //         select m;

            //int nbr = ll.Count();

            DataTable dt444 = new DataTable(); 
            SqlDataAdapter da444;
            da444 = new SqlDataAdapter("select distinct CodeQcm from E_RepUser where Code_eval = '"+ooid+ "' and CodeQcm in (select Code_QCM from E_QCM where Code_EvalByQCM =  '" + ooid + "')"  , con);
            da444.Fill(dt444);


            if (dt444.Rows.Count < ss.Count())
            {

                

                if (SNQ != null)
                {

                    E_DeadLlineEvalUsr zz = (from m in db.E_DeadLlineEvalUsr
                                             where m.Code_eval == ooid && m.MatUser == user.matricule
                                             select m).Single();



                    TimeSpan diff = (zz.Deadline - DateTime.Now);

                    int sssa = (int)(diff.TotalSeconds) ;

                    Session["MinF"] = sssa;


                    SqlCommand command8 = new SqlCommand("SELECT Code_QCM, Question, Reponse1, Reponse2,Reponse3, Reponse4 , NumQ from E_QCM where Code_EvalByQCM='" + ooid + "' and  NumQ = " + SNQ + "   ", con);
                    SqlDataAdapter da8 = new SqlDataAdapter(command8);
                    DataTable dt8 = new DataTable();
                    dt8.Clear();
                    da8.Fill(dt8);

                    for (int t = 0; t < dt8.Rows.Count; t++)
                    {
                        e_QCM.Code_QCM = Convert.ToString(dt8.Rows[i]["Code_QCM"]);

                        e_QCM.Question = Convert.ToString(dt8.Rows[i]["Question"]);

                        e_QCM.NumQ = Convert.ToInt32(SNQ);

                        e_QCM.Reponse1 = Convert.ToString(dt8.Rows[i]["Reponse1"]);

                        e_QCM.Reponse2 = Convert.ToString(dt8.Rows[i]["Reponse2"]);

                        e_QCM.Reponse3 = Convert.ToString(dt8.Rows[i]["Reponse3"]);

                        e_QCM.Reponse4 = Convert.ToString(dt8.Rows[i]["Reponse4"]);


                        return View(e_QCM);
                    }

                }

                else
                {

                    SqlCommand command33 = new SqlCommand("select DATEDIFF(SECOND, '0:00:00', Duree_Eval ) sec from E_Evaluation where Code_Eval='" + ooid + "'    ", con);
                    SqlDataAdapter da33 = new SqlDataAdapter(command33);
                    DataTable dt33 = new DataTable();
                    dt33.Clear();
                    da33.Fill(dt33);

                    for (int z = 0; z < dt33.Rows.Count; z++)
                    {

                        DateTime date =  System.DateTime.Now.AddSeconds(Convert.ToInt32(dt3.Rows[z]["sec"].ToString()));

                        SqlCommand cmd = new SqlCommand("Insert into E_DeadLlineEvalUsr (Code_eval,MatUser,Deadline) values (@Code_eval,@MatUser,@Deadline) ", con);

                        cmd.Parameters.AddWithValue("@Code_eval", ooid);
                        cmd.Parameters.AddWithValue("@MatUser", user.matricule);
                        cmd.Parameters.AddWithValue("@Deadline", date);
                        cmd.ExecuteNonQuery();


                    }

                    //if (i < dt.Rows.Count)
                    //{
                    e_QCM.Code_QCM = Convert.ToString(dt.Rows[i]["Code_QCM"]);

                    e_QCM.Question = Convert.ToString(dt.Rows[i]["Question"]);

                    e_QCM.NumQ = 1;

                    e_QCM.Reponse1 = Convert.ToString(dt.Rows[i]["Reponse1"]);

                    e_QCM.Reponse2 = Convert.ToString(dt.Rows[i]["Reponse2"]);

                    e_QCM.Reponse3 = Convert.ToString(dt.Rows[i]["Reponse3"]);

                    e_QCM.Reponse4 = Convert.ToString(dt.Rows[i]["Reponse4"]);


                    //i++;

                    return View(e_QCM);
                    //}
                }


               
                
            }

            else if (dt444.Rows.Count == ss.Count())
            {
                i = 0;

                return RedirectToAction("ResultQCM", "E_QCM", new { codeEval = id });


            }


            ViewBag.pp = i;



            return View();



        }


        [HttpGet]
        public ActionResult Consult(string id, string Reponse, string next, string Previous, string sld, string Terminer)
        {
            string ooid = id.Substring(0, 16);


            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());

            var verifEval = from m in db.E_ResultQCM
                            where m.Code_EvalByQCM == ooid && m.MatUser == user.matricule
                            select m;

            int vv = verifEval.Count();

            if (vv != 0)
            {
                return RedirectToAction("Index", "E_EvalRealiseeUser");
            }

            int i = 0;
            int slid = 0;
            Session["CodeEval"] = ooid;

            string oo = null;

            string aa = null;





            if (TempData["Q"] != null)
            {
                if (TempData["Q"].ToString() != "")
                    slid = Convert.ToInt32(TempData["Q"].ToString());
            }

            string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection con = new SqlConnection(constr);
            con.Open();


            SqlCommand command = new SqlCommand("SELECT Code_QCM, Question, Reponse1, Reponse2,Reponse3, Reponse4 , NumQ from E_QCM where Code_EvalByQCM='" + ooid + "'  order by  Date_Creation  ", con);
            SqlDataAdapter da = new SqlDataAdapter(command);
            DataTable dt = new DataTable();
            dt.Clear();
            da.Fill(dt);

            E_Evaluation dd = (from m in db.E_Evaluation
                               where m.Code_Eval == ooid
                               select m).Single();

            string duree = dd.Duree_Eval.ToString();

            //string MS = duree.Substring(8);

            string dr = "\"Feb 12, 2021 " + duree + "\"";

            //string ff = dr.Replace("\"","");

            //DateTime tt = Convert.ToDateTime(dr);

            Session["Duree"] = dr;

            ViewBag.Duree = dr;

            SqlCommand command4 = new SqlCommand("select DATEDIFF(SECOND, '0:00:00', CONVERT(VARCHAR(8),GETDATE(),108) )- DATEDIFF(SECOND, '0:00:00', Duree_Eval ) diff,  DATEDIFF(SECOND, '0:00:00', CONVERT(VARCHAR(8),GETDATE(),108) ) today ,    DATEDIFF(SECOND, '0:00:00', Duree_Eval ) sec from E_Evaluation where Code_Eval='" + ooid + "'    ", con);
            SqlDataAdapter da4 = new SqlDataAdapter(command4);
            DataTable dt4 = new DataTable();
            dt4.Clear();
            da4.Fill(dt4);

            for (int h = 0; h < dt4.Rows.Count; h++)
            {
                Session["distance"] = Convert.ToInt32(dt4.Rows[i]["diff"].ToString());
            }


            SqlCommand command3 = new SqlCommand("select DATEDIFF(SECOND, '0:00:00', Duree_Eval ) sec from E_Evaluation where Code_Eval='" + ooid + "'    ", con);
            SqlDataAdapter da3 = new SqlDataAdapter(command3);
            DataTable dt3 = new DataTable();
            dt3.Clear();
            da3.Fill(dt3);

            if (i < dt.Rows.Count)
            {
                Session["min"] = dt3.Rows[i]["sec"].ToString();

                Session["Evl"] = id;


                int SecEval = Convert.ToInt32(dt3.Rows[i]["sec"].ToString()); ;



            }

            //Session["min"] = 10;


            //SqlCommand command2 = new SqlCommand("SELECT Code_QCM, Question, Reponse1, Reponse2,Reponse3, Reponse4 from E_QCM where Code_EvalByQCM='" + id + "' and NumQ = 1   ", con);
            //SqlDataAdapter da2 = new SqlDataAdapter(command2);
            //DataTable dt2 = new DataTable();
            //dt2.Clear();
            //da2.Fill(dt2);

            //int pp = Convert.ToInt32(ViewBag.pp) + 1;

            Session["nbdiap"] = dt.Rows.Count;




            ViewBag.count = dt.Rows.Count;

            var ll = from m in db.E_RepUser
                     where m.MatUser == user.matricule && m.Code_eval == ooid
                     select m;

            int nbr = ll.Count();



            E_QCM e_QCM = new E_QCM();

            if (Terminer != null)
            {
                if (Terminer == "Terminer")
                {

                    var rp = from m in db.E_RepUser
                             where m.Code_eval == ooid && m.NumQ == (slid)
                             select m;

                    foreach (E_RepUser z in rp)
                    {
                        oo = z.Reponse;
                    }

                    if (rp.Count() != 0)
                    {
                        SqlCommand cmd2 = new SqlCommand("delete from  E_RepUser where Code_eval = '" + ooid + "' and NumQ = " + slid + "", con);

                        cmd2.ExecuteNonQuery();
                    }

                    string Question = TempData["Question"].ToString();
                    string CodeQcm = TempData["CodeQcm"].ToString();
                    string Eval = Session["CodeEval"].ToString();

                    E_RepUser e_RepUser = new E_RepUser();

                    e_RepUser.MatUser = user.matricule;

                    e_RepUser.Code_eval = Eval;

                    e_RepUser.NumQ = slid;

                    e_RepUser.CodeQcm = CodeQcm;

                    e_RepUser.Question = Question;

                    e_RepUser.Date_Creation = System.DateTime.Now;

                    if (Reponse.ToString() != "")
                        e_RepUser.Reponse = Reponse;
                    else
                    {
                        e_RepUser.Reponse = oo;

                    }

                    SqlCommand cmd = new SqlCommand("Insert into E_RepUser (Code_eval,NumQ, CodeQcm,Question,Reponse,Date_Creation,MatUser) values(@Code_eval, @NumQ, @CodeQcm, @Question,@Reponse,@Date_Creation,@MatUser)", con);

                    cmd.Parameters.AddWithValue("@Code_eval", e_RepUser.Code_eval);
                    cmd.Parameters.AddWithValue("@NumQ", e_RepUser.NumQ);
                    cmd.Parameters.AddWithValue("@CodeQcm", e_RepUser.CodeQcm);
                    cmd.Parameters.AddWithValue("@Question", e_RepUser.Question);
                    cmd.Parameters.AddWithValue("@Reponse", e_RepUser.Reponse);
                    cmd.Parameters.AddWithValue("@Date_Creation", e_RepUser.Date_Creation);
                    cmd.Parameters.AddWithValue("@MatUser", e_RepUser.MatUser);

                    cmd.ExecuteNonQuery();

                    db.E_RepUser.Add(e_RepUser);


                    return RedirectToAction("ResultQCM", "E_QCM", new { codeEval = Eval });


                }
            }

            if (next != null)
            {
                if (next == "Suivant")
                {
                    var verif = from m in db.E_RepUser
                                where m.Code_eval == ooid && m.NumQ == (slid + 1)
                                select m;

                    foreach (E_RepUser r in verif)
                    {
                        TempData["RS"] = r.Reponse;
                        //TempData["Question"] = r.Question;
                        //TempData["CodeQcm"] = r.CodeQcm;
                        //Session["CodeEval"] = r.Code_eval;

                    }

                    var rp = from m in db.E_RepUser
                             where m.Code_eval == ooid && m.NumQ == (slid)
                             select m;


                    foreach (E_RepUser z in rp)
                    {
                        oo = z.Reponse;
                    }

                    if (verif.Count() != 0)
                    {
                        SqlCommand cmd2 = new SqlCommand("delete from  E_RepUser where Code_eval = '" + ooid + "' and NumQ = " + slid + "", con);

                        cmd2.ExecuteNonQuery();
                    }

                    string Question = TempData["Question"].ToString();
                    string CodeQcm = TempData["CodeQcm"].ToString();
                    string Eval = Session["CodeEval"].ToString();

                    E_RepUser e_RepUser = new E_RepUser();

                    e_RepUser.MatUser = user.matricule;

                    e_RepUser.Code_eval = Eval;

                    e_RepUser.NumQ = slid;

                    e_RepUser.CodeQcm = CodeQcm;

                    e_RepUser.Question = Question;

                    e_RepUser.Date_Creation = System.DateTime.Now;

                    if (Reponse.ToString() != "")
                        e_RepUser.Reponse = Reponse;
                    else
                    {
                        e_RepUser.Reponse = oo;

                    }

                    SqlCommand cmd = new SqlCommand("Insert into E_RepUser (Code_eval,NumQ, CodeQcm,Question,Reponse,Date_Creation,MatUser) values(@Code_eval, @NumQ, @CodeQcm, @Question,@Reponse,@Date_Creation,@MatUser)", con);

                    cmd.Parameters.AddWithValue("@Code_eval", e_RepUser.Code_eval);
                    cmd.Parameters.AddWithValue("@NumQ", e_RepUser.NumQ);
                    cmd.Parameters.AddWithValue("@CodeQcm", e_RepUser.CodeQcm);
                    cmd.Parameters.AddWithValue("@Question", e_RepUser.Question);
                    cmd.Parameters.AddWithValue("@Reponse", e_RepUser.Reponse);
                    cmd.Parameters.AddWithValue("@Date_Creation", e_RepUser.Date_Creation);
                    cmd.Parameters.AddWithValue("@MatUser", e_RepUser.MatUser);

                    cmd.ExecuteNonQuery();

                    db.E_RepUser.Add(e_RepUser);

                    int c = Convert.ToInt32(slid) + 1;

                    E_QCM eq = (from m in db.E_QCM
                                where m.Code_EvalByQCM == ooid && m.NumQ == c
                                select m).Single();



                    return View(eq);

                }
            }


            if (Previous != null)
            {
                if (Previous == "Précédent")
                {
                    string Question = TempData["Question"].ToString();
                    string CodeQcm = TempData["CodeQcm"].ToString();
                    string Eval = Session["CodeEval"].ToString();

                    var verif = from m in db.E_RepUser
                                where m.Code_eval == ooid && m.NumQ == (slid - 1)
                                select m;

                    foreach (E_RepUser r in verif)
                    {
                        TempData["RP"] = r.Reponse;
                        TempData["RP"] = r.Reponse;
                        //TempData["Question"] = r.Question;
                        //TempData["CodeQcm"] = r.CodeQcm;
                        //Session["CodeEval"] = r.Code_eval;
                    }

                    var rp = from m in db.E_RepUser
                             where m.Code_eval == ooid && m.NumQ == (slid)
                             select m;


                    foreach (E_RepUser z in rp)
                    {
                        aa = z.Reponse;
                    }

                    if (verif.Count() != 0)
                    {
                        SqlCommand cmd2 = new SqlCommand("delete from  E_RepUser where Code_eval = '" + ooid + "' and NumQ = " + slid + "", con);

                        cmd2.ExecuteNonQuery();
                    }

                    E_RepUser e_RepUser = new E_RepUser();

                    e_RepUser.MatUser = user.matricule;

                    e_RepUser.Code_eval = Eval;

                    e_RepUser.CodeQcm = CodeQcm;

                    e_RepUser.NumQ = slid;

                    e_RepUser.Question = Question;

                    e_RepUser.Date_Creation = System.DateTime.Now;

                    if (Reponse.ToString() != "")
                        e_RepUser.Reponse = Reponse;
                    else
                    {
                        e_RepUser.Reponse = aa;

                    }

                    SqlCommand cmd = new SqlCommand("Insert into E_RepUser (Code_eval, NumQ, CodeQcm,Question,Reponse,Date_Creation,MatUser) values(@Code_eval, @NumQ, @CodeQcm, @Question,@Reponse,@Date_Creation,@MatUser)", con);

                    cmd.Parameters.AddWithValue("@Code_eval", e_RepUser.Code_eval);
                    cmd.Parameters.AddWithValue("@NumQ", e_RepUser.NumQ);
                    cmd.Parameters.AddWithValue("@CodeQcm", e_RepUser.CodeQcm);
                    cmd.Parameters.AddWithValue("@Question", e_RepUser.Question);
                    cmd.Parameters.AddWithValue("@Reponse", e_RepUser.Reponse);
                    cmd.Parameters.AddWithValue("@Date_Creation", e_RepUser.Date_Creation);
                    cmd.Parameters.AddWithValue("@MatUser", e_RepUser.MatUser);

                    cmd.ExecuteNonQuery();

                    db.E_RepUser.Add(e_RepUser);

                    int c = Convert.ToInt32(slid) - 1;

                    E_QCM eq = (from m in db.E_QCM
                                where m.Code_EvalByQCM == ooid && m.NumQ == c
                                select m).Single();

                    TempData["Reponse"] = Reponse;

                    return View(eq);

                }
            }

            if (nbr < dt.Rows.Count)
            {
                if (i < dt.Rows.Count)
                {
                    e_QCM.Code_QCM = Convert.ToString(dt.Rows[i]["Code_QCM"]);

                    e_QCM.Question = Convert.ToString(dt.Rows[i]["Question"]);

                    e_QCM.NumQ = 1;

                    e_QCM.Reponse1 = Convert.ToString(dt.Rows[i]["Reponse1"]);

                    e_QCM.Reponse2 = Convert.ToString(dt.Rows[i]["Reponse2"]);

                    e_QCM.Reponse3 = Convert.ToString(dt.Rows[i]["Reponse3"]);

                    e_QCM.Reponse4 = Convert.ToString(dt.Rows[i]["Reponse4"]);


                    i++;

                    return View(e_QCM);
                }
            }
            //else if (nbr == dt.Rows.Count)
            //{
            //    i = 0;

            //    return RedirectToAction("ResultQCM", "E_QCM" , new { codeEval = id });


            //}


            ViewBag.pp = i;



            return View();



        }

        static public int nbdiapo = -1;



        // GET: E_QCM/Details/5
        public ActionResult Details(int? id, string codeEval)
        {
            ViewBag.codeEval = codeEval;

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

        // GET: E_QCM/Create
        public ActionResult Create(string codeEval)
        {
            ViewBag.codeEval = codeEval;
            return View();
        }



        // POST: E_QCM/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Code_QCM,Code_EvalByQCM,Date_Creation,Question,Reponse1,Reponse2,Reponse3, Reponse4,Coeff")] E_QCM e_QCM, string codeEvl, string EtatRep1, string EtatRep2, string EtatRep3, string EtatRep4)
        {
            if (ModelState.IsValid)
            {
                if (EtatRep1 == "Faux" && EtatRep2 == "Faux" && EtatRep3 == "Faux" && EtatRep4 == "Faux")
                {
                    ModelState.AddModelError("", "Il faut au  moin une réponse vrai.");
                    ViewBag.codeEval = codeEvl;

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

                //if(countA >1)
                //{
                //    ModelState.AddModelError("", "Il faut une seule réponse vrai.");
                //    ViewBag.codeEval = codeEvl;
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

                var qcm = from m in db.E_QCM
                          where m.Code_EvalByQCM == e_QCM.Code_EvalByQCM
                          select m;

                int o = qcm.Count();

                if (o != 0)
                {
                    //foreach(E_QCM zz in qcm)
                    //{ 
                    e_QCM.NumQ = o + 1;
                    //}
                }
                else
                {
                    e_QCM.NumQ = 1;
                }


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
                    SqlCommand cmd = new SqlCommand("Insert into E_QCM (Code_QCM,NumQ, Code_EvalByQCM,Date_Creation,Question,Coeff,Reponse1,Reponse2,Date_Modif,EtatRep1,EtatRep2,EtatRep3,EtatRep4) values(@Code_QCM, @NumQ, @Code_EvalByQCM,@Date_Creation,@Question,@Coeff,@Reponse1,@Reponse2,@Date_Modif,@EtatRep1,@EtatRep2,@EtatRep3,@EtatRep4)", con);

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
                    SqlCommand cmd = new SqlCommand("Insert into E_QCM (Code_QCM,NumQ, Code_EvalByQCM,Date_Creation,Question,Coeff,Reponse1,Reponse2,Reponse3,Date_Modif,EtatRep1,EtatRep2,EtatRep3,EtatRep4) values(@Code_QCM, @NumQ, @Code_EvalByQCM,@Date_Creation,@Question,@Coeff,@Reponse1,@Reponse2,@Reponse4,@Date_Modif,@EtatRep1,@EtatRep2,@EtatRep3,@EtatRep4)", con);

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
                return RedirectToAction("Index", new { codeEval = e_QCM.Code_EvalByQCM });
            }

            return View(e_QCM);
        }

        // GET: E_QCM/Edit/5
        public ActionResult Edit(int? id, string codeEval)
        {
            ViewBag.codeEval = codeEval;

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




        public ActionResult Terminer(string codeEval, string score, string valide)
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());

            string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection con = new SqlConnection(constr);
            con.Open();

            int r = Convert.ToInt32(score);


            SqlCommand cmd = new SqlCommand("Update   E_ResultQCM set Score = '" + r + "' , Resultat = '" + valide + "' where MatUser = " + user.matricule + " and Code_EvalByQCM = '" + codeEval + "' ", con);


            cmd.ExecuteNonQuery();




            return RedirectToAction("Index", "EMenuEvalUser");
        }

        public ActionResult ResultQCMParSlide(string codeEval, string Slide , string Terminer , string sessionminf)
        {
            Session["MinF"] = sessionminf;

            string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection con = new SqlConnection(constr);
            con.Open();


           // SqlCommand command3 = new SqlCommand("select DATEDIFF(SECOND, '0:00:00', Duree_Eval ) sec from E_Evaluation where Code_Eval='" + codeEval + "'    ", con);
           // SqlDataAdapter da3 = new SqlDataAdapter(command3);
           // DataTable dt3 = new DataTable();
           // dt3.Clear();
           // da3.Fill(dt3);

           //for (int i=0; i< dt3.Rows.Count; i++)
           // {
           //     Session["min"] = dt3.Rows[i]["sec"].ToString();
                 
           // }


            if (Session["Evl"] != null)
            {
                codeEval = Session["Evl"].ToString();
            }

            Session["SNQ"] = Convert.ToInt32(Slide) + 1;

            codeEval = codeEval.Substring(0, 16);
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());


            int cpt = 0;

            int cptTotal = 0;

          

            DataTable dt = new DataTable();

            //SqlDataAdapter da;
            //da = new SqlDataAdapter("select Code_EvalByQCM, Code_QCM, E_RepUser.Date_Creation DateEval ,  E_RepUser.MatUser, E_QCM.Question, Coeff, E_QCM.Reponse1, E_QCM.EtatRep1,E_QCM.Reponse2,E_QCM.EtatRep2,E_QCM.Reponse3, E_QCM.EtatRep3,E_QCM.Reponse4,  E_QCM.EtatRep4, E_RepUser.Reponse ReponseUser from E_QCM inner join E_RepUser on E_RepUser.Code_eval = E_QCM.Code_EvalByQCM and E_RepUser.CodeQcm = E_QCM.Code_QCM where E_QCM.Code_EvalByQCM = '" + codeEval + "' and E_RepUser.MatUser = "+user.matricule+"", con);
            //da.Fill(dt);

            SqlDataAdapter da;
            da = new SqlDataAdapter("select distinct Code_EvalByQCM,  E_Formation.Code CodeForm, Code_QCM, E_ListEvaluationDiffus.Objet ObjEval,  E_ListEvaluationDiffus.deadline,  E_Formation.Objet ObjForm, convert(date,E_RepUser.Date_Creation,103) DateEval ,  E_RepUser.MatUser,    AspNetUsers.NomPrenom Usr , E_QCM.Question, Coeff,  E_QCM.Reponse1, E_QCM.EtatRep1, E_QCM.Reponse2, E_QCM.EtatRep2, E_QCM.Reponse3, E_QCM.EtatRep3, E_QCM.Reponse4, E_QCM.EtatRep4    from E_QCM inner join E_RepUser on E_RepUser.Code_eval = E_QCM.Code_EvalByQCM and E_RepUser.CodeQcm = E_QCM.Code_QCM inner join E_ListEvaluationDiffus on E_ListEvaluationDiffus.Code_Eval = E_QCM.Code_EvalByQCM left join E_Formation on E_Formation.CodeEval = E_QCM.Code_EvalByQCM   inner join AspNetUsers on AspNetUsers.matricule = E_ListEvaluationDiffus.Mat_usr where E_QCM.Code_EvalByQCM = '" + codeEval + "' and AspNetUsers.matricule = " + user.matricule + " and E_QCM.NumQ = "+ Slide+"", con);
            da.Fill(dt);


            ViewBag.CodeEval = codeEval;

            List<E_ResultQCMParSlide> list = new List<E_ResultQCMParSlide>();

            E_ResultQCMParSlide e = new E_ResultQCMParSlide();


            for (int i = 0; i < dt.Rows.Count; i++)
            {

                e.Code_EvalByQCM = dt.Rows[i]["Code_EvalByQCM"].ToString();

                e.ObjEval = dt.Rows[i]["ObjEval"].ToString();

                e.CodeForm = dt.Rows[i]["CodeForm"].ToString();

                e.ObjForm = dt.Rows[i]["ObjForm"].ToString();

                e.ObjEval = dt.Rows[i]["ObjEval"].ToString();

                e.Code_QCM = dt.Rows[i]["Code_QCM"].ToString();

                e.DeadLine = Convert.ToDateTime(dt.Rows[i]["deadline"].ToString());

                e.MatUser = dt.Rows[i]["MatUser"].ToString();

                e.Usr = dt.Rows[i]["Usr"].ToString();

                e.Question = dt.Rows[i]["Question"].ToString();

                e.Coeff = Convert.ToInt32(dt.Rows[i]["Coeff"].ToString());

                e.Reponse1 = dt.Rows[i]["Reponse1"].ToString();

                e.EtatRep1 = dt.Rows[i]["EtatRep1"].ToString();

                e.Reponse2 = dt.Rows[i]["Reponse2"].ToString();

                e.EtatRep2 = dt.Rows[i]["EtatRep2"].ToString();

                e.Reponse3 = dt.Rows[i]["Reponse3"].ToString();

                e.EtatRep3 = dt.Rows[i]["EtatRep3"].ToString();

                e.Reponse4 = dt.Rows[i]["Reponse4"].ToString();

                e.EtatRep4 = dt.Rows[i]["EtatRep4"].ToString();

                //e.ReponseUser = dt.Rows[i]["ReponseUser"].ToString();


                e.DateEval = Convert.ToDateTime(dt.Rows[i]["DateEval"].ToString());

                //if ((e.Reponse1 == e.ReponseUser && e.EtatRep1 == "Vrai") || (e.Reponse2 == e.ReponseUser && e.EtatRep2 == "Vrai") || (e.Reponse3 == e.ReponseUser && e.EtatRep3 == "Vrai") || (e.Reponse4 == e.ReponseUser && e.EtatRep4 == "Vrai"))
                //{
                //    cpt += e.Coeff;
                //}


                SqlCommand cmd = new SqlCommand("Insert into E_ResultQCMParSlide (Code_EvalByQCM,   CodeForm, ObjEval, ObjForm, Deadline, Code_QCM,DateEval,MatUser,Usr, Question,Coeff,Reponse1,EtatRep1,Reponse2,EtatRep2,Reponse3,EtatRep3,Reponse4,EtatRep4) values(@Code_EvalByQCM,   @CodeForm, @ObjEval, @ObjForm, @Deadline,@Code_QCM,@DateEval,@MatUser, @Usr,@Question,@Coeff,@Reponse1,@EtatRep1,@Reponse2,@EtatRep2,@Reponse3,@EtatRep3,@Reponse4,@EtatRep4)", con);

                cmd.Parameters.AddWithValue("@Code_EvalByQCM", e.Code_EvalByQCM);
                cmd.Parameters.AddWithValue("@ObjForm", e.ObjForm);
                cmd.Parameters.AddWithValue("@ObjEval", e.ObjEval);
                cmd.Parameters.AddWithValue("@CodeForm", e.CodeForm);
                cmd.Parameters.AddWithValue("@DeadLine", e.DeadLine);
                cmd.Parameters.AddWithValue("@Code_QCM", e.Code_QCM);
                cmd.Parameters.AddWithValue("@DateEval", e.DateEval);
                cmd.Parameters.AddWithValue("@MatUser", e.MatUser);
                cmd.Parameters.AddWithValue("@Usr", e.Usr);
                cmd.Parameters.AddWithValue("@Question", e.Question);
                cmd.Parameters.AddWithValue("@Coeff", e.Coeff);
                cmd.Parameters.AddWithValue("@Reponse1", e.Reponse1);
                cmd.Parameters.AddWithValue("@EtatRep1", e.EtatRep1);
                cmd.Parameters.AddWithValue("@Reponse2", e.Reponse2);
                cmd.Parameters.AddWithValue("@EtatRep2", e.EtatRep2);
                cmd.Parameters.AddWithValue("@Reponse3", e.Reponse3);
                cmd.Parameters.AddWithValue("@EtatRep3", e.EtatRep3);
                cmd.Parameters.AddWithValue("@Reponse4", e.Reponse4);
                cmd.Parameters.AddWithValue("@EtatRep4", e.EtatRep4);
                //cmd.Parameters.AddWithValue("@ReponseUser", e.ReponseUser);

                cmd.ExecuteNonQuery();



                //list.Add(e);
            }


            var listR = from m in db.E_ResultQCMParSlide
                        where m.Code_EvalByQCM == codeEval
                        select m;


            foreach (E_ResultQCMParSlide v in listR)
            {

                DataTable dt7 = new DataTable();
                SqlDataAdapter da7;
                da7 = new SqlDataAdapter("select  * from E_RepUser where CodeQcm='" + v.Code_QCM + "' and Ordre = 1", con);
                da7.Fill(dt7);

                if (dt7.Rows.Count != 0)
                {
                    for (int i = 0; i < dt7.Rows.Count; i++)
                    {
                        SqlCommand cmd = new SqlCommand("Update E_ResultQCMParSlide set Reponse1User = '" + dt7.Rows[i]["Reponse"].ToString() + "' where Code_QCM = '" + dt7.Rows[i]["CodeQcm"].ToString() + "'", con);
                        cmd.ExecuteNonQuery();
                    }

                }


                DataTable dt8 = new DataTable();
                SqlDataAdapter da8;
                da8 = new SqlDataAdapter("select  * from E_RepUser where CodeQcm='" + v.Code_QCM + "' and Ordre = 2", con);
                da8.Fill(dt8);

                if (dt8.Rows.Count != 0)
                {
                    for (int i = 0; i < dt8.Rows.Count; i++)
                    {
                        SqlCommand cmd = new SqlCommand("Update E_ResultQCMParSlide set Reponse2User = '" + dt8.Rows[i]["Reponse"].ToString() + "' where Code_QCM = '" + dt8.Rows[i]["CodeQcm"].ToString() + "'", con);
                        cmd.ExecuteNonQuery();
                    }

                }

                DataTable dt9 = new DataTable();
                SqlDataAdapter da9;
                da9 = new SqlDataAdapter("select  * from E_RepUser where CodeQcm='" + v.Code_QCM + "' and Ordre = 3", con);
                da9.Fill(dt9);

                if (dt9.Rows.Count != 0)
                {
                    for (int i = 0; i < dt9.Rows.Count; i++)
                    {
                        SqlCommand cmd = new SqlCommand("Update E_ResultQCMParSlide set Reponse3User = '" + dt9.Rows[i]["Reponse"].ToString() + "' where Code_QCM = '" + dt9.Rows[i]["CodeQcm"].ToString() + "'", con);
                        cmd.ExecuteNonQuery();
                    }

                }

                DataTable dt6 = new DataTable();
                SqlDataAdapter da6;
                da6 = new SqlDataAdapter("select  * from E_RepUser where CodeQcm='" + v.Code_QCM + "' and Ordre = 4", con);
                da6.Fill(dt6);

                if (dt6.Rows.Count != 0)
                {
                    for (int i = 0; i < dt6.Rows.Count; i++)
                    {
                        SqlCommand cmd = new SqlCommand("Update E_ResultQCMParSlide set Reponse4User = '" + dt6.Rows[i]["Reponse"].ToString() + "' where Code_QCM = '" + dt6.Rows[i]["CodeQcm"].ToString() + "'", con);
                        cmd.ExecuteNonQuery();
                    }

                }
           
                 
            DataTable dt66 = new DataTable();
            SqlDataAdapter da66;
            da66 = new SqlDataAdapter("select  * from E_ResultQCMParSlide where Code_EvalByQCM= '" + codeEval + "' and Code_QCM = '" + v.Code_QCM + "' ", con);
            da66.Fill(dt66);


            for (int oo = 0; oo < dt66.Rows.Count; oo++)
            {
                e.Reponse1User = dt66.Rows[oo]["Reponse1User"].ToString();

                e.Reponse2User = dt66.Rows[oo]["Reponse2User"].ToString();

                e.Reponse3User = dt66.Rows[oo]["Reponse3User"].ToString();

                e.Reponse4User = dt66.Rows[oo]["Reponse4User"].ToString();
            }
            }

            int cptQ = 0;


            var listQCM = from m in db.E_QCM
                          where m.Code_EvalByQCM == codeEval
                          select m;


            DataTable dtr = new DataTable();
            SqlDataAdapter dar;
            dar = new SqlDataAdapter("select  * from E_ResultQCMParSlide where Code_EvalByQCM= '" + codeEval + "' ", con);
            dar.Fill(dtr);

            string u1 = null;
            string u2 = null;
            string u3 = null;
            string u4 = null;
            string s1 = null;
            string s2 = null;
            string s3 = null;
            string s4 = null;

            for (int rr = 0; rr < dtr.Rows.Count; rr++)
            {
                int cpt2 = 0;

                DataTable dty = new DataTable();
                SqlDataAdapter day;
                day = new SqlDataAdapter("select  * from E_ResultQCMParSlide where Code_EvalByQCM= '" + codeEval + "' and  Code_QCM = '" + dtr.Rows[rr]["Code_QCM"].ToString() + "'", con);
                day.Fill(dty);

                List<string> liste = new List<string>();

                for (int yy = 0; yy < dty.Rows.Count; yy++)
                {
                    string a = dty.Rows[yy]["Reponse1"].ToString();

                    if ((dty.Rows[yy]["Reponse1User"].ToString() == dty.Rows[yy]["Reponse1"].ToString() && dty.Rows[yy]["EtatRep1"].ToString() == "Vrai") || (dty.Rows[yy]["Reponse2User"].ToString() == dty.Rows[yy]["Reponse1"].ToString() && dty.Rows[yy]["EtatRep1"].ToString() == "Vrai") || (dty.Rows[yy]["Reponse3User"].ToString() == dty.Rows[yy]["Reponse1"].ToString() && dty.Rows[yy]["EtatRep1"].ToString() == "Vrai") || (dty.Rows[yy]["Reponse4User"].ToString() == dty.Rows[yy]["Reponse1"].ToString() && dty.Rows[yy]["EtatRep1"].ToString() == "Vrai"))
                    {
                          u1 = "r1";
                        cpt2 += Convert.ToInt32(dty.Rows[yy]["Coeff"].ToString());
                    }

                    if ((dty.Rows[yy]["Reponse1User"].ToString() == dty.Rows[yy]["Reponse2"].ToString() && dty.Rows[yy]["EtatRep2"].ToString() == "Vrai") || (dty.Rows[yy]["Reponse2User"].ToString() == dty.Rows[yy]["Reponse2"].ToString() && dty.Rows[yy]["EtatRep2"].ToString() == "Vrai") || (dty.Rows[yy]["Reponse3User"].ToString() == dty.Rows[yy]["Reponse2"].ToString() && dty.Rows[yy]["EtatRep2"].ToString() == "Vrai") || (dty.Rows[yy]["Reponse4User"].ToString() == dty.Rows[yy]["Reponse2"].ToString() && dty.Rows[yy]["EtatRep2"].ToString() == "Vrai"))
                    {
                          u2 = "r2";
                        cpt2 += Convert.ToInt32(dty.Rows[yy]["Coeff"].ToString());
                    }

                    if ((dty.Rows[yy]["Reponse1User"].ToString() == dty.Rows[yy]["Reponse3"].ToString() && dty.Rows[yy]["EtatRep3"].ToString() == "Vrai") || (dty.Rows[yy]["Reponse2User"].ToString() == dty.Rows[yy]["Reponse3"].ToString() && dty.Rows[yy]["EtatRep3"].ToString() == "Vrai") || (dty.Rows[yy]["Reponse3User"].ToString() == dty.Rows[yy]["Reponse3"].ToString() && dty.Rows[yy]["EtatRep3"].ToString() == "Vrai") || (dty.Rows[yy]["Reponse4User"].ToString() == dty.Rows[yy]["Reponse3"].ToString() && dty.Rows[yy]["EtatRep3"].ToString() == "Vrai"))
                    {
                          u3 = "r3";
                        cpt2 += Convert.ToInt32(dty.Rows[yy]["Coeff"].ToString());
                    }


                    if ((dty.Rows[yy]["Reponse1User"].ToString() == dty.Rows[yy]["Reponse4"].ToString() && dty.Rows[yy]["EtatRep4"].ToString() == "Vrai") || (dty.Rows[yy]["Reponse2User"].ToString() == dty.Rows[yy]["Reponse4"].ToString() && dty.Rows[yy]["EtatRep4"].ToString() == "Vrai") || (dty.Rows[yy]["Reponse3User"].ToString() == dty.Rows[yy]["Reponse4"].ToString() && dty.Rows[yy]["EtatRep4"].ToString() == "Vrai") || (dty.Rows[yy]["Reponse4User"].ToString() == dty.Rows[yy]["Reponse4"].ToString() && dty.Rows[yy]["EtatRep4"].ToString() == "Vrai"))
                    {
                          u4 = "r4";
                        cpt2 += Convert.ToInt32(dty.Rows[yy]["Coeff"].ToString());
                    }


                }


                string ss;

                if (rr == dty.Rows.Count)
                    ss = dty.Rows[rr - 1]["Code_QCM"].ToString();
                else
                    ss = dty.Rows[rr]["Code_QCM"].ToString();


                var ee = from m in db.E_QCM
                         where m.Code_EvalByQCM == codeEval && m.Code_QCM == ss
                         select m;

                int contVrai = 0;



                foreach (E_QCM et in ee)
                {
                    if (et.EtatRep1 == "Vrai")
                    {
                          s1 = "r1";
                        contVrai += 1;
                    }
                    if (et.EtatRep2 == "Vrai")
                    {
                          s2 = "r2";
                        contVrai += 1;
                    }

                    if (et.EtatRep3 == "Vrai")
                    {
                          s3 = "r3";
                        contVrai += 1;
                    }

                    if (et.EtatRep4 == "Vrai")
                    {
                          s4 = "r4";
                        contVrai += 1;
                    }
                }

              
                if ( cpt2 < contVrai)
                {
                    if (s1 != u1)
                    {
                        Session["Incomp1"] = "Non coché";
                    }

                    if (s2 != u2)
                    {
                        Session["Incomp2"] = "Non coché";
                    }

                    if (s3 != u3)
                    {
                        Session["Incomp3"] = "Non coché";
                    }

                    if (s4 != u4)
                    {
                        Session["Incomp4"] = "Non coché";
                    }

                }
            }


            if (Terminer != null)
            {
                return RedirectToAction("ResultQCM", "E_QCM", new { codeEval = codeEval });

            }
     
            return View(e);

        
            //var listQCM = from m in db.E_QCM
            //              where m.Code_EvalByQCM == codeEval
            //              select m;


            //foreach (E_QCM e in listQCM)
            //{
            //      cptTotal += e.Coeff;


            //}
            //double scr = Math.Round((float)cpt / (float)cptTotal * 100);

            //ViewBag.Score = scr;

            //var prcVal = from m in db.E_Evaluation
            //             where m.Code_Eval == codeEval
            //             select m;

            //foreach (E_Evaluation d in prcVal)
            //{
            //    if (scr >= d.Pourc_Valid)
            //    {
            //        ViewBag.Valid = "Valide";
            //    }
            //    else if (scr < d.Pourc_Valid)
            //    {
            //        ViewBag.Valid = "Invalide";
            //    }
            //}


            //SqlCommand cmd2 = new SqlCommand("Update E_ResultQCM set Resultat = '" + ViewBag.Valid + "' , Score = " + scr + " where Code_EvalByQCM = '" + codeEval + "'", con);
            //cmd2.ExecuteNonQuery();


            //var vf = from m in db.E_ResultQCM
            //         where m.Code_EvalByQCM == codeEval && m.Resultat == "Invalide"
            //         select m;


            //foreach (E_ResultQCM r in vf)
            //{

            //    SqlCommand cmd = new SqlCommand("INSERT INTO [dbo].[E_ResultQCM_Historiq] (DateHisto, [Code_EvalByQCM] ,[CodeForm] ,[Code_QCM] ,[MatUser] ,[Usr]  ,[DateEval]  ,[Score] ,[Resultat],[ObjEval],[ObjForm],[DeadLine],[Question],[Coeff],[Reponse1],[EtatRep1],[Reponse2],[EtatRep2] ,[Reponse3],[EtatRep3],[Reponse4],[EtatRep4],[ReponseUser]) VALUES (@DateHisto, @Code_EvalByQCM ,@CodeForm ,@Code_QCM ,@MatUser ,@Usr  ,@DateEval  ,@Score ,@Resultat, @ObjEval ,@ObjForm ,@DeadLine ,@Question ,@Coeff ,@Reponse1 ,@EtatRep1 ,@Reponse2 ,@EtatRep2 ,@Reponse3 ,@EtatRep3 ,@Reponse4 ,@EtatRep4 ,@ReponseUser)", con);

            //    cmd.Parameters.AddWithValue("@DateHisto", System.DateTime.Now);
            //    cmd.Parameters.AddWithValue("@Code_EvalByQCM", r.Code_EvalByQCM);
            //    cmd.Parameters.AddWithValue("@Code_QCM", r.Code_QCM);
            //    cmd.Parameters.AddWithValue("@CodeForm", r.CodeForm);
            //    cmd.Parameters.AddWithValue("@MatUser", r.MatUser);
            //    cmd.Parameters.AddWithValue("@Usr", r.Usr);
            //    cmd.Parameters.AddWithValue("@DateEval", r.DateEval);
            //    cmd.Parameters.AddWithValue("@Score", r.Score);
            //    cmd.Parameters.AddWithValue("@Resultat", r.Resultat);
            //    cmd.Parameters.AddWithValue("@ObjEval", r.ObjEval);
            //    cmd.Parameters.AddWithValue("@ObjForm", r.ObjForm);
            //    cmd.Parameters.AddWithValue("@DeadLine", r.DeadLine);
            //    cmd.Parameters.AddWithValue("@Question", r.Question);
            //    cmd.Parameters.AddWithValue("@Coeff", r.Coeff);
            //    cmd.Parameters.AddWithValue("@Reponse1", r.Reponse1);
            //    cmd.Parameters.AddWithValue("@Reponse2", r.Reponse2);
            //    cmd.Parameters.AddWithValue("@Reponse3", r.Reponse3);
            //    cmd.Parameters.AddWithValue("@Reponse4", r.Reponse4);
            //    cmd.Parameters.AddWithValue("@ReponseUser", r.ReponseUser);
            //    cmd.Parameters.AddWithValue("@EtatRep1", r.EtatRep1);
            //    cmd.Parameters.AddWithValue("@EtatRep2", r.EtatRep2);
            //    cmd.Parameters.AddWithValue("@EtatRep3", r.EtatRep3);
            //    cmd.Parameters.AddWithValue("@EtatRep4", r.EtatRep4);

            //    cmd.ExecuteNonQuery();

            //    SqlCommand cmd3 = new SqlCommand("delete from E_ResultQCM where Code_EvalByQCM = '" + r.Code_EvalByQCM + "' and Resultat = 'Invalide'", con);
            //    cmd3.ExecuteNonQuery();

            //    SqlCommand cmd4 = new SqlCommand("delete from E_RepUser where Code_eval = '" + r.Code_EvalByQCM + "'", con);
            //    cmd4.ExecuteNonQuery();


            //    DataTable dt5 = new DataTable();

            //    SqlDataAdapter da5;
            //    da5 = new SqlDataAdapter("select [Code_Formation] ,[ObjForm] ,[MatUser] ,[Usr]  ,[DateTerm]  ,[Resultat] ,E_ResultFormation.[DeadLine] ,[Etat] from E_ResultFormation inner join [E_ListFormationDiffus] on [E_ListFormationDiffus].Code_formt = E_ResultFormation.Code_Formation where[E_ListFormationDiffus].Code_eval = '" + r.Code_EvalByQCM + "' ", con);
            //    da5.Fill(dt5);


            //    for (int i = 0; i < dt5.Rows.Count; i++)
            //    {

            //        SqlCommand cmd5 = new SqlCommand("INSERT INTO [dbo].[E_ResultFormation_Historiq] (DateHisto , [Code_Formation] ,[ObjForm] ,[MatUser] ,[Usr]  ,[DateTerm]  ,[Resultat] ,[DeadLine] ,[Etat])  VALUES (@DateHisto , @Code_Formation ,@ObjForm ,@MatUser ,@Usr  ,@DateTerm  ,@Resultat ,@DeadLine ,@Etat)", con);

            //        cmd5.Parameters.AddWithValue("@DateHisto", System.DateTime.Now);
            //        cmd5.Parameters.AddWithValue("@Code_Formation", dt5.Rows[i]["Code_Formation"].ToString());
            //        cmd5.Parameters.AddWithValue("@ObjForm", dt5.Rows[i]["ObjForm"].ToString());
            //        cmd5.Parameters.AddWithValue("@MatUser", dt5.Rows[i]["MatUser"].ToString());
            //        cmd5.Parameters.AddWithValue("@Usr", dt5.Rows[i]["Usr"].ToString());
            //        cmd5.Parameters.AddWithValue("@DateTerm", Convert.ToDateTime(dt5.Rows[i]["DateTerm"].ToString()));
            //        cmd5.Parameters.AddWithValue("@Resultat", dt5.Rows[i]["Resultat"].ToString());
            //        cmd5.Parameters.AddWithValue("@DeadLine", Convert.ToDateTime(dt5.Rows[i]["DeadLine"].ToString()));
            //        cmd5.Parameters.AddWithValue("@Etat", dt5.Rows[i]["Etat"].ToString());

            //        cmd5.ExecuteNonQuery();


            //        SqlCommand cmd6 = new SqlCommand("delete from E_ResultFormation where Code_Formation = '" + dt5.Rows[i]["Code_Formation"].ToString() + "' ", con);
            //        cmd6.ExecuteNonQuery();


            //        SqlCommand cmd7 = new SqlCommand("delete from E_SlideUsr where Code_Formation = '" + dt5.Rows[i]["Code_Formation"].ToString() + "' ", con);
            //        cmd7.ExecuteNonQuery();

            // }

            //}






            //return View(list);
        }


        public ActionResult ResultQCM(string codeEval)
        {
             
            if (Session["Evl"] != null)
            {
                codeEval = Session["Evl"].ToString();
            }


            codeEval = codeEval.Substring(0, 16);
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());


            int cptQ = 0;

            int cptTotal = 0;

            string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection con = new SqlConnection(constr);
            con.Open();

            DataTable dt = new DataTable();

            //SqlDataAdapter da;
            //da = new SqlDataAdapter("select Code_EvalByQCM, Code_QCM, E_RepUser.Date_Creation DateEval ,  E_RepUser.MatUser, E_QCM.Question, Coeff, E_QCM.Reponse1, E_QCM.EtatRep1,E_QCM.Reponse2,E_QCM.EtatRep2,E_QCM.Reponse3, E_QCM.EtatRep3,E_QCM.Reponse4,  E_QCM.EtatRep4, E_RepUser.Reponse ReponseUser from E_QCM inner join E_RepUser on E_RepUser.Code_eval = E_QCM.Code_EvalByQCM and E_RepUser.CodeQcm = E_QCM.Code_QCM where E_QCM.Code_EvalByQCM = '" + codeEval + "' and E_RepUser.MatUser = "+user.matricule+"", con);
            //da.Fill(dt);

            SqlDataAdapter da;
            da = new SqlDataAdapter("select distinct Code_EvalByQCM,  E_Formation.Code CodeForm, Code_QCM, E_ListEvaluationDiffus.Objet ObjEval,  E_ListEvaluationDiffus.deadline,  E_Formation.Objet ObjForm, CONVERT(date, E_RepUser.Date_Creation,103) DateEval ,  E_RepUser.MatUser,    AspNetUsers.NomPrenom Usr , E_QCM.Question, Coeff,  E_QCM.Reponse1, E_QCM.EtatRep1, E_QCM.Reponse2, E_QCM.EtatRep2, E_QCM.Reponse3, E_QCM.EtatRep3, E_QCM.Reponse4, E_QCM.EtatRep4  from E_QCM inner join E_RepUser on E_RepUser.Code_eval = E_QCM.Code_EvalByQCM and E_RepUser.CodeQcm = E_QCM.Code_QCM inner join E_ListEvaluationDiffus on E_ListEvaluationDiffus.Code_Eval = E_QCM.Code_EvalByQCM left join E_Formation on E_Formation.CodeEval = E_QCM.Code_EvalByQCM   inner join AspNetUsers on AspNetUsers.matricule = E_ListEvaluationDiffus.Mat_usr where E_QCM.Code_EvalByQCM = '" + codeEval + "' and AspNetUsers.matricule = " + user.matricule + "", con);
            da.Fill(dt);
             

            ViewBag.CodeEval = codeEval;

            List<E_ResultQCM> list = new List<E_ResultQCM>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                E_ResultQCM e = new E_ResultQCM();

                e.Code_EvalByQCM = dt.Rows[i]["Code_EvalByQCM"].ToString();

                e.ObjEval = dt.Rows[i]["ObjEval"].ToString();

                e.CodeForm = dt.Rows[i]["CodeForm"].ToString();

                e.ObjForm = dt.Rows[i]["ObjForm"].ToString();

                e.ObjEval = dt.Rows[i]["ObjEval"].ToString();

                e.Code_QCM = dt.Rows[i]["Code_QCM"].ToString();

                e.DeadLine = Convert.ToDateTime(dt.Rows[i]["deadline"].ToString());

                e.MatUser = dt.Rows[i]["MatUser"].ToString();

                e.Usr = dt.Rows[i]["Usr"].ToString();

                e.Question = dt.Rows[i]["Question"].ToString();

                e.Coeff = Convert.ToInt32(dt.Rows[i]["Coeff"].ToString());

                e.Reponse1 = dt.Rows[i]["Reponse1"].ToString();

                e.EtatRep1 = dt.Rows[i]["EtatRep1"].ToString();

                e.Reponse2 = dt.Rows[i]["Reponse2"].ToString();

                e.EtatRep2 = dt.Rows[i]["EtatRep2"].ToString();

                e.Reponse3 = dt.Rows[i]["Reponse3"].ToString();

                e.EtatRep3 = dt.Rows[i]["EtatRep3"].ToString();

                e.Reponse4 = dt.Rows[i]["Reponse4"].ToString();

                e.EtatRep4 = dt.Rows[i]["EtatRep4"].ToString();

                //e.ReponseUser = dt.Rows[i]["ReponseUser"].ToString();


                e.DateEval = Convert.ToDateTime(dt.Rows[i]["DateEval"].ToString());

                //if ((e.Reponse1 == e.ReponseUser && e.EtatRep1 == "Vrai") || (e.Reponse2 == e.ReponseUser && e.EtatRep2 == "Vrai") || (e.Reponse3 == e.ReponseUser && e.EtatRep3 == "Vrai") || (e.Reponse4 == e.ReponseUser && e.EtatRep4 == "Vrai"))
                //{
                //    cpt += e.Coeff;
                //}


                SqlCommand cmd = new SqlCommand("Insert into E_ResultQCM (Code_EvalByQCM,   CodeForm, ObjEval, ObjForm, Deadline, Code_QCM,DateEval,MatUser,Usr, Question,Coeff,Reponse1,EtatRep1,Reponse2,EtatRep2,Reponse3,EtatRep3,Reponse4,EtatRep4) values(@Code_EvalByQCM,   @CodeForm, @ObjEval, @ObjForm, @Deadline,@Code_QCM,@DateEval,@MatUser, @Usr,@Question,@Coeff,@Reponse1,@EtatRep1,@Reponse2,@EtatRep2,@Reponse3,@EtatRep3,@Reponse4,@EtatRep4)", con);

                cmd.Parameters.AddWithValue("@Code_EvalByQCM", e.Code_EvalByQCM);
                cmd.Parameters.AddWithValue("@ObjForm", e.ObjForm);
                cmd.Parameters.AddWithValue("@ObjEval", e.ObjEval);
                cmd.Parameters.AddWithValue("@CodeForm", e.CodeForm);
                cmd.Parameters.AddWithValue("@DeadLine", e.DeadLine);
                cmd.Parameters.AddWithValue("@Code_QCM", e.Code_QCM);
                cmd.Parameters.AddWithValue("@DateEval", e.DateEval);
                cmd.Parameters.AddWithValue("@MatUser", e.MatUser);
                cmd.Parameters.AddWithValue("@Usr", e.Usr);
                cmd.Parameters.AddWithValue("@Question", e.Question);
                cmd.Parameters.AddWithValue("@Coeff", e.Coeff);
                cmd.Parameters.AddWithValue("@Reponse1", e.Reponse1);
                cmd.Parameters.AddWithValue("@EtatRep1", e.EtatRep1);
                cmd.Parameters.AddWithValue("@Reponse2", e.Reponse2);
                cmd.Parameters.AddWithValue("@EtatRep2", e.EtatRep2);
                cmd.Parameters.AddWithValue("@Reponse3", e.Reponse3);
                cmd.Parameters.AddWithValue("@EtatRep3", e.EtatRep3);
                cmd.Parameters.AddWithValue("@Reponse4", e.Reponse4);
                cmd.Parameters.AddWithValue("@EtatRep4", e.EtatRep4);
                //cmd.Parameters.AddWithValue("@ReponseUser", e.ReponseUser);

                cmd.ExecuteNonQuery();


                var listR = from m in db.E_ResultQCMParSlide
                            where m.Code_EvalByQCM == codeEval
                            select m;



                foreach (E_ResultQCMParSlide v in listR)
                {

                    DataTable dt7 = new DataTable();
                    SqlDataAdapter da7;
                    da7 = new SqlDataAdapter("select  * from E_RepUser where CodeQcm='" + v.Code_QCM + "' and Ordre = 1", con);
                    da7.Fill(dt7);

                    if (dt7.Rows.Count != 0)
                    {
                        for (int t = 0; t < dt7.Rows.Count; t++)
                        {
                            SqlCommand cmd3 = new SqlCommand("Update E_ResultQCM set Reponse1User = '" + dt7.Rows[t]["Reponse"].ToString() + "' where Code_QCM = '" + dt7.Rows[t]["CodeQcm"].ToString() + "'", con);
                            cmd3.ExecuteNonQuery();
                        }

                    }


                    DataTable dt8 = new DataTable();
                    SqlDataAdapter da8;
                    da8 = new SqlDataAdapter("select  * from E_RepUser where CodeQcm='" + v.Code_QCM + "' and Ordre = 2", con);
                    da8.Fill(dt8);

                    if (dt8.Rows.Count != 0)
                    {
                        for (int t = 0; t < dt8.Rows.Count; t++)
                        {
                            SqlCommand cmd3 = new SqlCommand("Update E_ResultQCM set Reponse2User = '" + dt8.Rows[t]["Reponse"].ToString() + "' where Code_QCM = '" + dt8.Rows[t]["CodeQcm"].ToString() + "'", con);
                            cmd3.ExecuteNonQuery();
                        }

                    }

                    DataTable dt9 = new DataTable();
                    SqlDataAdapter da9;
                    da9 = new SqlDataAdapter("select  * from E_RepUser where CodeQcm='" + v.Code_QCM + "' and Ordre = 3", con);
                    da9.Fill(dt9);

                    if (dt9.Rows.Count != 0)
                    {
                        for (int t = 0; t < dt9.Rows.Count; t++)
                        {
                            SqlCommand cmd3 = new SqlCommand("Update E_ResultQCM set Reponse3User = '" + dt9.Rows[t]["Reponse"].ToString() + "' where Code_QCM = '" + dt9.Rows[t]["CodeQcm"].ToString() + "'", con);
                            cmd3.ExecuteNonQuery();
                        }

                    }

                    DataTable dt6 = new DataTable();
                    SqlDataAdapter da6;
                    da6 = new SqlDataAdapter("select  * from E_RepUser where CodeQcm='" + v.Code_QCM + "' and Ordre = 4", con);
                    da6.Fill(dt6);

                    if (dt6.Rows.Count != 0)
                    {
                        for (int t = 0; t < dt6.Rows.Count; t++)
                        {
                            SqlCommand cmd3 = new SqlCommand("Update E_ResultQCM set Reponse4User = '" + dt6.Rows[t]["Reponse"].ToString() + "' where Code_QCM = '" + dt6.Rows[t]["CodeQcm"].ToString() + "'", con);
                            cmd3.ExecuteNonQuery();
                        }

                    }

                    //E_ResultQCM ff = (from m in db.E_ResultQCM
                    //                  where m.Code_EvalByQCM == codeEval
                    //                  select m).Single();

                    DataTable dt66 = new DataTable();
                    SqlDataAdapter da66;
                    da66 = new SqlDataAdapter("select  * from E_ResultQCM where Code_EvalByQCM= '" + codeEval+ "' and Code_QCM = '"+dt.Rows[i]["Code_QCM"].ToString()+"' ", con);
                    da66.Fill(dt66);


                    for (int oo= 0; oo <dt66.Rows.Count; oo++)
                    { 
                        e.Reponse1User = dt66.Rows[oo]["Reponse1User"].ToString() ;

                        e.Reponse2User = dt66.Rows[oo]["Reponse2User"].ToString();

                        e.Reponse3User = dt66.Rows[oo]["Reponse3User"].ToString();

                        e.Reponse4User = dt66.Rows[oo]["Reponse4User"].ToString();
                    }



                    //if ((e.Reponse1 == e.Reponse1User && e.EtatRep1 == "Vrai") || (e.Reponse1 == e.Reponse2User && e.EtatRep1 == "Vrai") || (e.Reponse1 == e.Reponse3User && e.EtatRep1 == "Vrai") || (e.Reponse1 == e.Reponse4User && e.EtatRep1 == "Vrai") || (e.Reponse2 == e.Reponse1User && e.EtatRep2 == "Vrai") || (e.Reponse2 == e.Reponse2User && e.EtatRep2 == "Vrai") || (e.Reponse2 == e.Reponse3User && e.EtatRep2 == "Vrai") || (e.Reponse2 == e.Reponse4User && e.EtatRep2 == "Vrai") || (e.Reponse3 == e.Reponse1User && e.EtatRep3 == "Vrai") || (e.Reponse3 == e.Reponse2User && e.EtatRep3 == "Vrai") || (e.Reponse3 == e.Reponse3User && e.EtatRep3 == "Vrai") || (e.Reponse3 == e.Reponse4User && e.EtatRep3 == "Vrai") || (e.Reponse4 == e.Reponse1User && e.EtatRep4 == "Vrai") || (e.Reponse4 == e.Reponse2User && e.EtatRep4 == "Vrai") || (e.Reponse4 == e.Reponse3User && e.EtatRep4 == "Vrai") || (e.Reponse4 == e.Reponse4User && e.EtatRep4 == "Vrai"))
                    //{
                    //    cpt += e.Coeff;
                    //}

                    
                }

                list.Add(e);
            }

            var listQCM = from m in db.E_QCM
                              where m.Code_EvalByQCM == codeEval
                              select m;


            DataTable dtr = new DataTable();
            SqlDataAdapter dar;
            dar = new SqlDataAdapter("select  * from E_ResultQCM where Code_EvalByQCM= '" + codeEval + "' ", con);
            dar.Fill(dtr);

            for(int rr =0; rr< dtr.Rows.Count; rr++)
            {
                int cpt = 0;

                DataTable dty = new DataTable();
                SqlDataAdapter day;
                day = new SqlDataAdapter("select  * from E_ResultQCM where Code_EvalByQCM= '" + codeEval + "' and  Code_QCM = '"+ dtr.Rows[rr]["Code_QCM"].ToString() +"'", con);
                day.Fill(dty);

                List<string> liste = new List<string>();

                for (int yy = 0; yy < dty.Rows.Count; yy++)
                {
                    string a = dty.Rows[yy]["Reponse1"].ToString();

                    if ((dty.Rows[yy]["Reponse1User"].ToString()== dty.Rows[yy]["Reponse1"].ToString() && dty.Rows[yy]["EtatRep1"].ToString() == "Vrai") || (dty.Rows[yy]["Reponse2User"].ToString() == dty.Rows[yy]["Reponse1"].ToString() && dty.Rows[yy]["EtatRep1"].ToString() == "Vrai") || (dty.Rows[yy]["Reponse3User"].ToString() == dty.Rows[yy]["Reponse1"].ToString() && dty.Rows[yy]["EtatRep1"].ToString() == "Vrai") || (dty.Rows[yy]["Reponse4User"].ToString() == dty.Rows[yy]["Reponse1"].ToString() && dty.Rows[yy]["EtatRep1"].ToString() == "Vrai"))
                    {
                        cpt += Convert.ToInt32( dty.Rows[yy]["Coeff"].ToString()) ;
                    }

                    if ((dty.Rows[yy]["Reponse1User"].ToString() == dty.Rows[yy]["Reponse2"].ToString() && dty.Rows[yy]["EtatRep2"].ToString() == "Vrai") || (dty.Rows[yy]["Reponse2User"].ToString() == dty.Rows[yy]["Reponse2"].ToString() && dty.Rows[yy]["EtatRep2"].ToString() == "Vrai") || (dty.Rows[yy]["Reponse3User"].ToString() == dty.Rows[yy]["Reponse2"].ToString() && dty.Rows[yy]["EtatRep2"].ToString() == "Vrai") || (dty.Rows[yy]["Reponse4User"].ToString() == dty.Rows[yy]["Reponse2"].ToString() && dty.Rows[yy]["EtatRep2"].ToString() == "Vrai"))
                    {
                        cpt += Convert.ToInt32(dty.Rows[yy]["Coeff"].ToString());
                    }

                    if ((dty.Rows[yy]["Reponse1User"].ToString() == dty.Rows[yy]["Reponse3"].ToString() && dty.Rows[yy]["EtatRep3"].ToString() == "Vrai") || (dty.Rows[yy]["Reponse2User"].ToString() == dty.Rows[yy]["Reponse3"].ToString() && dty.Rows[yy]["EtatRep3"].ToString() == "Vrai") || (dty.Rows[yy]["Reponse3User"].ToString() == dty.Rows[yy]["Reponse3"].ToString() && dty.Rows[yy]["EtatRep3"].ToString() == "Vrai") || (dty.Rows[yy]["Reponse4User"].ToString() == dty.Rows[yy]["Reponse3"].ToString() && dty.Rows[yy]["EtatRep3"].ToString() == "Vrai"))
                    {
                        cpt += Convert.ToInt32(dty.Rows[yy]["Coeff"].ToString());
                    }


                    if ((dty.Rows[yy]["Reponse1User"].ToString() == dty.Rows[yy]["Reponse4"].ToString() && dty.Rows[yy]["EtatRep4"].ToString() == "Vrai") || (dty.Rows[yy]["Reponse2User"].ToString() == dty.Rows[yy]["Reponse4"].ToString() && dty.Rows[yy]["EtatRep4"].ToString() == "Vrai") || (dty.Rows[yy]["Reponse3User"].ToString() == dty.Rows[yy]["Reponse4"].ToString() && dty.Rows[yy]["EtatRep4"].ToString() == "Vrai") || (dty.Rows[yy]["Reponse4User"].ToString() == dty.Rows[yy]["Reponse4"].ToString() && dty.Rows[yy]["EtatRep4"].ToString() == "Vrai"))
                    {
                        cpt += Convert.ToInt32(dty.Rows[yy]["Coeff"].ToString());
                    }


                }

                
                string ss;

                if (rr == dty.Rows.Count) 
                      ss = dty.Rows[rr-1]["Code_QCM"].ToString(); 
                else
                    ss = dty.Rows[rr]["Code_QCM"].ToString();


                var ee = from m in db.E_QCM
                         where m.Code_EvalByQCM == codeEval && m.Code_QCM == ss
                         select m ;

                int contVrai = 0;

           

                foreach(E_QCM e in ee)
                {
                    if (e.EtatRep1 == "Vrai")
                        contVrai += 1;

                    if (e.EtatRep2 == "Vrai")
                        contVrai += 1;

                    if (e.EtatRep3 == "Vrai")
                        contVrai += 1;

                    if (e.EtatRep4 == "Vrai")
                        contVrai += 1;
                }

                E_QCM coef = (from m in db.E_QCM
                         where m.Code_EvalByQCM == codeEval
                         select m).Take(1).Single();

                if (contVrai != cpt)
                    cpt = 0;
                else
                    cpt = coef.Coeff;

                 

                cptQ += cpt;
            }

            foreach (E_QCM ee in listQCM)
                {
                    //if ((e.Reponse3 != null && e.Reponse1 != null && e.Reponse2 != null) || (e.Reponse4 != null && e.Reponse1 != null && e.Reponse2 != null) || (e.Reponse3 != null && e.Reponse4 != null && e.Reponse1 != null && e.Reponse2 != null))
                    cptTotal += ee.Coeff;


                }

                double scr = Math.Round((float)cptQ / (float)cptTotal * 100);

                ViewBag.Score = scr;

                var prcVal = from m in db.E_Evaluation
                             where m.Code_Eval == codeEval
                             select m;

                foreach (E_Evaluation d in prcVal)
                {
                    if (scr >= d.Pourc_Valid)
                    {
                        ViewBag.Valid = "Valide";
                    }
                    else if (scr < d.Pourc_Valid)
                    {
                        ViewBag.Valid = "Invalide";
                    }
                }


                SqlCommand cmd2 = new SqlCommand("Update E_ResultQCM set Resultat = '" + ViewBag.Valid + "' , Score = " + scr + " where Code_EvalByQCM = '" + codeEval + "'", con);
                cmd2.ExecuteNonQuery();


                var vf = from m in db.E_ResultQCM
                         where m.Code_EvalByQCM == codeEval && m.Resultat == "Invalide"
                         select m;


                foreach (E_ResultQCM r in vf)
                {
                        if (r.Reponse1User == null)
                            r.Reponse1User = " ";

                        if (r.Reponse2User == null)
                            r.Reponse2User = " ";

                        if (r.Reponse3User == null)
                            r.Reponse3User = " ";

                        if (r.Reponse4User == null)
                            r.Reponse4User = " ";

                SqlCommand cmd4 = new SqlCommand("INSERT INTO [dbo].[E_ResultQCM_Historiq] (DateHisto, [Code_EvalByQCM] ,[CodeForm] ,[Code_QCM] ,[MatUser] ,[Usr]  ,[DateEval]  ,[Score] ,[Resultat],[ObjEval],[ObjForm],[DeadLine],[Question],[Coeff],[Reponse1],[EtatRep1],[Reponse2],[EtatRep2] ,[Reponse3],[EtatRep3],[Reponse4],[EtatRep4],[Reponse1User],[Reponse2User],[Reponse3User],[Reponse4User]) VALUES (@DateHisto, @Code_EvalByQCM ,@CodeForm ,@Code_QCM ,@MatUser ,@Usr  ,@DateEval  ,@Score ,@Resultat, @ObjEval ,@ObjForm ,@DeadLine ,@Question ,@Coeff ,@Reponse1 ,@EtatRep1 ,@Reponse2 ,@EtatRep2 ,@Reponse3 ,@EtatRep3 ,@Reponse4 ,@EtatRep4 ,@Reponse1User,@Reponse2User,@Reponse3User,@Reponse4User)", con);

                    cmd4.Parameters.AddWithValue("@DateHisto", System.DateTime.Now);
                    cmd4.Parameters.AddWithValue("@Code_EvalByQCM", r.Code_EvalByQCM);
                    cmd4.Parameters.AddWithValue("@Code_QCM", r.Code_QCM);
                    cmd4.Parameters.AddWithValue("@CodeForm", r.CodeForm);
                    cmd4.Parameters.AddWithValue("@MatUser", r.MatUser);
                    cmd4.Parameters.AddWithValue("@Usr", r.Usr);
                    cmd4.Parameters.AddWithValue("@DateEval", r.DateEval);
                    cmd4.Parameters.AddWithValue("@Score", r.Score);
                    cmd4.Parameters.AddWithValue("@Resultat", r.Resultat);
                    cmd4.Parameters.AddWithValue("@ObjEval", r.ObjEval);
                    cmd4.Parameters.AddWithValue("@ObjForm", r.ObjForm);
                    cmd4.Parameters.AddWithValue("@DeadLine", r.DeadLine);
                    cmd4.Parameters.AddWithValue("@Question", r.Question);
                    cmd4.Parameters.AddWithValue("@Coeff", r.Coeff);
                    cmd4.Parameters.AddWithValue("@Reponse1", r.Reponse1);
                    cmd4.Parameters.AddWithValue("@Reponse2", r.Reponse2);
                    cmd4.Parameters.AddWithValue("@Reponse3", r.Reponse3);
                    cmd4.Parameters.AddWithValue("@Reponse4", r.Reponse4);
                    cmd4.Parameters.AddWithValue("@Reponse1User", r.Reponse1User);
                    cmd4.Parameters.AddWithValue("@Reponse2User", r.Reponse2User);
                    cmd4.Parameters.AddWithValue("@Reponse3User", r.Reponse3User);
                    cmd4.Parameters.AddWithValue("@Reponse4User", r.Reponse4User);
                    cmd4.Parameters.AddWithValue("@EtatRep1", r.EtatRep1);
                    cmd4.Parameters.AddWithValue("@EtatRep2", r.EtatRep2);
                    cmd4.Parameters.AddWithValue("@EtatRep3", r.EtatRep3);
                    cmd4.Parameters.AddWithValue("@EtatRep4", r.EtatRep4);

                    cmd4.ExecuteNonQuery();

                    SqlCommand cmd3 = new SqlCommand("delete from E_ResultQCM where Code_EvalByQCM = '" + r.Code_EvalByQCM + "' and Resultat = 'Invalide'", con);
                    cmd3.ExecuteNonQuery();

                    SqlCommand cmd44 = new SqlCommand("delete from E_RepUser where Code_eval = '" + r.Code_EvalByQCM + "'", con);
                    cmd44.ExecuteNonQuery();


                    DataTable dt5 = new DataTable();

                    SqlDataAdapter da5;
                    da5 = new SqlDataAdapter("select [Code_Formation] ,[ObjForm] ,[MatUser] ,[Usr]  ,[DateTerm]  ,[Resultat] ,E_ResultFormation.[DeadLine] ,[Etat] from E_ResultFormation inner join [E_ListFormationDiffus] on [E_ListFormationDiffus].Code_formt = E_ResultFormation.Code_Formation where[E_ListFormationDiffus].Code_eval = '" + r.Code_EvalByQCM + "' ", con);
                    da5.Fill(dt5);


                    for (int ii = 0; ii < dt5.Rows.Count; ii++)
                    {

                        SqlCommand cmd5 = new SqlCommand("INSERT INTO [dbo].[E_ResultFormation_Historiq] (DateHisto , [Code_Formation] ,[ObjForm] ,[MatUser] ,[Usr]  ,[DateTerm]  ,[Resultat] ,[DeadLine] ,[Etat])  VALUES (@DateHisto , @Code_Formation ,@ObjForm ,@MatUser ,@Usr  ,@DateTerm  ,@Resultat ,@DeadLine ,@Etat)", con);

                        cmd5.Parameters.AddWithValue("@DateHisto", System.DateTime.Now);
                        cmd5.Parameters.AddWithValue("@Code_Formation", dt5.Rows[ii]["Code_Formation"].ToString());
                        cmd5.Parameters.AddWithValue("@ObjForm", dt5.Rows[ii]["ObjForm"].ToString());
                        cmd5.Parameters.AddWithValue("@MatUser", dt5.Rows[ii]["MatUser"].ToString());
                        cmd5.Parameters.AddWithValue("@Usr", dt5.Rows[ii]["Usr"].ToString());
                        cmd5.Parameters.AddWithValue("@DateTerm", Convert.ToDateTime(dt5.Rows[ii]["DateTerm"].ToString()));
                        cmd5.Parameters.AddWithValue("@Resultat", dt5.Rows[ii]["Resultat"].ToString());
                        cmd5.Parameters.AddWithValue("@DeadLine", Convert.ToDateTime(dt5.Rows[ii]["DeadLine"].ToString()));
                        cmd5.Parameters.AddWithValue("@Etat", dt5.Rows[ii]["Etat"].ToString());

                        cmd5.ExecuteNonQuery();


                        SqlCommand cmd6 = new SqlCommand("delete from E_ResultFormation where Code_Formation = '" + dt5.Rows[ii]["Code_Formation"].ToString() + "' ", con);
                        cmd6.ExecuteNonQuery();


                        SqlCommand cmd7 = new SqlCommand("delete from E_SlideUsr where Code_Formation = '" + dt5.Rows[ii]["Code_Formation"].ToString() + "' ", con);
                        cmd7.ExecuteNonQuery();

                    }

                }
                

            
            return View(list);
        }



        //POST: E_QCM/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier.Pour
        // plus de détails, voir https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Code_QCM,Code_EvalByQCM,Date_Creation,Question,Reponse1,Reponse2,Reponse3,Reponse4,Coeff")] E_QCM e_QCM , string EtatRep1 , string EtatRep2 , string EtatRep3 , string EtatRep4)
        {
            if (ModelState.IsValid)
            {
                if (EtatRep1 == "Faux" && EtatRep2 == "Faux" && EtatRep3 == "Faux" && EtatRep4 == "Faux")
                {
                    ModelState.AddModelError("", "Il faut une réponse vrai.");
                    ViewBag.codeEval = e_QCM.Code_EvalByQCM ;

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

                if (countA > 1)
                {
                    ModelState.AddModelError("", "Il faut une seule réponse vrai.");
                    ViewBag.codeEval = e_QCM.Code_EvalByQCM;
                    return View(e_QCM);
                }


                var list = from m in db.E_QCM
                           where m.Id == e_QCM.Id
                           select m;


                foreach(E_QCM e in list)
                {
                    e_QCM.Code_EvalByQCM = e.Code_EvalByQCM;

                    e_QCM.Code_QCM = e.Code_QCM;

                    e_QCM.Date_Creation = e.Date_Creation;

                    e_QCM.EtatRep1 = e.EtatRep1;

                    e_QCM.EtatRep2 = e.EtatRep2;

                    e_QCM.EtatRep3 = e.EtatRep3;

                    e_QCM.EtatRep4 = e.EtatRep4;


                }

                //e_QCM.Date_Modif = System.DateTime.Now;


                string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
                SqlConnection con = new SqlConnection(constr);
                con.Open();


                SqlCommand cmd = new SqlCommand("UPDATE e_QCM set Date_Modif= '" + System.DateTime.Now + "', Question =  '" + e_QCM.Question + "' , Reponse1 = '" + e_QCM.Reponse1 + "' , EtatRep1 = '" + e_QCM.EtatRep1 + "' ,   Reponse2 = '" + e_QCM.Reponse2 + "', EtatRep2 = '" + e_QCM.EtatRep2 + "' , Reponse3 = '" + e_QCM.Reponse3 + "', EtatRep3 = '" + e_QCM.EtatRep3 + "' , Reponse4 = '" + e_QCM.Reponse4 + "', EtatRep4 = '" + e_QCM.EtatRep4 + "' ,  Coeff = " + e_QCM.Coeff + " where  Code_QCM= '" + e_QCM.Code_QCM + "'  ", con);
                cmd.ExecuteNonQuery();

                //db.Entry(e_QCM).State = EntityState.Detached;

                //db.Entry(e_QCM).State = EntityState.Modified;

                db.SaveChanges();

                return RedirectToAction("Index", new { codeEval = e_QCM.Code_EvalByQCM });
            }
            return View(e_QCM);
        }

        // GET: E_QCM/Delete/5
        public ActionResult Delete(int? id, string codeEval)
        {
            ViewBag.codeEval = codeEval;

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
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            E_QCM e_QCM = db.E_QCM.Find(id);
            db.E_QCM.Remove(e_QCM);
            db.SaveChanges();
            return RedirectToAction("Index",  new { codeEval = e_QCM.Code_EvalByQCM });
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
