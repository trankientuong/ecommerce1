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

namespace WebApplication.Tests.Controllers
{
    [TestClass]
    public class SANPHAMsControllerTest
    {
        [TestMethod]
        public void TestIndex()
        {
            var controller = new SANPHAMsController();

            var result = controller.Index() as ViewResult;            
            Assert.IsNotNull(result);

            var model = result.Model as List<SANPHAM>;
            Assert.IsNotNull(model);

            var db = new CsK24T25Entities();
            Assert.AreEqual(db.SANPHAMs.Count(), model.Count());
        }

        [TestMethod]
        public void TestIndex2()
        {
            var controller = new SANPHAMsController();

            var result = controller.Index2() as ViewResult;
            Assert.IsNotNull(result);

            var model = result.Model as List<SANPHAM>;
            Assert.IsNotNull(result);

            var db = new CsK24T25Entities();
            Assert.AreEqual(db.SANPHAMs.Count(), model.Count());
        }

        [TestMethod]
        public void TestCreateG()
        {
            var controller = new SANPHAMsController();

            var result = controller.Create() as ViewResult;
            Assert.IsNotNull(result);            
        }

        [TestMethod]
        public void TestCreateP()
        {
            var rand = new Random();
            var sanpham = new SANPHAM
            {
                MANCC = rand.Next(1, 5),
                MAKM = 7,
                LOAISP = rand.Next(1, 5),
                KICHTHUOC = rand.NextDouble().ToString(),
                MAUSAC = rand.NextDouble().ToString(),
                SOLUONG = rand.Next(),
                THONGTIN = rand.NextDouble().ToString(),
                GIABAN = -rand.Next(),
                NGAYTHEM = DateTime.Now,
                NGAYCAPNHAT = DateTime.Now
            };

            var controller = new SANPHAMsController();

            var result0 = controller.Create(sanpham, null) as ViewResult;
            Assert.IsNotNull(result0);
            Assert.AreEqual("Gia ban nho hon 0", controller.ModelState["GIABAN"].Errors[0].ErrorMessage);

            sanpham.GIABAN = -sanpham.GIABAN;
            controller.ModelState.Clear();

            result0 = controller.Create(sanpham, null) as ViewResult;
            Assert.IsNotNull(result0);
            Assert.AreEqual("Picture not found!", controller.ModelState[""].Errors[0].ErrorMessage);

            
            var picture =new Mock<HttpPostedFileBase>();
            var server = new Mock<HttpServerUtilityBase>();
            var context = new Mock<HttpContextBase>();
            context.Setup(c => c.Server).Returns(server.Object);
            controller.ControllerContext = new ControllerContext(context.Object,
                new System.Web.Routing.RouteData(), controller);

            var fileName = String.Empty;
            server.Setup(s => s.MapPath(It.IsAny<string>())).Returns<string>(s => s);
            picture.Setup(p => p.SaveAs(It.IsAny<string>())).Callback<string>(s => fileName = s);

            using (var scope = new TransactionScope())
            {
                controller.ModelState.Clear();
                var result1 = controller.Create(sanpham, picture.Object) as RedirectToRouteResult;
                Assert.IsNotNull(result1);
                Assert.AreEqual("Index", result1.RouteValues["action"]);

                var db = new CsK24T25Entities();
                var entity = db.SANPHAMs.SingleOrDefault(p => p.TENSP == sanpham.TENSP && p.THONGTIN == sanpham.THONGTIN);
                Assert.IsNotNull(entity);
                Assert.AreEqual(sanpham.GIABAN, entity.GIABAN);

                Assert.IsTrue(fileName.StartsWith("~/Upload/Product/"));
                Assert.IsTrue(fileName.EndsWith(entity.MASP.ToString()));
            }
        }

        [TestMethod]
        public void TestEditG()
        {
            var db = new CsK24T25Entities();
            
            var controller = new SANPHAMsController();
            var result0 = controller.Edit(0) as HttpNotFoundResult;
            Assert.IsNotNull(result0);


            var product = db.SANPHAMs.First();
            var result1 = controller.Edit(product.MASP) as ViewResult;
            
            Assert.IsNotNull(result1);

            var model = result1.Model as SANPHAM;
            Assert.IsNotNull(model);
            Assert.AreEqual(product.MANCC, model.MANCC);
            Assert.AreEqual(product.MAKM, model.MAKM);
            Assert.AreEqual(product.LOAISP, model.LOAISP);
            Assert.AreEqual(product.SOLUONG, model.SOLUONG);
            Assert.AreEqual(product.TINHTRANG, model.TINHTRANG);
            Assert.AreEqual(product.KICHTHUOC, model.KICHTHUOC);
            Assert.AreEqual(product.MAUSAC, model.MAUSAC);
            Assert.AreEqual(product.THONGTIN, model.THONGTIN);
            Assert.AreEqual(product.GIABAN, model.GIABAN);
            Assert.AreEqual(product.NGAYTHEM, model.NGAYTHEM);
            Assert.AreEqual(product.NGAYCAPNHAT, model.NGAYCAPNHAT);

        }

