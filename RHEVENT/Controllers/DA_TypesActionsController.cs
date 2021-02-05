using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RHEVENT.Models;

namespace RHEVENT.Controllers
{
    public class DA_TypesActionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: DA_TypesActions
        public ActionResult Index()
        {
            return View(db.DA_TypesActions.OrderBy(x => x.TypeActhat).ThenBy(x => x.TypeAction).ToList());
            //var listtypesaction = (from m in db.DA_TypesActions
            //                       join n in db.DA_TypesAchats on m.TypeActhat equals n.Id
            //                       orderby m.TypeActhat, m.TypeAction
            //                       select m);
            //return View(listtypesaction);
        }

        // GET: DA_TypesActions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DA_TypesActions dA_TypesActions = db.DA_TypesActions.Find(id);
            if (dA_TypesActions == null)
            {
                return HttpNotFound();
            }
            return View(dA_TypesActions);
        }

        // GET: DA_TypesActions/Create
        public ActionResult Create()
        {
            DA_TypesActions typeactionn = new DA_TypesActions();
            typeactionn.listesachats = db.DA_TypesAchats.OrderBy(obj => obj.TypeAchat).ToList<DA_TypesAchats>();
            return View(typeactionn);
        }
        
       
        // POST: DA_TypesActions/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string TypeAchatt, string TypeActionn)
        {
            DA_TypesActions dA_TypesActions = new DA_TypesActions();

            dA_TypesActions.TypeActhat =TypeAchatt;
            dA_TypesActions.TypeAction = TypeActionn;

            bool IsTypeActionNameExist = db.DA_TypesActions.Any
        (x => x.TypeAction == dA_TypesActions.TypeAction && x.TypeActhat == dA_TypesActions.TypeActhat && x.Id != dA_TypesActions.Id);
            if (IsTypeActionNameExist == true)
            {
                ViewBag.typeachat = TypeAchatt;
                ViewBag.TypeAction = TypeActionn;
                ModelState.AddModelError("TypeAction", "Ce Type d'Action existe déja");
            }
            
            else if (ModelState.IsValid)
            {
                dA_TypesActions.TypeActhat =TypeAchatt;
                dA_TypesActions.TypeAction = TypeActionn;
                db.DA_TypesActions.Add(dA_TypesActions);
                try
                {
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch(DbEntityValidationException ex)
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
            dA_TypesActions.listesachats = db.DA_TypesAchats.OrderBy(obj => obj.TypeAchat).ToList<DA_TypesAchats>();

            return View(dA_TypesActions);
        }


        //public JsonResult IsTypeActionNameExist(string TypeAction, int? Id, string TypeAchatt)
        //{
        //    var validateName = db.DA_TypesActions.FirstOrDefault
        //                        (x => x.TypeAction == TypeAction && x.Id != Id && x.TypeActhat == TypeAchatt);
        //    if (validateName != null)
        //    {
        //        return Json(false, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        return Json(true, JsonRequestBehavior.AllowGet);
        //    }
        //}

        // GET: DA_TypesActions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DA_TypesActions dA_TypesActions = db.DA_TypesActions.Find(id);
            dA_TypesActions.listesachats = db.DA_TypesAchats.OrderBy(obj => obj.TypeAchat).ToList<DA_TypesAchats>();

            if (dA_TypesActions == null)
            {
                return HttpNotFound();
            }
            return View(dA_TypesActions);
        }

        // POST: DA_TypesActions/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,TypeAction,TypeActhat")] DA_TypesActions dA_TypesActions)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dA_TypesActions).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(dA_TypesActions);
        }

        // GET: DA_TypesActions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DA_TypesActions dA_TypesActions = db.DA_TypesActions.Find(id);
            if (dA_TypesActions == null)
            {
                return HttpNotFound();
            }
            return View(dA_TypesActions);
        }

        // POST: DA_TypesActions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DA_TypesActions dA_TypesActions = db.DA_TypesActions.Find(id);
            db.DA_TypesActions.Remove(dA_TypesActions);
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
