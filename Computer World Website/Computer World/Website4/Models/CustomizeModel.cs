using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Website4.Models
{
    [MetadataType(typeof (ModelMetaData))]
    public partial class Model
    {

    }

    public class ModelMetaData
    {
        [Display(Name = "Marka")]
        public Nullable<int> BrandId { get; set; }

        [Display (Name ="Model")]
        public string Mname { get; set; }
    }

}