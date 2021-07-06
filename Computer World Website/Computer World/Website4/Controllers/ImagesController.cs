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
    public class ImagesController : Controller
    {
        private DB_CWorld_V3Entities db = new DB_CWorld_V3Entities();

        // GET: Images
        //Fotoğraflar Listesi Gitiren Eylem
        public ActionResult Index()
        {
            if (Session["Admin_Name"] == null)
            {
                return RedirectToAction("Login_page", "Members");
            }
            var images = db.Images.Include(i => i.Product);
            return View(images.ToList());
        }

        // GET: Images/Details/5
        //Fotoğraf Detayı Gitiren Eylem
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Image image = db.Images.Find(id);
            if (image == null)
            {
                return HttpNotFound();
            }
            return View(image);
        }

        // GET: Images/Create
        //Fotoğraf Oluşturma Eylemi
        public ActionResult Create()
        {
            if (Session["Admin_Name"] == null)
            {
                return RedirectToAction("Login_page", "Members");
            }
            //Ürün Listesi İçin Yeni Listeyi Oluşturup  ViewBag Ekleriz, Selectlist(Veritabandan Value=id,Text=Pname)
            ViewBag.ProductId = new SelectList(db.Products, "id", "Pname");
            return View();
        }

        // POST: Images/Create
        //Yeni Fotoğraf Ekleyince Bu Eylem Çalışır
        /*Note: Bind[Exclude="id"] Fotoğraf Objeyi Hepsini Alıyoruz ID Haric
                Bind[Include="id"] Fotoğraf Objeyinin ID'si Alıyoruz Sadece*/
        [HttpPost]
        [ValidateAntiForgeryToken]
        //Kullanıcı Bize Fotoğrafı Dosya "Fileimage" Olarak Gönderdi 
        public ActionResult Create([Bind(Include =  "id,ProductId,Image_")] Image image, HttpPostedFileBase fileimage)
        {
            //Annotationler Sağlandıysa
            if (ModelState.IsValid)
            { //Fotoğrafın Yolu Kaydetmek için Path Stringi Oluşturulur
                string path = "";
                //Fotoğraf Yüklenmediyse
                    if (fileimage == null)
                    {
                    //Hatayı TempData["result-error"] Aktardık Kullancıya Gösterdik
                    TempData["result-error"] ="Fotoğraf Yüklenmeli";
                    ViewBag.ProductId = new SelectList(db.Products, "id", "Pname");
                    return View();
                    }
                     //Dosyanın Adı Çok Uzun İse
                   if (fileimage.FileName.Length > 100)
                    {
                    //Hatayı TempData["result-error"] Aktardık Kullancıya Gösterdik
                    TempData["result-error"] = "Fotoğraf Adı Çok Uzun lütfen Adı Kısaltıp Birdaha Yükleyiniz.";
                    ViewBag.ProductId = new SelectList(db.Products, "id", "Pname");
                    return View();
                     }
                    //Dosya Yülendiyse Eğer
                   else if (fileimage.FileName.Length > 0)
                    {
                        //Fotoğraf Kaydedilecek Yolu Verdik
                        path = "~/images/Product/" + Path.GetFileName(fileimage.FileName);
                        //Ayarladığımız Yola Kaydediyouruz
                        fileimage.SaveAs(Server.MapPath(path));
                    }
                   //Yolu İmage Objeye Veriyoruz
                    image.Image_ = path;

                //Veri Taban Kaydetme Kullanıcıya Sunuç Verdik Sayfayi Geri Gösterdik 
                db.Images.Add(image);
                db.SaveChanges();
                TempData["result-success"] = "Fotoğraf Yüklendi";
                ViewBag.ProductId = new SelectList(db.Products, "id", "Pname");
                return View();
            }
            //Annotationler Sağlanmadıysa Sayfayı Geri Göster
            ViewBag.ProductId = new SelectList(db.Products, "id", "Pname", image.ProductId);
            return View(image);
        }

        // GET: Images/Edit/5
        //Edit Eylemi Fotoğraf Değiştirme Sayfası Döndürür
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Image image = db.Images.Find(id);
            if (image == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductId = new SelectList(db.Products, "id", "Pname", image.ProductId);
            return View(image);
        }

        // POST: Images/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,ProductId,Image_")] Image image, HttpPostedFileBase fileimage)
        {
            if (ModelState.IsValid)
            {
                string path = "";

                if (fileimage == null)
                {
                    TempData["result-error"] = "Fotoğraf Yüklenmeli";
                    ViewBag.ProductId = new SelectList(db.Products, "id", "Pname");
                    return View();
                }
                if (fileimage.FileName.Length > 100)
                {
                    //Hatayı TempData["result-error"] Aktardık Kullancıya Gösterdik
                    TempData["result-error"] = "Fotoğraf Adı Çok Uzun lütfen Adı Kısaltıp Birdaha Yükleyiniz.";
                    ViewBag.ProductId = new SelectList(db.Products, "id", "Pname");
                    return View();
                }
                else if (fileimage.FileName.Length > 0)
                {
                    path = "~/images/Product/" + Path.GetFileName(fileimage.FileName);
                    fileimage.SaveAs(Server.MapPath(path));
                }
                image.Image_ = path;



                TempData["result-success"] = "Fotoğraf Yüklendi";

                db.Entry(image).State = EntityState.Modified;
                db.SaveChanges();
                ViewBag.ProductId = new SelectList(db.Products, "id", "Pname");
                return View();
            }
            ViewBag.ProductId = new SelectList(db.Products, "id", "Pname", image.ProductId);
            return View(image);
        }

        // GET: Images/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Image image = db.Images.Find(id);
            if (image == null)
            {
                return HttpNotFound();
            }
            return View(image);
        }

        // POST: Images/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Image image = db.Images.Find(id);
            db.Images.Remove(image);
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
