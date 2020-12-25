using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using RHEVENT.Models;

namespace RHEVENT.Controllers
{
    public class E_FormationController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: E_Formation
        public ActionResult Index()
        {

            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());

            string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection con = new SqlConnection(constr);
            con.Open();

            DataTable dt = new DataTable();

            DataTable dt2 = new DataTable();


            SqlDataAdapter da;
            da = new SqlDataAdapter(" SELECT  distinct   [code]   FROM [E_Formation] where Etat_Formation ='Active' and  [Matricule_Formateur] =  '" + user.matricule + "'", con);
            da.Fill(dt);

            
        
           List<E_Formation> list = new List<E_Formation>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                 
                E_Formation e = new E_Formation();

                string dd = dt.Rows[i]["code"].ToString();


                SqlDataAdapter da2;
                da2 = new SqlDataAdapter(" SELECT  Date_Creation, Objet , Id   FROM [E_Formation] where   Code =  '" + dd + "'", con);
                da2.Fill(dt2);


                e.Code = dt.Rows[i]["code"].ToString();
                 
                 e.Matricule_Formateur = e.ImageName = e.Chemin = e.Etat_Formation =  null;

                e.NumDiapo = 0;

                e.Date_Creation = Convert.ToDateTime(dt2.Rows[i]["Date_Creation"]);

                e.Objet = dt2.Rows[i]["Objet"].ToString();

                e.Id = Convert.ToInt32(dt2.Rows[i]["Id"]);

