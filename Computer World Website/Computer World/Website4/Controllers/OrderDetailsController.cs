using Rotativa;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Website4.Models;

namespace Website4.Controllers
{
    public class OrderDetailsController : Controller
    {
        private DB_CWorld_V3Entities db = new DB_CWorld_V3Entities();

        // GET: OrderDetails
        public ActionResult Index()
        {
            var orderDetails = db.OrderDetails.Include(o => o.Order).Include(o => o.Product);
            return View(orderDetails.ToList());
        }

        // GET: OrderDetails/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderDetail orderDetail = db.OrderDetails.Find(id);
            if (orderDetail == null)
            {
                return HttpNotFound();
            }
            return View(orderDetail);
        }
        //Sipariş Detayı Kullancıya Gösterir
        public ActionResult GetOrderDetails(int ? orderid)
        {
            var orderdetails = db.OrderDetails.Where(o => o.OrderId == orderid).ToList();
            return View(orderdetails);

        }
        //Sipariş Detayı Admine Gösterir
        public ActionResult GODForAdmin(int? orderid)
        {
            var orderdetails = db.OrderDetails.Where(o => o.OrderId == orderid).ToList();
            return View(orderdetails);

        }
        //Siparişi Yazdırmak için Faturayı Web Sayfası  Olarak Hazırlanır
        public ActionResult GODForPrint(int? orderid)
        {
            var orderdetails = db.OrderDetails.Where(o => o.OrderId == orderid).ToList();
            return View(orderdetails);

        }
        //Kullanıcı Yada Admin Siparişi  Yazırmak isteyince 
        public ActionResult PrintOrderDetails(int? Oid)
        {
            //Rotativa Kullanrak Verdiğimiz Sayfayı Pdf Olarak  Çıktısını Alabiliriz.
            var print = new ActionAsPdf("GODForPrint", new { orderid= Oid });
            return print;
        }
        //Siparişi Aldıktan Sonra Sepeti Temizlem Eylemi
        public ActionResult DeleteBasketAfterSave()
        {
            int memberid = Convert.ToInt32(Session["Member_id"]);
            var basket = db.Baskets.Where(m => m.MemberId == memberid).ToList();
            foreach(var item in basket)
            {
                db.Baskets.Remove(item);
                db.SaveChanges();
            }


            return View();
        }
        //Siparil Slimek İstediğimiz vakit Önce IDye Göre Sipariş Detay Silinir  
        public ActionResult DeleteOrder(int ? orderid)
        {
            //Bu "orderid" Sipariş ID'ye Sipariş Detay Tablosundan Bağlanan Tüm üyeleri Silmek için Bir Listeye Aktardık 
            var orderdetails = db.OrderDetails.Where(o => o.OrderId == orderid).ToList();
            var order = db.Orders.Find(orderid);
            //listedeki Ürünler Tek Tek Siliyoruz
            foreach (var item in orderdetails)
            {
                db.OrderDetails.Remove(item);
                db.SaveChanges();
            }
            //sonra O Siparişi Sileriz;
            db.Orders.Remove(order);
            db.SaveChanges();
            return View();

        }

        //Özet:
        /*  Sepet Ürünler:      Sipariş:1       SiparişDetay:
         *  1.ürün                               1 - 1.Ürün                       
         *  2.ürün                               1 - 2.Ürün
         */

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,OrderId,ProductId,Quantity,PTotalPrice")] OrderDetail  orderDetail)
        {
            int orderid = (int)orderDetail.OrderId;

            int memberid = Convert.ToInt32(Session["Member_id"]);
            //Sepetteki Ürünler Listeye Aktardık
            var baskets = db.Baskets.Include(b => b.Product).Where(m => m.MemberId == memberid).ToList();
            //Yeni Sipariş Detay Listesi Üretik
            List<OrderDetail> orderDetaillist = new List<OrderDetail>();
            //Sepetteki Her Ürün Bilgileri SiparişDetay Objeye Eklenir Sonra Bu Obje Liste Eklenir 
            foreach (var item in baskets)
            {
                OrderDetail orderDetails = new OrderDetail();
                orderDetails.OrderId = orderid;
                orderDetails.ProductId = item.ProductId;
                orderDetails.Quantity = item.Quantity;
                orderDetaillist.Add(orderDetails);

            }
            if (ModelState.IsValid)
            {
                //Sonra Bu Listenin Her Elemanı Tek Tek Ekleriz
                foreach(var item in orderDetaillist)
                {
                    db.OrderDetails.Add(item);
                    db.SaveChanges();

                }
                //Ekledikten Sonra Septekki Tüm Ürünleri Sileriz
                return RedirectToAction("DeleteBasketAfterSave");
            }

            return View(orderDetail);
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
