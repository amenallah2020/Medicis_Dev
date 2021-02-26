using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RHEVENT.Models;

namespace RHEVENT.Controllers
{
    public class E_listeUsrEvalController : Controller
    {
       

        private ApplicationDbContext db = new ApplicationDbContext();

        public IEnumerable<E_listeUsr> liste2 = null;

        public static string st = null;


        // GET: E_listeUsr
        public ActionResult Index(string id)
        {
            //if (TempData["CodeFormation"] != null)
            //{
            //    st = TempData["CodeFormation"].ToString();
            //    ViewBag.form = st;
            //}

            if (Session["CodeEval"] != null)
            {
                st = Session["CodeEval"].ToString();
                ViewBag.form = st;
            }


            E_GrpByUsr e = db.E_GrpByUsr.Find(Convert.ToInt32(id));

            if (e == null)
            {
                var list = from m in db.E_listeUsr
                           where m.Code_grp == id
                           select m;

                var idgrp = from m in db.E_GrpByUsr
                            where m.Code == id 
                            select m;

                foreach(E_GrpByUsr d in idgrp)
                {
                    ViewBag.idGrp = d.Id;
                }

                foreach(E_listeUsr ee in list)
                {
                    ViewBag.codeGrp = ee.Code_grp;

                    //ViewBag.codeForm = ee.Code_formt;

                   
                }

                liste2 = list;
            }
            else
            {
                ViewBag.codeGrp = e.Code;

                //ViewBag.codeForm = e.Code_Formation;
           
          
             
             liste2 = from m in db.E_listeUsr
                        where m.Code_grp == e.Code
                        select m;
            }

            return View(liste2.ToList());
        }

        //[HttpGet, ActionName("GetListUsr")]
        //[ValidateAntiForgeryToken]
        public ActionResult GetListUsr(string id)
        {
            
            E_GrpByUsr e = db.E_GrpByUsr.Find(Convert.ToInt32(id));

            if (e == null)
            {
                var list = from m in db.E_listeUsr
                           where m.Code_grp == id
                           select m;

                var idgrp = from m in db.E_GrpByUsr
                            where m.Code == id
                            select m;

                foreach (E_GrpByUsr d in idgrp)
                {
                    ViewBag.idGrp = d.Id;
                }

                foreach (E_listeUsr ee in list)
                {
                    ViewBag.codeGrp = ee.Code_grp;
 
                }

                liste2 = list;
            }
            else
            {
                ViewBag.codeGrp = e.Code;
 
                liste2 = from m in db.E_listeUsr
                         where m.Code_grp == e.Code
                         select m;
            }

            return View(liste2.ToList());
        }

        // GET: E_listeUsr/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            E_listeUsr e_listeUsr = db.E_listeUsr.Find(id);
            if (e_listeUsr == null)
            {
                return HttpNotFound();
            }
            return View(e_listeUsr);
        }

        public ActionResult CreateUsrGrp(string id)
        {
            var UserQuery = from m in db.Users
                            select m;
             
            string codeg = TempData["Data2"].ToString();
 
            ViewBag.codeg = TempData["Data2"].ToString();
             
            List<E_GrpByUsr> listG = new List<E_GrpByUsr>();

            listG.Insert(0, new E_GrpByUsr { Id = 0, Code = codeg });

            List<E_GrpByUsr> CodeGroup = listG;

            ViewBag.CodeGroup = CodeGroup;

             
            List<ApplicationUser> listUser = UserQuery.ToList<ApplicationUser>();

            ViewBag.listUser = listUser;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateUsrGrp(E_listeUsr e_listeUsr)
        {
            if (ModelState.IsValid)
            { 
                string codeg = TempData["Group"].ToString();

                var usr = from m in db.Users
                          where m.matricule == e_listeUsr.SelectUsr
                          select m;

                foreach (ApplicationUser a in usr)
                {
                    e_listeUsr.Mat_usr = a.matricule;

                    e_listeUsr.Nom_usr = a.NomPrenom;
                }

                e_listeUsr.Code_grp = codeg;
 
                var idGrp = from m in db.E_GrpByUsr
                            where m.Code == e_listeUsr.Code_grp
                            select m;
                int ss = 0;
                foreach (E_GrpByUsr t in idGrp)
                {
                    ss = t.Id;
                }

                db.E_listeUsr.Add(e_listeUsr);

                db.SaveChanges();

                return RedirectToAction("GetListUsr", new { id = ss });
            }

            return View(e_listeUsr);
        }


        // GET: E_listeUsr/Create
        public ActionResult Create(string id)
        {
            var UserQuery = from m in db.Users
                            orderby m.NomPrenom
                            select m;

            //string codef = TempData["Data1"].ToString();

            string codeg = TempData["Data2"].ToString();

            //ViewBag.codeF = TempData["Data1"].ToString();

            ViewBag.codeg = TempData["Data2"].ToString();

           

            //List<E_Formation> listF = new List<E_Formation>();

            //listF.Insert(0, new E_Formation { Id = 0, Code = codef });

            //List<E_Formation> CodeForm = listF;

            //ViewBag.CodeFor = CodeForm;


            List<E_GrpByUsr> listG = new List<E_GrpByUsr>();

            listG.Insert(0, new E_GrpByUsr { Id = 0, Code = codeg });

            List<E_GrpByUsr> CodeGroup = listG;

            ViewBag.CodeGroup = CodeGroup;



            List<ApplicationUser> listUser = UserQuery.ToList<ApplicationUser>();

            ViewBag.listUser = listUser;

            return View();
        }

        // POST: E_listeUsr/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( E_listeUsr e_listeUsr)
        {
            if (ModelState.IsValid)
            {
                //string codef = TempData["Form"].ToString();

                string codeg = TempData["Group"].ToString();

                var usr = from m in db.Users
                           where m.matricule == e_listeUsr.SelectUsr
                           select m;

               foreach(ApplicationUser a in usr)
                {
                    e_listeUsr.Mat_usr = a.matricule;

                    e_listeUsr.Nom_usr = a.NomPrenom;
                }

                e_listeUsr.Code_grp = codeg;

                //e_listeUsr.Code_formt = codef;

                var idGrp = from m in db.E_GrpByUsr
                            where m.Code == e_listeUsr.Code_grp
                            select m;
                int ss = 0;
                foreach (E_GrpByUsr t in idGrp)
                {
                      ss = t.Id;
                }

                db.E_listeUsr.Add(e_listeUsr);

                db.SaveChanges();

                return RedirectToAction("Index", new { id = ss });
            }

            return View(e_listeUsr);
        }

        // GET: E_listeUsr/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            E_listeUsr e_listeUsr = db.E_listeUsr.Find(id);
            if (e_listeUsr == null)
            {
                return HttpNotFound();
            }
            return View(e_listeUsr);
        }

