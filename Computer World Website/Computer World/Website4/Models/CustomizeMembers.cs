using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Website4.Models
{
    [MetadataType(typeof(MemberMetaData))]
    public partial class Member
    {
        [Display(Name = "Şifre Tekrar")]
        [Required(ErrorMessage = "Şifre Tekrarı Girilmeli")]
        [Compare("Password_", ErrorMessage = "şifreler uyuşmuyor!!")]
        public string ConfPass { get; set; }

    }

    public class MemberMetaData
    {



        [Required(ErrorMessage = "Bu alan Boş Geçirilmez")]
        [Display(Name = "AD")]
        public string FirstName { get; set; }
        [Display(Name = "Soyad")]
        [Required(ErrorMessage = "Bu alan Boş Geçirilmez")]
        public string LastName { get; set; }
        [RegularExpression(@"\b[\w\.-]+@[\w\.-]+\.\w{2,4}\b", ErrorMessage = "Email geçerli değil.")]
        [Required(ErrorMessage = "Bu alan Boş Geçirilmez")]
        public string Email { get; set; }
        [Display(Name = "Şifre")]
        [Required(ErrorMessage = "Şifre alanı Boş Geçirilmez")]
        [StringLength(64, MinimumLength = 8, ErrorMessage = "Şifre 8 ile 16 Karakter Arasında Olmamlıdır.")]
        //[RegularExpression(@"((?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[\W]).{8,64})", ErrorMessage = "Şifre Güçlü Değil Büyük Küçük Harflar ve numaralar içermeli")]
        public string Password_ { get; set; }

        [Display(Name = "Telefon")]
        [Required(ErrorMessage = "Telefon alanı Boş Geçirilmez")]
        [StringLength(16, MinimumLength = 8, ErrorMessage = "Telefon Numarınızı Kontrol Ediniz")]
        public string Phone { get; set; }
        [Display(Name = "Adres")]
        [Required(ErrorMessage = "Şifre alanı Boş Geçirilmez")]
        public string Address_ { get; set; }
        [Display(Name = "Cinsiyet")]
        [Required(ErrorMessage = "Şifre alanı Boş Geçirilmez")]
        public Nullable<bool> Gender { get; set; }
        [Display(Name = "Kullanıcı/Admin")]
        public Nullable<bool> Type_ { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order> Orders { get; set; }
    }
}
