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
using RHEVENT.Models;

namespace RHEVENT.Controllers
{
    public class DA_WorkflowTypAchController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: DA_WorkflowTypAch
        public ActionResult Index()
        {
            return View(db.DA_WorkflowTypAch.ToList());
        }

        // GET: DA_WorkflowTypAch/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DA_WorkflowTypAch dA_WorkflowTypAch = db.DA_WorkflowTypAch.Find(id);
            if (dA_WorkflowTypAch == null)
            {
                return HttpNotFound();
            }
            return View(dA_WorkflowTypAch);
        }

        // GET: DA_WorkflowTypAch/Create
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int Num, string Intervenant, DA_WorkflowTypAch dA_WorkflowTypAch)
        {
            if (ModelState.IsValid)
            {
                int idtypee = Convert.ToInt32(Session["idtyp"].ToString());
                DA_TypesAchats dA_TypesAchats = db.DA_TypesAchats.Find(idtypee);

                string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
                SqlConnection con = new SqlConnection(constr);

                con.Open();

                SqlDataAdapter da11 = new SqlDataAdapter("SELECT Max(Num) FROM DA_WorkflowTypAch where Id_type='" + idtypee + "'", con);
                DataTable dt11 = new DataTable();
                da11.Fill(dt11);
                int maxnum =Convert.ToInt32(dt11.Rows[0][0].ToString());
                if(Num> maxnum)
                {
                    Num = maxnum + 1;
                }

                dA_WorkflowTypAch.Id_type = Convert.ToInt32(Session["idtyp"].ToString());
                dA_WorkflowTypAch.Num = Num;
                dA_WorkflowTypAch.Intervenant = Intervenant;
                db.DA_WorkflowTypAch.Add(dA_WorkflowTypAch);
                db.SaveChanges();

                int id_ajouté = dA_WorkflowTypAch.Id;
                
                SqlCommand cmd = new SqlCommand("update DA_WorkflowTypAch set Num = Num+1  where Num >= '" + Num + "' and Id !='"+ id_ajouté + "' and Id_type='"+ idtypee + "'", con);
                cmd.ExecuteNonQuery();

                string workf = "";
                SqlDataAdapter da = new SqlDataAdapter("SELECT Intervenant FROM DA_WorkflowTypAch where Id_type='" + idtypee + "' order by Num", con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                for(int i=0;i<dt.Rows.Count;i++)
                {
                    if (workf == "")
                    {
                        workf = dt.Rows[i][0].ToString();
                    }
                    else
                    {
                        workf = workf + " --> " + dt.Rows[i][0].ToString();
                    }
                }
                workf = workf.Replace("'", " ");
                SqlCommand cmd1 = new SqlCommand("update DA_TypesAchats set Workflow = '" + workf + "'  where Id='" + idtypee + "'", con);
                cmd1.ExecuteNonQuery();
                con.Close();

                return RedirectToAction("Edit", "DA_TypesAchats", new { id = Convert.ToInt32(Session["idtyp"].ToString()) });
                //return RedirectToAction("Index");
            }

            return View(dA_WorkflowTypAch);
        }
        // POST: DA_WorkflowTypAch/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create1([Bind(Include = "Id,Id_type,Num,Intervenant")] DA_WorkflowTypAch dA_WorkflowTypAch)
        {
            if (ModelState.IsValid)
            {
                dA_WorkflowTypAch.Id_type = Convert.ToInt32(Session["idtyp"].ToString());
                dA_WorkflowTypAch.Num = dA_WorkflowTypAch.Num;
                dA_WorkflowTypAch.Intervenant = dA_WorkflowTypAch.Intervenant;

                db.DA_WorkflowTypAch.Add(dA_WorkflowTypAch);
                db.SaveChanges();
                return RedirectToAction("Edit", "DA_TypesAchats", new { id = Convert.ToInt32(Session["idtyp"].ToString()) });
            }

            return View(dA_WorkflowTypAch);
        }

        // GET: DA_WorkflowTypAch/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DA_WorkflowTypAch dA_WorkflowTypAch = db.DA_WorkflowTypAch.Find(id);
            if (dA_WorkflowTypAch == null)
            {
                return HttpNotFound();
            }
            return View(dA_WorkflowTypAch);
        }

        // POST: DA_WorkflowTypAch/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Id_type,Num,Intervenant")] DA_WorkflowTypAch dA_WorkflowTypAch)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dA_WorkflowTypAch).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(dA_WorkflowTypAch);
        }

        // GET: DA_WorkflowTypAch/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DA_WorkflowTypAch dA_WorkflowTypAch = db.DA_WorkflowTypAch.Find(id);
            if (dA_WorkflowTypAch == null)
            {
                return HttpNotFound();
            }
            return View(dA_WorkflowTypAch);
        }

        // POST: DA_WorkflowTypAch/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DA_WorkflowTypAch dA_WorkflowTypAch = db.DA_WorkflowTypAch.Find(id);
            db.DA_WorkflowTypAch.Remove(dA_WorkflowTypAch);
            db.SaveChanges();

            int Num = dA_WorkflowTypAch.Num;
            int idtypee = dA_WorkflowTypAch.Id_type;

            string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection con = new SqlConnection(constr);

            con.Open();
            SqlCommand cmd = new SqlCommand("update DA_WorkflowTypAch set Num = Num-1  where Num >= '" + Num + "' and Id_type='" + idtypee + "'", con);
            cmd.ExecuteNonQuery();

            string workf = "";
            SqlDataAdapter da = new SqlDataAdapter("SELECT Intervenant FROM DA_WorkflowTypAch where Id_type='" + idtypee + "' order by Num", con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (workf == "")
                {
                    workf = dt.Rows[i][0].ToString();
                }
                else
                {
                    workf = workf + " --> " + dt.Rows[i][0].ToString();
                }
            }
            workf = workf.Replace("'", " ");
            SqlCommand cmd1 = new SqlCommand("update DA_TypesAchats set Workflow = '" + workf + "'  where Id='" + idtypee + "'", con);
            cmd1.ExecuteNonQuery();

            con.Close();

            return RedirectToAction("Edit", "DA_TypesAchats", new { id = Convert.ToInt32(Session["idtyp"].ToString()) });
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
