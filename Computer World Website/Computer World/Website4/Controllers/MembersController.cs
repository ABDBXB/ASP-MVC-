using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Website4.Models;

namespace Website4.Controllers
{
    public class MembersController : Controller
    {
        private DB_CWorld_V3Entities db = new DB_CWorld_V3Entities();

        //kullanıcıya Şifre Untuma Sayfası Getirir
        [HttpGet]
        public ActionResult ForgetPassword()
        {
            return View();
        }

        //Kullanıcı Göderdiği Email Göre Şifre Untuma Fonksiyonu Çalışacaktır
        [HttpPost]
        public ActionResult ForgetPassword(String Email)
        {

            //To Validate Google recaptcha
            var response = Request["g-recaptcha-response"];
            string secretKey = "6LcCEQ4bAAAAAGwxxI0ZG5w6tc7GazenJdXlfpRY";
            var client = new WebClient();
            var result = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secretKey, response));
            var obj = JObject.Parse(result);
            var status = (bool)obj.SelectToken("success");

            //check the status is true or not
            if (status == true)
            {
                //Email'e Göre Sorgulama Sonucu Member Objesine Eklendi
                var Member = db.Members.Where(m => m.Email == Email).FirstOrDefault();

                //Member Boş Değilse (Girilen Email Bir Kullanıcıya Ait)
                if (Member != null)
                {
                    //Gönderelecek Email İçin Kullancının Adı Ve Soyadı Birleştirilir
                    string MemberName = Member.FirstName + " " + Member.LastName;
                    //Kullancının Yeni Şifresi Hazırlanır(random değer - deizi - Şifre Stringi)
                    string[] password = { "A", "1", "F", "B", "W", "Y", "9", "5", "I", "Q", "H", "C", "J", "K", "D", "M", "l", "L", "E", "O", "Z" };
                    string pass = null;
                    Random rand = new Random();
                    int number = 0;
                    for (int i = 0; i < 6; i++)
                    {
                        number = rand.Next(password.Length);
                        pass = pass + password[number].ToString();
                    }
                    //Yeni Şifre Elmizde

                    /*SmtpClient Hizmetin Ayarları:
                    SSL(Secure Sockets Layer) Şifreleme Kullanarak Göderilir
                    Zaman Aşımı Ayarlanır VB...
                    Önemli Olan Ağ Kimlik Bilgisi. Gmail Hesapı Ve Şifresi Virilir
                    Not: Bu İşlem Gerçekleştimek İçin Gmail Ayarlarına Uygulama Erişimi Etkinleştirilmeli*/
                    SmtpClient Mclient = new SmtpClient("smtp.gmail.com", 587);
                    Mclient.EnableSsl = true;
                    Mclient.Timeout = 10000;
                    Mclient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    Mclient.UseDefaultCredentials = false;
                    Mclient.Credentials = new NetworkCredential("konyarestoran@gmail.com", "1245GHKA.S");
                    //Yeni Gödelecek Mesaj Ayarlanır
                    MailMessage msg = new MailMessage();
                    //Hangi Hesaba Gedecek
                    msg.To.Add(Email);
                    //Kimin Tarafından Göndelecek
                    msg.From = new MailAddress("konyarestoran@gmail.com");
                    //Mesajın Başlığı / Konumu
                    msg.Subject = "Bilgisayar Dünyası Şifre Yenileme.";
                    //Mesajın İçeriğini:
                    //Kullancı Adı Verildi Ve Yeni Şifre=> sayin Abdulrahman Basaleh Yeni şifreniz: BlQEIM.
                    msg.Body = "sayin " + MemberName + " Yeni şifreniz: " + pass;
                    //En Son Mesaj Göderilir.
                    Mclient.Send(msg);

                    //Yeni Verilen Şifre Şifreleme
                    SHA256 sHA256 = new SHA256CryptoServiceProvider();
                    byte[] bytes = sHA256.ComputeHash(Encoding.UTF8.GetBytes(pass));
                    StringBuilder builder = new StringBuilder();
                    foreach (var item in bytes)
                    {
                        builder.Append(item.ToString("x2"));
                    }
                    //Kullanıcının Şifresi Değiştirdikten Sonra
                    Member.Password_ = builder.ToString();
                    Member.ConfPass = builder.ToString();
                    //Olan Diğişiklikler Veritabana Kaydedilir
                    db.SaveChanges();
                    //Uyarı Alınıda Mesaj Yazıldı
                    ViewBag.NoMember = "Yeni Şifreniz Emailinize Göderildi";


                }
                else
                {
                    //Boş ise Demeki Veri Tabanda Böyle Bir Email Yok (Kullanıcı Hesapı Yok) Uyarı Yazılır
                    ViewBag.NoMember = "Girdiniz Email Hiç Bir Kullanıcıya Ait Değil";
                    return View();
                }





            }
            else
            {
                ViewBag.Message = "Lütfen Captcha Control Edin !!";
                return View();
            }

