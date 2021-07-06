using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Website4.Models;

namespace Website4.Controllers
{
    public class ProductsController : Controller
    {
        private DB_CWorld_V3Entities db = new DB_CWorld_V3Entities();

        // GET: Products
        //Admine Ürünleri Göstermek İçin filtreler
        public ActionResult Index(int? id)
        {
            if (Session["Admin_Name"] == null)
            {
                return RedirectToAction("Login_page", "Members");
            }
            if (id == 1)
            {
                var products = db.Products.OrderBy(p => p.Price).ToList();
                return View(products);
            }
            else if (id == 2)
            {
                var products = db.Products.OrderByDescending(p => p.Price).ToList();
                return View(products);

            }
            else if (id == 3)
            {
                var products = db.Products.Include(p => p.Category1).Where(c => c.Category == 1).ToList();
                return View(products);

            }
            else if (id == 4)
            {
                var products = db.Products.Include(p => p.Category1).Where(c => c.Category == 2).ToList();
                return View(products);

            }
            else if (id == 5)
            {
                var products = db.Products.Include(p => p.Category1).Where(c => c.Category == 3).ToList();
                return View(products);

            }
            else if (id == 6)
            {
                var products = db.Products.Include(p => p.Category1).Where(c => c.Category == 4).ToList();
                return View(products);

            }
            else
            {
                var products = db.Products.OrderBy(p => p.Price).Include(p => p.Category1).Include(p => p.Model);
                return View(products.ToList());
            }
            
        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        //Ürün Oluşturma Sayfası İki Liste İstiyor 
        public ActionResult Create()
        {
            if (Session["Admin_Name"] == null)
            {
                return RedirectToAction("Login_page", "Members");
            }
            //DropDownlistenin (Valuesu,Text Değerler) Alıyoruz
            ViewBag.Category = new SelectList(GetCategorieList(), "id", "Cname");
            ViewBag.Brands = new SelectList(GetBrandsList(), "id", "Bname");
            return View();
        }

        //Bu Fonkisyon Kategori Listesi Getirir
        public List<Category> GetCategorieList()
        {
            List<Category> categories = db.Categories.ToList();
            return categories;

        }
        //Marka Listesi Getirir
        public List<Brand> GetBrandsList()
        {
            List<Brand> brands = db.Brands.ToList();
            return brands;

        }
        //[HttpPost]
        //public ActionResult GetBrandList(int Cateid)
        //{
        //    var sql = (from model in db.Models
        //               join bra in db.Brands
        //               on model.BrandId equals bra.id
        //               join pro in db.Products
        //               on model.id equals pro.ModelId
        //               join cate in db.Categories
        //               on pro.Category equals cate.id
        //               where cate.id == Cateid
        //               select  new
        //               {
        //                   brandid = bra.id,
        //                   bname=bra.Bname
        //               }

        //        ).Distinct().ToList();
        //    List<Brand> brands = new List<Brand>();

        //    foreach (var i in sql)
        //    {
        //        Brand brand = new Brand();
        //        brand.id = i.brandid;
        //        brand.Bname = i.bname;
        //        brands.Add(brand);

        //    }

        //    ViewBag.brandlist = new SelectList(brands, "id", "Bname");
        //    return PartialView("DisplayBrands");
        //}
        //Kullanıcı Seçtiği Markanın IDsine Göre Modelleri Gitirip
        public ActionResult GetModelList(int Brandid)
        {
            //Sorgulama Yapıldı SelectListeye Eklendi
            List<Model> modellist = db.Models.Where(b => b.BrandId== Brandid).ToList();
            ViewBag.modellist = new SelectList(modellist, "id", "Mname");
            //Kısmi Görünüm : Sayfa Değil Sayfanın Bir Parçası Döndürür.
            return PartialView("DisplayModels");
        }


        //Ürün Oluşturma
        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,ModelId,Category,Pname,Explanation,Price,Quantity,Image_,Status_")]  Product products, HttpPostedFileBase image)
        {
            //Yeni Eklenen Ürünün Stok Değeri Her Zaman "False" => VAR Olarak Ayalanır Çünkü Anntation Kullanarak Kontrol Ettik
            products.Status_ = false;
            if (ModelState.IsValid)
            {
                //Fotoğraf Eklenmediyse Veri Döndür Uyarı Ver.
                if (image == null)
                {
                    ViewBag.Category = new SelectList(db.Categories, "id", "Cname");
                    ViewBag.ModelId = new SelectList(db.Models, "id", "Mname");
                    ViewBag.Brands = new SelectList(GetBrandsList(), "id", "Bname");
                    ViewBag.error = "En az Bir Fotoğraf Yüklenmeli";
                    return View();
                }
                //Fotoğrafın Adı Çok Uzun ise Uyarı Döndürür.
                if (image.FileName.Length > 100)
                {
                    ViewBag.Category = new SelectList(db.Categories, "id", "Cname");
                    ViewBag.ModelId = new SelectList(db.Models, "id", "Mname");
                    ViewBag.Brands = new SelectList(GetBrandsList(), "id", "Bname");
                    ViewBag.error = "Fotoğraf Adı Çok Uzun lütfen Adı Kısaltıp Birdaha Yükleyiniz.";
                    return View();
                }
                //Fotoğrafın Yolu Kaydetmek için Path Stringi Oluşturulur
                string path = "";
                //Dosya Yülendiyse Eğer
                if (image.FileName.Length > 0)
                {
                    path = "~/images/Product/" + Path.GetFileName(image.FileName);
                    image.SaveAs(Server.MapPath(path));
                }
                //Yolu İmage Objeye Veriyoruz
                products.Image_ = path;
                db.Products.Add(products);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Category = new SelectList(db.Categories, "id", "Cname", products.Category);
            ViewBag.ModelId = new SelectList(db.Models, "id", "Mname", products.ModelId);
            return View(products);
        }
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }
        //Admin Ürün Silmek İstediğinde Bu Eylem Çalışır
        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteImageProduct(int id)
        {
            //IDye Göre Ürünü buluyoruz
            Product product = db.Products.Find(id);
            var OrderDetails = db.OrderDetails.Where(p => p.ProductId == id).ToList();
            var Baskets = db.Baskets.Where(p => p.ProductId == id).ToList();
            //Bir Kullanıcı Onu Aldıysa silemeyiz Çünkü Alındı
            if(OrderDetails.Count > 0){
                ViewBag.OrderError = "Bu Ürün Kullanıcılar Tarafından Alındığı için Silemeyiz";
                return View(product);
            }
            //Bu Üründen Her Hangi Sepete Eklendiyse O Sepetten Silenecek.
            if (Baskets.Count > 0)
            {
                foreach (var item in Baskets)
                {
                    db.Baskets.Remove(item);
                }
            }
            var ProductImages = db.Images.Where(p => p.ProductId == id);
            //Ürünün Fotoğrafları Varsa Bunları Alıp Siliyoruz 
            foreach(var item in ProductImages)
            {
                db.Images.Remove(item);
            }
            //Fotoğrafları Sildikten Sonra Ürünü Silebiliriz / Uyarı Veririz :)
            db.Products.Remove(product);
            //VeriTaban Değişiklikleri Kaydet
            db.SaveChanges();
            ViewBag.DeleteDone = "Ürün Başarıyla Silindi";
            return RedirectToAction("Index");
        }


        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.Category = new SelectList(db.Categories, "id", "Cname", product.Category);
            ViewBag.ModelId = new SelectList(db.Models, "id", "Mname", product.ModelId);
            return View(product);
        }

