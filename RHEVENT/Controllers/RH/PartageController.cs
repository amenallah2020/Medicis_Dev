using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using RHEVENT.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace RHEVENT.Controllers
{
    public class PartageController : Controller
    {
        // GET: Partage
        private ApplicationDbContext db = new ApplicationDbContext();
        UserManager<IdentityUser> userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>());

        [Authorize(Roles = "Admin_Medicis,RH_Admin")]
        public ActionResult Partage_image()
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.nom_prenom = user.nom + " " + user.prenom;
            ViewBag.email = user.Email;
            return View();
        }
          
        [HttpPost]
        [Authorize(Roles = "Admin_Medicis,RH_Admin")]

        public async Task<ActionResult> Partage_image(HttpPostedFileBase file,Image Image)
        {
            if (file != null)
            {
                string ext = Path.GetExtension(file.FileName);

                if (ext.Equals(".jpg") || ext.Equals(".png") || ext.Equals(".gif") || ext.Equals(".jpeg"))
                {
                   // try
                 //   {
                        string path = Server.MapPath("~/../RH_IMAGES_FOLDER/");

                        ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
                        Image img = new Image();
                        string id_image = System.DateTime.Now.Year +""+ System.DateTime.Now.Month +""+ System.DateTime.Now.Day +""+ System.DateTime.Now.Hour +""+ System.DateTime.Now.Minute +""+ System.DateTime.Now.Second +""+ System.DateTime.Now.Millisecond+"";
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

                      
                        Task taskcopy = Task.Run(() => file.SaveAs(path + Path.GetFileName(id_image+ext)));
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
        [Authorize(Roles = "Admin_Medicis,RH_Admin")]
        public ActionResult Ecran()
        {
            return View(db.Images.ToList().Select(img => img).OrderByDescending(img => img.date_upload));
        }   
    }
}