        [TestMethod]
        public void TestEditP()
        {
            var rand = new Random();
            var db = new CsK24T25Entities();
            var sanpham = db.SANPHAMs.AsNoTracking().First();
            sanpham.TENSP = rand.NextDouble().ToString();
            sanpham.KICHTHUOC = rand.NextDouble().ToString();
            sanpham.MAUSAC = rand.NextDouble().ToString();
            sanpham.SOLUONG = rand.Next();
            sanpham.THONGTIN = rand.NextDouble().ToString();
            sanpham.GIABAN = -rand.Next();

           

            var controller = new SANPHAMsController();

            var result0 = controller.Edit(sanpham, null) as ViewResult;
            Assert.IsNotNull(result0);
            Assert.AreEqual("Gia ban nho hon 0", controller.ModelState["GIABAN"].Errors[0].ErrorMessage);

           

           


            var picture = new Mock<HttpPostedFileBase>();
            var server = new Mock<HttpServerUtilityBase>();
            var context = new Mock<HttpContextBase>();
            context.Setup(c => c.Server).Returns(server.Object);
            controller.ControllerContext = new ControllerContext(context.Object,
                new System.Web.Routing.RouteData(), controller);

            var fileName = String.Empty;
            server.Setup(s => s.MapPath(It.IsAny<string>())).Returns<string>(s => s);
            picture.Setup(p => p.SaveAs(It.IsAny<string>())).Callback<string>(s => fileName = s);

            using (var scope = new TransactionScope())
            {
                sanpham.GIABAN = -sanpham.GIABAN;
                controller.ModelState.Clear();
                var result1 = controller.Edit(sanpham, picture.Object) as RedirectToRouteResult;
                Assert.IsNotNull(result1);
                Assert.AreEqual("Index", result1.RouteValues["action"]);
                var entity = db.SANPHAMs.Find(sanpham.MASP);
                Assert.IsNotNull(entity);
                Assert.AreEqual(sanpham.TENSP, entity.TENSP);
                Assert.AreEqual(sanpham.THONGTIN, entity.THONGTIN);
                Assert.AreEqual(sanpham.GIABAN, entity.GIABAN);

                Assert.AreEqual("~/Upload/Product/" + sanpham.MASP,fileName);

                
            }
        }
        [TestMethod]
        public void TestDeleteG()
        {
            var db = new CsK24T25Entities();

            var controller = new SANPHAMsController();
            var result0 = controller.Edit(0) as HttpNotFoundResult;
            Assert.IsNotNull(result0);


            var product = db.SANPHAMs.First();
            var result1 = controller.Delete(product.MASP) as ViewResult;

            Assert.IsNotNull(result1);

            var model = result1.Model as SANPHAM;
            Assert.IsNotNull(model);
            Assert.AreEqual(product.MANCC, model.MANCC);
            Assert.AreEqual(product.MAKM, model.MAKM);
            Assert.AreEqual(product.LOAISP, model.LOAISP);
            Assert.AreEqual(product.SOLUONG, model.SOLUONG);
            Assert.AreEqual(product.TINHTRANG, model.TINHTRANG);
            Assert.AreEqual(product.KICHTHUOC, model.KICHTHUOC);
            Assert.AreEqual(product.MAUSAC, model.MAUSAC);
            Assert.AreEqual(product.THONGTIN, model.THONGTIN);
            Assert.AreEqual(product.GIABAN, model.GIABAN);
            Assert.AreEqual(product.NGAYTHEM, model.NGAYTHEM);
            Assert.AreEqual(product.NGAYCAPNHAT, model.NGAYCAPNHAT);

        }

