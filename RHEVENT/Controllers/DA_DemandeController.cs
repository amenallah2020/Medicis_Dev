using System;
using System.Collections.Generic;
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
    public class DA_DemandeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        UserManager<IdentityUser> userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>());

        public ActionResult DemandesAll()
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            string matriculee = user.matricule;

            var list = (from m in db.DA_Demande
                        join n in db.DA_DemUsersTraitees
                        on m.Réference equals n.Reference
                        where n.Matricule == matriculee
                        orderby m.Date_action descending
                        select m);


            DA_Demande dem = new DA_Demande();
            dem.listesDemandes = list.ToList();

            return View(dem);

            
        }

        // GET: DA_Demande

        public ActionResult demande_dem(string id, string statut, string dem,string labor)
        {
            Session["reff"] = id;
            Session["statut"] = statut;
            Session["demandeure"] = dem;
            Session["labbb"] = labor;
            return RedirectToAction("demande");
        }

        public ActionResult demande_dem1(string id,int rowid)
        {
            Session["reff"] = id;
            Session["idd"] = rowid;
            return RedirectToAction("demandeTraitee");
        }

        public ActionResult demandeTraitee()
        {

            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            string demandeurr = user.nom + " " + user.prenom;
            ViewBag.demandeurr = demandeurr;
            Session["userconnecté"] = demandeurr;

            string reference = Session["reff"].ToString();
            if (Session["reff"] != null)
            {


                var list1 = (from m in db.DA_Budget
                             where m.Réference == reference
                             orderby m.Fournisseur
                             select m);
                var list2 = (from m in db.DA_ProduitsDem
                             where m.Réference == reference
                             orderby m.Laboratoire
                             select m);


                DA_Demandee demandee = new DA_Demandee();

                DA_Demande dem = db.DA_Demande.Find(Session["idd"]);


                demandee.listesbudget = list1.ToList();
                demandee.listeproduits = list2.ToList();

                demandee.dadem = dem;

                return View(demandee);
            }
            return RedirectToAction("DemandesAll");
        }
        


        public ActionResult demande(/*string id*/)
        {
            string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection con = new SqlConnection(constr);
            con.Open();
            SqlDataAdapter da5 = new SqlDataAdapter("SELECT Pourcentage FROM DA_ProduitsDem where Réference = '" + Session["reff"].ToString() + "'", con);
            DataTable dt5 = new DataTable();
            da5.Fill(dt5);

            SqlDataAdapter da51 = new SqlDataAdapter("SELECT count(Id) FROM DA_Budget where Réference = '" + Session["reff"].ToString() + "'", con);
            DataTable dt51 = new DataTable();
            da51.Fill(dt51);

            con.Close();
            float pourcentagedem = 0;
            int nbrArticle = Convert.ToInt32(dt51.Rows[0][0].ToString());

            for (int i = 0; i < dt5.Rows.Count; i++)
            {
                float pourcent =(float) Convert.ToDouble(dt5.Rows[i][0].ToString());
                pourcentagedem = pourcentagedem + pourcent;
            }
            ViewBag.pourcentagedem = pourcentagedem.ToString();

            if (nbrArticle == 0)
            {
                return Content("<script language='javascript' type='text/javascript'>alert('Veuillez Ajouter au moins un article à la demande');window.location = '/DA_Budget/Create';</script>");
            }
            else if(pourcentagedem != 100)
            {
                return Content("<script language='javascript' type='text/javascript'>alert('Veuillez Verifier les pourcentages');window.location = '/DA_ProduitsDem/Create';</script>");
            }
            else
            {
                ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
                string demandeurr = user.nom + " " + user.prenom;
                ViewBag.demandeurr = demandeurr;
                Session["userconnecté"] = demandeurr;

                string reference = Session["reff"].ToString();
                if (reference != null)
                {
                    var list = (from m in db.DA_Materiels_Dem
                                where m.Réference == reference
                                orderby m.Désignation
                                select m);

                    var list1 = (from m in db.DA_Budget
                                 where m.Réference == reference
                                 orderby m.Fournisseur
                                 select m);
                    var list2 = (from m in db.DA_ProduitsDem
                                 where m.Réference == reference
                                 orderby m.Laboratoire
                                 select m);

                    var listemotiffs = (from m in db.MotifsRejets
                                        orderby m.MotifRejet
                                        select m);

                    DA_Demandee demandee = new DA_Demandee();

                    con.Open();

                    SqlDataAdapter da7 = new SqlDataAdapter("SELECT Date_reception FROM DA_Demande where Réference ='" + Session["reff"].ToString() + "'", con);
                    DataTable dt7 = new DataTable();
                    da7.Fill(dt7);
                    con.Close();
                    DateTime daterecep = Convert.ToDateTime(dt7.Rows[0][0].ToString());
                    Session["daterecepdem"] = daterecep.ToString();
                    


                        SqlDataAdapter da = new SqlDataAdapter("SELECT Id,TypeAchat FROM DA_Demande where Réference = '" + reference + "'", con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    int idd = Convert.ToInt32(dt.Rows[0][0].ToString());
                    string typeachat = dt.Rows[0][1].ToString();


                    string fonctionn = user.fonction;
                    SqlDataAdapter da1 = new SqlDataAdapter("SELECT Num FROM DA_WorkflowTypAch where Id_type =(SELECT Id FROM DA_TypesAchats where TypeAchat ='" + typeachat + "') and Intervenant='" + fonctionn + "' ", con);
                    DataTable dt1 = new DataTable();
                    da1.Fill(dt1);
                    if (dt1.Rows.Count > 0)
                    {
                        int numer = Convert.ToInt32(dt1.Rows[0][0].ToString());
                        ViewBag.numero = numer;
                    }

                    else
                    {
                        ViewBag.numero = 999;
                    }


                    con.Close();


                    DA_Demande dem = db.DA_Demande.Find(idd);

                    demandee.listemateriels = list.ToList();
                    demandee.listesbudget = list1.ToList();
                    demandee.listeproduits = list2.ToList();
                    demandee.listeMotifs = listemotiffs.ToList();
                    demandee.listefournisseurs = db.DA_Fournisseurs.OrderBy(obj => obj.Raison).ToList<DA_Fournisseurs>();
                    demandee.listelabo = db.DA_Labo.OrderBy(obj => obj.Laboratoire).ToList<DA_Labo>();
                    demandee.dadem = dem;

                    return View(demandee);
                }
                return View();
            }
        }

            public ActionResult MesDemandes()
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            string demandeurr = user.nom + " " + user.prenom;
            ViewBag.demandeurr = demandeurr;
            Session["userconnecté"] = demandeurr;

            var list = (from m in db.DA_Demande
                       where m.Demandeur == demandeurr
                        orderby m.Date_demande descending
                        select m)/*.Take(20)*/;

            DA_Demande dem = new DA_Demande();

            dem.listesLabo = db.DA_Labo.ToList<DA_Labo>();
            dem.listesachats = db.DA_TypesAchats.ToList<DA_TypesAchats>();
            dem.listesactions = db.DA_TypesActions.ToList<DA_TypesActions>();
            dem.listesDemandes = list.ToList();
            dem.Date_demande = DateTime.Now;

            //dem.TypeAchat = dem.listesachats[0].TypeAchat.ToString();
            ViewData.Model = dem;

            string fonctionn = user.fonction;
            string matriculee = user.matricule;

            string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection con = new SqlConnection(constr);
            con.Open();
            SqlDataAdapter da55 = new SqlDataAdapter("SELECT count(Id) FROM DA_Demande where (Validee = '0') and ((etat_prochain = '" + fonctionn + "') or (Statut = '0' and matrsign = '" + matriculee + "'))", con);
            SqlDataAdapter da555 = new SqlDataAdapter("SELECT count(Id) FROM DA_Demande where (Validee = '0') and (Etat = '-1') and (Demandeur = '" + demandeurr + "')", con);

            DataTable dt55 = new DataTable();
            da55.Fill(dt55);
            DataTable dt555 = new DataTable();
            da555.Fill(dt555);
            con.Close();
            Session["nb"] = dt55.Rows[0][0].ToString();
            Session["nb1"] = dt555.Rows[0][0].ToString();
            return View(dem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MesDemandes(DA_Demande dA_Demande)
        {
            string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection con = new SqlConnection(constr);
            con.Open();

            string dayy = DateTime.Now.Day.ToString();
            string monthh = DateTime.Now.Month.ToString();
            string yearr = DateTime.Now.Year.ToString();
            if (dayy.Length == 1) { dayy = "0" + dayy; }
            if (monthh.Length == 1) { monthh = "0" + monthh; }
            string derniere_ref = "DA-" + yearr + "-" + monthh + "-" + dayy + "-";

            SqlDataAdapter da = new SqlDataAdapter("SELECT top(1) Réference FROM DA_Demande where Réference like '" + derniere_ref + "%' order by Réference desc ", con);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                string reff = Convert.ToString(dt.Rows[0][0]);
                reff = reff.Substring(14, 2);
                string index = (Convert.ToInt32(reff) + 1).ToString();
                if (index.Length == 1) { index = "0" + index; }
                derniere_ref = derniere_ref + index;
            }
            else
            {
                derniere_ref = derniere_ref + "01";
            }

            if (ModelState.IsValid)
            {
              
                ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
                dA_Demande.Réference = derniere_ref;
                dA_Demande.Demandeur = user.nom + " " + user.prenom;
                
                dA_Demande.Etat = "-1";
                dA_Demande.matrsign = user.signataire;

                db.DA_Demande.Add(dA_Demande);
                db.SaveChanges();
             
                return RedirectToAction("Create", "DA_Budget");
            }

            return View(dA_Demande);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateMat(string codee, string designationn, string versionn, string fournisseurr,DateTime datesouh)
        {
            if (ModelState.IsValid)
            {

                //dA_Materiels_Dem.Réference = Session["reff"].ToString();
                //dA_Materiels_Dem.Code = codee;
                //dA_Materiels_Dem.Désignation = designationn;
                //dA_Materiels_Dem.Version = versionn;
                //dA_Materiels_Dem.Fournisseur = fournisseurr;

                //db.DA_Materiels_Dem.Add(dA_Materiels_Dem);
                //db.SaveChanges();

                string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
                SqlConnection con = new SqlConnection(constr);
                con.Open();
                SqlCommand cmd = new SqlCommand("Insert into DA_Materiels_Dem (Réference,Code,Désignation,Version,Fournisseur,Date_Recp_Souh) " +
                    "values(@Réference,@Code,@Désignation,@Version,@Fournisseur,@Date_Recp_Souh)", con);

                cmd.Parameters.AddWithValue("@Réference", Session["reff"].ToString());
                cmd.Parameters.AddWithValue("@Code", codee);
                cmd.Parameters.AddWithValue("@Désignation", designationn);
                cmd.Parameters.AddWithValue("@Version", versionn);
                cmd.Parameters.AddWithValue("@Fournisseur", fournisseurr);
                cmd.Parameters.AddWithValue("@Date_Recp_Souh", datesouh);

                cmd.ExecuteNonQuery();
                con.Close();
                return RedirectToAction("demande");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateBudget(string Description, string PrixUnitaire, string Quantité, string Fournisseur,DA_Budget dA_Budget)
        {
            if (ModelState.IsValid)
            {
                
                float Total = (float)(Convert.ToDouble(PrixUnitaire) * Convert.ToDouble(Quantité));
                float PrixUnitaire1 = (float)Convert.ToDouble(PrixUnitaire);
                int Quantité1 = Convert.ToInt32(Quantité);

                dA_Budget.Réference = Session["reff"].ToString();
                dA_Budget.Description = Description;
                dA_Budget.PrixUnitaire = PrixUnitaire1;
                dA_Budget.Quantité = Quantité1;
                dA_Budget.Total = Total;
                dA_Budget.Fournisseur = Fournisseur;

                db.DA_Budget.Add(dA_Budget);
                db.SaveChanges();

                string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
                SqlConnection con = new SqlConnection(constr);

                con.Open();
                SqlDataAdapter da = new SqlDataAdapter("SELECT SUM(Total) FROM DA_Budget where Réference ='" + Session["reff"].ToString() + "'", con);
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

                //string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
                //SqlConnection con = new SqlConnection(constr);
                //con.Open();
                //SqlCommand cmd = new SqlCommand("Insert into DA_Budget (Réference,Description,PrixUnitaire,Quantité,Total,Fournisseur) " +
                //    "values(@Réference,@Description,@PrixUnitaire,@Quantité,@Total,@Fournisseur)", con);

                //cmd.Parameters.AddWithValue("@Réference", Session["reff"].ToString());
                //cmd.Parameters.AddWithValue("@Description", Description);
                //cmd.Parameters.AddWithValue("@PrixUnitaire", PrixUnitaire);
                //cmd.Parameters.AddWithValue("@Quantité", Quantité);
                //cmd.Parameters.AddWithValue("@Total", Total);
                //cmd.Parameters.AddWithValue("@Fournisseur", Fournisseur);

                //cmd.ExecuteNonQuery();
                //con.Close();
                return RedirectToAction("demande");
            }
            return View();
        }
        public ActionResult DemendesEnAttente()
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            string fonctionn = user.fonction;
            string matriculee = user.matricule;
            string demandeurr = user.nom + " " + user.prenom;
            ViewBag.demandeurr = demandeurr;
            Session["userconnecté"] = demandeurr;

            var list = (from m in db.DA_Demande
                        where (m.Validee == "0") && 
                        ((m.etat_prochain == fonctionn) || ((m.Statut == "0") && (m.matrsign == matriculee)))
                        orderby m.Date_demande descending
                        select m)/*.Take(20)*/;
           

            DA_Demande dem = new DA_Demande();

            dem.listesLabo = db.DA_Labo.ToList<DA_Labo>();
            dem.listesachats = db.DA_TypesAchats.ToList<DA_TypesAchats>();
            dem.listesactions = db.DA_TypesActions.ToList<DA_TypesActions>();
            dem.listesDemandes = list.ToList();
            dem.Date_demande = DateTime.Now;
            ViewData.Model = dem;

            
           

            string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection con = new SqlConnection(constr);
            con.Open();
            
            SqlDataAdapter da55 = new SqlDataAdapter("SELECT count(Id) FROM DA_Demande where (Validee = '0') and ((etat_prochain = '" + fonctionn + "') or (Statut = '0' and matrsign = '" + matriculee + "'))", con);
            SqlDataAdapter da555 = new SqlDataAdapter("SELECT count(Id) FROM DA_Demande where (Validee = '0') and (Etat = '-1') and (Demandeur = '" + demandeurr + "')", con);

            DataTable dt55 = new DataTable();
            da55.Fill(dt55);
            DataTable dt555 = new DataTable();
            da555.Fill(dt555);
            con.Close();
            Session["nb"] = dt55.Rows[0][0].ToString();
            Session["nb1"] = dt555.Rows[0][0].ToString();
            return View(dem);
        }

        public ActionResult DemendesEnAttente1()
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            string fonctionn = user.fonction;
            string matriculee = user.matricule;
            string demandeurr = user.nom + " " + user.prenom;
            ViewBag.demandeurr = demandeurr;

            var list = (from m in db.DA_Demande
                        where ((m.Validee == "0") && (m.matrsign == matriculee) && (m.Statut == "0"))
                        orderby m.Date_demande descending
                        select m)/*.Take(20)*/;

            DA_Demande dem = new DA_Demande();

            dem.listesLabo = db.DA_Labo.ToList<DA_Labo>();
            dem.listesachats = db.DA_TypesAchats.ToList<DA_TypesAchats>();
            dem.listesactions = db.DA_TypesActions.ToList<DA_TypesActions>();
            dem.listesDemandes = list.ToList();
            dem.Date_demande = DateTime.Now;
            ViewData.Model = dem;
            return View(dem);
        }

        // GET: DA_Demande/Details/5 
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DA_Demande dA_Demande = db.DA_Demande.Find(id);
            if (dA_Demande == null)
            {
                return HttpNotFound();
            }
            return View(dA_Demande);
        }

        // GET: DA_Demande/Create
        public ActionResult Create()
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            string demandeurr = user.nom + " " + user.prenom;
            ViewBag.demandeurr = demandeurr;
            Session["userconnecté"] = demandeurr;

            var listlabs = (from m in db.DA_LaboUser
                         where m.Matricule ==user.matricule && m.Etat==1
                         orderby m.Laboratoire
                         select m);


            DA_Demande dem = new DA_Demande();
            //dem.listesLabo = db.DA_Labo.OrderBy(obj => obj.Laboratoire).ToList<DA_Labo>();
            //dem.listesLabo1 = listlabs.ToList();
            dem.listesachats = db.DA_TypesAchats.OrderBy(obj => obj.TypeAchat).ToList<DA_TypesAchats>();
            dem.listesactions = db.DA_TypesActions.OrderBy(obj => obj.TypeAction).ToList<DA_TypesActions>();
            dem.Date_demande = DateTime.Now;
            //ViewData.Model = dem;

            var list1 = (from m in db.DA_TypesAchats
                         orderby m.TypeAchat
                         select m);
            ViewBag.liste1 = list1.ToList();
            ViewBag.listlabs = listlabs.ToList();

            string fonctionn = user.fonction;
            string matriculee = user.matricule;


            string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection con = new SqlConnection(constr);
            con.Open();
            SqlDataAdapter da55 = new SqlDataAdapter("SELECT count(Id) FROM DA_Demande where (Validee = '0') and ((etat_prochain = '" + fonctionn + "') or (Statut = '0' and matrsign = '" + matriculee + "'))", con);
            SqlDataAdapter da555 = new SqlDataAdapter("SELECT count(Id) FROM DA_Demande where (Validee = '0') and (Etat = '-1') and (Demandeur = '"+demandeurr+"')", con);

            DataTable dt55 = new DataTable();
            da55.Fill(dt55);
            DataTable dt555 = new DataTable();
            da555.Fill(dt555);
            con.Close();
            Session["nb"] = dt55.Rows[0][0].ToString();
            Session["nb1"] = dt555.Rows[0][0].ToString();
            return View(dem);

        }




        public JsonResult getActionByAchat(int ID)
        {

            DA_TypesAchats achat = (from m in db.DA_TypesAchats
                                    where m.Id == ID
                                    select m).Single();


            var action = from m in db.DA_TypesActions
                         where m.TypeActhat == achat.TypeAchat
                         select m;

            List<DA_TypesActions> list = action.ToList();


            db.Configuration.ProxyCreationEnabled = false;

            //return Json(new SelectList(list, "Id", "TypeAction") , JsonRequestBehavior.AllowGet);

            return Json(list, JsonRequestBehavior.AllowGet);
        }


        // POST: DA_Demande/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DA_Demande dA_Demande , string id /*,DateTime Date_reception, DateTime Date_action*/)
        {
            //if (Date_action < Date_reception)
            //{
            //    return View("Errordatedemande");
            //    //return Content("<script language='javascript' type='text/javascript'>alert('La date d'action doit etre superieure à celle de reception');window.location = '/DA_Demande/Create';</script>");
            //}
            //else


            

            if (dA_Demande.Date_action > dA_Demande.Date_reception)
            {
                string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
                SqlConnection con = new SqlConnection(constr);
                con.Open();
                SqlDataAdapter da2 = new SqlDataAdapter("SELECT Code FROM DA_TypesAchats where TypeAchat = '" + dA_Demande.TypeAchat + "'", con);
                DataTable dt2 = new DataTable();
                da2.Fill(dt2);
                string typpe = Convert.ToString(dt2.Rows[0][0]);

                SqlDataAdapter da1 = new SqlDataAdapter("SELECT Code FROM DA_Labo where Laboratoire = '" + dA_Demande.Labo + "'", con);
                DataTable dt1 = new DataTable();
                da1.Fill(dt1);
                string labbo = Convert.ToString(dt1.Rows[0][0]);

                DateTime datt = Convert.ToDateTime(dA_Demande.Date_reception);
                string dayy1 = datt.Day.ToString();
                string monthh1 = datt.Month.ToString();
                string yearr1 = datt.Year.ToString();
                if (dayy1.Length == 1) { dayy1 = "0" + dayy1; }
                if (monthh1.Length == 1) { monthh1 = "0" + monthh1; }
                string derniere_ref1 = yearr1 + "-" + monthh1 + "-" + dayy1 + "-";
                string derniere_ref = labbo + "-" + typpe + "-" + derniere_ref1;

                string dayy = DateTime.Now.Day.ToString();
                string monthh = DateTime.Now.Month.ToString();
                string yearr = DateTime.Now.Year.ToString();
                if (dayy.Length == 1) { dayy = "0" + dayy; }
                if (monthh.Length == 1) { monthh = "0" + monthh; }


                SqlDataAdapter da = new SqlDataAdapter("SELECT top(1) Réference FROM DA_Demande where Réference like '" + derniere_ref + "%' order by Réference desc ", con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                con.Close();
                if (dt.Rows.Count > 0)
                {
                    string reff = Convert.ToString(dt.Rows[0][0]);
                    reff = reff.Substring(19, 2);
                    string index = (Convert.ToInt32(reff) + 1).ToString();
                    if (index.Length == 1) { index = "0" + index; }
                    derniere_ref = derniere_ref + index;
                }
                else
                {
                    derniere_ref = derniere_ref + "01";
                }

                if (ModelState.IsValid)
                {

                    ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
                    dA_Demande.Réference = derniere_ref;
                    dA_Demande.Demandeur = user.nom + " " + user.prenom;
                    dA_Demande.Etat = "-1";
                    dA_Demande.Validee = "0";
                    dA_Demande.Statut = "-1";
                    dA_Demande.etat_prochain = "0";
                    dA_Demande.Date_demande = DateTime.Now.Date;
                    dA_Demande.matrsign = user.signataire;

                    //dA_Demande.Date_reception = Date_reception;
                    //dA_Demande.Date_action = Date_action;
                   
                    db.DA_Demande.Add(dA_Demande);
                    try
                    {
                        db.SaveChanges();
                        Session["reff"] = derniere_ref;
                        Session["labbb"] = dA_Demande.Labo;
                        Session["statut"] = "-1";
                        Session["demandeure"] = user.nom + " " + user.prenom;
                        Session["userconnecté"] = user.nom + " " + user.prenom;
                        return RedirectToAction("Create", "DA_Budget");
                    }
                    catch (DbEntityValidationException ex)
                    {
                        foreach (var entityValidationErrors in ex.EntityValidationErrors)
                        {
                            foreach (var validationError in entityValidationErrors.ValidationErrors)
                            {
                                Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                            }
                        }
                    }

                }
               
            }
            dA_Demande.listesLabo = db.DA_Labo.OrderBy(obj => obj.Laboratoire).ToList<DA_Labo>();
            dA_Demande.listesachats = db.DA_TypesAchats.OrderBy(obj => obj.TypeAchat).ToList<DA_TypesAchats>();
            dA_Demande.listesactions = db.DA_TypesActions.OrderBy(obj => obj.TypeAction).ToList<DA_TypesActions>();
            dA_Demande.Date_demande = DateTime.Now;
            
            var list1 = (from m in db.DA_TypesAchats
                         orderby m.TypeAchat
                         select m);
            ViewBag.liste1 = list1.ToList();

            ApplicationUser user1 = db.Users.Find(User.Identity.GetUserId());
            var listlabs = (from m in db.DA_LaboUser
                            where m.Matricule == user1.matricule && m.Etat == 1
                            orderby m.Laboratoire
                            select m);
            ViewBag.listlabs = listlabs.ToList();

            return View(dA_Demande);
            
        }

        // GET: DA_Demande/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DA_Demande dem = db.DA_Demande.Find(id);

            dem.listesLabo = db.DA_Labo.OrderBy(obj => obj.Laboratoire).ToList<DA_Labo>();
            dem.listesachats = db.DA_TypesAchats.OrderBy(obj => obj.TypeAchat).ToList<DA_TypesAchats>();
            dem.listesactions = db.DA_TypesActions.OrderBy(obj => obj.TypeAction).ToList<DA_TypesActions>();
            dem.Date_demande = DateTime.Now;
            
            if (dem == null)
            {
                return HttpNotFound();
            }
            
            return View(dem);
        }

        // POST: DA_Demande/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Réference,Demandeur,Gamme,Objet,Cibles,AvecSans,Argumentaires,Budget,Date_demande,Date_reception,Date_action")] DA_Demande dA_Demande)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dA_Demande).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("MesDemandes");
            }
            return View(dA_Demande);
        }

        // GET: DA_Demande/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DA_Demande dA_Demande = db.DA_Demande.Find(id);
            if (dA_Demande == null)
            {
                return HttpNotFound();
            }
            return View(dA_Demande);
        }

        // POST: DA_Demande/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DA_Demande dA_Demande = db.DA_Demande.Find(id);
            db.DA_Demande.Remove(dA_Demande);
            db.SaveChanges();
            return RedirectToAction("MesDemandes");
        }



        [HttpPost]
        public ActionResult ValiderDemande()
        {
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
            if (pourcentagedem != 100)
            {
                return Content("<script language='javascript' type='text/javascript'>alert('Veuillez Verifier les pourcentages');window.location = '/DA_ProduitsDem/Create';</script>");

                //return RedirectToAction("Create", "DA_ProduitsDem");
            }
            else
            {
                ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
                DA_DemUsersTraitees demusertrait = new DA_DemUsersTraitees();
                demusertrait.Matricule = user.matricule;
                demusertrait.Reference = Session["reff"].ToString();

                con.Open();
                SqlDataAdapter da = new SqlDataAdapter("SELECT Statut,TypeAchat,Validee FROM DA_Demande where Réference ='" + Session["reff"].ToString() + "'", con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                con.Close();

                string Statutt = dt.Rows[0][0].ToString();
                string typeachat = dt.Rows[0][1].ToString();
                string valide = dt.Rows[0][2].ToString();
                if (valide == "0")
                {
                    int statu = Convert.ToInt32(Statutt);
                    statu = statu + 1;
                    SqlCommand cmd = new SqlCommand("", con);
                    if (Statutt == "-1")
                    {
                        cmd = new SqlCommand("update DA_Demande set Etat = '0',Statut='0',etat_prochain='1' where  Réference='" + Session["reff"].ToString() + "'", con);
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();

                        db.DA_DemUsersTraitees.Add(demusertrait);
                        db.SaveChanges();
                        return RedirectToAction("MesDemandes");
                    }
                    else if (Statutt == "0")
                    {
                        SqlDataAdapter da1 = new SqlDataAdapter("SELECT Intervenant FROM DA_WorkflowTypAch where Id_type =(SELECT Id FROM DA_TypesAchats where TypeAchat ='" + typeachat + "') and Num = 2 ", con);
                        DataTable dt1 = new DataTable();
                        da1.Fill(dt1);
                        string suivant = dt1.Rows[0][0].ToString();

                        cmd = new SqlCommand("update DA_Demande set Etat = 'N+1',Statut='1',etat_prochain='" + suivant + "' where  Réference ='" + Session["reff"].ToString() + "'", con);
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();

                        db.DA_DemUsersTraitees.Add(demusertrait);
                        db.SaveChanges();
                        return RedirectToAction("DemendesEnAttente");
                    }
                    else
                    {
                        con.Open();
                        
                        SqlDataAdapter da1 = new SqlDataAdapter("SELECT Intervenant,Num FROM DA_WorkflowTypAch where Id_type =(SELECT Id FROM DA_TypesAchats where TypeAchat ='" + typeachat + "') and Num='" + statu + "' ", con);
                        DataTable dt1 = new DataTable();
                        da1.Fill(dt1);
                        string inter = dt1.Rows[0][0].ToString();
                        int numerr = Convert.ToInt32(dt1.Rows[0][1].ToString()) + 1;
                        string intervenant = dt1.Rows[0][0].ToString();

                        SqlDataAdapter da12 = new SqlDataAdapter("SELECT Intervenant FROM DA_WorkflowTypAch where Id_type =(SELECT Id FROM DA_TypesAchats where TypeAchat ='" + typeachat + "') and Num='" + numerr + "' ", con);
                        DataTable dt12 = new DataTable();
                        da12.Fill(dt12);
                        string inter2 = "";
                        if (dt12.Rows.Count > 0)
                        {
                            inter2 = dt12.Rows[0][0].ToString();
                        }
                        cmd = new SqlCommand("update DA_Demande set Etat = '" + intervenant + "',Statut=" + statu + ",etat_prochain='" + inter2 + "' where  Réference='" + Session["reff"].ToString() + "'", con);
                        
                        cmd.ExecuteNonQuery();
                        con.Close();
                        SqlDataAdapter da11 = new SqlDataAdapter("SELECT top(1) Intervenant FROM DA_WorkflowTypAch where Id_type =(SELECT Id FROM DA_TypesAchats where TypeAchat ='" + typeachat + "') order by Num desc", con);
                        DataTable dt11 = new DataTable();
                        da11.Fill(dt11);
                        string inter1 = dt11.Rows[0][0].ToString();

                        if (inter == inter1)
                        {
                            SqlCommand cmd1 = new SqlCommand("update DA_Demande set Validee = '1',Etat='66' where  Réference='" + Session["reff"].ToString() + "'", con);
                            con.Open();
                            cmd1.ExecuteNonQuery();
                            con.Close();
                        }
                        db.DA_DemUsersTraitees.Add(demusertrait);
                        db.SaveChanges();
                        return RedirectToAction("DemendesEnAttente");
                    }
                    

                }
                else
                {
                    return RedirectToAction("MesDemandes");
                }

                
            }
        }

        public ActionResult AnnulerDemande()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AnnulerDemande(string id)
        {
            string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection con = new SqlConnection(constr);
            con.Open();

            SqlCommand cmd = new SqlCommand("delete from DA_Demande where Réference ='" + Session["reff"].ToString() + "'", con);
            SqlCommand cmd1 = new SqlCommand("delete from DA_Budget where Réference ='" + Session["reff"].ToString() + "'", con);
            SqlCommand cmd2 = new SqlCommand("delete from DA_ProduitsDem where Réference ='" + Session["reff"].ToString() + "'", con);

            cmd.ExecuteNonQuery();
            cmd1.ExecuteNonQuery();
            cmd2.ExecuteNonQuery(); cmd.ExecuteNonQuery();

            return RedirectToAction("MesDemandes");
        }

        [HttpPost]
        public ActionResult RejeterDemande(string MotifRejet)
        {
            if (MotifRejet == "")
            {
                return Content("<script language='javascript' type='text/javascript'>alert('Veuillez Sélectionnez un motif de rejet');window.location = '/DA_Demande/demande';</script>");
            }
            else
            {
                ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
                DA_DemUsersTraitees demusertrait = new DA_DemUsersTraitees();
                demusertrait.Matricule = user.matricule;
                demusertrait.Reference = Session["reff"].ToString();

                string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
                SqlConnection con = new SqlConnection(constr);
                con.Open();
                SqlDataAdapter da11 = new SqlDataAdapter("SELECT Conséquense FROM MotifsRejets where MotifRejet ='" + MotifRejet + "'", con);
                DataTable dt11 = new DataTable();
                da11.Fill(dt11);
                string consequence = dt11.Rows[0][0].ToString();
                if (consequence == "Cloture")
                {
                    SqlCommand cmd1 = new SqlCommand("update DA_Demande set Statut = '100',Etat='100',etat_prochain='0',MotifRejet='"+ MotifRejet + "' where  Réference='" + Session["reff"].ToString() + "'", con);
                    cmd1.ExecuteNonQuery();
                }
                else if (consequence == "Retour Pour Correction")
                {
                    SqlCommand cmd1 = new SqlCommand("update DA_Demande set Statut = '-1',Etat='-1',etat_prochain='0',AvecSans='99',MotifRejet='" + MotifRejet + "' where  Réference='" + Session["reff"].ToString() + "'", con);
                    cmd1.ExecuteNonQuery();
                }


                db.DA_DemUsersTraitees.Add(demusertrait);
                db.SaveChanges();
                con.Close();
                return RedirectToAction("DemendesEnAttente");
            }
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
