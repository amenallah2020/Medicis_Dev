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
    public class DA_ProduitsDemController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        UserManager<IdentityUser> userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>());

        // GET: DA_ProduitsDem
        public ActionResult Index()
        {
            return View(db.DA_ProduitsDem.ToList());
        }

        // GET: DA_ProduitsDem/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DA_ProduitsDem dA_ProduitsDem = db.DA_ProduitsDem.Find(id);
            if (dA_ProduitsDem == null)
            {
                return HttpNotFound();
            }
            return View(dA_ProduitsDem);
        }

        // GET: DA_ProduitsDem/Create
        public ActionResult Create(int Id = 0)
        {
            string reference = Session["reff"].ToString();
            var list = (from m in db.DA_ProduitsDem
                        where m.Réference == reference
                        orderby m.Laboratoire
                        select m);

            string labbb = Session["labbb"].ToString();
            var listproduits = (from m in db.DA_Produits
                                where m.Laboratoire == labbb
                                orderby m.Désignation
                                select m);


            var listproduits1 = (from m in db.DA_Produits
                                orderby m.Désignation
                                select m);

            DA_ProduitsDem produitsdemande = new DA_ProduitsDem();
           
            if (Id != 0)
            {
                produitsdemande = db.DA_ProduitsDem.Where(x => x.Id == Id).FirstOrDefault();
                produitsdemande.SelectedCodeArray = produitsdemande.Code.Split(',').ToArray();
            }
            produitsdemande.ProduitsCollection = listproduits.ToList();
            produitsdemande.ProduitsCollection1 = listproduits1.ToList();
            produitsdemande.listesProduitsDem = list.ToList();
            produitsdemande.listesLaboratoire = db.DA_Labo.OrderBy(obj => obj.Laboratoire).ToList<DA_Labo>();

            string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection con = new SqlConnection(constr);
            con.Open();
            SqlDataAdapter da5 = new SqlDataAdapter("SELECT Pourcentage FROM DA_ProduitsDem where Réference = '" + Session["reff"].ToString() + "'", con);
            DataTable dt5 = new DataTable();
            da5.Fill(dt5);
            con.Close();
            float pourcentagedem = 0;

            for (int i = 0; i < dt5.Rows.Count; i++)
            {
                float pourcent = (float)Convert.ToDouble(dt5.Rows[i][0].ToString());
                pourcentagedem = pourcentagedem + pourcent;
            }
            ViewBag.pourcentagedem = pourcentagedem.ToString();
            ViewBag.pourcentagerestant = (100-pourcentagedem).ToString();
            return View(produitsdemande);
        }

        public ActionResult Produits_Dem(string id, string statut, string dem,string labor)
        {
            Session["reff"] = id;
            Session["statut"] = statut;
            Session["demandeure"] = dem;
            Session["labbb"] = labor;
            return RedirectToAction("Create");
        }


        // POST: DA_ProduitsDem/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DA_ProduitsDem dA_ProduitsDem)
        {
            if(dA_ProduitsDem.SelectedCodeArray != null)
            {
                dA_ProduitsDem.Code = string.Join(",", dA_ProduitsDem.SelectedCodeArray);
            }
            else
            {
                dA_ProduitsDem.Code = "";
            }
            
            if (ModelState.IsValid)
            {
                if (dA_ProduitsDem.Id == 0)
                {
                    string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
                    SqlConnection con = new SqlConnection(constr);
                    con.Open();
                    SqlDataAdapter da2 = new SqlDataAdapter("SELECT Budget FROM DA_Demande where Réference = '" + Session["reff"].ToString() + "'", con);
                    DataTable dt2 = new DataTable();
                    da2.Fill(dt2);
                    float budgett = (float)Convert.ToDouble(dt2.Rows[0][0]);
                    dA_ProduitsDem.Montant = (budgett * (dA_ProduitsDem.Pourcentage/100)).ToString();
                    
                    dA_ProduitsDem.Réference = Session["reff"].ToString();
                    db.DA_ProduitsDem.Add(dA_ProduitsDem);
                    Session["reff"] = dA_ProduitsDem.Réference;

                    SqlDataAdapter da5 = new SqlDataAdapter("SELECT Pourcentage FROM DA_ProduitsDem where Réference = '" + Session["reff"].ToString() + "'", con);
                    DataTable dt5 = new DataTable();
                    da5.Fill(dt5);
                    con.Close();
                    float pourcentagedem = 0;

                    for (int i = 0; i < dt5.Rows.Count; i++)
                    {
                        float pourcent = (float)Convert.ToDouble(dt5.Rows[i][0].ToString());
                        pourcentagedem = pourcentagedem + pourcent;
                    }
                    ViewBag.pourcentagedem = pourcentagedem.ToString();
                    ViewBag.pourcentagerestant = (100 - pourcentagedem).ToString();
                }
                else
                {
                    db.Entry(dA_ProduitsDem).State = EntityState.Modified;
                }
                db.SaveChanges();

                

            }
            return RedirectToAction("Create", new { Id = 0 });
            //return RedirectToAction("Index");
        }

        // GET: DA_ProduitsDem/Edit/5
        public ActionResult Edit(int? id)
        {
            DA_ProduitsDem produitsdemande = new DA_ProduitsDem();
            
            if (id != 0)
            {
                produitsdemande = db.DA_ProduitsDem.Where(x => x.Id == id).FirstOrDefault();
                produitsdemande.SelectedCodeArray = produitsdemande.Code.Split(',').ToArray();
            }

            string labbb = Session["labbb"].ToString();
            var listproduits = (from m in db.DA_Produits
                                where m.Laboratoire == labbb
                                orderby m.Désignation
                                select m);

            var listproduits1 = (from m in db.DA_Produits
                                 orderby m.Désignation
                                 select m);

            produitsdemande.ProduitsCollection = listproduits.ToList();
            produitsdemande.ProduitsCollection1 = listproduits1.ToList();

            produitsdemande.listesLaboratoire = db.DA_Labo.OrderBy(obj => obj.Laboratoire).ToList<DA_Labo>();

            return View(produitsdemande);
        }

        // POST: DA_ProduitsDem/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( DA_ProduitsDem dA_ProduitsDem)
        {
            if (dA_ProduitsDem.SelectedCodeArray != null)
            {
                dA_ProduitsDem.Code = string.Join(",", dA_ProduitsDem.SelectedCodeArray);
            }
            else
            {
                dA_ProduitsDem.Code = "";
            }
            

            if (ModelState.IsValid)
            {
                if (dA_ProduitsDem.Id == 0)
                {
                    db.DA_ProduitsDem.Add(dA_ProduitsDem);
                }
                else
                {
                    string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
                    SqlConnection con = new SqlConnection(constr);
                    con.Open();
                    SqlDataAdapter da2 = new SqlDataAdapter("SELECT Budget FROM DA_Demande where Réference = '" + Session["reff"].ToString() + "'", con);
                    DataTable dt2 = new DataTable();
                    da2.Fill(dt2);
                    float budgett = (float)Convert.ToDouble(dt2.Rows[0][0]);
                    dA_ProduitsDem.Montant = (budgett * (dA_ProduitsDem.Pourcentage / 100)).ToString();

                    SqlDataAdapter da5 = new SqlDataAdapter("SELECT Pourcentage FROM DA_ProduitsDem where Réference = '" + Session["reff"].ToString() + "'", con);
                    DataTable dt5 = new DataTable();
                    da5.Fill(dt5);
                    con.Close();
                    float pourcentagedem = 0;

                    for (int i = 0; i < dt5.Rows.Count; i++)
                    {
                        float pourcent = (float)Convert.ToDouble(dt5.Rows[i][0].ToString());
                        pourcentagedem = pourcentagedem + pourcent;
                    }
                    ViewBag.pourcentagedem = pourcentagedem.ToString();
                    ViewBag.pourcentagerestant = (100 - pourcentagedem).ToString();

                    db.Entry(dA_ProduitsDem).State = EntityState.Modified;
                }
                db.SaveChanges();
            }
            return RedirectToAction("Create", new { Id = 0 });
        }

        // GET: DA_ProduitsDem/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DA_ProduitsDem dA_ProduitsDem = db.DA_ProduitsDem.Find(id);
            if (dA_ProduitsDem == null)
            {
                return HttpNotFound();
            }
            return View(dA_ProduitsDem);
        }

        // POST: DA_ProduitsDem/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DA_ProduitsDem dA_ProduitsDem = db.DA_ProduitsDem.Find(id);
            db.DA_ProduitsDem.Remove(dA_ProduitsDem);
            db.SaveChanges();
            return RedirectToAction("Create");
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
