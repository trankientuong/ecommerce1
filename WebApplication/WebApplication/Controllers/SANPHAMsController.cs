using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;
using System.Transactions;

namespace WebApplication.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SANPHAMsController : Controller
    {
        private CsK24T25Entities db = new CsK24T25Entities();
        
        // GET: SANPHAMs
        public ActionResult Index()
        {            
           // var sANPHAMs = db.SANPHAMs.Include(s => s.KHUYENMAI).Include(s => s.LOAISANPHAM).Include(s => s.NHACUNGCAP);
            return View(db.SANPHAMs.ToList());
        }
      
        [AllowAnonymous]
        public ActionResult Search(string keyword)
        {
            var model = db.SANPHAMs.ToList();
            model = model.Where(p =>p.TENSP.ToLower().Contains(keyword.ToLower())).ToList();
            ViewBag.keyword = keyword;
            return View("Index2",model);
        }

        [AllowAnonymous]
        //For Customer to View Products
        public ActionResult Index2()
        {
            var model = db.SANPHAMs.ToList();
            return View(model);
           // return View();
        }
        [AllowAnonymous]
        // GET: SANPHAMs/Details/5
        public ActionResult Details(int id)
        {          
            var model = db.SANPHAMs.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            
            return View(model);
        }
        [AllowAnonymous]
        public ActionResult Picture(int id)
        {
            var path = Server.MapPath(PICTURE_PATH);
            return File(path + id, "images");
        }

        // GET: SANPHAMs/Create
        public ActionResult Create()
        {
            ViewBag.MAKM = new SelectList(db.KHUYENMAIs, "MAKHUYENMAI", "TENKHUYENMAI");
            ViewBag.LOAISP = new SelectList(db.LOAISANPHAMs, "MALOAISP", "TENLOAISP");
            ViewBag.MANCC = new SelectList(db.NHACUNGCAPs, "MANHACUNGCAP", "TENNHACUNGCAP");
            return View();
        }

        // POST: SANPHAMs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost,ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SANPHAM model,HttpPostedFileBase picture)
        {
            ValidateProduct(model);
            if (ModelState.IsValid)
            {
                if (picture != null)
                {
                    using (var scope = new TransactionScope())
                    {
                        db.SANPHAMs.Add(model);
                        db.SaveChanges();                        
                        // store picture
                        var path = Server.MapPath(PICTURE_PATH);
                        picture.SaveAs(path + model.MASP);
                        scope.Complete();
                        return RedirectToAction("Index");
                    }
                }
                else ModelState.AddModelError("", "Picture not found!");
            }
            ViewBag.MAKM = new SelectList(db.KHUYENMAIs, "MAKHUYENMAI", "TENKHUYENMAI", model.MAKM);
            ViewBag.LOAISP = new SelectList(db.LOAISANPHAMs, "MALOAISP", "TENLOAISP", model.LOAISP);
            ViewBag.MANCC = new SelectList(db.NHACUNGCAPs, "MANHACUNGCAP", "TENNHACUNGCAP", model.MANCC);

            return View(model);
        }

        private const string PICTURE_PATH = "~/Upload/Product/";
        private void ValidateProduct(SANPHAM sanpham)
        {
            if(sanpham.GIABAN < 0)
            {
                ModelState.AddModelError("GIABAN", "Gia ban nho hon 0");                
            }
        }

        // GET: SANPHAMs/Edit/5
        public ActionResult Edit(int id)
        {       
            var model = db.SANPHAMs.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            ViewBag.MAKM = new SelectList(db.KHUYENMAIs, "MAKHUYENMAI", "TENKHUYENMAI", model.MAKM);
            ViewBag.LOAISP = new SelectList(db.LOAISANPHAMs, "MALOAISP", "TENLOAISP", model.LOAISP);
            ViewBag.MANCC = new SelectList(db.NHACUNGCAPs, "MANHACUNGCAP", "TENNHACUNGCAP", model.MANCC);

            return View(model);
        }

        // POST: SANPHAMs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost,ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SANPHAM model,HttpPostedFileBase picture)
        {
            ValidateProduct(model);
            if (ModelState.IsValid)
            {
                using (var scope = new TransactionScope())
                {

                    db.Entry(model).State = EntityState.Modified;
                    db.SaveChanges();
                    if(picture != null)
                    {
                        // store picture
                        var path = Server.MapPath(PICTURE_PATH);
                        picture.SaveAs(path + model.MASP);
                    }



                    scope.Complete();
                    return RedirectToAction("Index");
                }
            }
            ViewBag.MAKM = new SelectList(db.KHUYENMAIs, "MAKHUYENMAI", "TENKHUYENMAI", model.MAKM);
            ViewBag.LOAISP = new SelectList(db.LOAISANPHAMs, "MALOAISP", "TENLOAISP", model.LOAISP);
            ViewBag.MANCC = new SelectList(db.NHACUNGCAPs, "MANHACUNGCAP", "TENNHACUNGCAP", model.MANCC);
            return View(model);
        }

        // GET: SANPHAMs/Delete/5
        public ActionResult Delete(int id)
        {
            
            var model = db.SANPHAMs.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // POST: SANPHAMs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            using (var scope = new TransactionScope()) 
            {
                var model = db.SANPHAMs.Find(id);
                db.SANPHAMs.Remove(model);
                db.SaveChanges();
                var path = Server.MapPath(PICTURE_PATH);
                System.IO.File.Delete(path + model.MASP);
                


                scope.Complete();
                return RedirectToAction("Index");
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
