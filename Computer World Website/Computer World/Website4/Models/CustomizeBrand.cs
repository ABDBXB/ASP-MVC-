using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;


namespace Website4.Models
{
    [MetadataType(typeof(BrandMetaData))]
    public partial class Brand
    {

    }

    public class BrandMetaData
    {
        [Display(Name = "Marka")]
        public int id { get; set; }
        [Display(Name = "Marka")]
        public string Bname { get; set; }
    }
}