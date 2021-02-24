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

            foreach(E_Evaluation r in list)
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


            E_RepUser e_RepUser = new  E_RepUser();

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
        public ActionResult Consult(string id, string Reponse , string next  , string Previous , string sld , string Terminer)
        {
            if (Session["userconnecté"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
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


            }
            return View();



        }

        static public int nbdiapo = -1;

       

        // GET: E_QCM/Details/5
        public ActionResult Details(int? id , string codeEval)
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
        public ActionResult Create([Bind(Include = "Id,Code_QCM,Code_EvalByQCM,Date_Creation,Question,Reponse1,Reponse2,Reponse3, Reponse4,Coeff")] E_QCM e_QCM, string codeEvl , string EtatRep1, string EtatRep2 , string EtatRep3 , string EtatRep4)
        {
            if (ModelState.IsValid)
            {
                if (EtatRep1=="Faux" && EtatRep2 == "Faux" && EtatRep3 == "Faux" && EtatRep4 == "Faux")
                {
                    ModelState.AddModelError("", "Il faut une réponse vrai.");
                    ViewBag.codeEval = codeEvl;
                    
                    return View(e_QCM);
                }

                int countA = 0;

                 List<string> aaa = new List<string>();

                aaa.Add(EtatRep1);

                aaa.Add(EtatRep2);

                aaa.Add(EtatRep3);

                aaa.Add(EtatRep4);

                for  (int i=0; i< aaa.Count; i++)
                {
                    if (aaa[i].ToString() == "Vrai")
                    {
                        countA += 1;
                    }
                }

                if(countA >1)
                {
                    ModelState.AddModelError("", "Il faut une seule réponse vrai.");
                    ViewBag.codeEval = codeEvl;
                    return View(e_QCM);
                }


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
                               select m ;

                int o = qcm.Count();

                if(o != 0)
                {
                    //foreach(E_QCM zz in qcm)
                    //{ 
                         e_QCM.NumQ = o +1;
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

                else if (e_QCM.Reponse3 != null )
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
                return RedirectToAction("Index", new   { codeEval = e_QCM.Code_EvalByQCM } );
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

       


        public ActionResult Terminer(string codeEval , string score , string valide)
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());

            string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection con = new SqlConnection(constr);
            con.Open();

            int r = Convert.ToInt32 (score);

              
          SqlCommand cmd = new SqlCommand("Update   E_ResultQCM set Score = '"+r+"' , Resultat = '"+ valide + "' where MatUser = "+user.matricule+ " and Code_EvalByQCM = '"+codeEval+"' ", con);

                 
         cmd.ExecuteNonQuery();

                
             
            
            return RedirectToAction("Index","EMenuEvalUser");
        }



        public ActionResult ResultQCM( string codeEval)
        {
             
              

            if (Session["Evl"] != null)
            {
                codeEval = Session["Evl"].ToString();
            }


            codeEval = codeEval.Substring(0, 16);
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());


            int cpt = 0;

            int cptTotal = 0;

        string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection con = new SqlConnection(constr);
            con.Open();

            DataTable dt = new DataTable();

            //SqlDataAdapter da;
            //da = new SqlDataAdapter("select Code_EvalByQCM, Code_QCM, E_RepUser.Date_Creation DateEval ,  E_RepUser.MatUser, E_QCM.Question, Coeff, E_QCM.Reponse1, E_QCM.EtatRep1,E_QCM.Reponse2,E_QCM.EtatRep2,E_QCM.Reponse3, E_QCM.EtatRep3,E_QCM.Reponse4,  E_QCM.EtatRep4, E_RepUser.Reponse ReponseUser from E_QCM inner join E_RepUser on E_RepUser.Code_eval = E_QCM.Code_EvalByQCM and E_RepUser.CodeQcm = E_QCM.Code_QCM where E_QCM.Code_EvalByQCM = '" + codeEval + "' and E_RepUser.MatUser = "+user.matricule+"", con);
            //da.Fill(dt);

            SqlDataAdapter da;
            da = new SqlDataAdapter("select distinct Code_EvalByQCM,  E_Formation.Code CodeForm, Code_QCM, E_ListEvaluationDiffus.Objet ObjEval,  E_ListEvaluationDiffus.deadline,  E_Formation.Objet ObjForm, E_RepUser.Date_Creation DateEval ,  E_RepUser.MatUser,    AspNetUsers.NomPrenom Usr , E_QCM.Question, Coeff,  E_QCM.Reponse1, E_QCM.EtatRep1, E_QCM.Reponse2, E_QCM.EtatRep2, E_QCM.Reponse3, E_QCM.EtatRep3, E_QCM.Reponse4, E_QCM.EtatRep4, E_RepUser.Reponse ReponseUser from E_QCM inner join E_RepUser on E_RepUser.Code_eval = E_QCM.Code_EvalByQCM and E_RepUser.CodeQcm = E_QCM.Code_QCM inner join E_ListEvaluationDiffus on E_ListEvaluationDiffus.Code_Eval = E_QCM.Code_EvalByQCM left join E_Formation on E_Formation.CodeEval = E_QCM.Code_EvalByQCM   inner join AspNetUsers on AspNetUsers.matricule = E_ListEvaluationDiffus.Mat_usr where E_QCM.Code_EvalByQCM = '" + codeEval + "' and AspNetUsers.matricule = " + user.matricule + "", con);
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

                e.DeadLine = Convert.ToDateTime (dt.Rows[i]["deadline"].ToString());

                e.MatUser = dt.Rows[i]["MatUser"].ToString();

                e.Usr = dt.Rows[i]["Usr"].ToString();

                e.Question = dt.Rows[i]["Question"].ToString();

                e.Coeff = Convert.ToInt32 (dt.Rows[i]["Coeff"].ToString());

                e.Reponse1 = dt.Rows[i]["Reponse1"].ToString();

                e.EtatRep1 = dt.Rows[i]["EtatRep1"].ToString();

                e.Reponse2 = dt.Rows[i]["Reponse2"].ToString();

                e.EtatRep2 = dt.Rows[i]["EtatRep2"].ToString();

                e.Reponse3 = dt.Rows[i]["Reponse3"].ToString();

                e.EtatRep3 = dt.Rows[i]["EtatRep3"].ToString();

                e.Reponse4 = dt.Rows[i]["Reponse4"].ToString();

                e.EtatRep4 = dt.Rows[i]["EtatRep4"].ToString();

                e.ReponseUser = dt.Rows[i]["ReponseUser"].ToString();

                 
                e.DateEval = Convert.ToDateTime (dt.Rows[i]["DateEval"].ToString());

                if ((e.Reponse1 == e.ReponseUser && e.EtatRep1 == "Vrai") || (e.Reponse2 == e.ReponseUser && e.EtatRep2 == "Vrai") || (e.Reponse3 == e.ReponseUser && e.EtatRep3 == "Vrai") || (e.Reponse4 == e.ReponseUser && e.EtatRep4 == "Vrai"))
                {
                    cpt += e.Coeff; 
                }


                SqlCommand cmd = new SqlCommand("Insert into E_ResultQCM (Code_EvalByQCM,   CodeForm, ObjEval, ObjForm, Deadline, Code_QCM,DateEval,MatUser,Usr, Question,Coeff,Reponse1,EtatRep1,Reponse2,EtatRep2,Reponse3,EtatRep3,Reponse4,EtatRep4,ReponseUser) values(@Code_EvalByQCM,   @CodeForm, @ObjEval, @ObjForm, @Deadline,@Code_QCM,@DateEval,@MatUser, @Usr,@Question,@Coeff,@Reponse1,@EtatRep1,@Reponse2,@EtatRep2,@Reponse3,@EtatRep3,@Reponse4,@EtatRep4,@ReponseUser)", con);

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
                cmd.Parameters.AddWithValue("@ReponseUser", e.ReponseUser);

                cmd.ExecuteNonQuery();


               
                list.Add(e);
            }


            var listQCM = from m in db.E_QCM
                          where m.Code_EvalByQCM == codeEval
                          select m;


            foreach (E_QCM e in listQCM)
            {
                //if ((e.Reponse3 != null && e.Reponse1 != null && e.Reponse2 != null) || (e.Reponse4 != null && e.Reponse1 != null && e.Reponse2 != null) || (e.Reponse3 != null && e.Reponse4 != null && e.Reponse1 != null && e.Reponse2 != null))
                cptTotal += e.Coeff;


            }
            double scr = Math.Round((float)cpt / (float)cptTotal * 100);

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


            SqlCommand cmd2 = new SqlCommand("Update E_ResultQCM set Resultat = '"+ ViewBag.Valid+"' , Score = "+scr+ " where Code_EvalByQCM = '"+codeEval+"'", con);
            cmd2.ExecuteNonQuery();


            var vf = from m in db.E_ResultQCM
                     where m.Code_EvalByQCM == codeEval && m.Resultat == "Invalide"
                     select m;


            foreach(E_ResultQCM r in vf)
            {

                SqlCommand cmd = new SqlCommand("INSERT INTO [dbo].[E_ResultQCM_Historiq] (DateHisto, [Code_EvalByQCM] ,[CodeForm] ,[Code_QCM] ,[MatUser] ,[Usr]  ,[DateEval]  ,[Score] ,[Resultat],[ObjEval],[ObjForm],[DeadLine],[Question],[Coeff],[Reponse1],[EtatRep1],[Reponse2],[EtatRep2] ,[Reponse3],[EtatRep3],[Reponse4],[EtatRep4],[ReponseUser]) VALUES (@DateHisto, @Code_EvalByQCM ,@CodeForm ,@Code_QCM ,@MatUser ,@Usr  ,@DateEval  ,@Score ,@Resultat, @ObjEval ,@ObjForm ,@DeadLine ,@Question ,@Coeff ,@Reponse1 ,@EtatRep1 ,@Reponse2 ,@EtatRep2 ,@Reponse3 ,@EtatRep3 ,@Reponse4 ,@EtatRep4 ,@ReponseUser)", con);

                cmd.Parameters.AddWithValue("@DateHisto", System.DateTime.Now);
                cmd.Parameters.AddWithValue("@Code_EvalByQCM", r.Code_EvalByQCM);
                cmd.Parameters.AddWithValue("@Code_QCM", r.Code_QCM);
                cmd.Parameters.AddWithValue("@CodeForm", r.CodeForm);
                cmd.Parameters.AddWithValue("@MatUser", r.MatUser);
                cmd.Parameters.AddWithValue("@Usr", r.Usr);
                cmd.Parameters.AddWithValue("@DateEval", r.DateEval);
                cmd.Parameters.AddWithValue("@Score", r.Score);
                cmd.Parameters.AddWithValue("@Resultat", r.Resultat);
                cmd.Parameters.AddWithValue("@ObjEval", r.ObjEval); 
                cmd.Parameters.AddWithValue("@ObjForm", r.ObjForm);
                cmd.Parameters.AddWithValue("@DeadLine", r.DeadLine);
                cmd.Parameters.AddWithValue("@Question", r.Question);
                cmd.Parameters.AddWithValue("@Coeff", r.Coeff);
                cmd.Parameters.AddWithValue("@Reponse1", r.Reponse1);
                cmd.Parameters.AddWithValue("@Reponse2", r.Reponse2);
                cmd.Parameters.AddWithValue("@Reponse3", r.Reponse3);
                cmd.Parameters.AddWithValue("@Reponse4", r.Reponse4);
                cmd.Parameters.AddWithValue("@ReponseUser", r.ReponseUser);
                cmd.Parameters.AddWithValue("@EtatRep1", r.EtatRep1);
                cmd.Parameters.AddWithValue("@EtatRep2", r.EtatRep2);
                cmd.Parameters.AddWithValue("@EtatRep3", r.EtatRep3);
                cmd.Parameters.AddWithValue("@EtatRep4", r.EtatRep4);

                cmd.ExecuteNonQuery();

                SqlCommand cmd3 = new SqlCommand("delete from E_ResultQCM where Code_EvalByQCM = '" + r.Code_EvalByQCM + "' and Resultat = 'Invalide'", con);
                cmd3.ExecuteNonQuery();

                SqlCommand cmd4 = new SqlCommand("delete from E_RepUser where Code_eval = '" + r.Code_EvalByQCM + "'", con);
                cmd4.ExecuteNonQuery();


                DataTable dt5 = new DataTable();

                SqlDataAdapter da5;
                da5 = new SqlDataAdapter("select [Code_Formation] ,[ObjForm] ,[MatUser] ,[Usr]  ,[DateTerm]  ,[Resultat] ,E_ResultFormation.[DeadLine] ,[Etat] from E_ResultFormation inner join [E_ListFormationDiffus] on [E_ListFormationDiffus].Code_formt = E_ResultFormation.Code_Formation where[E_ListFormationDiffus].Code_eval = '" + r.Code_EvalByQCM + "' ", con);
                da5.Fill(dt5);


                for (int i=0; i< dt5.Rows.Count; i++)
                {
                    
                    SqlCommand cmd5 = new SqlCommand("INSERT INTO [dbo].[E_ResultFormation_Historiq] (DateHisto , [Code_Formation] ,[ObjForm] ,[MatUser] ,[Usr]  ,[DateTerm]  ,[Resultat] ,[DeadLine] ,[Etat])  VALUES (@DateHisto , @Code_Formation ,@ObjForm ,@MatUser ,@Usr  ,@DateTerm  ,@Resultat ,@DeadLine ,@Etat)", con);

                    cmd5.Parameters.AddWithValue("@DateHisto", System.DateTime.Now);
                    cmd5.Parameters.AddWithValue("@Code_Formation", dt5.Rows[i]["Code_Formation"].ToString());
                    cmd5.Parameters.AddWithValue("@ObjForm", dt5.Rows[i]["ObjForm"].ToString());
                    cmd5.Parameters.AddWithValue("@MatUser", dt5.Rows[i]["MatUser"].ToString());
                    cmd5.Parameters.AddWithValue("@Usr", dt5.Rows[i]["Usr"].ToString());
                    cmd5.Parameters.AddWithValue("@DateTerm", Convert.ToDateTime (dt5.Rows[i]["DateTerm"].ToString()));
                    cmd5.Parameters.AddWithValue("@Resultat", dt5.Rows[i]["Resultat"].ToString());
                    cmd5.Parameters.AddWithValue("@DeadLine", Convert.ToDateTime (dt5.Rows[i]["DeadLine"].ToString()));
                    cmd5.Parameters.AddWithValue("@Etat", dt5.Rows[i]["Etat"].ToString());

                    cmd5.ExecuteNonQuery();


                    SqlCommand cmd6 = new SqlCommand("delete from E_ResultFormation where Code_Formation = '" + dt5.Rows[i]["Code_Formation"].ToString() + "' ", con);
                    cmd6.ExecuteNonQuery();


                    SqlCommand cmd7 = new SqlCommand("delete from E_SlideUsr where Code_Formation = '" + dt5.Rows[i]["Code_Formation"].ToString() + "' ", con);
                    cmd7.ExecuteNonQuery();

                }

            }





            return View(list);
        }



        // POST: E_QCM/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
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
