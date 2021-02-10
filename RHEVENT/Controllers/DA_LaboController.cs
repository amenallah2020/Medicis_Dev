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
using Microsoft.AspNet.Identity.EntityFramework;
using RHEVENT.Models;

namespace RHEVENT.Controllers
{
    public class DA_LaboController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        UserManager<IdentityUser> userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>());

        private SqlConnection con;


        public ActionResult LaboUser(string matricule, string nomprenom)
        {

            var list = (from m in db.DA_LaboUser
                        where m.Matricule == matricule
                        orderby m.Laboratoire
                        select m);

            ViewBag.userr = nomprenom;
            return View(list.ToList());
        }

        [HttpPost]
        public ActionResult LaboUser(FormCollection formCollection, string matricule)
        {
            string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection con = new SqlConnection(constr);
            con.Open();

            string[] listelabb = null;

            if (formCollection["Laboo"] == null)
            {
                listelabb = null;
            }
           else
            {
                listelabb = formCollection["Laboo"].Split(new char[] { ',' });
            }


            int etatt = 0;
            SqlCommand cmd11 = new SqlCommand("update DA_LaboUser set Etat = " + etatt + " where  Matricule='" + matricule + "'", con);
            cmd11.ExecuteNonQuery();

            etatt = 1;


            if (listelabb != null)
            {
                foreach (string labb in listelabb)
                {
                    SqlCommand cmd1 = new SqlCommand("update DA_LaboUser set Etat = " + etatt + " where  Laboratoire='" + labb + "' and Matricule='" + matricule + "'", con);
                    cmd1.ExecuteNonQuery();

                }
            }
            con.Close();
            return RedirectToAction("Index", "ApplicationUsers");
        }

        [HttpPost]
        public ActionResult Addlaboo(DA_Labo obj)
        {
            AddDetails(obj);
            return View();
        }
        private void connection()
        {
            string constr = ConfigurationManager.ConnectionStrings["SqlConn"].ToString();
            con = new SqlConnection(constr);

        }
        private void AddDetails(DA_Labo obj)
        {
            connection();
            SqlCommand com = new SqlCommand("AddLabo", con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@Code", obj.Code);
            com.Parameters.AddWithValue("@Laboratoire", obj.Laboratoire);
            //com.Parameters.AddWithValue("@Adresse", obj.Adresse);
            //com.Parameters.AddWithValue("@Tel", obj.Tel);
            //com.Parameters.AddWithValue("@Mobile", obj.Mobile);
            con.Open();
            com.ExecuteNonQuery();
            con.Close();

        }

        // GET: DA_Labo
        public ActionResult Index()
        {
            return View(db.DA_Labo.OrderBy(m=>m.Laboratoire).ToList());
        }

        // GET: DA_Labo/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DA_Labo dA_Labo = db.DA_Labo.Find(id);
            if (dA_Labo == null)
            {
                return HttpNotFound();
            }
            return View(dA_Labo);
        }

        // GET: DA_Labo/Create
        public ActionResult Create()
        {
            return View();
        }
        public JsonResult IsLaboNameExist(string LaboName, int? Id)
        {
            var validateName = db.DA_Labo.FirstOrDefault
                                (x => x.Laboratoire == LaboName && x.Id != Id);
            if (validateName != null)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }


       

        // POST: DA_Labo/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Code,Laboratoire/*,Adresse,Tel,Mobile*/")] DA_Labo dA_Labo,string Laboratoire)
        {
            bool IsLaboNameExist = db.DA_Labo.Any
        (x => x.Laboratoire == dA_Labo.Laboratoire && x.Id != dA_Labo.Id);
            if (IsLaboNameExist == true)
            {
                ModelState.AddModelError("Laboratoire", "Ce laboratoire existe déja");
            }

            bool IsLaboNameExist1 = db.DA_Labo.Any
       (x => x.Code == dA_Labo.Code && x.Id != dA_Labo.Id);
            if (IsLaboNameExist == true)
            {
                ModelState.AddModelError("Code", "Ce code existe déja");
            }
            dA_Labo.Laboratoire = Laboratoire;

            if (ModelState.IsValid)
            {
                
                db.DA_Labo.Add(dA_Labo);
                db.SaveChanges();

                string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
                SqlConnection con = new SqlConnection(constr);
                con.Open();

                int etatt = 0;

                SqlDataAdapter da2 = new SqlDataAdapter("SELECT matricule FROM AspNetUsers", con);
                DataTable dt2 = new DataTable();
                da2.Fill(dt2);
                for (int i = 0; i < dt2.Rows.Count; i++)
                {
                    string matricule = Convert.ToString(dt2.Rows[i][0]);

                    SqlCommand cmd = new SqlCommand("INSERT INTO DA_LaboUser (Matricule,Laboratoire,Etat)" +
                    " values(@Matricule,@Laboratoire,@Etat)", con);

                    cmd.Parameters.AddWithValue("@Matricule", matricule);
                    cmd.Parameters.AddWithValue("@Laboratoire", dA_Labo.Laboratoire);
                    cmd.Parameters.AddWithValue("@Etat", etatt);
                    cmd.ExecuteNonQuery();

                }
                con.Close();
                return RedirectToAction("Index");
            }

            return View(dA_Labo);
        }

        public void newlabuser(string laboratoiree)
        {
            string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection con = new SqlConnection(constr);
            con.Open();

            int etatt = 0;

            SqlDataAdapter da2 = new SqlDataAdapter("SELECT matricule FROM AspNetUsers", con);
            DataTable dt2 = new DataTable();
            da2.Fill(dt2);
            for (int i = 0; i < dt2.Rows.Count; i++)
            {
                string matricule = Convert.ToString(dt2.Rows[i][0]);

                SqlCommand cmd = new SqlCommand("INSERT INTO DA_LaboUser (Matricule,Laboratoire,Etat)" +
                " values(@Matricule,@Laboratoire,@Etat)", con);

                cmd.Parameters.AddWithValue("@Matricule", matricule);
                cmd.Parameters.AddWithValue("@Laboratoire", laboratoiree);
                cmd.Parameters.AddWithValue("@Etat", etatt);
                cmd.ExecuteNonQuery();
            }
            con.Close();
        }

        // GET: DA_Labo/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DA_Labo dA_Labo = db.DA_Labo.Find(id);
            if (dA_Labo == null)
            {
                return HttpNotFound();
            }
            Session["laboratoiree"] = dA_Labo.Laboratoire;
          
            return View(dA_Labo);
        }
       
        // POST: DA_Labo/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Code,Laboratoire/*,Adresse,Tel,Mobile*/")] DA_Labo dA_Labo)
        {
            if (ModelState.IsValid)
            {
                
                //string labou = dA_Labo.Laboratoire;
                db.Entry(dA_Labo).State = EntityState.Modified;
                db.SaveChanges();

                string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
                SqlConnection con = new SqlConnection(constr);
                con.Open();
                SqlCommand cmd1 = new SqlCommand("update DA_LaboUser set Laboratoire = '" + dA_Labo.Laboratoire + "' where  Laboratoire='" + Session["laboratoiree"] + "'", con);
                cmd1.ExecuteNonQuery();
                con.Close();

                return RedirectToAction("Index");
            }
            return View(dA_Labo);
        }

        // GET: DA_Labo/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DA_Labo dA_Labo = db.DA_Labo.Find(id);
            if (dA_Labo == null)
            {
                return HttpNotFound();
            }
            return View(dA_Labo);
        }

        // POST: DA_Labo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {

            DA_Labo dA_Labo = db.DA_Labo.Find(id);
            db.DA_Labo.Remove(dA_Labo);

            string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection con = new SqlConnection(constr);
            con.Open();
            SqlCommand cmd1 = new SqlCommand("delete from DA_LaboUser where Laboratoire = '" + dA_Labo.Laboratoire + "'", con);
            cmd1.ExecuteNonQuery();
            con.Close();

            db.SaveChanges();
            return RedirectToAction("Index");

            //string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            //SqlConnection con = new SqlConnection(constr);
            //con.Open();

            //int etatt = 0;

            //SqlDataAdapter da2 = new SqlDataAdapter("SELECT matricule FROM AspNetUsers", con);
            //DataTable dt2 = new DataTable();
            //da2.Fill(dt2);
            //for (int i = 0; i < dt2.Rows.Count; i++)
            //{
            //    string matricule = Convert.ToString(dt2.Rows[i][0]);

            //    SqlDataAdapter da22 = new SqlDataAdapter("SELECT Laboratoire FROM DA_Labo", con);
            //    DataTable dt22 = new DataTable();
            //    da22.Fill(dt22);

            //    for (int ii = 0; ii < dt22.Rows.Count; ii++)
            //    {
            //        string labbb = Convert.ToString(dt22.Rows[ii][0]);

            //        SqlCommand cmd = new SqlCommand("INSERT INTO DA_LaboUser (Matricule,Laboratoire,Etat)" +
            //        " values(@Matricule,@Laboratoire,@Etat)", con);

            //        cmd.Parameters.AddWithValue("@Matricule", matricule);
            //        cmd.Parameters.AddWithValue("@Laboratoire", labbb);
            //        cmd.Parameters.AddWithValue("@Etat", etatt);
            //        cmd.ExecuteNonQuery();
            //    }
            //}
            //con.Close();
            //return RedirectToAction("Index");
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