            return View();








        
        }




        //kullanıcıya Kullanıcı Ekleme Sayfası Getirir
        [HttpGet]
        public ActionResult AddMember()
        {

            return View();
        }

        //Kullanıcıdan Girilen Biligiler Bu Eyleme Gönderilir.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddMember([Bind(Include = "id,FirstName,LastName,Email,Password_,Phone,Address_,Gender,Type_,ConfPass")] Member member)
        {
            //Hata Alanı Temizle
            ViewBag.MemberError = "";
            //Eğer Tüm Annotationler Doğruysa Bu işlemleri Gerçekleştirilir
            //Ör: Şifre ŞifreTekrar Aynı / Email Düzgün Bir Email / Gerekli Alanlar Boş Bırakılmamış
            if (ModelState.IsValid)
            {
                //Bu Kullancının Hesapı Var mı Diye Kontrol Ettik
            var dbmember = db.Members.Where(m => m.Email == member.Email).ToList();
            //liste Sayısı 0'den Büyük Bu Email Bir Kullancıya Ait Demek
            if (dbmember.Count > 0)
            {
                //Kullanıcıya Uyarı !!. 
                ViewBag.MemberError = "Zaten Hesapınız var";
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
            //Kullanıcı Olduğu İçin Her zaman Onun Tipi "True" (Kullanıcı Olmalı)
            member.Type_ = true;
            
                //Kullanıcı Veri tabana Başarıyla Kaydedildi
                db.Members.Add(member);
                db.SaveChanges();
                //Bu Yüzden Giriş Sayfasına Göderiliyor
                return RedirectToAction("Login_page");
            }
            //Annotationlerde Bir Hata Varsa Girdiği Bilgiler Döndürerek Aynı Sayfaya Yönlendiriyor
            return View(member);
        }


        //kullanıcıya Bilgileri Değiştirme Sayfası Getirir
        [HttpGet]
        public ActionResult EditMember(int? id)
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

        //Kullanıcı Tarafından Değiştirlen Biligiler Bu Eyleme Gönderilir.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditMember([Bind(Include = "id,FirstName,LastName,Email,Password_,Phone,Address_,Gender,Type_,ConfPass")] Member member)
        {
            //Annotationler Hepsi Doğru İse Modelin Durumu "True" İse.
                if (ModelState.IsValid)
                {
                    //Girilen Şifre Şifrelenir
                    SHA256 sHA256 = new SHA256CryptoServiceProvider();
                    byte[] bytes = sHA256.ComputeHash(Encoding.UTF8.GetBytes(member.Password_));
                    StringBuilder builder = new StringBuilder();
                    foreach (var item in bytes)
                    {
                        builder.Append(item.ToString("x2"));
                    }
                    string password = builder.ToString();
                //Şifre Ve Şifre Tekrarı Kontrol Edilecek Bu Yüzden Password_ ve ConfPass Aynı Omalı()
                    member.Password_ = password;
                    member.ConfPass = password;
                //Kullancının Tipi "True"=> Kullanıcı != Admin Değil
                    member.Type_ = true;
                //Girilen Değerleri Kontrol Edilir Ve VeriTabanada Güncellenir
                    db.Entry(member).State = EntityState.Modified;
                    db.SaveChanges();
                //Güncellendi Mesaj Gönderir
                ViewBag.EditDone = "Bilgileriniz Başarıyla Kaydeldi";
                return View();
                }

                return View(member);
        }



        //Login Yükleme Fonkisyonu
        [HttpGet]
        public ActionResult Login_page()
        {

            return View();
        }

        //Login Post Fonkisyonu
        [HttpPost]
        public ActionResult Login_page(Member Model)
        {
            //kullanıcı tarafından Gnderilen Model.Password_ ve Model.Email Boş olma Kontrolu 
            if (Model.Password_==null|| Model.Email==null)
            {
                //Gelen Değerler Eğer Boş ise Kullanıcıya Hata alanında Bu Uyarı Verilecek
                ViewBag.LoginError = "Lütfen Kullanıcı Adı ve Şifreyi Giriniz.";
                return View();

            }
            /*Girilen Boş Değilse : Girilen Şifre SHA256 kullanarak,using System.Security.Cryptography; kütüphanesi Ekleyerek
            Şifreinin Şifrelenmesi Foreach kullanarak Yeni Eklene StringBuildere Eklenir  */
            SHA256 sHA256 = new SHA256CryptoServiceProvider();
            byte[] bytes = sHA256.ComputeHash(Encoding.UTF8.GetBytes(Model.Password_));
            StringBuilder builder = new StringBuilder();
            foreach (var item in bytes)
            {
                builder.Append(item.ToString("x2"));
            }
            string password = builder.ToString();
            /*VeriTabandaki Şifreler SHA256 İle Şifrelendiği İçin Önce Girilen Şifrelenir Sonra Veritabandakilerle 
             Karşılaştırılır*/

            var member = db.Members.FirstOrDefault(x => x.Email == Model.Email && x.Password_ == password);
            /*Veritabanda Böyle bir Kullanıcı Bulunduysa (Gelen Boş Değilse (Not null/!=))*/
            if (member != null)
            {
                //Kullanıcının Tipi "False" İse Admin Sayfasına Yönlendirilir
                if (member.Type_ == false)
                {
                    //İsim Bilgileri Admin_Name Session'a Eklenir
                    Session["Admin_Name"] = member.FirstName + " " + member.LastName ;
                    //Admin AdminPage Controldeki Admin_Page Eyleme Göderilir
                    return RedirectToAction("Admin_Page", "AdminPage");
                }
                else
                {
                    //Kullanıcı Admin Değilse Kullanıcıdır /Onun Bilgileri Alınır 
                    //Tam İsmi Member_Name Session'a Eklenir
                    //ID'si Member_id Session'a Eklenir
                    Session["Member_Name"] = member.FirstName + " " + member.LastName;
                    Session["Member_id"] = member.id;
                    //Kullanıcı Home Controldeki Home_Page Eyleme Göderilir
                    return RedirectToAction("Home_Page", "Home");
                }


            }
            else
            {
                //Kullanıcı Veri Taban'da Bulamadıysa Hata İfadesi Hazırlayıp Kullancıya Gösterir
                ViewBag.LoginError = "Kullanıcı Adı Yada Şifre Hatalıdır!!";
                return View();
            }

            
        }




    }
}
