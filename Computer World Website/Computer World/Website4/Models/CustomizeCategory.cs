using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Website4.Models
{
    [MetadataType(typeof(CategoryMetaData))]
    public partial  class Category
    {
    }

    public class CategoryMetaData
    {
        public int id { get; set; }
        [Display (Name = "Kategori")]
        public string Cname { get; set; }

    }
}