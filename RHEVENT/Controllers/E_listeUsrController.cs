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
    public class E_listeUsrController : Controller
    {
       

        private ApplicationDbContext db = new ApplicationDbContext();

        public IEnumerable<E_listeUsr> liste2 = null;

        public static string st = null;


        // GET: E_listeUsr
        public ActionResult Index(string id)
        {
            //if (TempData["CodeFormation"] != null)
            //{
            //    st = TempData["CodeFormation"].ToString();
            //    ViewBag.form = st;
            //}

            if (Session["CodeFormation"] != null)
            {
                st = Session["CodeFormation"].ToString();
                ViewBag.form = st;
            }


            E_GrpByUsr e = db.E_GrpByUsr.Find(Convert.ToInt32(id));

            if (e == null)
            {
                var list = from m in db.E_listeUsr
                           where m.Code_grp == id
                           select m;

                var idgrp = from m in db.E_GrpByUsr
                            where m.Code == id 
                            select m;

                foreach(E_GrpByUsr d in idgrp)
                {
                    ViewBag.idGrp = d.Id;
                }

                foreach(E_listeUsr ee in list)
                {
                    ViewBag.codeGrp = ee.Code_grp;

                    //ViewBag.codeForm = ee.Code_formt;

                   
                }

                liste2 = list;
            }
            else
            {
                ViewBag.codeGrp = e.Code;

                //ViewBag.codeForm = e.Code_Formation;
           
          
             
             liste2 = from m in db.E_listeUsr
                        where m.Code_grp == e.Code
                        select m;
            }

            return View(liste2.ToList());
        }

        //[HttpGet, ActionName("GetListUsr")]
        //[ValidateAntiForgeryToken]
        public ActionResult GetListUsr(string id)
        {
            
            E_GrpByUsr e = db.E_GrpByUsr.Find(Convert.ToInt32(id));

            string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection con = new SqlConnection(constr);
            con.Open();



            if (e == null)
            {
                var list = from m in db.E_listeUsr
                           where m.Code_grp == id
                           select m;

                var idgrp = from m in db.E_GrpByUsr
                            where m.Code == id
                            select m;

                foreach (E_GrpByUsr d in idgrp)
                {
                    ViewBag.idGrp = d.Id;
                }

                foreach (E_listeUsr ee in list)
                {
                    ViewBag.codeGrp = ee.Code_grp;
 
                }

                liste2 = list;
            }
            else
            {
                ViewBag.codeGrp = e.Code;

                liste2 = from m in db.E_listeUsr
                         where m.Code_grp == e.Code
                         select m;
                 

            }

            return View(liste2.ToList());
        }

        // GET: E_listeUsr/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            E_listeUsr e_listeUsr = db.E_listeUsr.Find(id);
            if (e_listeUsr == null)
            {
                return HttpNotFound();
            }
            return View(e_listeUsr);
        }

        public ActionResult CreateUsrGrp(string id , string codeGrp , string searchString)
        {
          
            string codeg = null;

            if (TempData["Data2"] != null)
            { 
                  codeg = TempData["Data2"].ToString();
 
                ViewBag.codeg = TempData["Data2"].ToString();
            }
            else
            {
                 codeg = codeGrp;

                ViewBag.codeg = codeGrp;

                TempData["Data2"]= codeGrp;

                TempData["Group"] = codeGrp;
            }

            IQueryable<ApplicationUser> UserQuery;

            if (searchString != null)
            {
                  UserQuery = from m in db.Users
                                where m.Etat == searchString
                                orderby m.NomPrenom
                                select m; 
            }
            else
            {
                  UserQuery = from m in db.Users
                                where m.Etat == "Interne"
                              orderby m.NomPrenom
                              select m; 
            }


            List<E_GrpByUsr> listG = new List<E_GrpByUsr>();

            listG.Insert(0, new E_GrpByUsr { Id = 0, Code = codeg });

            List<E_GrpByUsr> CodeGroup = listG;

            ViewBag.CodeGroup = CodeGroup;

             
            List<ApplicationUser> listUser = UserQuery.ToList<ApplicationUser>();

            ViewBag.listUser = listUser;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateUsrGrp(E_listeUsr e_listeUsr, string btnValid, string searchString)
        {
            if (ModelState.IsValid)
            {
               

                string codeg = TempData["Group"].ToString();


                if (searchString != null && btnValid == null)
                {
                    //TempData["Data2"] = e_listeUsr.Code_grp;

                    Session["EtatUsrbyGrp"] = searchString;
                    return RedirectToAction("CreateUsrGrp", "E_listeUsr", new { searchString = searchString , codeGrp = codeg });
                }
                IQueryable<E_GrpByUsr> idGrp;


                if (e_listeUsr.Code_grp != null)
                { 
                   idGrp = from m in db.E_GrpByUsr
                            where m.Code == e_listeUsr.Code_grp
                            select m;

                }
                else
                {
                    idGrp = from m in db.E_GrpByUsr
                            where m.Code == codeg
                            select m;

                }

                int ss = 0;
                foreach (E_GrpByUsr t in idGrp)
                {
                    ss = t.Id;
                }

                if (ss == 0)
                {
                    E_GrpByUsr g = (from m in db.E_GrpByUsr
                                    where m.Code == codeg
                                   select m).Single();

                    ss = g.Id; 

                }

                var usr = from m in db.Users
                          where m.matricule == e_listeUsr.SelectUsr
                          select m;

              

                foreach (ApplicationUser a in usr)
                {
                    e_listeUsr.Mat_usr = a.matricule;

                    e_listeUsr.Nom_usr = a.NomPrenom;

                    e_listeUsr.Etat = a.Etat;
                }

                e_listeUsr.Code_grp = codeg;
 
                

                db.E_listeUsr.Add(e_listeUsr);

                db.SaveChanges();

                return RedirectToAction("GetListUsr", new { id = ss });
            }

            return View(e_listeUsr);
        }


        [HttpGet]
        public ActionResult DiffUser(string id, string codef, string searchString ,string bloq)
        {
            ViewBag.Bloq = "";

            if (bloq != null)
            {
                ViewBag.Bloq = bloq;
            }

            E_Formation e = (from m in db.E_Formation
                            where m.Code == codef
                            select m).Take(1).Single();

            ViewBag.CodeFormation = e.Code;

            ViewBag.DateCreatFormation = e.Date_Creation.ToShortDateString();

            ViewBag.ObjetFormation = e.Objet;
             
            Session["CodeFUser"] = codef;

            IQueryable<ApplicationUser> UserQuery;

            if (searchString != null)
            {
                UserQuery = from m in db.Users
                            where m.Etat == searchString
                            orderby m.NomPrenom
                            select m;
            }
            else
            {
                UserQuery = from m in db.Users
                            where m.Etat == "Interne"
                            orderby m.NomPrenom
                            select m;
            }

            List<ApplicationUser> listUser = UserQuery.ToList<ApplicationUser>();

            ViewBag.listUser = listUser;

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DiffUser(E_listeUsr e_listeUsr, DateTime dateLim,   string btnValid, string searchString)
        {

             
            if (ModelState.IsValid)
            {
                string f = Session["CodeFUser"].ToString();


                if (searchString != null && searchString != "" && btnValid == null)
                {
                    //TempData["Data2"] = e_listeUsr.Code_grp;

                    Session["EtatUsrFor"] = searchString;

                    return RedirectToAction("DiffUser", "E_listeUsr", new { codef = f,  searchString = searchString  });
                }
               

                var usr = from m in db.Users
                          where m.matricule == e_listeUsr.SelectUsr
                          select m;

                string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
                SqlConnection con = new SqlConnection(constr);
                con.Open();

                //string OldChaine = System.DateTime.Now.ToShortDateString();

                //string NewChaine = OldChaine.Replace("/", "");

                //string date = System.DateTime.Now.ToShortDateString();

                //string gr = "G_" + NewChaine;

                //DataTable dt = new DataTable(); 
                //SqlDataAdapter da;
                //da = new SqlDataAdapter("SELECT   * FROM E_listeUsr where Code_grp like '%''" + gr + "' '%' ", con);
                //da.Fill(dt);

                //int a = dt.Rows.Count;

                //int p = a + 1;

                //if (p <= 9)
                //    e_listeUsr.Code_grp = gr + "_0" + (a + 1);
                //else
                //    e_listeUsr.Code_grp = gr + "_" + (a + 1);


                foreach (ApplicationUser aa in usr)
                {
                    e_listeUsr.Mat_usr = aa.matricule;

                    e_listeUsr.Nom_usr = aa.NomPrenom;

                    e_listeUsr.Etat = aa.Etat;
                }


                DataTable dt2 = new DataTable();

                SqlDataAdapter da2;
                da2 = new SqlDataAdapter("select * from E_ListFormationDiffus  where Code_formt = '" + f + "' and  Mat_usr = '" + e_listeUsr.Mat_usr + "' ", con);
                da2.Fill(dt2);

                int count = dt2.Rows.Count;

                if (count != 0)
                {
                    ViewBag.Bloq = "La diffusion a été annulée : la formation " + f + " est déjà diffusée à " + e_listeUsr.Nom_usr + "  !";

                    return RedirectToAction("DiffUser", "E_listeUsr", new { codef = f, bloq = @ViewBag.Bloq });
                }



                db.E_listeUsr.Add(e_listeUsr);

                db.SaveChanges();


                /////////////////////////////

               

                DataTable dt35 = new DataTable();
                SqlDataAdapter da35;
                da35 = new SqlDataAdapter("select top(1) * from E_Formation where Code = '" + f + "' ", con);
                da35.Fill(dt35);


                for (int p = 0; p < dt35.Rows.Count; p++)
                {
                    if (dt35.Rows[p]["CodeEval"].ToString() != null)
                    {
                        if (dt35.Rows[p]["CodeEval"].ToString() != "")
                        {
                            string ce = dt35.Rows[p]["CodeEval"].ToString();

                        DataTable dtq = new DataTable();
                        SqlDataAdapter daq;
                        daq = new SqlDataAdapter("select top(1) * from E_QCM where Code_EvalByQCM = '" + ce + "' ", con);
                        daq.Fill(dtq);

                        if (dtq.Rows.Count == 0)
                        {
                            ViewBag.Bloq = "La diffusion a été annulée : L'évaluation est sans QCM !";

                            return RedirectToAction("DiffUser", "E_listeUsr", new { codef = f, bloq = @ViewBag.Bloq });
                        }

                    }
                    }
                }

                //DataTable dt3 = new DataTable();

                //SqlDataAdapter da3;
                //da3 = new SqlDataAdapter("select E_listeUsr.Mat_usr ,E_listeUsr.Code_grp  , AspNetUsers.NomPrenom from E_listeUsr inner join AspNetUsers on AspNetUsers.matricule = E_listeUsr.Mat_usr where Code_grp = '" + e_listeUsr.Code_grp + "' ", con);
                //da3.Fill(dt3);



                //for (int j = 0; j < dt3.Rows.Count; j++)
                //{
                   

                    
                //}

                 

                ApplicationUser user = db.Users.Find(User.Identity.GetUserId());

                var form = (from m in db.E_Formation
                            where m.Code == f
                            select m).Take(1);

                E_Formation e_f = ((from m in db.E_Formation
                                    where m.Code == f
                                    select m).Take(1)).Single();

                string dd = null;
                foreach (E_Formation o in form)
                {
                    dd = o.Objet;
                }


                //foreach (string ide in ids)
                //{
                    //var g = db.E_GrpByUsr.Find(int.Parse(ide));

                    //g.Etat_Grp = "Oui";

                    //db.Entry(g).State = EntityState.Modified;

                     
                    SqlCommand cmd = new SqlCommand("INSERT INTO E_ListFormationDiffus  ([Mat_usr]  ,[Nom_usr]   ,[Code_formt], code_eval, DateDiffus, MatFormateur, Objet, deadline) " +
                        "SELECT  [Mat_usr]  ,[Nom_usr]   ,'" + f + "', '" + e_f.CodeEval + "' ,'" + System.DateTime.Now + "' , '" + user.matricule + "', '" + dd + "' , CONVERT(nvarchar, '" + dateLim + "',103)     FROM [E_listeUsr] " +                    
                        "" + "" + "where  Mat_usr  = '" + e_listeUsr.Mat_usr + "' and Code_grp is null ", con);


                    cmd.ExecuteNonQuery();

                    foreach (E_Formation o in form)
                    {

                        if (o.CodeEval != null)
                        {

                            DataTable dt = new DataTable();

                            SqlDataAdapter da;
                            da = new SqlDataAdapter("SELECT Objet_Eval FROM E_Evaluation where Code_Eval = '" + o.CodeEval + "'", con);
                            da.Fill(dt);

                            string objEvl = dt.Rows[0]["Objet_Eval"].ToString();




                            SqlCommand cmd3 = new SqlCommand("INSERT INTO E_ListEvaluationDiffus  ([Mat_usr]  ,[Nom_usr]    ,[Code_eval],Code_Formation, DateDiffus, MatFormateur, Objet, deadline) " +
                              "SELECT  [Mat_usr]  ,[Nom_usr]      ,'" + o.CodeEval + "', '" + o.Code + "', '" + System.DateTime.Now + "' , '" + user.matricule + "', '" + objEvl + "' , CONVERT(nvarchar, '" + dateLim + "',103)     FROM [E_listeUsr] " +
                             
                              "" + "" + "where   Mat_usr  = '" + e_listeUsr.Mat_usr + "' and Code_grp is null ", con);

                            cmd3.ExecuteNonQuery();



                        }
                    }


                    SqlCommand cmd2 = new SqlCommand("update dbo.E_Formation set EtatDiff = 'Diffusee' where Code =  '" + f + "' ", con);

                    cmd2.ExecuteNonQuery();


                    DataTable dt4 = new DataTable();
                    SqlDataAdapter da4;
                    da4 = new SqlDataAdapter("SELECT AspNetUsers.NomPrenom Nom_Dest, AspNetUsers.Email Email_Dest FROM [E_listeUsr]    inner join AspNetUsers on AspNetUsers.matricule =[E_listeUsr].Mat_usr  where  [E_listeUsr].Mat_usr   = '" + e_listeUsr.Mat_usr + "'    ", con);
                    da4.Fill(dt4);

                    for (int j = 0; j < dt4.Rows.Count; j++)
                    {
                        string Nom_dest = dt4.Rows[j]["Nom_Dest"].ToString();

                        string Email_dest = dt4.Rows[j]["Email_Dest"].ToString();

                        Serveur serveur = (from m in db.Serveur
                                           select m).Single();

                    string Message = "<HTML><Head></Head><Body>Bonjour<div><br/></div><div>" + "Vous avez une formation à réaliser.</div>" + "<div> Lien Formation :  " + "<a href=" + serveur.Serv + "/E_Formation/SlideUsr?codeF=" + e_f.Code + ">" + e_f.Code + " : " + e_f.Objet + "</a></div><div><br/><br/>Bien Cordialement</div></Body></HTML>";

                    SqlCommand cmd4 = new SqlCommand("Insert into Emails (Destinataire ,Email_Destinataire ,Sujet ,Message ,Current_User_Event ,Date_email ,Etat_Envoi) values (@Destinataire ,@Email_Destinataire ,@Sujet ,@Message ,@Current_User_Event ,@Date_email ,@Etat_Envoi)", con);

                        cmd4.Parameters.AddWithValue("@Destinataire", Nom_dest);
                        cmd4.Parameters.AddWithValue("@Email_Destinataire", Email_dest);
                        cmd4.Parameters.AddWithValue("@Sujet", e_f.Objet);
                        cmd4.Parameters.AddWithValue("@Message", Message);
                        cmd4.Parameters.AddWithValue("@Current_User_Event", user.NomPrenom);
                        cmd4.Parameters.AddWithValue("@Date_email", System.DateTime.Now);
                        cmd4.Parameters.AddWithValue("@Etat_Envoi", "0");
                        cmd4.ExecuteNonQuery();


                    }

                    //}
                    db.SaveChanges();

                //}



                return RedirectToAction("Index", "E_Formation");
            }

            return View(e_listeUsr);
        }



        // GET: E_listeUsr/Create
        public ActionResult Create(string id , string codeGrp    , string searchString)
        {

          
            //string codef = TempData["Data1"].ToString();
            //ViewBag.codeF = TempData["Data1"].ToString();

            string codeg = null;

            if (TempData["Data2"] != null)
            { 
                  codeg = TempData["Data2"].ToString();
             
                ViewBag.codeg = TempData["Data2"].ToString();

               
            }
            else
            {
                codeg = codeGrp;

                ViewBag.codeg = codeGrp;

                //ViewBag.codeF = codeForm;

                TempData["Group"] = codeGrp;

                //TempData["Form"] = codeForm;
            }


            //List<E_Formation> listF = new List<E_Formation>();

            //listF.Insert(0, new E_Formation { Id = 0, Code = codef });

            //List<E_Formation> CodeForm = listF;

            //ViewBag.CodeFor = CodeForm;


            List<E_GrpByUsr> listG = new List<E_GrpByUsr>();

            listG.Insert(0, new E_GrpByUsr { Id = 0, Code = codeg });

            List<E_GrpByUsr> CodeGroup = listG;

            ViewBag.CodeGroup = CodeGroup;

            IQueryable<ApplicationUser> UserQuery;

            if (searchString != null)
            {
                  UserQuery = from m in db.Users
                              where m.Etat == searchString
                            select m;
            }
            else
            {
                UserQuery = from m in db.Users
                            where m.Etat == "Interne"
                            select m;
            }

            List<ApplicationUser> listUser = UserQuery.ToList<ApplicationUser>();

            ViewBag.listUser = listUser;

            return View();
        }

        // POST: E_listeUsr/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( E_listeUsr e_listeUsr , string btnValid, string searchString)
        {
            if (ModelState.IsValid)
            {
               
                string codeg = TempData["Group"].ToString();

                //string codeF = TempData["Form"].ToString();

                if (searchString != null && btnValid == null)
                {
                    //TempData["Data2"] = e_listeUsr.Code_grp;

                    Session["EtatUsrFor"] = searchString;

                    return RedirectToAction("Create", "E_listeUsr", new { searchString = searchString, codeGrp = codeg });
                }
                //string codef = TempData["Form"].ToString();

               
                var usr = from m in db.Users
                           where m.matricule == e_listeUsr.SelectUsr
                           select m;

               foreach(ApplicationUser a in usr)
                {
                    e_listeUsr.Mat_usr = a.matricule;

                    e_listeUsr.Nom_usr = a.NomPrenom;

                    e_listeUsr.Etat = a.Etat;
                }

                e_listeUsr.Code_grp = codeg;

                //e_listeUsr.Code_formt = codef;

                var idGrp = from m in db.E_GrpByUsr
                            where m.Code == e_listeUsr.Code_grp
                            select m;
                int ss = 0;
                foreach (E_GrpByUsr t in idGrp)
                {
                      ss = t.Id;
                }

                db.E_listeUsr.Add(e_listeUsr);

                db.SaveChanges();

                return RedirectToAction("Index", new { id = ss });
            }

            return View(e_listeUsr);
        }

        // GET: E_listeUsr/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            E_listeUsr e_listeUsr = db.E_listeUsr.Find(id);
            if (e_listeUsr == null)
            {
                return HttpNotFound();
            }
            return View(e_listeUsr);
        }

        // POST: E_listeUsr/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Mat_usr,Nom_usr,Code_grp,Code_formt")] E_listeUsr e_listeUsr)
        {
            if (ModelState.IsValid)
            {
                db.Entry(e_listeUsr).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(e_listeUsr);
        }

        public ActionResult DeleteUsrGrp(int? id)
        {
             
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            E_listeUsr e_listeUsr = db.E_listeUsr.Find(id);
            if (e_listeUsr == null)
            {
                return HttpNotFound();
            }

            var r = from m in db.E_GrpByUsr
                    where m.Code == e_listeUsr.Code_grp
                    select m;

            foreach(E_GrpByUsr e in r)
            {
                Session["Idgrp"] = e.Id;

            }

            return View(e_listeUsr);
        }

        [HttpPost, ActionName("DeleteUsrGrp")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmedUsrGrp(int id)
        {
            //string ff = TempData["CodeFormation"].ToString();

            //ViewBag.form = ff;

            E_listeUsr e_listeUsr = db.E_listeUsr.Find(id);

            var idcode = from m in db.E_GrpByUsr
                         where m.Code == e_listeUsr.Code_grp
                         select m;
            int o = 0;
            foreach (E_GrpByUsr e in idcode)
            {
                o = e.Id;
            }
            db.E_listeUsr.Remove(e_listeUsr);
            db.SaveChanges();
            return RedirectToAction("GetListUsr", new { id = o });
        }


        // GET: E_listeUsr/Delete/5
        public ActionResult Delete(int? id)
        {
            //string ff = TempData["CodeFormation"].ToString();

            //ViewBag.form = ff;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            E_listeUsr e_listeUsr = db.E_listeUsr.Find(id);
            if (e_listeUsr == null)
            {
                return HttpNotFound();
            }

            var idcode = from m in db.E_GrpByUsr
                         where m.Code == e_listeUsr.Code_grp
                         select m;
            
            foreach (E_GrpByUsr e in idcode)
            {
                Session["idgrp"] = e.Id;
            }
            return View(e_listeUsr);
        }

        // POST: E_listeUsr/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            //string ff = TempData["CodeFormation"].ToString();

            //ViewBag.form = ff;

            E_listeUsr e_listeUsr = db.E_listeUsr.Find(id);

            var idcode = from m in db.E_GrpByUsr
                         where m.Code == e_listeUsr.Code_grp
                         select m;
            int o = 0;
            foreach(E_GrpByUsr e in idcode)
            {
                o = e.Id;
            }
            db.E_listeUsr.Remove(e_listeUsr);
            db.SaveChanges();
            return RedirectToAction("Index", new { id = o });
        }

        [HttpPost]
        public ActionResult DeleteUsr(int id)
        {
            //string ff = TempData["CodeFormation"].ToString();

            //ViewBag.form = ff;

            E_listeUsr e_listeUsr = db.E_listeUsr.Find(id);

            var idcode = from m in db.E_GrpByUsr
                         where m.Code == e_listeUsr.Code_grp
                         select m;
            int o = 0;
            foreach (E_GrpByUsr e in idcode)
            {
                o = e.Id;
            }
            db.E_listeUsr.Remove(e_listeUsr);
            db.SaveChanges();
            return RedirectToAction("Index", new { id = o });
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
