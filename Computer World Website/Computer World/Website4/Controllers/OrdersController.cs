using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using Rotativa;
using System.Web.Mvc;
using Website4.Models;

namespace Website4.Controllers
{
    public class OrdersController : Controller
    {
        private DB_CWorld_V3Entities db = new DB_CWorld_V3Entities();

        // GET: Orders
        //Eylem Siparişleri Listelem Sayfasına Yönlendirir
        public ActionResult Index()
        {
            if (Session["Admin_Name"] == null)
            {
                return RedirectToAction("Login_page", "Members");
            }
            var orders = db.Orders.Include(o => o.Member);
            return View(orders.ToList());
        }



        // GET: Orders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }



        //Kullancının ID'sine Göre Onun Siparişleri Veritabandan Getirir
        public ActionResult GetOrders()
        {
            int memberid = Convert.ToInt32(Session["Member_id"]);
            if (memberid == 0)
            {
                TempData["BosSiparis"] = "Siparişiniz Yok (Giriş Yapınız ;)";
                var EmptyOrder = db.Orders.Where(m => m.MemberId == memberid).ToList();
                return View(EmptyOrder);
            }
           
            var order = db.Orders.OrderByDescending(i => i.id).Where(m=>m.MemberId==memberid).ToList();
            return View(order);
        }
        //Admin TeslimEt Basınca Siparişin Durumu "True" Telimedildi olarak değişir
        public ActionResult UpdateDeliveryState(int? orderid)
        {
            Order order = db.Orders.Where(o => o.id == orderid).FirstOrDefault();
            order.Status_ = true;
            db.SaveChanges();
            return RedirectToAction("Index", "Orders");
        }


       
        //Kullanıcı Alışveriş Tamamla Basınca Yeni Yeni Sipariş Oluşturduk
        public ActionResult order()
        {
            
            int memberid = Convert.ToInt32(Session["Member_id"]);
            var member = db.Members.Find(memberid);
            Order order = new Order();
            order.MemberId = memberid;
            order.TotalPrice = 0;
            order.BuyingDate = DateTime.Now;
            order.DeliveryAdrees = member.Address_;
            order.Status_ = false;
            order.Active = false;

            //Siparişin Nitelikleri Hazır Şuanda Kaydetme Eylemine Gönderiyoruz

            return RedirectToAction("Create", "Orders", order);
        }


           //Biligler Bizim Tarafımızdan Girilecek => Eylem Tipi HttpPost Olması Gerek Yok(Olmaz)
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,MemberId,TotalPrice,BuyingDate,DeliveryAdrees,Status_,Active")] Order order)
        {
         
            //Annotaitonler Sağlandıysa
            if (ModelState.IsValid)
            {
                //Siparişi Ekle Ve Bu Siparişe Bağlı Bir Sipariş detay Aç
                db.Orders.Add(order);
                db.SaveChanges();
                OrderDetail orderDetail = new OrderDetail();
                //Yeni Eklenen Sipariş Detayın Sipariş Nomarası Yeni Oluşturduğmuz Siparişin IDsidir
                orderDetail.OrderId = order.id;
                return RedirectToAction("Create", "OrderDetails", orderDetail);

            }

            return View();
        }



        // GET: Orders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            ViewBag.MemberId = new SelectList(db.Members, "id", "FirstName", order.MemberId);
            return View(order);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,MemberId,TotalPrice,BuyingDate,DeliveryAdrees,Status_,Active")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MemberId = new SelectList(db.Members, "id", "FirstName", order.MemberId);
            return View(order);
        }

        // GET: Orders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            //Bu "orderid" Sipariş ID'ye Sipariş Detay Tablosundan Bağlanan Tüm üyeleri Silmek için Bir Listeye Aktardık 
            var orderdetails = db.OrderDetails.Where(o => o.OrderId == id).ToList();
            Order order = db.Orders.Find(id);
            //listedeki Ürünler Tek Tek Siliyoruz
            foreach (var item in orderdetails)
            {
                db.OrderDetails.Remove(item);
                db.SaveChanges();
            }
            //sonra O Siparişi Sileriz;
            db.Orders.Remove(order);
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
