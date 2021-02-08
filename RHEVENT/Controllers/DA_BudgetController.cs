using System;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using RHEVENT.Models;

namespace RHEVENT.Controllers
{
    public class DA_BudgetController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        UserManager<IdentityUser> userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>());

        public ActionResult Réception(int? id, string Date_recep)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DA_Budget dA_Budget = db.DA_Budget.Find(id);
            //ViewBag.referencce = dA_Budget.Réference;
            if (dA_Budget == null)
            {
                return HttpNotFound();
            }
            return View(dA_Budget);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Réception(int id,string Date_recep)
        {
            DA_Budget dA_Budget = db.DA_Budget.Find(id);
            dA_Budget.Date_Recp = Date_recep.ToString();
            
            string datereception = "";
            datereception = Date_recep.ToString();
            string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection con = new SqlConnection(constr);

            con.Open();
            string url = "/DA_Budget/Réception/"+id;
            //SqlDataAdapter da7 = new SqlDataAdapter("SELECT Date_reception FROM DA_Demande where Réference ='" + Session["reff"].ToString() + "'", con);
            //DataTable dt7 = new DataTable();
            //da7.Fill(dt7);
            //DateTime daterecep =Convert.ToDateTime(dt7.Rows[0][0].ToString());
            DateTime daterecepligne = Convert.ToDateTime("2021-01-01");
            if (datereception == "")
            {
                //return RedirectToAction("demande", "DA_Demande");
                return Content("<script language='javascript' type='text/javascript'>alert('Date de reception non Valide ');window.location = '"+url+"';</script>");
            }
            else
            {
                daterecepligne = Convert.ToDateTime(datereception);

                //    if (daterecepligne > daterecep)
                //    {
                //        //return RedirectToAction("demande", "DA_Demande");
                //        return Content("<script language='javascript' type='text/javascript'>alert('La date de reception par ligne doit etre inferieure à celle de reception souhaitée pour la demande');window.location = '/DA_Demande/demande';</script>");
                //    }
                //    else
                //    {

                try
                {
                    db.Entry(dA_Budget).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var entityValidationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in entityValidationErrors.ValidationErrors)
                        {
                            Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                            Response.Redirect(validationError.ErrorMessage);

                        }
                    }
                }
               

                    string test = "1";
                    SqlDataAdapter da = new SqlDataAdapter("SELECT Date_Recp FROM DA_Budget where Réference ='" + Session["reff"].ToString() + "'", con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string date = dt.Rows[i][0].ToString();
                        if (date == "")
                        {
                            test = "0";
                        }
                    }
                    //SqlCommand cmd = new SqlCommand("update DA_Budget set Date_Recp = '" + datereception + "' where  Id='" + id + "'", con);
                    //cmd.ExecuteNonQuery();
                    if (test == "0")
                    {
                        SqlCommand cmd1 = new SqlCommand("update DA_Demande set Etat = '77' where  Réference='" + dA_Budget.Réference + "'", con);
                        cmd1.ExecuteNonQuery();
                    }
                    else if (test == "1")
                    {
                        SqlCommand cmd11 = new SqlCommand("update DA_Demande set Etat = '88' where  Réference='" + dA_Budget.Réference + "'", con);
                        cmd11.ExecuteNonQuery();

                    }
                    con.Close();

                    return RedirectToAction("demande", "DA_Demande");
                //}
            }

        }
        // GET: DA_Budget
        public ActionResult Index()
        {
            string reference = Session["reff"].ToString();
            var list = (from m in db.DA_Budget
                        where m.Réference == reference
                        orderby m.Fournisseur
                        select m);

            DA_Budget budgett = new DA_Budget();
            budgett.listesbudget = list.ToList();

            return View(budgett);
            //return View(db.<c.ToList());
        }
        public ActionResult Budget_Dem(string id, string statut, string dem)
        {
            Session["reff"] = id;
            Session["statut"] = statut;
            Session["demandeure"] = dem;
            return RedirectToAction("Create");
        }
       

        // GET: DA_Budget/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DA_Budget dA_Budget = db.DA_Budget.Find(id);
            if (dA_Budget == null)
            {
                return HttpNotFound();
            }
            return View(dA_Budget);
        }

        // GET: DA_Budget/Create
        public ActionResult Create()
        {
            //Session["checkboxx"] = "0";
            string reference = Session["reff"].ToString();
            if (reference != null)
            {
                var list = (from m in db.DA_Budget
                            where m.Réference == reference
                            orderby m.Date_Recp_Souh
                            select m);

                var listmateriels = (from m in db.DA_Materiels
                                     where m.Type.Equals("Matériels")
                                     orderby m.Désignation
                            select m);

                var listservices = (from m in db.DA_Materiels
                                    where m.Type.Equals("Services")
                                    orderby m.Désignation
                                    select m);

                DA_Budget budgett = new DA_Budget();

                budgett.listesfournisseurs = db.DA_Fournisseurs.OrderBy(obj => obj.Raison).ToList<DA_Fournisseurs>();
                budgett.listesbudget = list.ToList();
                budgett.listeMateriels = listmateriels.ToList();
                budgett.listeServices = listservices.ToList();

                return View(budgett);
            }
            else
            {
                return RedirectToAction("MesDemandes", "DA_Demande");
            }
        }

        // POST: DA_Budget/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Réference,Article,Quantité,PrixUnitaire,Total,Fournisseur,Date_Recp_Souh,Description,Type,PlafondBudget")] DA_Budget dA_Budget,string typeArticle)
        {
            dA_Budget.Type = typeArticle;
            dA_Budget.Réference = Session["reff"].ToString();
            dA_Budget.Total = (float)(dA_Budget.PrixUnitaire * dA_Budget.Quantité);


            string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection con = new SqlConnection(constr);

            
            con.Open();
            SqlDataAdapter da7 = new SqlDataAdapter("SELECT Date_reception FROM DA_Demande where Réference ='" + Session["reff"].ToString() + "'", con);
            DataTable dt7 = new DataTable();
            da7.Fill(dt7);

            SqlDataAdapter da71 = new SqlDataAdapter("SELECT PlafondBudget FROM DA_Materiels where Désignation  ='" + dA_Budget.Article + "'", con);
            DataTable dt71 = new DataTable();
            da71.Fill(dt71);
            string budgetplaf = " ";

            if(dt71.Rows.Count > 0)
            {
                budgetplaf = dt71.Rows[0][0].ToString();
            }
            dA_Budget.PlafondBudget = budgetplaf;

            con.Close();
            DateTime daterecep = Convert.ToDateTime(dt7.Rows[0][0].ToString());
            DateTime daterecepligne = Convert.ToDateTime("2021-01-01");
            string datereception = "";
            datereception = dA_Budget.Date_Recp_Souh.ToString();
            
                daterecepligne = Convert.ToDateTime(datereception);


            //if (daterecepligne > daterecep)
            //{
            //    //ModelState.AddModelError("", "La date de reception par ligne doit etre inferieure à celle de reception souhaitée pour la demande");
            //    //return RedirectToAction("demande", "DA_Demande");
            //    return Content("<script language='javascript' type='text/javascript'>alert('La date de reception par ligne doit etre inferieure à celle de reception souhaitée pour la demande');window.location = '/DA_Budget/Create';</script>");
            //}
            //else
            if (daterecepligne <= daterecep)
            {

            var errors = ModelState.Values.SelectMany(v => v.Errors);

                    if (ModelState.IsValid)
                    {
                        db.DA_Budget.Add(dA_Budget);
                        db.SaveChanges();
                        Session["reff"] = dA_Budget.Réference;
                        //return RedirectToAction("Index");

                        
                        con.Open();
                        SqlDataAdapter da = new SqlDataAdapter("SELECT SUM(Total) FROM [RH_MEDICIS].[dbo].[DA_Budget] where Réference ='" + Session["reff"].ToString() + "'", con);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        float budgett = (float)(Convert.ToDouble(dt.Rows[0][0]));

                        SqlCommand cmd = new SqlCommand("update DA_Demande set Budget = " + budgett + " where  Réference='" + Session["reff"].ToString() + "'", con);
                        cmd.ExecuteNonQuery();

                        SqlDataAdapter da2 = new SqlDataAdapter("SELECT Id,Pourcentage FROM DA_ProduitsDem where Réference = '" + Session["reff"].ToString() + "'", con);
                        DataTable dt2 = new DataTable();
                        da2.Fill(dt2);
                        for (int i = 0; i < dt2.Rows.Count; i++)
                        {
                            int idd = Convert.ToInt32(dt2.Rows[i][0]);
                            int pourcent = Convert.ToInt32(dt2.Rows[i][1]);
                            float montant = budgett * (float)(pourcent) / 100;
                            double mtn = Convert.ToDouble(montant.ToString());
                            montant = (float)(Math.Round(mtn, 3));
                            SqlCommand cmd1 = new SqlCommand("update DA_ProduitsDem set Montant = " + montant + " where  Id='" + idd + "'", con);
                            cmd1.ExecuteNonQuery();

                        }

                        con.Close();
                        return RedirectToAction("Create", "DA_Budget");
                    }
                }


            string reference = Session["reff"].ToString();
            var list = (from m in db.DA_Budget
                        where m.Réference == reference
                        orderby m.Fournisseur
                        select m);

            var listmateriels = (from m in db.DA_Materiels
                                 where m.Type.Equals("Matériels")
                                 orderby m.Désignation
                                 select m);

            var listservices = (from m in db.DA_Materiels
                                where m.Type.Equals("Services")
                                orderby m.Désignation
                                select m);



            dA_Budget.listesfournisseurs = db.DA_Fournisseurs.OrderBy(obj => obj.Raison).ToList<DA_Fournisseurs>();
            dA_Budget.listesbudget = list.ToList();
            dA_Budget.listeMateriels = listmateriels.ToList();
            dA_Budget.listeServices = listservices.ToList();
            return View(dA_Budget);
        }

        // GET: DA_Budget/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            string reference = Session["reff"].ToString();
            
            var listmateriels = (from m in db.DA_Materiels
                                 where m.Type.Equals("Matériels")
                                 orderby m.Désignation
                                 select m);

            var listservices = (from m in db.DA_Materiels
                                where m.Type.Equals("Services")
                                orderby m.Désignation
                                select m);

            DA_Budget dA_Budget = db.DA_Budget.Find(id);
            dA_Budget.listesfournisseurs = db.DA_Fournisseurs.OrderBy(obj => obj.Raison).ToList<DA_Fournisseurs>();
            dA_Budget.listeMateriels = listmateriels.ToList();
            dA_Budget.listeServices = listservices.ToList();
           
            if (dA_Budget == null)
            {
                return HttpNotFound();
            }
            return View(dA_Budget);
        }

        // POST: DA_Budget/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Réference,Article,Quantité,PrixUnitaire,Total,Fournisseur,Date_Recp_Souh,Description,Type")] DA_Budget dA_Budget)
        {
            if (ModelState.IsValid)
            {
                dA_Budget.Total = (float)(dA_Budget.PrixUnitaire * dA_Budget.Quantité);
                db.Entry(dA_Budget).State = EntityState.Modified;
                db.SaveChanges();

                string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
                SqlConnection con = new SqlConnection(constr);

                con.Open();
                SqlDataAdapter da = new SqlDataAdapter("SELECT SUM(Total) FROM [RH_MEDICIS].[dbo].[DA_Budget] where Réference ='" + Session["reff"].ToString() + "'", con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                float budgett = (float)(Convert.ToDouble(dt.Rows[0][0]));

                SqlCommand cmd = new SqlCommand("update DA_Demande set Budget = " + budgett + " where  Réference='" + Session["reff"].ToString() + "'", con);
                cmd.ExecuteNonQuery();

                SqlDataAdapter da2 = new SqlDataAdapter("SELECT Id,Pourcentage FROM DA_ProduitsDem where Réference = '" + Session["reff"].ToString() + "'", con);
                DataTable dt2 = new DataTable();
                da2.Fill(dt2);
                for (int i = 0; i < dt2.Rows.Count; i++)
                {
                    int idd = Convert.ToInt32(dt2.Rows[i][0]);
                    int pourcent = Convert.ToInt32(dt2.Rows[i][1]);
                    float montant = budgett * (float)(pourcent) / 100;
                    double mtn = Convert.ToDouble(montant.ToString());
                    montant = (float)(Math.Round(mtn, 3));
                    SqlCommand cmd1 = new SqlCommand("update DA_ProduitsDem set Montant = " + montant + " where  Id='" + idd + "'", con);
                    cmd1.ExecuteNonQuery();
                }
                con.Close();
                return RedirectToAction("Create", "DA_Budget");
            }
            return View(dA_Budget);
        }

        // GET: DA_Budget/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DA_Budget dA_Budget = db.DA_Budget.Find(id);
            //ViewBag.referencce = dA_Budget.Réference;
            if (dA_Budget == null)
            {
                return HttpNotFound();
            }
            return View(dA_Budget);
        }

        // POST: DA_Budget/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DA_Budget dA_Budget = db.DA_Budget.Find(id);
            db.DA_Budget.Remove(dA_Budget);
            db.SaveChanges();


            string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection con = new SqlConnection(constr);

            con.Open();
            SqlDataAdapter da = new SqlDataAdapter("SELECT SUM(Total) FROM [RH_MEDICIS].[dbo].[DA_Budget] where Réference ='" + dA_Budget.Réference + "'", con);
            DataTable dt = new DataTable();
            da.Fill(dt);

            float budgett = (float)(Convert.ToDouble(dt.Rows[0][0]));

            SqlCommand cmd = new SqlCommand("update DA_Demande set Budget = " + budgett + " where  Réference='" + dA_Budget.Réference + "'", con);
            cmd.ExecuteNonQuery();

            SqlDataAdapter da2 = new SqlDataAdapter("SELECT Id,Pourcentage FROM DA_ProduitsDem where Réference = '" + Session["reff"].ToString() + "'", con);
            DataTable dt2 = new DataTable();
            da2.Fill(dt2);
            for (int i = 0; i < dt2.Rows.Count; i++)
            {
                int idd = Convert.ToInt32(dt2.Rows[i][0]);
                int pourcent = Convert.ToInt32(dt2.Rows[i][1]);
                float montant = budgett * (float)(pourcent) / 100;
                double mtn = Convert.ToDouble(montant.ToString());
                montant = (float)(Math.Round(mtn, 3));
                SqlCommand cmd1 = new SqlCommand("update DA_ProduitsDem set Montant = " + montant + " where  Id='" + idd + "'", con);
                cmd1.ExecuteNonQuery();
            }
            con.Close();

            return RedirectToAction("Create", "DA_Budget");
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