        //Admin Ürünün Bilgileri Değiştirmek istediğiz vakit Bu Eylem Çalışır
        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,ModelId,Category,Pname,Explanation,Price,Quantity,Image_,Status_")] Product product, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                //Admin Yeni Bir Fotoğraf Yüklemediyse
                if (image == null)
                {
                    //IDye Göre Ürünü Bulup Özellikleri Aktarıyoruz
                    var productinsert = db.Products.Where(p => p.id == product.id).FirstOrDefault();
                    productinsert.ModelId = product.ModelId;
                    productinsert.Category = product.Category;
                    productinsert.Pname = product.Pname;
                    productinsert.Price = product.Price;
                    productinsert.Explanation = product.Explanation;
                    productinsert.Quantity = product.Quantity;
                    //Admin Ürünü Stok Durumu Girmdiyse
                    if (product.Status_ == null)
                    {
                        //Yeni Eklenen Adet Sıfırdan Büyükse 
                        if (product.Quantity > 0)
                        {
                            //Stokta bu ürün vardır Demek
                            productinsert.Status_ = false;
                        }
                    }
                    else
                    {
                        //Durumu Girdiye Durum Ne Girdiyse O
                        productinsert.Status_ = product.Status_;
                    }
                    //Kaydet
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    //Fotoğtaf Boş Değil Ama Adı Çok Uzun Uyarı Ver Döndür Sayfayı 
                    if (image.FileName.Length > 100)
                    {
                        ViewBag.Category = new SelectList(db.Categories, "id", "Cname");
                        ViewBag.ModelId = new SelectList(db.Models, "id", "Mname");
                        ViewBag.error = "Fotoğraf Adı Çok Uzun lütfen Adı Kısaltıp Birdaha Yükleyiniz.";
                        return View();
                    }
                    //Fotoğrafı Kaydet
                    string path = "";
                    if (image.FileName.Length > 0)
                    {
                        path = "~/images/Product/" + Path.GetFileName(image.FileName);
                        image.SaveAs(Server.MapPath(path));
                    }
                    product.Image_ = path;

                    //Yeni Eklenen Adet Sıfırdan Büyükse 
                    if (product.Status_ == null)
                    {
                        //Yeni Eklenen Adet Sıfırdan Büyükse 
                        if (product.Quantity > 0)
                        {
                            //Stokta bu ürün vardır
                            product.Status_ = false;
                        }
                    }
                    //Yoksa Girileni Kaydet



                    db.Entry(product).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            ViewBag.Category = new SelectList(db.Categories, "id", "Cname", product.Category);
            ViewBag.ModelId = new SelectList(db.Models, "id", "Mname", product.ModelId);
            return View(product);
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
