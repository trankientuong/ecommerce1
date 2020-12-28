using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    
    public class REVIEWRATINGsController : Controller
    {
        private CsK24T25Entities db = new CsK24T25Entities();

        // GET: REVIEWRATINGs
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var rEVIEWRATINGs = db.REVIEWRATINGs.Include(r => r.SANPHAM);
            return View(rEVIEWRATINGs.ToList());
        }

        // GET: REVIEWRATINGs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            REVIEWRATING rEVIEWRATING = db.REVIEWRATINGs.Find(id);
            if (rEVIEWRATING == null)
            {
                return HttpNotFound();
            }
            return View(rEVIEWRATING);
        }

        // GET: REVIEWRATINGs/Create
        public ActionResult Create()
        {
            ViewBag.MASANPHAM = new SelectList(db.SANPHAMs, "MASP", "TENSP");
            return View();
        }

        [AllowAnonymous]
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult C(string HOTEN, string NOIDUNG, int MaSP, string Rating)
        {
            if (ModelState.IsValid)
            {
                var db = new CsK24T25Entities();
                var rate = new REVIEWRATING();
                rate.HOTEN = HOTEN;
                rate.NOIDUNG = NOIDUNG;
                rate.MASANPHAM = MaSP;
                rate.SOSAODANHGIA = Double.Parse(Rating);
                rate.THOIGIANDANG = DateTime.Now;
                db.REVIEWRATINGs.Add(rate);
                db.SaveChanges();
                return RedirectToAction("Details", "SANPHAMs", new { id = MaSP });
            }
            return View();
        }

        // POST: REVIEWRATINGs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,MASANPHAM,MATAIKHOAN,HOTEN,NOIDUNG,THOIGIANDANG,SOSAODANHGIA")] REVIEWRATING rEVIEWRATING)
        {
            if (ModelState.IsValid)
            {
                db.REVIEWRATINGs.Add(rEVIEWRATING);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MASANPHAM = new SelectList(db.SANPHAMs, "MASP", "TENSP", rEVIEWRATING.MASANPHAM);
            return View(rEVIEWRATING);
        }

        // GET: REVIEWRATINGs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            REVIEWRATING rEVIEWRATING = db.REVIEWRATINGs.Find(id);
            if (rEVIEWRATING == null)
            {
                return HttpNotFound();
            }
            ViewBag.MASANPHAM = new SelectList(db.SANPHAMs, "MASP", "TENSP", rEVIEWRATING.MASANPHAM);
            return View(rEVIEWRATING);
        }

        // POST: REVIEWRATINGs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,MASANPHAM,MATAIKHOAN,HOTEN,NOIDUNG,THOIGIANDANG,SOSAODANHGIA")] REVIEWRATING rEVIEWRATING)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rEVIEWRATING).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MASANPHAM = new SelectList(db.SANPHAMs, "MASP", "TENSP", rEVIEWRATING.MASANPHAM);
            return View(rEVIEWRATING);
        }

        // GET: REVIEWRATINGs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            REVIEWRATING rEVIEWRATING = db.REVIEWRATINGs.Find(id);
            if (rEVIEWRATING == null)
            {
                return HttpNotFound();
            }
            return View(rEVIEWRATING);
        }

        // POST: REVIEWRATINGs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            REVIEWRATING rEVIEWRATING = db.REVIEWRATINGs.Find(id);
            db.REVIEWRATINGs.Remove(rEVIEWRATING);
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
