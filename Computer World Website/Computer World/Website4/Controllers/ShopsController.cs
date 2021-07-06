using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;
using Website4.Models;

namespace Website4.Controllers
{
    public class ShopsController : Controller
    {
        private DB_CWorld_V3Entities db = new DB_CWorld_V3Entities();
        // GET: Shops
        //Ürünler Sayfası Döndürü / Filtreleme Yapar 
        [HttpGet]
        public ActionResult Index(int ? brand, int? filter,int ? cat)
        {
            //IndexVM Modelin içinde Ürün listesi - Markalar listesi - Model listesi
            IndexVM model = new IndexVM();
            var products = db.Products.ToList();
            //ilk Başta View'den Gelen Filter IDye göre Sorgulama İşlenleri Yapıyoruz Çok Seçenek Bulanduğu İçin Switch CASE Kullandık
            //Fiyt Filtreleme
            switch (filter)
            {
                //Kullanıcı 1 Filtreleme Tıklayınca Demekki 250 - 500 Arasındaki Ürünler İstiyor
                case 1:
                    products = db.Products.Where(x => x.Price >= 250 && x.Price <= 500).ToList();
                    //Yeni Oluşturduğumuz Model Üç calssdan Veri Aldığı için Sayfa Döndürmeden Önce Doldurmamız lazım
                    model.products = products;
                    model.brands = GetBrands();
                    model.categories = GetCategorys();
                    return View(model);
                case 2:
                    products = db.Products.Where(x => x.Price >= 500 && x.Price <= 750).ToList();
                    model.products = products;
                    model.brands = GetBrands();
                    model.categories = GetCategorys();
                    return View(model);
                case 3:
                    products = db.Products.Where(x => x.Price >= 750 && x.Price <= 1000).ToList();
                    model.products = products;
                    model.brands = GetBrands();
                    model.categories = GetCategorys();
                    return View(model);
                case 4:
                    products = db.Products.Where(x => x.Price >= 1000 && x.Price <= 3000).ToList();
                    model.products = products;
                    model.brands = GetBrands();
                    model.categories = GetCategorys();
                    return View(model);
            }
            //Marka Filtreleme / Gelen Marka Idisine Göre Sorgulama Yapıyoruz.
            if(brand!=null)
            {
                products = db.Products.Include(p => p.Category1).Include(p => p.Model).Where(b => b.Model.BrandId== brand).ToList();
                model = new IndexVM();
                model.products = products;
                model.brands = GetBrands();
                model.categories = GetCategorys();
                return View(model);
            }
            //Kategori Filtreleme / Gelen Kategori Idisine Göre Sorgulama Yapıyoruz.
            else if (cat != null)
            {
                products = db.Products.Where(p => p.Category1.id==cat).ToList();
                model = new IndexVM();
                model.products = products;
                model.brands = GetBrands();
                model.categories = GetCategorys();
                return View(model);

            }
            else
            {
                //Filtreleme Yoksa Tüm Değerler Boş Girilmişse Asıl Sorgulama Yap (Filter Varsa Temizle ;) )
                products = db.Products.Include(p => p.Category1).Include(p => p.Model).ToList();

                model = new IndexVM();
                model.products = products;
                model.brands = GetBrands();
                model.categories = GetCategorys();
                return View(model);

            }

            



        }



        [HttpPost, ActionName("Index")]
        //Ürünler Sayfasından İstek Gelince (Satın Al Doğmesi Basılınca) Olacak Sepete Ekleme Olayı
        public ActionResult CreateBasket(int? quantity, int? productid)
        {
            int memberid = Convert.ToInt32(Session["Member_id"]);
            //Giriş Yapılmadıysa Uyarı Verilir
            if (memberid == 0)
            {
                IndexVM model = new IndexVM();
                var products = db.Products.ToList();

                ViewBag.LoginError = "Lütfen Ürün Almadan Giriş Yapınız!";

                products = db.Products.Include(p => p.Category1).Include(p => p.Model).ToList();

                model = new IndexVM();
                model.products = products;
                model.brands = GetBrands();
                model.categories = GetCategorys();

                return View(model);
            }
            Product product = db.Products.Where(p => p.id == productid).FirstOrDefault();
            int maxquantity = (int)product.Quantity;
            Basket basketitem = db.Baskets.Where(m => m.MemberId == memberid).Where(p => p.ProductId == productid).FirstOrDefault();
            if (basketitem == null)
            {
                Basket bas = new Basket();
                bas.MemberId = memberid;
                bas.ProductId = productid;
                bas.Quantity = quantity;
                bas.Active = true;
                db.Baskets.Add(bas);
                db.SaveChanges();
                return RedirectToAction("GetMemberBasket", "Baskets", new { memberid = memberid });

            }
            else if (basketitem != null)
            {
                if (maxquantity >= (basketitem.Quantity + quantity))
                {
                    int sumquantity = (int)(basketitem.Quantity + quantity);
                    basketitem.Quantity = sumquantity;
                    db.SaveChanges();
                    return RedirectToAction("GetMemberBasket", "Baskets", new { memberid = memberid });
                }
                else if (maxquantity < (basketitem.Quantity + quantity))
                {

                    TempData["not_enough"] = "Sepetekiyle Birlikte Stokta Bulunduğu Adet Aşımı!!";
                }


            }
            return RedirectToAction("Index", "Details", new { id = productid });


        }




        //Markalar Listesi Döndürür
        private List<Brand> GetBrands()
        {
            var brand = db.Brands.ToList();
            return brand;

        }
        //Kategori Listesi Döndürür
        private List<Category> GetCategorys()
        {
            var categories = db.Categories.ToList();
            return categories;

        }
    }
}