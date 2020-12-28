using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;
using WebGrease.Css.Ast;

namespace WebApplication.Controllers
{
    public class ShoppingCartController : Controller
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

        // GET: ShoppingCart
        public ActionResult Index()
        {
            GetShoppingCart();
            var hashtable = new Hashtable();
            foreach (var billDetail in ShoppingCart)
            {
                if (hashtable[billDetail.SANPHAM.MASP] != null)
                {
                    (hashtable[billDetail.SANPHAM.MASP] as CHITIETDONHANG).SOLUONG += billDetail.SOLUONG;
                }
                else hashtable[billDetail.SANPHAM.MASP] = billDetail;
            }
            ShoppingCart.Clear();
            foreach(CHITIETDONHANG billDetail in hashtable.Values)
            {
                ShoppingCart.Add(billDetail);
            }
            return View(ShoppingCart);
        }

       [HttpPost]
        // GET: ShoppingCart/Create
        public ActionResult Create(int productId,int quantity)
        {
            GetShoppingCart();
            var product = db.SANPHAMs.Find(productId);
            ShoppingCart.Add(new CHITIETDONHANG 
            {
                SANPHAM = product,
                SOLUONG = quantity           
            });
            return RedirectToAction("Index");
        }

        
        [HttpPost]
        public ActionResult Edit(int[] product_id,int[] quantity)
        {
            GetShoppingCart();
            ShoppingCart.Clear();
            if(product_id != null)
            for(int i =0;i< product_id.Length; i++)
                    if(quantity[i] > 0)
            {
                    var product = db.SANPHAMs.Find(product_id[i]);
                    ShoppingCart.Add(new CHITIETDONHANG 
                    { 
                        SANPHAM = product,
                        SOLUONG = quantity[i]                                       
                    });
            }
            Session["ShoppingCart"] = ShoppingCart;
            return RedirectToAction("Index");
        }

    
        // GET: ShoppingCart/Delete/5
        public ActionResult Delete()
        {
            GetShoppingCart();
            ShoppingCart.Clear();
            Session["ShoppingCart"] = ShoppingCart;
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