        // POST: E_listeUsr/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Mat_usr,Nom_usr,Code_grp,Code_formt")] E_listeUsr e_listeUsr)
        {
            if (ModelState.IsValid)
            {
                db.Entry(e_listeUsr).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(e_listeUsr);
        }

        public ActionResult DeleteUsrGrp(int? id)
        {
             
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            E_listeUsr e_listeUsr = db.E_listeUsr.Find(id);
            if (e_listeUsr == null)
            {
                return HttpNotFound();
            }

            var r = from m in db.E_GrpByUsr
                    where m.Code == e_listeUsr.Code_grp
                    select m;

            foreach(E_GrpByUsr e in r)
            {
                Session["Idgrp"] = e.Id;

            }

            return View(e_listeUsr);
        }

        [HttpPost, ActionName("DeleteUsrGrp")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmedUsrGrp(int id)
        {
            //string ff = TempData["CodeFormation"].ToString();

            //ViewBag.form = ff;

            E_listeUsr e_listeUsr = db.E_listeUsr.Find(id);

            var idcode = from m in db.E_GrpByUsr
                         where m.Code == e_listeUsr.Code_grp
                         select m;
            int o = 0;
            foreach (E_GrpByUsr e in idcode)
            {
                o = e.Id;
            }
            db.E_listeUsr.Remove(e_listeUsr);
            db.SaveChanges();
            return RedirectToAction("GetListUsr", new { id = o });
        }


        // GET: E_listeUsr/Delete/5
        public ActionResult Delete(int? id)
        {
            //string ff = TempData["CodeFormation"].ToString();

            //ViewBag.form = ff;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            E_listeUsr e_listeUsr = db.E_listeUsr.Find(id);
            if (e_listeUsr == null)
            {
                return HttpNotFound();
            }

            var idcode = from m in db.E_GrpByUsr
                         where m.Code == e_listeUsr.Code_grp
                         select m;
            
            foreach (E_GrpByUsr e in idcode)
            {
                Session["idgrp"] = e.Id;
            }
            return View(e_listeUsr);
        }

        // POST: E_listeUsr/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            //string ff = TempData["CodeFormation"].ToString();

            //ViewBag.form = ff;

            E_listeUsr e_listeUsr = db.E_listeUsr.Find(id);

            var idcode = from m in db.E_GrpByUsr
                         where m.Code == e_listeUsr.Code_grp
                         select m;
            int o = 0;
            foreach(E_GrpByUsr e in idcode)
            {
                o = e.Id;
            }
            db.E_listeUsr.Remove(e_listeUsr);
            db.SaveChanges();
            return RedirectToAction("Index", new { id = o });
        }

        [HttpPost]
        public ActionResult DeleteUsr(int id)
        {
            //string ff = TempData["CodeFormation"].ToString();

            //ViewBag.form = ff;

            E_listeUsr e_listeUsr = db.E_listeUsr.Find(id);

            var idcode = from m in db.E_GrpByUsr
                         where m.Code == e_listeUsr.Code_grp
                         select m;
            int o = 0;
            foreach (E_GrpByUsr e in idcode)
            {
                o = e.Id;
            }
            db.E_listeUsr.Remove(e_listeUsr);
            db.SaveChanges();
            return RedirectToAction("Index", new { id = o });
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
