using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Website4.Models
{
    [MetadataType(typeof(ImageMetaData))]
    public partial class Image
    {

    }
    public class ImageMetaData
    {
        public int id { get; set; }
        public Nullable<int> ProductId { get; set; }
        [StringLength(99,  ErrorMessage = "Fotoğraf Adı Çok Uzun lütfen Adı Kısaltıp Birdaha Yükleyiniz.")]
        [Display(Name = "Fotoğraf")]
        public string Image_ { get; set; }

        public virtual Product Product { get; set; }
    }
}