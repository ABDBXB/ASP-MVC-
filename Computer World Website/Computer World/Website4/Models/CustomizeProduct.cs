using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Website4.Models
{
    [MetadataType(typeof(ProductMetaData))]
    public partial class Product
    {
    }

    public class ProductMetaData
    {
        public int id { get; set; }
        [Required(ErrorMessage = "Bu Alan Boş Geçirilmez.")]
        public Nullable<int> ModelId { get; set; }
        [Required(ErrorMessage = "Bu Alan Boş Geçirilmez.")]
        public Nullable<int> Category { get; set; }
        [Required(ErrorMessage = "Bu Alan Boş Geçirilmez.")]
        [Display(Name="Ürün Adı")]
        public string Pname { get; set; }
        [Required(ErrorMessage = "Bu Alan Boş Geçirilmez.")]
        [StringLength(800, MinimumLength = 10, ErrorMessage = "Açıklama 10 ile 800 Karakter Arasında Olmalıdır ")]
        [Display(Name ="Açıklama")]
        public string Explanation { get; set; }
        [Required(ErrorMessage ="Fiyat Boş Geçirilmez.")]
        [RegularExpression(@"[0-9]*$", ErrorMessage = "Bu Alan'da Sadece Sayılar Geçerlidir.")]
        [Display(Name ="Fiyat")]
        [Range(1,1000000,ErrorMessage = "Lütfen Fiyat Bilgisi Kontrol Ediniz.!!")]
        public Nullable<double> Price { get; set; }
        [Required(ErrorMessage = "Bu Alan Boş Geçirilmez.")]
        [Display (Name ="Adet")]
        [Range(0, 1000000, ErrorMessage = "Lütfen Adet Bilgisi Kontrol Ediniz.!!")]
        public Nullable<int> Quantity { get; set; }
        
        [Display (Name ="Fotoğraf")]
        public string Image_ { get; set; }
        [Display(Name ="Stok Durumu")]
        public Nullable<bool> Status_ { get; set; }
        [Display (Name ="Kategori")]
        public virtual Category Category1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Image> Images { get; set; }
        [Display (Name ="Model")]
        public virtual Model Model { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}