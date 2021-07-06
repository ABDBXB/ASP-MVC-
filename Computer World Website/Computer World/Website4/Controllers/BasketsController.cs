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
    public class BasketsController : Controller
    {
        private DB_CWorld_V3Entities db = new DB_CWorld_V3Entities();




        //Kullanıcın Sepet Bilgiler Getirir
        public ActionResult GetMemberBasket( int ? memberid)
        {
            //Kullanıcı Hesabı Yoksa Sepet Yok.
            if (memberid == null)
            {
                TempData["BosSepet"] = "Sepetiniz Boş (Giriş Yapınız ;)";
                return View();
            }else
            {
                //Kullanıcı Hesabıyla Giriş Yapıldıysa Sepeti Göster
                var baskets = db.Baskets.Include(b => b.Product).Where(m => m.MemberId == memberid).ToList();
                return View(baskets);
            }
            
        }
        //Stok Aşımı Hatası Göserir 
        [HttpGet]
        public ActionResult CreateBasket()
        {
            return View();
        }

        //Sepet Oluşturma || Sepete Ekleme
        [HttpPost]
        public ActionResult CreateBasket(int ? quantity,int ? productid)
        {
            int memberid = Convert.ToInt32(Session["Member_id"]);
            //kullanıcı Hesabıyla Giriş Yapılmadıysa Uyarı Ver
            if (memberid == 0)
            {
                ViewBag.LoginError = "Lütfen Ürün Almadan Giriş Yapınız!";
                return RedirectToAction("Home_Page","Home");
            }
            //Kullanıcı Giriş Yaptıysa / Aldığı Ürünün Özellikleri Gitir
            Product product = db.Products.Where(p => p.id == productid).FirstOrDefault();
            //İstenilen Ürürnün Max Adeti Değişkene Aldık 
            int maxquantity = (int)product.Quantity;
            //Bu kullanıcı Önceden Bu Üründen Almışsa Bana Getir
            Basket basketitem = db.Baskets.Where(m =>m.MemberId == memberid).Where(p => p.ProductId==productid).FirstOrDefault();
            //Kullanıcı Önceden Bu Üründen Almadıysa O zaman Sepete Yeni Ürün Olarak Ekle
            if (basketitem == null )
            {
                //Yeni Sepet Elemanı Oluşturup Bilgileri Verdik
                Basket bas = new Basket();
                bas.MemberId = memberid;
                bas.ProductId = productid;
                bas.Quantity = quantity;
                bas.Active = true;
                //Sepete Ekledik
                db.Baskets.Add(bas);
                db.SaveChanges();
                return RedirectToAction("GetMemberBasket", new { memberid = memberid });

            }else if(basketitem != null)
            {
                //Kullanıcı Bu Ürün Önceden Almışsa İstediği Adet Kontrol Edilir.
                //Eğer Önceden Eklediği Adet Ve Yeni Alacağı Adet Stokta Varsa.
                if(maxquantity>=(basketitem.Quantity+quantity))
                {
                    //iki Adedinin Toplamı Alır
                    int sumquantity = (int)(basketitem.Quantity + quantity);
                    //Alınmış Ürünün Yeni Adedi Olarak Güngellenir.
                    basketitem.Quantity = sumquantity;
                    db.SaveChanges();
                    return RedirectToAction("GetMemberBasket", new { memberid = memberid });
                }//Eğer Alacağı Adetler Stoktakilerden Fazla İse Uyarı Göster
                else if(maxquantity < (basketitem.Quantity + quantity))
                {

                    ViewBag.notEnough = "Sepetekiyle Birlikte Stokta Bulunduğu Adet Aşımı!!";
                    return View();
                }


            }
            return RedirectToAction("Index","Details",new { id = productid});


        }


        //Sepetten Ürün Sil -> Sepeti  Göster
        public ActionResult DeleteConfirmed(int id)
        {
            Basket basket = db.Baskets.Find(id);
            db.Baskets.Remove(basket);
            db.SaveChanges();
            return RedirectToAction("GetMemberBasket", new { memberid = Session["Member_id"]  });
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