        [TestMethod]
        public void TestDeleteP()
        {
            var rand = new Random();
            var db = new CsK24T25Entities();
            var sanpham = db.SANPHAMs.AsNoTracking().First();
            sanpham.TENSP = rand.NextDouble().ToString();
            sanpham.KICHTHUOC = rand.NextDouble().ToString();
            sanpham.MAUSAC = rand.NextDouble().ToString();
            sanpham.SOLUONG = rand.Next();
            sanpham.THONGTIN = rand.NextDouble().ToString();
            sanpham.GIABAN = -rand.Next();



            var controller = new SANPHAMsController();

            var result0 = controller.Edit(sanpham, null) as ViewResult;
            Assert.IsNotNull(result0);
            Assert.AreEqual("Gia ban nho hon 0", controller.ModelState["GIABAN"].Errors[0].ErrorMessage);
            
            var server = new Mock<HttpServerUtilityBase>();
            var context = new Mock<HttpContextBase>();
            context.Setup(c => c.Server).Returns(server.Object);
            controller.ControllerContext = new ControllerContext(context.Object,
                new System.Web.Routing.RouteData(), controller);

            var filePath = String.Empty;
            var tempDir = System.IO.Path.GetTempFileName();
            server.Setup(s => s.MapPath(It.IsAny<string>())).Returns<string>(s => 
            {
                filePath = s;
                return tempDir;
            });
            

            using (var scope = new TransactionScope())
            {
                System.IO.File.Delete(tempDir);
                System.IO.Directory.CreateDirectory(tempDir);
                tempDir = tempDir + "/";
                System.IO.File.Create(tempDir + sanpham.MASP).Close();
                Assert.IsTrue(System.IO.File.Exists(tempDir + sanpham.MASP));
                var result1 = controller.DeleteConfirmed(sanpham.MASP) as RedirectToRouteResult;
                Assert.IsNotNull(result1);
                Assert.AreEqual("Index", result1.RouteValues["action"]);
                var entity = db.SANPHAMs.Find(sanpham.MASP);
                Assert.IsNull(entity);
                Assert.AreEqual(sanpham.TENSP, entity.TENSP);
                Assert.AreEqual(sanpham.THONGTIN, entity.THONGTIN);
                Assert.AreEqual(sanpham.GIABAN, entity.GIABAN);

                Assert.AreEqual("~/Upload/Product/" , filePath);
                Assert.IsFalse(System.IO.File.Exists(tempDir + sanpham.MASP));


            }
        }

        

        [TestMethod]
        public void TestPicture()
        {
            var controller = new SANPHAMsController();


            var server = new Mock<HttpServerUtilityBase>();
            var context = new Mock<HttpContextBase>();
            context.Setup(c => c.Server).Returns(server.Object);
            controller.ControllerContext = new ControllerContext(context.Object,
                new System.Web.Routing.RouteData(), controller);

            var filePath = String.Empty;
            var tempDir = System.IO.Path.GetTempFileName();
            server.Setup(s => s.MapPath(It.IsAny<string>())).Returns<string>(s => filePath = s);

            var result = controller.Picture(0) as FilePathResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("~/Upload/Product/0", result.FileName);
            Assert.AreEqual("images",result.ContentType);

        }

        [TestMethod]
        public void TestDispose()
        {
            using(var controller = new SANPHAMsController())
            {

            }
        }

        [TestMethod]
        public void TestDetails()
        {
            var db = new CsK24T25Entities();

            var controller = new SANPHAMsController();
            var result0 = controller.Details(0) as HttpNotFoundResult;
            Assert.IsNotNull(result0);


            var product = db.SANPHAMs.First();
            var result1 = controller.Details(product.MASP) as ViewResult;

            Assert.IsNotNull(result1);

            var model = result1.Model as SANPHAM;
            Assert.IsNotNull(model);
            Assert.AreEqual(product.MANCC, model.MANCC);
            Assert.AreEqual(product.MAKM, model.MAKM);
            Assert.AreEqual(product.LOAISP, model.LOAISP);
            Assert.AreEqual(product.SOLUONG, model.SOLUONG);
            Assert.AreEqual(product.TINHTRANG, model.TINHTRANG);
            Assert.AreEqual(product.KICHTHUOC, model.KICHTHUOC);
            Assert.AreEqual(product.MAUSAC, model.MAUSAC);
            Assert.AreEqual(product.THONGTIN, model.THONGTIN);
            Assert.AreEqual(product.GIABAN, model.GIABAN);
            Assert.AreEqual(product.NGAYTHEM, model.NGAYTHEM);
            Assert.AreEqual(product.NGAYCAPNHAT, model.NGAYCAPNHAT);

        }

        [TestMethod]
        public void TestSearch()
        {

            var db = new CsK24T25Entities();
            var sanphams = db.SANPHAMs.ToList();
            var keyword = sanphams.First().TENSP.Split().First();
            sanphams = sanphams.Where(p => p.TENSP.ToLower().Contains(keyword.ToLower())).ToList();

            var controller = new SANPHAMsController();
            var result = controller.Search(keyword) as ViewResult;
            Assert.IsNotNull(result);

            var model = result.Model as List<SANPHAM>;
            Assert.IsNotNull(result);

            Assert.AreEqual(sanphams.Count(), model.Count());
            Assert.AreEqual("Index2", result.ViewName);
            Assert.AreEqual(keyword, result.ViewData["keyword"]);

        }
    }
}
