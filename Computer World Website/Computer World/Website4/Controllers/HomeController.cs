using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website4.Models;

namespace Website4.Controllers
{
    public class HomeController : Controller
    {
        private DB_CWorld_V3Entities db = new DB_CWorld_V3Entities();
        // AnaSafayı Gösterir
        public ActionResult Home_Page()
        {
           
                var products = db.Products.Include(p => p.Category1).Include(p => p.Model).OrderByDescending(i => i.id);

                return View(products);

            

        }

        //AnaSayfadan Gelen İstek (Bir Ürün Sepete Ekleme)
        [HttpPost,ActionName("Home_Page")]
        public ActionResult CreateBasket(int? quantity, int? productid)
        {
            int memberid = Convert.ToInt32(Session["Member_id"]);
            //Kullanıcı Giriş Yapmadıysa Giriş Uyarısı Gösterir
            if (memberid == 0)
            {
                ViewBag.LoginError = "Lütfen Ürün Almadan Giriş Yapınız!";
                //Anasayfayı Göstermek için Ürünler Listesi Göndermemiz Gerekiyor
                var products = db.Products.Include(p => p.Category1).Include(p => p.Model);
                return View(products);
            }
            Product product = db.Products.Where(p => p.id == productid).FirstOrDefault();
            //İstenilen Ürünün Max Adedi Alınır
            int maxquantity = (int)product.Quantity;
            Basket basketitem = db.Baskets.Where(m => m.MemberId == memberid).Where(p => p.ProductId == productid).FirstOrDefault();
            //Kullanıcı Önceden Bu Üründen Almadıysa O zaman Sepete Yeni Ürün Olarak Ekle
            if (basketitem == null)
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
                return RedirectToAction("GetMemberBasket","Baskets", new { memberid = memberid });

            }
            else if (basketitem != null)
            {
                //Kullanıcı Bu Ürün Önceden Almışsa İstediği Adet Kontrol Edilir.
                //Eğer Önceden Eklediği Adet Ve Yeni Alacağı Adet Stokta Varsa.
                if (maxquantity >= (basketitem.Quantity + quantity))
                {
                    int sumquantity = (int)(basketitem.Quantity + quantity);
                    basketitem.Quantity = sumquantity;
                    db.SaveChanges();
                    return RedirectToAction("GetMemberBasket","Baskets", new { memberid = memberid });
                }
                else if (maxquantity < (basketitem.Quantity + quantity))
                {

                    TempData["not_enough"] = "Sepetekiyle Birlikte Stokta Bulunduğu Adet Aşımı!!";
                }


            }
            return RedirectToAction("Index", "Details", new { id = productid });


        }





    }
}