                list.Add(e);

                }

              

            return View(list);

            //ApplicationUser user = db.Users.Find(User.Identity.GetUserId());

            //var FormQuery = from m in db.E_Formation
            //                where m.Etat_Formation == "Active" &&   m.Matricule_Formateur == user.matricule
            //                select m;

                 
            //return View(FormQuery);
        }


        [HttpGet]
        public ActionResult Create()
        { 
            return View();
        }

        [HttpPost]
        public ActionResult Create(System.Collections.Generic.IEnumerable<HttpPostedFileBase> files, E_Formation e_Formation)
        {
            string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection con = new SqlConnection(constr);
            con.Open();

            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());

            e_Formation.Matricule_Formateur = user.matricule;

            e_Formation.Date_Creation = System.DateTime.Now;

            e_Formation.Etat_Formation = "Active";

            string OldChaine = System.DateTime.Now.ToShortDateString();

            string NewChaine = OldChaine.Replace("/", "");

            string date = System.DateTime.Now.ToShortDateString();

            DataTable dt = new DataTable();

            SqlDataAdapter da;
            da = new SqlDataAdapter("SELECT   *  FROM E_Formation where CONVERT(VARCHAR(10), Date_Creation, 103)='" + date + "'", con);
            da.Fill(dt);

            int a = dt.Rows.Count;

            //var FormQuery = from m in db.E_Formation
            //                  where m.Date_Creation.ToShortDateString() == System.DateTime.Now.ToShortDateString()
            //                  orderby m.Date_Creation descending
            //                  select m;

            //int a = FormQuery.Count();

             

            int p = a + 1;

            if (p<=9)
                e_Formation.Code = "F_" + NewChaine + "_0" + (a + 1);
            else
                e_Formation.Code = "F" + NewChaine + "_1" + (a + 1);


            string nom_formation = e_Formation.Code;

            string folderName = Server.MapPath("\\SlideImages\\");
            string pathString = System.IO.Path.Combine(folderName, nom_formation);
            System.IO.Directory.CreateDirectory(pathString);
            
            int i = 0;

            foreach (var file in files)
            {
                if (file != null && file.ContentLength > 0)
                {
                    string ext = Path.GetExtension(file.FileName);

                    if (ext.Equals(".jpg") || ext.Equals(".png") || ext.Equals(".gif") || ext.Equals(".jpeg") || ext.Equals(".JPEG") || ext.Equals(".JPG") || ext.Equals(".PNG") || ext.Equals(".GIF"))
                    {
                        i++;
                        string filename = Path.GetFileName(file.FileName);
                        string fichier = "~/SlideImages/"+nom_formation+"/"+filename;

                        System.Drawing.Bitmap bmpPostedImage = new System.Drawing.Bitmap(file.InputStream);
                        System.Drawing.Image image = (System.Drawing.Image)bmpPostedImage;
                        bmpPostedImage = new Bitmap(700, 350);
                        Graphics graphic = Graphics.FromImage(bmpPostedImage);
                        graphic.DrawImage(image, 0, 0, 700, 350);
                        graphic.DrawImage(image, new Rectangle(0, 0, 700, 350), 0, 0, 0, 0, GraphicsUnit.Pixel);
                        graphic.Dispose();
                        bmpPostedImage.Save(Server.MapPath(fichier));

                        //file.SaveAs(Server.MapPath(fichier));



                        string chemin = "~/SlideImages/"+nom_formation+"/"+filename;
                        SqlCommand cmd = new SqlCommand("Insert into E_Formation (Code,Objet,Etat_Formation,Date_Creation,Matricule_Formateur,ImageName,Numdiapo,Chemin) values(@Code,@Objet,@Etat_Formation,@Date_Creation,@Matricule_Formateur,@ImageName,@Numdiapo,@Chemin)", con);

                        cmd.Parameters.AddWithValue("@Code", e_Formation.Code);
                        cmd.Parameters.AddWithValue("@Objet", e_Formation.Objet);
                        cmd.Parameters.AddWithValue("@Etat_Formation", e_Formation.Etat_Formation);
                        cmd.Parameters.AddWithValue("@Date_Creation", e_Formation.Date_Creation);
                        cmd.Parameters.AddWithValue("@Matricule_Formateur", e_Formation.Matricule_Formateur);
                        cmd.Parameters.AddWithValue("@ImageName", filename);
                        cmd.Parameters.AddWithValue("@Numdiapo", i);
                        //cmd.Parameters.AddWithValue("@NumFormation", nom_formation);
                        cmd.Parameters.AddWithValue("@Chemin", chemin);
                        cmd.ExecuteNonQuery();

                       
                    }
                    else
                    {
                        return Content("<script language='javascript' type='text/javascript'>alert('Type de fichier invalide!');</script>");
                    }
                }
                else
                {

                }
            }


            con.Close();
            return RedirectToAction("Index", "E_Formation");
        }


        static public int numdiapo = -1;
        static public int nbdiapo = -1;
        static public string source = "0";


        [HttpGet]
        public ActionResult Consult(string id)
        {
            //E_Formation e = db.E_Formation.Find(id);

            BindDataList1(id);
            return View();
        }
        public void BindDataList1(string nomform)
        {

            string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection con = new SqlConnection(constr);
            con.Open();


            SqlCommand command = new SqlCommand("SELECT Chemin from E_Formation where Code='" + nomform + "' order by  NumDiapo ", con);
            SqlDataAdapter da = new SqlDataAdapter(command);
            DataTable dt = new DataTable();
            dt.Clear();
            da.Fill(dt);

            SqlCommand command1 = new SqlCommand("SELECT max(Numdiapo) from E_Formation where Code='" + nomform + "'", con);
            SqlDataAdapter da1 = new SqlDataAdapter(command1);
            DataTable dt1 = new DataTable();
            da1.Fill(dt1);

            string nbdiapo1 = Convert.ToString(dt1.Rows[0][0]);
            if (dt.Rows.Count >= 1)
            {
                nbdiapo = Convert.ToInt16(nbdiapo1);
            }
            Session["nbdiap"] = nbdiapo;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                TempData[i.ToString()] = Convert.ToString(dt.Rows[i][0]);
            }
            con.Close();

        }

        // GET: E_Formation/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            E_Formation e_Formation = db.E_Formation.Find(id);
            if (e_Formation == null)
            {
                return HttpNotFound();
            }
            return View(e_Formation);
        }

       
        // GET: E_Formation/Edit/5
        public ActionResult Edit(string id)
        {
            //if (id == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}

            //E_Formation e_Formation = db.E_Formation.Find(id);

            var list = (from m in db.E_Formation
                        where m.Code == id
                        select m).Take(1);

            E_Formation e_Formation = new E_Formation();

            foreach (E_Formation ss in list)
            {
                e_Formation = db.E_Formation.Find(ss.Id);
            }

            if (e_Formation == null)
            {
                return HttpNotFound();
            }
            return View(e_Formation);
        }

        // POST: E_Formation/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Bind(Include = "Id,Code,Objet,Etat_Formation,Date_Creation,Etat_Diffusion,Matricule_Formateur")]E_Formation e_Formation,
        public ActionResult Edit( System.Collections.Generic.IEnumerable<HttpPostedFileBase> files   )
        {
             
            if (ModelState.IsValid)
            {
                string code = Session["code"].ToString();

                string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
                SqlConnection con = new SqlConnection(constr);
                con.Open();

                var listf = from m in db.E_Formation
                            where m.Code == code 
                            select m;

                E_Formation e = new E_Formation();

                foreach (E_Formation eee in listf)
                { 
                    e = db.E_Formation.Find(eee.Id);
                }

                string nom_formation = e.Code;


                string folderName = Server.MapPath("\\SlideImages\\");

                //string DelImg = Server.MapPath("\\DeleteImg\\");

                //string pathDelImg = System.IO.Path.Combine(DelImg, nom_formation);

                string pathString = System.IO.Path.Combine(folderName, nom_formation);


                string path = Server.MapPath("~/SlideImages/" + nom_formation);
                string[] filess = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
                foreach (string fil in filess)
                {
                    System.IO.File.Delete(fil);
                }
            //then delete folder
            Directory.Delete(path);



                //System.IO.Directory.Move(pathString, pathDelImg);

                System.IO.Directory.CreateDirectory(pathString);

                 

                SqlCommand cmd2 = new SqlCommand("delete FROM E_Formation where Code='" + e.Code + "' ", con);
                cmd2.ExecuteNonQuery();


                int i = 0;

                foreach (var file in files)
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        string ext = Path.GetExtension(file.FileName);

                        if (ext.Equals(".jpg") || ext.Equals(".png") || ext.Equals(".gif") || ext.Equals(".jpeg") || ext.Equals(".JPEG") || ext.Equals(".JPG") || ext.Equals(".PNG") || ext.Equals(".GIF"))
                        {
                            i++;
                            string filename = Path.GetFileName(file.FileName);

                            
                            string fichier = "~/SlideImages/" + nom_formation + "/" + filename;

                            System.Drawing.Bitmap bmpPostedImage = new System.Drawing.Bitmap(file.InputStream);
                            System.Drawing.Image image = (System.Drawing.Image)bmpPostedImage;
                            bmpPostedImage = new Bitmap(700, 350);
                            Graphics graphic = Graphics.FromImage(bmpPostedImage);
                            graphic.DrawImage(image, 0, 0, 700, 350);
                            graphic.DrawImage(image, new Rectangle(0, 0, 700, 350), 0, 0, 0, 0, GraphicsUnit.Pixel);
                            graphic.Dispose();
                            bmpPostedImage.Save(Server.MapPath(fichier));

                             
                            //file.SaveAs(Server.MapPath(fichier));

                           

                            string chemin = "~/SlideImages/" + nom_formation + "/" + filename;
                            SqlCommand cmd = new SqlCommand("Insert into E_Formation (Code,Objet,Etat_Formation,Date_Creation,Matricule_Formateur,ImageName,Numdiapo,Chemin) values(@Code,@Objet,@Etat_Formation,@Date_Creation,@Matricule_Formateur,@ImageName,@Numdiapo,@Chemin)", con);

                            cmd.Parameters.AddWithValue("@Code", e.Code);
                            cmd.Parameters.AddWithValue("@Objet", e.Objet);
                            cmd.Parameters.AddWithValue("@Etat_Formation", e.Etat_Formation);
                            cmd.Parameters.AddWithValue("@Date_Creation", e.Date_Creation);
                            cmd.Parameters.AddWithValue("@Matricule_Formateur", e.Matricule_Formateur);
                            cmd.Parameters.AddWithValue("@ImageName", filename);
                            cmd.Parameters.AddWithValue("@Numdiapo", i);
                            //cmd.Parameters.AddWithValue("@NumFormation", nom_formation);
                            cmd.Parameters.AddWithValue("@Chemin", chemin);
                            cmd.ExecuteNonQuery();


                         


                        }
                        else
                        {
                            return Content("<script language='javascript' type='text/javascript'>alert('Type de fichier invalide!');</script>");
                        }
                    }
                }
                    

                //db.Entry(e_Formation).State = EntityState.Modified;
                //db.SaveChanges();
               
            }
            return RedirectToAction("Index", "E_Formation");
            //return View(e_Formation);
        }

        // GET: E_Formation/Delete/5
        public ActionResult Delete(string id)
        {
            //if (id == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}
            //E_Formation e_Formation = db.E_Formation.Find(id);
            //if (e_Formation == null)
            //{
            //    return HttpNotFound();
            //}


            var list = (from m in db.E_Formation
                        where m.Code == id
                        select m).Take(1);

            E_Formation e_Formation = new E_Formation();

            foreach (E_Formation ss in list)
            {
                e_Formation = db.E_Formation.Find(ss.Id);
            }

            return View(e_Formation);
        }

        // POST: E_Formation/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed()
        {
            string code = Session["code"].ToString();

            string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection con = new SqlConnection(constr);
            con.Open();

            SqlCommand cmd = new SqlCommand("delete FROM E_Formation where Code='" + code + "' ", con);
            cmd.ExecuteNonQuery();


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
