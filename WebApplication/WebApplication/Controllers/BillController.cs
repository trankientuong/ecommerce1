using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Transactions;
using System.Web.Mvc;
using System.Web.Security;
using WebApplication.Models;
using System.Net;

namespace WebApplication.Controllers
{
    public class BillController : Controller
    {
        private CsK24T25Entities db = new CsK24T25Entities();
        private List<CHITIETDONHANG> ShoppingCart = null;
        
        private void GetShoppingCart()
        {
            //var session = System.Web.HttpContext.Current.Session;
            if (Session["ShoppingCart"] != null)
                ShoppingCart = Session["ShoppingCart"] as List<CHITIETDONHANG>;
            else
            {
                ShoppingCart = new List<CHITIETDONHANG>();
                Session["ShoppingCart"] = ShoppingCart;
            }
        }
        // GET: Bill
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var model = db.DONHANGs.ToList();
            return View(model);
        }

        public ActionResult Create()
        {           
            GetShoppingCart();            
            ViewBag.Cart = ShoppingCart;
            return View();           
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DONHANG model)
        {
            ValidateBill(model);
            if (ModelState.IsValid)
            {
                using(var scope = new TransactionScope())
                {
                    model.NGAYTAODONHANG = DateTime.Now;
                    db.DONHANGs.Add(model);
                    db.SaveChanges();

                    foreach (var item in ShoppingCart)
                    {
                        db.CHITIETDONHANGs.Add(new CHITIETDONHANG
                        {
                            MADONHANG = model.MADONHANG,
                            MASANPHAM = item.SANPHAM.MASP,
                            GIATIENBAN = item.SANPHAM.GIABAN,
                            SOLUONG = item.SOLUONG

                        });
                    }
                    db.SaveChanges();
                    scope.Complete();
                    Session["ShoppingCart"] = null;
                    return RedirectToAction("Index2", "SANPHAMs");
                }               
            }
            GetShoppingCart();
            ViewBag.Cart = ShoppingCart;
            return View(model);
        }

        public ActionResult Delete(int id)
        {
            var od = db.CHITIETDONHANGs.Find(id);
            db.CHITIETDONHANGs.Remove(od);
            db.SaveChanges();
            var model = db.DONHANGs.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {           
            DONHANG dh = db.DONHANGs.Find(id);
            db.DONHANGs.Remove(dh);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        private void ValidateBill(DONHANG model)
        {
            var regex = new Regex("[0-9]{3}");
            GetShoppingCart();
            if(ShoppingCart.Count == 0)
            {
                ModelState.AddModelError("", " There is no item in shopping cart!");
            }
            if (!regex.IsMatch(model.SODIENTHOAINGNHAN))
            {
                ModelState.AddModelError("", " Wrong phone number!");
            }
        }      
    }
}