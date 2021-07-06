using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Website4.Models;

namespace Website3.Controllers
{
    public class AdminPageController : Controller
    {
        private DB_CWorld_V3Entities db = new DB_CWorld_V3Entities();

        //Admin Ana Sayfası getirir (Kullanıcılar Listelenir)
        // GET: AdminPage
        public ActionResult Admin_Page()
        {
            //Korumak İçin Admin Hesabıyla Giriş Yapılmadıysa Giriş Yapıp Sonra Girebilir.
            if (Session["Admin_Name"] == null)
            {
                return RedirectToAction("Login_page", "Members");
            }
            return View(db.Members.OrderByDescending(m => m.id).ToList());
        }

        public ActionResult CreateMembers()
        {
            if (Session["Admin_Name"] == null)
            {
                return RedirectToAction("Login_page", "Members");
            }
            return View();
        }
        // Admin İçin Yeni Kullanıcı Ekleme Eylemi
        // POST: Members/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateMembers([Bind(Include = "id,FirstName,LastName,Email,Password_,Phone,Address_,Gender,Type_,ConfPass")] Member member)
        {
            //Admin Yeni Eklenen Kullancının Tipi Seçmezse Otomatik Kullanıcı Olarak Ayarlanır
            if (member.Type_ == null)
            {
                member.Type_ = true;
            }
            //Annotationler Sağlandıysa
            if (ModelState.IsValid)
            {
                //Bu Kullancının Hesapı Var mı Diye Kontrol Ettik
                var dbmember = db.Members.Where(m => m.Email == member.Email).ToList();
                //liste Sayısı 0'den Büyük Bu Email Bir Kullancıya Ait Demek
                if (dbmember.Count > 0)
                {
                    //Kullanıcıya Uyarı !!. 
                    ViewBag.MemberError = "Bu Hesap Önceden Ekelendi";
                    return View();
                }
                //Veritabanda Kaydı YOK Girilen Şifre Şifrelenir
                SHA256 sHA256 = new SHA256CryptoServiceProvider();
                byte[] bytes = sHA256.ComputeHash(Encoding.UTF8.GetBytes(member.Password_));
                StringBuilder builder = new StringBuilder();
                foreach (var item in bytes)
                {
                    builder.Append(item.ToString("x2"));
                }
                //Girilen Şifre Yerine Koyulur => Veri Tabana Yüklemey Hazır
                member.Password_ = builder.ToString();
                member.ConfPass = builder.ToString();

                //Yeni Kullanıcı Ekle
                db.Members.Add(member);
                db.SaveChanges();
                return RedirectToAction("Admin_Page");
            }
            //Sağlanmadıysa Geri Döndür Hata Yeri Söyle
            return View(member);
        }








        //Seçilen Kullancının Detay Sayfası getirir
        [HttpGet]
        public ActionResult DetailsMembers(int? id)
        {
            //ID Yoksa Hata
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Member member = db.Members.Find(id);
            //Bu ID'ye Ait Kullanıcı Yoksa Hata
            if (member == null)
            {
                return HttpNotFound();
            }
            //Kullanıcının Bilgileri View'da Göster
            return View(member);
        }

        //Üyenin Bilgileri Değiştirme Sayfası Getirir
        [HttpGet]
        public ActionResult EditMembers(int? id)
        {
            //Eylem Kullancının IDisine Göre Değiştirir. Bu ID Boş İse Hata Verir
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Veritaban'dan Sorgulama Yapar,Gelen ID Bir Kullancıya Ait Değilse Hata verir
            Member member = db.Members.Find(id);
            if (member == null)
            {
                return HttpNotFound();
            }
            //IDli Bir Kullancıya Ait O zaman Sayfayı Getir
            return View(member);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditMembers([Bind(Include = "id,FirstName,LastName,Email,Password_,Phone,Address_,Gender,Type_,ConfPass")] Member member)
        {
            if (ModelState.IsValid)
            {
                //Şifre Şifrelenir
            SHA256 sHA256 = new SHA256CryptoServiceProvider();
            byte[] bytes = sHA256.ComputeHash(Encoding.UTF8.GetBytes(member.Password_));
            StringBuilder builder = new StringBuilder();
            foreach (var item in bytes)
            {
                builder.Append(item.ToString("x2"));
            }
            string password = builder.ToString();
                member.Password_ = password;
                member.ConfPass = password;
            //Veri Tabanadaki Kullanıcıyı Değiştirir
                db.Entry(member).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Admin_Page");
            }
            return View(member);
        }

        //Silme Onaylama Sayfası Getirir
        public ActionResult DeleteMembers(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Member Member = db.Members.Find(id);
            if (Member == null)
            {
                return HttpNotFound();
            }
            return View(Member);
        }

        //Silme Onay Basılırsa
        // POST: Login/Delete/5
        [HttpPost, ActionName("DeleteMembers")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteMemebers(int id)
        {
            //Önce Kullancını Ettiği Siparişler Varsa Sileriz Sonra Kullancıyaı Silebiliriz.
            var orderslist = db.Orders.Where(m => m.MemberId == id).ToList();
            if (orderslist.Count > 0)
            {

            
            foreach(var item in orderslist)
            {
                var orderdesailslist = db.OrderDetails.Where(o => o.OrderId == item.id).ToList();
                foreach(var odetails in orderdesailslist)
                {
                    db.OrderDetails.Remove(odetails);
                    db.SaveChanges();
                }
                db.Orders.Remove(item);
                db.SaveChanges();
            }
            }
            //Veritabanda Bulup Silme İşlemi Gerçekleştirir
            Member MEMBER = db.Members.Find(id);

            db.Members.Remove(MEMBER);
            db.SaveChanges();
            return RedirectToAction("Admin_Page");
        }






    }
}