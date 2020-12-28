using Moq;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplication.Controllers;
using WebApplication.Models;
using System.Web;
using System.Web.UI;
using System.Transactions;
using System.IO;
using System.Runtime.Remoting.Messaging;
using System.Collections;
using System.Web.Routing;
using System.Web.SessionState;

namespace WebApplication.Tests.Controllers
{
    public class MockHttpSession : HttpSessionStateBase
    {
        public Hashtable Buffer = new Hashtable();
        public override object this[string key]
        {
            get
            {
                return Buffer[key];
            }
            set
            {
                Buffer[key] = value;
            }
        }
    }

        [TestClass]
        public class ShoppingCartControllerTest
        {
            [TestMethod]
            public void TestIndex()
            {
                var session = new MockHttpSession();
                var context = new Mock<HttpContextBase>();
                context.Setup(c => c.Session).Returns(session);

                var controller = new ShoppingCartController();
                controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);

                session["ShoppingCart"] = null;
                var result = controller.Index() as ViewResult;
                Assert.IsNotNull(result);

                var model = result.Model as List<CHITIETDONHANG>;
                Assert.IsNotNull(model);
                Assert.AreEqual(0, model.Count);

                var db = new CsK24T25Entities();
                var product = db.SANPHAMs.First();
                var shoppingcart = new List<CHITIETDONHANG>();
                shoppingcart.Add(new CHITIETDONHANG
                {
                    SANPHAM = product,
                    SOLUONG = 1
                });
                var billDetail = new CHITIETDONHANG();
                billDetail.SANPHAM = product;
                billDetail.SOLUONG = 2;
                shoppingcart.Add(billDetail);

                session["ShoppingCart"] = shoppingcart;
                result = controller.Index() as ViewResult;
                Assert.IsNotNull(result);

                model = result.Model as List<CHITIETDONHANG>;
                Assert.IsNotNull(model);
                Assert.AreEqual(1, model.Count);
                Assert.AreEqual(product.MASP, model.First().SANPHAM.MASP);
                Assert.AreEqual(3, model.First().SOLUONG);

            }

            [TestMethod]
            public void TestCreate()
            {
                var session = new MockHttpSession();
                var context = new Mock<HttpContextBase>();
                context.Setup(c => c.Session).Returns(session);

                var controller = new ShoppingCartController();
                controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);

                var db = new CsK24T25Entities();
                var product = db.SANPHAMs.First();
                var result = controller.Create(product.MASP, 2) as RedirectToRouteResult;
                Assert.IsNotNull(result);
                Assert.AreEqual("Index", result.RouteValues["action"]);

                var shoppingCart = session["ShoppingCart"] as List<CHITIETDONHANG>;
                Assert.IsNotNull(shoppingCart);
                Assert.AreEqual(1, shoppingCart.Count);
                Assert.AreEqual(product.MASP, shoppingCart.First().SANPHAM.MASP);
                Assert.AreEqual(2, shoppingCart.First().SOLUONG);

            }

        } 
}

