using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RHEVENT.Models;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace RHEVENT.Controllers
{
    [Authorize(Roles = "Admin_Medicis,RH_Admin")]
    public class ImagesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        UserManager<IdentityUser> userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>());
        // GET: Images
        [Authorize(Roles = "Admin_Medicis,RH_Admin")]
        public ActionResult Index()
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.nom_prenom = user.nom + " " + user.prenom;
            ViewBag.email = user.Email;
            return View(db.Images.ToList());
        }
        // GET: Images/Details/5
        [Authorize(Roles = "Admin_Medicis,RH_Admin")]
        public ActionResult Details(int? id)
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.nom_prenom = user.nom + " " + user.prenom;
            ViewBag.email = user.Email;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Image image = db.Images.Find(id);
            if (image == null)
            {
                return HttpNotFound();
            }
            return View(image);
        }

        // GET: Images/Create
        [Authorize(Roles = "Admin_Medicis,RH_Admin")]
        public ActionResult Create()
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.nom_prenom = user.nom + " " + user.prenom;
            ViewBag.email = user.Email;
            return View();
        }

        // POST: Images/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin_Medicis,RH_Admin")]
        public async Task<ActionResult> Create(HttpPostedFileBase file, Image Image)
        {
            if (file != null)
            {
                string ext = Path.GetExtension(file.FileName);

                if (ext.Equals(".jpg") || ext.Equals(".png") || ext.Equals(".gif") || ext.Equals(".jpeg") || ext.Equals(".JPEG") || ext.Equals(".JPG") || ext.Equals(".PNG") || ext.Equals(".GIF"))
                {
                    // try
                    //   {   
                    string path = Server.MapPath("~/../RH_IMAGES_FOLDER/");

                    ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
                    Image img = new Image();
                    string id_image = System.DateTime.Now.Year + "" + System.DateTime.Now.Month + "" + System.DateTime.Now.Day + "" + System.DateTime.Now.Hour + "" + System.DateTime.Now.Minute + "" + System.DateTime.Now.Second + "" + System.DateTime.Now.Millisecond + "";
                    img.image_id = id_image;
                    img.UserId = User.Identity.GetUserId();
                    img.UserName = user.UserName;
                    img.titre = Image.titre;
                    img.text = Image.text;
                    img.type_photo = Image.type_photo;
                    img.lien = "../RH_IMAGES_FOLDER/" + id_image + ext;
                    img.date_upload = System.DateTime.Now;
                    db.Images.Add(img);
                    await db.SaveChangesAsync();


                    Task taskcopy = Task.Run(() => file.SaveAs(path + Path.GetFileName(id_image + ext)));
                    return View("OK");
                    /* }
                     catch (Exception ex)
                     {
                         return View("Error");
                     }*/
                }
                else
                {
                    return Content("<script language='javascript' type='text/javascript'>alert('Type de fichier invalide!');</script>");
                }
            }
            else
            {
                return View("Error");
            }
        }

        // GET: Images/Edit/5
        [Authorize(Roles = "Admin_Medicis,RH_Admin")]
        public ActionResult Edit(int? id)
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.nom_prenom = user.nom + " " + user.prenom;
            ViewBag.email = user.Email;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Image image = db.Images.Find(id);
            if (image == null)
            {
                return HttpNotFound();
            }
            return View(image);
        }

        // POST: Images/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin_Medicis,RH_Admin")]
        public ActionResult Edit([Bind(Include = "Id,image_id,UserId,UserName,titre,text,lien,date_upload,suppresseur,date_suppression,type_photo")] Image image)
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.nom_prenom = user.nom + " " + user.prenom;
            ViewBag.email = user.Email;
            if (ModelState.IsValid)
            {
                db.Entry(image).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(image);
        }

        // GET: Images/Delete/5
        [Authorize(Roles = "Admin_Medicis,RH_Admin")]
        public ActionResult Delete(int? id)
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.nom_prenom = user.nom + " " + user.prenom;
            ViewBag.email = user.Email;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Image image = db.Images.Find(id);
            if (image == null)
            {
                return HttpNotFound();
            }
            return View(image);
        }

        // POST: Images/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin_Medicis,RH_Admin")]
        public ActionResult DeleteConfirmed(int id)
        { 
            
            Image image = db.Images.Find(id);
            //return RedirectToAction(Server.MapPath("~/"+image.lien));
            System.IO.File.Delete(Server.MapPath("~/" + image.lien));
            db.Images.Remove(image);
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
