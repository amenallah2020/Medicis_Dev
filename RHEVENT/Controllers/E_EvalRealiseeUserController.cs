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
    public class E_EvalRealiseeUserController : Controller
    {
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

            DataTable dt = new DataTable();

            SqlDataAdapter da;
            da = new SqlDataAdapter("select distinct Code_EvalByQCM , objEval, CONVERT(date,DateEval,103) DateEval, Score , Resultat from E_ResultQCM where MatUser = " + user.matricule + " union select distinct  Code_EvalByQCM ,  objEval,  CONVERT(date, DateEval,103) DateEval,  Score , Resultat from [E_ResultQCM_Historiq]  where  MatUser = " + user.matricule + "  ", con);
            da.Fill(dt);

            List<E_ResultQCM> list = new List<E_ResultQCM>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                E_ResultQCM resultQCMs = new E_ResultQCM();

                resultQCMs.Code_EvalByQCM = dt.Rows[i]["Code_EvalByQCM"].ToString();

                resultQCMs.ObjEval = dt.Rows[i]["ObjEval"].ToString();

                resultQCMs.DateEval = Convert.ToDateTime (dt.Rows[i]["DateEval"].ToString());

                resultQCMs.Score = Convert.ToInt32 (dt.Rows[i]["Score"].ToString());

                resultQCMs.Resultat =  dt.Rows[i]["Resultat"].ToString() ;

                list.Add(resultQCMs);
            }

                return View(list);
        }
    }
}