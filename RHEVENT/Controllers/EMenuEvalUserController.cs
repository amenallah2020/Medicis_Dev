using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using RHEVENT.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RHEVENT.Controllers
{
    public class EMenuEvalUserController : Controller
    {
        public IQueryable<E_Formation> lF;

        static string cc;

        private ApplicationDbContext db = new ApplicationDbContext();
        UserManager<IdentityUser> userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>());

        // GET: 
        [Authorize]
        public ActionResult Index()
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.nom_prenom = user.nom + " " + user.prenom;
            ViewBag.email = user.Email;

          

            string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection con = new SqlConnection(constr);
            con.Open();

            
                SqlCommand command1 = new SqlCommand("select distinct Code_eval ,  Objet  from [E_ListEvaluationDiffus]  left join E_ResultFormation on E_ResultFormation.Code_Formation = [E_ListEvaluationDiffus].code_formation where ([E_ListEvaluationDiffus].Code_eval not in (select E_ResultQCM.Code_EvalByQCM from E_ResultQCM) and[E_ListEvaluationDiffus].Code_Formation in ( select Code_Formation from E_ResultFormation) and[E_ListEvaluationDiffus].Mat_usr = " + user.matricule+ ") or ([E_ListEvaluationDiffus].Code_Formation is null and [E_ListEvaluationDiffus].Code_eval not in (select E_ResultQCM.Code_EvalByQCM from E_ResultQCM)  and[E_ListEvaluationDiffus].Mat_usr = " + user.matricule + "  )", con);
                SqlDataAdapter da1 = new SqlDataAdapter(command1);
                DataTable dt1 = new DataTable();
                da1.Fill(dt1);

            Session["nb"] = dt1.Rows.Count;
            Session["nbEAR"] = dt1.Rows.Count;

            for (int i = 0; i < dt1.Rows.Count; i++)
            {
                string aa  = Convert.ToString(dt1.Rows[i][0])+ " - " + Convert.ToString(dt1.Rows[i][1]);

                TempData[i.ToString()] = aa;
            }

            //for (int j = 0; j < dt1.Rows.Count; j++)
            //{

            //    TempData[j.ToString()] = Convert.ToString(dt1.Rows[j][1]);
            //}

            //for (int n = 0; n < dt1.Rows.Count; n++)
            //{
            //    cc = Convert.ToString(dt1.Rows[n][0]);

            //    var t = from m in db.E_Formation
            //            where m.Code == cc
            //            select m;

            //    foreach (E_Formation d in t)
            //    {
            //        TempData[n.ToString()] = d.Objet;
            //    }

            //}



            return View();
        }
    }
}