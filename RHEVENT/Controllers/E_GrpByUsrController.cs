using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RHEVENT.Models;

using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace RHEVENT.Controllers
{
    public class E_GrpByUsrController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public IEnumerable<E_listeUsr> liste2 = null;

        public IQueryable<E_ListFormationDiffus> verif;


        // GET: E_GrpByUsr

        [HttpPost]  
        public ActionResult Diff(FormCollection formCollection, string id,DateTime dateLim)
        {
            string codeF = id;

            if (formCollection["grpId"] == null)
            {
                ViewBag.Bloq = "La diffusion a été annulée : Il faut séléctionner au moin un groupe!";

                return RedirectToAction("Index", "E_GrpByUsr", new { id = codeF, bloq = @ViewBag.Bloq });
                 
            }

            string[] ids = formCollection["grpId"].Split(new char[]{','});

            var grp = db.E_GrpByUsr.Find(int.Parse(ids[0]));

         

            string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection con = new SqlConnection(constr);
            con.Open();

             
          
            DataTable dt35 = new DataTable(); 
            SqlDataAdapter da35;
            da35 = new SqlDataAdapter("select top(1) * from E_Formation where Code = '" + codeF + "' ", con);
            da35.Fill(dt35);


           for (int p=0; p< dt35.Rows.Count; p++)
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

                            return RedirectToAction("Index", "E_GrpByUsr", new { id = codeF, bloq = @ViewBag.Bloq });
                        }
                    }
                }
            }

            //DataTable dt3 = new DataTable();

            //SqlDataAdapter da3;
            //da3 = new SqlDataAdapter("select E_listeUsr.Mat_usr ,E_listeUsr.Code_grp  , AspNetUsers.NomPrenom from E_listeUsr inner join AspNetUsers on AspNetUsers.matricule = E_listeUsr.Mat_usr where Code_grp = '" + grp.Code + "' ", con);
            //da3.Fill(dt3);
             
           

           //for (int j=0; j< dt3.Rows.Count; j++)
           // {
           //     DataTable dt2 = new DataTable();

           //     SqlDataAdapter da2;
           //     da2 = new SqlDataAdapter("select * from E_ListFormationDiffus  where Code_formt = '"+codeF+"' and  Mat_usr = '"+ dt3.Rows[j]["Mat_usr"].ToString() +"'", con);
           //     da2.Fill(dt2);

                

           //     int count = dt2.Rows.Count;

           //     if (count != 0)
           //     { 
           //         ViewBag.Bloq = "La diffusion a été annulée : la formation " + codeF + " est déjà diffusée à " + dt3.Rows[j]["NomPrenom"].ToString() + " de groupe " + dt3.Rows[j]["Code_grp"].ToString() + " !";

           //         return RedirectToAction("Index", "E_GrpByUsr", new { id = codeF, bloq = @ViewBag.Bloq });
           //     }

              
           // }

            

            //Session["Bloq"] = "";

            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());

            var form = (from m in db.E_Formation
                       where m.Code == codeF
                       select m).Take(1);

            E_Formation e_f = ((from m in db.E_Formation
                        where m.Code == codeF
                        select m).Take(1)).Single();

            string dd = null;
            foreach (E_Formation o in form)
            {
                  dd = o.Objet;
            }
          
           
            foreach (string ide in ids)
            {
                var g = db.E_GrpByUsr.Find(int.Parse(ide));
                 
                //g.Etat_Grp = "Oui";
                 
                db.Entry(g).State = EntityState.Modified;


                //string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
                //SqlConnection con = new SqlConnection(constr);
                //con.Open();

                //foreach (E_Formation e in form)
                //{

                SqlCommand cmd = new SqlCommand("INSERT INTO E_ListFormationDiffus  ([Mat_usr]  ,[Nom_usr] ,[Code_grp]  ,[Code_formt], code_eval, DateDiffus, MatFormateur, Objet, deadline) " +
                    "SELECT  [Mat_usr]  ,[Nom_usr]   ,[Code_grp]   ,'" + id + "', '" + e_f.CodeEval +"' ,'"+System.DateTime.Now+"' , '"+user.matricule+"', '"+ dd+ "' , CONVERT(nvarchar, '" + dateLim + "',103)     FROM [E_listeUsr] " +
                    "" +  "inner join [dbo].E_GrpByUsr on[dbo].E_GrpByUsr.Code = [dbo].[E_listeUsr].Code_grp  " +
                    "" +   "" +   "where  E_GrpByUsr.id = '" + g.Id + "' and not( E_listeUsr.Mat_usr in (select Mat_usr from E_ListFormationDiffus   where Code_formt = '"+id+"' )) ", con);

              
                cmd.ExecuteNonQuery();

                foreach (E_Formation o in form)
                {

                    if (o.CodeEval != null)
                    {

                        DataTable dt = new DataTable();

                        SqlDataAdapter da;
                        da = new SqlDataAdapter("SELECT Objet_Eval FROM E_Evaluation where Code_Eval = '"+o.CodeEval+"'", con);
                        da.Fill(dt);

                        string objEvl = dt.Rows[0]["Objet_Eval"].ToString();

                        SqlCommand cmd3 = new SqlCommand("INSERT INTO E_ListEvaluationDiffus  ([Mat_usr]  ,[Nom_usr] ,[Code_grp]  ,[Code_eval],Code_Formation, DateDiffus, MatFormateur, Objet, deadline) " +
                          "SELECT  [Mat_usr]  ,[Nom_usr]   ,[Code_grp]   ,'" + o.CodeEval + "', '"+o.Code+"', '" + System.DateTime.Now + "' , '" + user.matricule + "', '" + objEvl + "' , CONVERT(nvarchar, '" + dateLim + "',103)     FROM [E_listeUsr] " +
                          "" + "inner join [dbo].E_GrpByUsr on[dbo].E_GrpByUsr.Code = [dbo].[E_listeUsr].Code_grp  " +
                          "" + "" + "where  E_GrpByUsr.id = '" + g.Id + "' and not( E_listeUsr.Mat_usr in (select Mat_usr from E_ListEvaluationDiffus   where Code_Formation = '"+id+"' ))", con);
                             
                            cmd3.ExecuteNonQuery();
                        


                    }
                }


                SqlCommand cmd2 = new SqlCommand("update dbo.E_Formation set EtatDiff = 'Diffusee' where Code =  '" + codeF + "' ", con);
                 
                cmd2.ExecuteNonQuery();


                DataTable dt4 = new DataTable(); 
                SqlDataAdapter da4;
                da4 = new SqlDataAdapter("SELECT AspNetUsers.NomPrenom Nom_Dest, AspNetUsers.Email Email_Dest FROM [E_listeUsr]   inner join [dbo].E_GrpByUsr on[dbo].E_GrpByUsr.Code = [dbo].[E_listeUsr].Code_grp   inner join AspNetUsers on AspNetUsers.matricule =[E_listeUsr].Mat_usr where  E_GrpByUsr.id  = '" + g.Id + "'  and  not(AspNetUsers.Email in ( select Email_Destinataire from Emails where Message like '%"+id+"%')) ", con);
                da4.Fill(dt4);
                 
                for (int j=0; j<dt4.Rows.Count; j++)
                {
                    string Nom_dest = dt4.Rows[j]["Nom_Dest"].ToString();

                    string Email_dest = dt4.Rows[j]["Email_Dest"].ToString();

                    Serveur serveur = (from m in db.Serveur
                                       select m).Single();

                    string Message = "Bonjour"+ "\n" + "Vous avez une formation à réaliser." +"\n" + "Lien: "+ serveur.Serv + "/E_formation/SlideUsr?codeF="+ e_f.Code;

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
                 
            }




            //foreach (E_Formation f in form)
            //{
            //    return RedirectToAction("Index", new { id = f.Id });
            //}

            return RedirectToAction("Index","E_Formation");

            //return View();
        }


        public ActionResult Group()
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());

            var list = from m in db.E_GrpByUsr
                       where m.Matricule_Usr == user.matricule
                       select m;

            return View(list.ToList());
        }


        public ActionResult Index(string id , string bloq)
        {
            ViewBag.Bloq = "";

            if (bloq != null)
            {
                ViewBag.Bloq = bloq;
            }

            ViewBag.l = Convert.ToDateTime("01-01-001") ;

            var l = from m in db.E_Formation
                    where m.Code == id
                    select m;

            int count = l.Count();

            if (count ==0)
            { 
                E_Formation e = db.E_Formation.Find(Convert.ToInt32(id));

                ViewBag.CodeFormation = e.Code;

                ViewBag.DateCreatFormation = e.Date_Creation.ToShortDateString();

                ViewBag.ObjetFormation = e.Objet;

            }
            else
            {
                foreach(E_Formation f in l)
                {
                    ViewBag.CodeFormation = f.Code;

                    ViewBag.DateCreatFormation = f.Date_Creation.ToShortDateString();

                    ViewBag.ObjetFormation = f.Objet;

                }

            }

            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());


            var list = from m in db.E_GrpByUsr
                       where m.Matricule_Usr == user.matricule
                       select m;

            return View(list.ToList());
        }

        // GET: E_GrpByUsr/Details/5
        public ActionResult Details(int? id)
        {
          
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            
            E_GrpByUsr e_GrpByUsr = db.E_GrpByUsr.Find(id);


            string codef = TempData["Form"].ToString();

            string codeg = e_GrpByUsr.Code;

            var listUsr = from m in db.E_listeUsr
                          where  m.Code_grp == codeg
                          select m;

            E_listeUsr e_listeUsr = new E_listeUsr();

            foreach (E_listeUsr l in listUsr)
            {
                //e_listeUsr.Code_formt = l.Code_formt;

                e_listeUsr.Code_grp = l.Code_grp;

                e_listeUsr.Mat_usr = l.Mat_usr;

                e_listeUsr.Nom_usr = l.Nom_usr;
            }

            //if (e_GrpByUsr == null)
            //{
            //    return HttpNotFound();
            //}

            return View(e_listeUsr);
        }

        public ActionResult GetUsrs(string id, string s)
        {
            string ff = TempData["CodeFormation"].ToString();

            ViewBag.form = ff;

            E_GrpByUsr e = db.E_GrpByUsr.Find(Convert.ToInt32(id));

            if (e == null)
            {
                var list = from m in db.E_listeUsr
                           where m.Code_grp == id
                           select m;

                foreach (E_listeUsr ee in list)
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

        public ActionResult DetailGrp(string id)
        { 
            E_GrpByUsr e = db.E_GrpByUsr.Find(Convert.ToInt32(id));

            if (e == null)
            {
                var list = from m in db.E_listeUsr
                           where m.Code_grp == id
                           select m;

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

        [HttpGet]
        public ActionResult CreateG(string id, string searchString)
        {
            Session["EtatUsr"] = null;

            List<ApplicationUser> listUser = new List<ApplicationUser>();

           E_GrpByUsr e = db.E_GrpByUsr.Find(Convert.ToInt32(id));

            if (searchString != null)
            {
                Session["EtatUsr"] = searchString;

                var us = from m in db.Users
                     where m.Etat == searchString
                     orderby m.NomPrenom
                     select m ;

                  
                listUser = us.ToList();

                if (listUser == null)
                { 
                    listUser.Insert(0, new ApplicationUser { matricule = "0", NomPrenom = " " });
                }
            }
            else
            {
                var us = from m in db.Users
                         where m.Etat == "Interne"
                         orderby m.NomPrenom
                         select m;

                listUser = us.ToList();
            }
             
          

            ViewBag.listUser = listUser;

            return View();
        }

        // GET: E_GrpByUsr/Create
        public ActionResult Create(string id)
        {
            //E_Formation e = db.E_Formation.Find(id);

            var r = from m in db.E_Formation
                    where m.Code == id
                    select m;

            List<E_Formation> list = r.ToList<E_Formation>();

            if (list.Count == 0)
            {
                E_GrpByUsr e = db.E_GrpByUsr.Find(Convert.ToInt32(id)); 
            }


            foreach (E_Formation e in r)
            {  
              ViewBag.CodeFormation = e.Code;
                 
            }


            List<E_Formation> CodeForm = r.ToList<E_Formation>();

            ViewBag.CodeFor = CodeForm;


            //var UserQuery = from m in db.Users
            //                select m;


            List<ApplicationUser> listUser = db.Users.ToList();

            ViewBag.listUser = listUser;

            return View( );
        }

        // POST: E_GrpByUsr/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Code,Matricule_Usr,NomPrenom_Usr,Etat_Grp,Code_Formation")] E_GrpByUsr e_GrpByUsr)
        {
            if (ModelState.IsValid)
            {
               
                db.E_GrpByUsr.Add(e_GrpByUsr);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(e_GrpByUsr);
        }

        [HttpGet]
        public ActionResult Diffuser(string id, E_GrpByUsr e_GrpByUsr)
        { 

            E_GrpByUsr e = db.E_GrpByUsr.Find(Convert.ToInt32(id));

            //if (e == null)
            //{
            //    var list = from m in db.E_listeUsr
            //               where m.Code_grp == id
            //               select m;

            //    foreach (E_listeUsr ee in list)
            //    {
            //        ViewBag.codeGrp = ee.Code_grp;

            //        ViewBag.codeForm = ee.Code_formt;


            //    }

            //    liste2 = list;
            //}
            //else
            //{
            //    ViewBag.codeGrp = e.Code;

            //    ViewBag.codeForm = e.Code_Formation;



            //    liste2 = from m in db.E_listeUsr
            //             where m.Code_grp == e.Code
            //             select m;
            //}

            return View(liste2.ToList());
        }


        [HttpPost] 
        public ActionResult Diffuser(int id)
        {
            if (ModelState.IsValid)
            {
                
                E_GrpByUsr e_GrpByUsr = db.E_GrpByUsr.Find(id);

                //e_GrpByUsr.Etat_Grp = "Oui";

           

                db.Entry(e_GrpByUsr).State = EntityState.Modified;

                string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
                SqlConnection con = new SqlConnection(constr);
                con.Open();


                SqlCommand cmd = new SqlCommand("INSERT INTO E_ListFormationDiffus  ([Mat_usr]  ,[Nom_usr] ,[Code_grp]  ,[Code_formt]) SELECT  [Mat_usr]  ,[Nom_usr]   ,[Code_grp]   ,[Code_formt] FROM [E_listeUsr] inner join[RH_MEDICIS].[dbo].E_GrpByUsr on[dbo].E_GrpByUsr.Code = [dbo].[E_listeUsr].Code_grp  where  E_GrpByUsr.id = '" + id + "' ", con);
                cmd.ExecuteNonQuery();


                //DataTable dt = new DataTable();

                //SqlDataAdapter da;
                //da = new SqlDataAdapter("SELECT    E_GrpByUsr.Code , Code_Formation, [E_Formation].id FROM E_GrpByUsr inner join [E_Formation] on [E_Formation].Code = E_GrpByUsr.Code_Formation where E_GrpByUsr.id='" + id + "'", con);
                //da.Fill(dt);

                //string codef = Convert.ToString(dt.Rows[0][1]);

                //string codeg = Convert.ToString(dt.Rows[0][0]);


                //SqlCommand cmd2 = new SqlCommand("delete from [E_listeUsr] where [Code_grp]= '" + codeg + "' and [Code_formt] = '"+ codef + "' ", con);
                //cmd2.ExecuteNonQuery();

                //SqlCommand cmd3 = new SqlCommand("delete from [E_GrpByUsr] where [Code]= '" + codeg + "'", con);
                //cmd3.ExecuteNonQuery();

                db.SaveChanges();


                //var form = from m in db.E_Formation
                //           where m.Code == e_GrpByUsr.Code_Formation
                //           select m;

                //foreach(E_Formation f in form)
                //{
                //    return RedirectToAction("Index", new { id = f.Id });
                //}

             

            }

            return View();
        }

        //public void MessageBox(String Message)
        //{
        //    Page.ClientScript.RegisterStartupScript(Page.GetType(), "MessageBox", "<script language='javascript'>  alert('" + Message + "');</script>");
        //}

        [ValidateAntiForgeryToken]
        public ActionResult AddUserG(E_GrpByUsr e_GrpByUsr, E_listeUsr e_listeUsr, string id , string btnValid, string searchString)
        {
            if (ModelState.IsValid)
            {
                 if(searchString != null   && btnValid == null)
                {
                    return RedirectToAction("CreateG", "E_GrpByUsr",  new { searchString = searchString });
                }

                var listc = from m in db.E_GrpByUsr
                            where m.Code == e_GrpByUsr.Code
                            select m;

                int c = listc.ToList().Count;

                if (c != 0)
                { 
                    ViewBag.ErrorMessage = $"Ce nom est déjà utilisé par un autre groupe ";
                     
                    return View("ExistGrpG"); 

                }
 
                ApplicationUser login = db.Users.Find(User.Identity.GetUserId());

                //e_GrpByUsr.Etat_Grp = "Non";

                e_GrpByUsr.Matricule_Usr = login.matricule;


                if (ModelState.IsValid)
                {
                    db.E_GrpByUsr.Add(e_GrpByUsr);

                }

                db.Entry<E_listeUsr>(e_listeUsr).State = EntityState.Added;

                var list = from m in db.Users
                           where m.matricule == e_GrpByUsr.Utilisateur
                           select m;


                foreach (ApplicationUser u in list)
                {
                    e_listeUsr.Code_grp = e_GrpByUsr.Code;

                    e_listeUsr.Mat_usr = u.matricule;

                    e_listeUsr.Nom_usr = u.NomPrenom;

                    e_listeUsr.Etat = u.Etat;


                }

                if (ModelState.IsValid)
                {
                    db.E_listeUsr.Add(e_listeUsr);
                    db.SaveChanges();

                    return RedirectToAction("GetListUsr", "E_listeUsr", new { id = e_GrpByUsr.Id });
                }

                return View(e_GrpByUsr);
            }
            return View();
        }


        //[HttpPost, ActionName("AddUser")]
        //[HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUser( E_GrpByUsr e_GrpByUsr,  E_listeUsr e_listeUsr, string id)
        { 
            if (ModelState.IsValid)
            { 
                var listc = from m in db.E_GrpByUsr
                            where m.Code == e_GrpByUsr.Code 
                            select m;

                int c = listc.ToList().Count;

                if (c != 0)
                {

                    ViewBag.ErrorMessage = $"Ce nom est déjà utilisé par un autre groupe ";

                    //string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
                    //SqlConnection con = new SqlConnection(constr);
                    //con.Open();

                    //DataTable dt = new DataTable();

                    //SqlDataAdapter da;
                    //da = new SqlDataAdapter("SELECT    [Code] FROM [E_Formation] where id='" + e_GrpByUsr.Code_Formation + "'", con);
                    //da.Fill(dt);

                    //ViewBag.codeF = Convert.ToString(dt.Rows[0][0]);

                     
                    return View("ExistGrp");

                
                }


                //E_Formation e = db.E_Formation.Find(Convert.ToInt32 (e_GrpByUsr.Code_Formation));
                 
                //e_GrpByUsr.Code_Formation = e.Code;
                 

                ApplicationUser login = db.Users.Find(User.Identity.GetUserId());

                //e_GrpByUsr.Etat_Grp = "Non";
                 
                e_GrpByUsr.Matricule_Usr = login.matricule;


                if (ModelState.IsValid)
                {
                    db.E_GrpByUsr.Add(e_GrpByUsr);

                }

                db.Entry<E_listeUsr>(e_listeUsr).State = EntityState.Added;

                var list = from m in db.Users
                           where m.matricule == e_GrpByUsr.Utilisateur
                           select m;


                foreach (ApplicationUser u in list)
                {
                    e_listeUsr.Code_grp = e_GrpByUsr.Code;

                    e_listeUsr.Mat_usr = u.matricule;

                    e_listeUsr.Nom_usr = u.NomPrenom;

                    //e_listeUsr.Code_formt = e.Code;

                }

                if (ModelState.IsValid)
                {
                    db.E_listeUsr.Add(e_listeUsr);
                    db.SaveChanges();

                    return RedirectToAction("Index","E_listeUsr", new { id = e_GrpByUsr.Id });
                }

                return View(e_GrpByUsr);
            } 
            return View();
        }

        // GET: E_GrpByUsr/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            E_GrpByUsr e_GrpByUsr = db.E_GrpByUsr.Find(id);
            if (e_GrpByUsr == null)
            {
                return HttpNotFound();
            }
            return View(e_GrpByUsr);
        }

        // POST: E_GrpByUsr/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Code,Matricule_Usr,NomPrenom_Usr,Etat_Grp,Code_Formation")] E_GrpByUsr e_GrpByUsr)
        {
            if (ModelState.IsValid)
            {
                db.Entry(e_GrpByUsr).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(e_GrpByUsr);
        }


        public ActionResult DeleteG(int? id)
        {
           
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            E_GrpByUsr e_GrpByUsr = db.E_GrpByUsr.Find(id);
            if (e_GrpByUsr == null)
            {
                return HttpNotFound();
            }
            return View(e_GrpByUsr);
        }

        [HttpPost]
        public async Task<ActionResult> DeleteGrp(int id)
        {
            string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection con = new SqlConnection(constr);
            con.Open();

            DataTable dt = new DataTable();

            SqlDataAdapter da;
            da = new SqlDataAdapter("SELECT    E_GrpByUsr.Code  FROM E_GrpByUsr where E_GrpByUsr.id='" + id + "'", con);
            da.Fill(dt);

            string codeg = Convert.ToString(dt.Rows[0][0]);

            SqlCommand cmd = new SqlCommand("delete FROM E_listeUsr where Code_grp='" + codeg + "' ", con);
            cmd.ExecuteNonQuery();

            SqlCommand cmd2 = new SqlCommand("delete FROM E_GrpByUsr where id='" + id + "'", con);
            cmd2.ExecuteNonQuery();

            con.Close();

            return RedirectToAction("Group");
        }


        // POST: E_GrpByUsr/Delete/5
        [HttpPost, ActionName("DeleteG")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmedG(int id)
        { 
            string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection con = new SqlConnection(constr);
            con.Open();

            DataTable dt = new DataTable();

            SqlDataAdapter da;
            da = new SqlDataAdapter("SELECT    E_GrpByUsr.Code  FROM E_GrpByUsr where E_GrpByUsr.id='" + id + "'", con);
            da.Fill(dt);

            string codeg = Convert.ToString(dt.Rows[0][0]);
 
            SqlCommand cmd = new SqlCommand("delete FROM E_listeUsr where Code_grp='" + codeg + "' ", con);
            cmd.ExecuteNonQuery();

            SqlCommand cmd2 = new SqlCommand("delete FROM E_GrpByUsr where id='" + id + "'", con);
            cmd2.ExecuteNonQuery();
             
            con.Close();
                
            return RedirectToAction("Group");
        }


        // GET: E_GrpByUsr/Delete/5
        public ActionResult Delete(int? id)
        {
            string ff = TempData["CodeFormation"].ToString();

            ViewBag.form = ff;



            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            E_GrpByUsr e_GrpByUsr = db.E_GrpByUsr.Find(id);
            if (e_GrpByUsr == null)
            {
                return HttpNotFound();
            }
            return View(e_GrpByUsr);
        }

        [HttpPost]
        public async Task<ActionResult> DeleteGroup(int id)
        {
             
            string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection con = new SqlConnection(constr);
            con.Open();

            DataTable dt = new DataTable();

            SqlDataAdapter da;
            da = new SqlDataAdapter("SELECT    E_GrpByUsr.Code  FROM E_GrpByUsr where E_GrpByUsr.id='" + id + "'", con);
            da.Fill(dt);

            string codeg = Convert.ToString(dt.Rows[0][0]);

            

            SqlCommand cmd = new SqlCommand("delete FROM E_listeUsr where Code_grp='" + codeg + "' ", con);
            cmd.ExecuteNonQuery();

            SqlCommand cmd2 = new SqlCommand("delete FROM E_GrpByUsr where id='" + id + "'", con);
            cmd2.ExecuteNonQuery();


            con.Close();
 
 
            return RedirectToAction("Group");
        }


        // POST: E_GrpByUsr/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {

            string ff = TempData["CodeFormation"].ToString();

            ViewBag.form = ff;

            string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection con = new SqlConnection(constr);
            con.Open();

            DataTable dt = new DataTable();

            SqlDataAdapter da;
            da = new SqlDataAdapter("SELECT    E_GrpByUsr.Code  FROM E_GrpByUsr where E_GrpByUsr.id='" + id + "'", con);
            da.Fill(dt);

            string codeg = Convert.ToString(dt.Rows[0][0]);

            //string codef = Convert.ToString(dt.Rows[0][1]);

            //string idF = Convert.ToString(dt.Rows[0][2]);


            SqlCommand cmd = new SqlCommand("delete FROM E_listeUsr where Code_grp='" + codeg + "' ", con);
            cmd.ExecuteNonQuery();

            SqlCommand cmd2 = new SqlCommand("delete FROM E_GrpByUsr where id='" + id + "'", con);
            cmd2.ExecuteNonQuery();

 
            con.Close();

            //var list = from m in db.E_GrpByUsr
            //           where m.Id == id
            //           select m;


            //E_GrpByUsr e_GrpByUsr = new E_GrpByUsr();

            //foreach(E_GrpByUsr ee in list)
            //{ 
            //    E_listeUsr e_listeUsr = new E_listeUsr();






            //    var listUsr = from m in db.E_listeUsr
            //                  where (m.Code_formt == ee.Code_Formation && m.Code_grp == ee.Code)
            //                  select m;


            //    List<E_listeUsr> listff = new List<E_listeUsr>();

            //    listff = listUsr.ToList();



            //    foreach (E_listeUsr pp in listUsr)
            //    {
            //        e_listeUsr.Code_formt = pp.Code_formt;

            //        e_listeUsr.Code_grp = pp.Code_grp;

            //        e_listeUsr.Mat_usr = pp.Mat_usr;

            //        e_listeUsr.Nom_usr = pp.Nom_usr;
            //    }

            //    db.E_listeUsr.Remove(e_listeUsr);


            //    db.Entry<E_listeUsr>(e_listeUsr).State = EntityState.Deleted;


            //    db.Entry<E_GrpByUsr>(e_GrpByUsr).State = EntityState.Added;

            //    e_GrpByUsr.Code = ee.Code;

            //    e_GrpByUsr.Code_Formation = ee.Code_Formation;

            //}

            //db.E_GrpByUsr.Remove(e_GrpByUsr);

            //db.SaveChanges();

           

            return RedirectToAction("Index", new { id = ff });
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
