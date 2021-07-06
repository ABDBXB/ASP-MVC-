using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website4.Models;
using System.Data.Entity;


namespace Website4.Controllers
{
    public class DetailsController : Controller
    {
        private DB_CWorld_V3Entities db = new DB_CWorld_V3Entities();
        //GET: Details
        Product product = new Product();
        /*Bu Sayfa Hem Fotoğraflar Hem Ürünler Listeleyecek Bu yüzden 
        iki class bir classta (ProImage => Prodcut = Ürün , Image = Fotoğraf) Birleştirdik,
        Sayfada birden Fazla Fotoğraf Listelenecek Bu yüzden image Liste olarak Alınır
        Sadece Bir Ürünün Özellikleri Göstereceğimiz için bir Product Yeter Bize(Gerektirir)*/
        ProImage ProImage = new ProImage();

        //Kullanıcı için Ürün Detay
        public ActionResult Index(int? id)
        {
            product = db.Products.Find(id);
            var img = db.Images.Where(i => i.ProductId == id).ToList();
            ProImage.products = product;
            ProImage.Images = img;

            return View(ProImage);
        }

    }
}