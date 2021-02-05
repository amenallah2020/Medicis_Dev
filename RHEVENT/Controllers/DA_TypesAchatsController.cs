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
using RHEVENT.Models.RH;

namespace RHEVENT.Controllers
    {
        public class DA_TypesAchatsController : Controller
        {
            private ApplicationDbContext db = new ApplicationDbContext();

            // GET: DA_TypesAchats
            public ActionResult Index()
            {
                return View(db.DA_TypesAchats.ToList().OrderBy(x => x.Code));
            }

            // GET: DA_TypesAchats/Details/5
            public ActionResult Details(int? id)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                DA_TypesAchats dA_TypesAchats = db.DA_TypesAchats.Find(id);
                if (dA_TypesAchats == null)
                {
                    return HttpNotFound();
                }
                return View(dA_TypesAchats);
            }

            // GET: DA_TypesAchats/Create
            public ActionResult Create()
            {
                return View();
            }

        // POST: DA_TypesAchats/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DA_TypesAchats dA_TypesAchats)
        {
            if (ModelState.IsValid)
            {
                dA_TypesAchats.Workflow = "N+1";
                db.DA_TypesAchats.Add(dA_TypesAchats);
                db.SaveChanges();

                DA_WorkflowTypAch wkf = new DA_WorkflowTypAch();
                wkf.Id_type = dA_TypesAchats.Id;
                wkf.Num = 1;
                wkf.Intervenant = "N+1";
                db.DA_WorkflowTypAch.Add(wkf);
                db.SaveChanges();

                string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
                SqlConnection con = new SqlConnection(constr);

                con.Open();
               
                string workf = "N+1";
                
                SqlCommand cmd1 = new SqlCommand("update DA_TypesAchats set Workflow = '" + workf + "'  where Id='" + dA_TypesAchats.Id + "'", con);
                cmd1.ExecuteNonQuery();

                con.Close();


                return RedirectToAction("Edit", new { id = dA_TypesAchats.Id });
            }

            return View(dA_TypesAchats);
        }

        // GET: DA_TypesAchats/Edit/5
        public ActionResult Edit(int? id)
        {
            var list = (from m in db.DA_WorkflowTypAch
                        where m.Id_type == id
                        orderby m.Num
                        select m);

            Session["idtyp"] = id;


            //List<FonctionsUsers> fonctionlist = new List<FonctionsUsers>();
            //fonctionlist = (from a in db.FonctionsUsers select a).ToList();
            //fonctionlist.Insert(0, new FonctionsUsers { Id = 0, Fonction = "" });
            //ViewBag.listfonction = fonctionlist;


            var listfonction = (from m in db.FonctionsUsers
                                orderby m.Fonction
                                select m);
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DA_TypesAchats dA_TypesAchats = db.DA_TypesAchats.Find(id);
            dA_TypesAchats.listesworkflow = list.ToList();
            dA_TypesAchats.listesintervenant = listfonction.ToList();
            if (dA_TypesAchats == null)
            {
                return HttpNotFound();
            }
            return View(dA_TypesAchats);
        }

        // POST: DA_TypesAchats/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DA_TypesAchats dA_TypesAchats)
        {
            if (ModelState.IsValid)
            {

                db.Entry(dA_TypesAchats).State = EntityState.Modified;
                db.SaveChanges();

                string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
                SqlConnection con = new SqlConnection(constr);
                int idtypee = dA_TypesAchats.Id;
                con.Open();
                string workf = "";
                SqlDataAdapter da = new SqlDataAdapter("SELECT Intervenant FROM DA_WorkflowTypAch where Id_type='" + idtypee + "' order by Num", con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if(workf=="")
                    {
                        workf = dt.Rows[i][0].ToString();
                    }
                    else
                    {
                        workf = workf + " --> " + dt.Rows[i][0].ToString();
                    }
                    
                }
                SqlCommand cmd1 = new SqlCommand("update DA_TypesAchats set Workflow = '" + workf + "'  where Id='" + idtypee + "'", con);
                cmd1.ExecuteNonQuery();
                con.Close();

                return RedirectToAction("Index");
            }
            return View(dA_TypesAchats);
        }

        // GET: DA_TypesAchats/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DA_TypesAchats dA_TypesAchats = db.DA_TypesAchats.Find(id);
            if (dA_TypesAchats == null)
            {
                return HttpNotFound();
            }
            return View(dA_TypesAchats);
        }

            // POST: DA_TypesAchats/Delete/5
            [HttpPost, ActionName("Delete")]
            [ValidateAntiForgeryToken]
            public ActionResult DeleteConfirmed(int id)
            {
                DA_TypesAchats dA_TypesAchats = db.DA_TypesAchats.Find(id);
                db.DA_TypesAchats.Remove(dA_TypesAchats);
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
