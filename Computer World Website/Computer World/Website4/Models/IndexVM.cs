using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Website4.Models;

namespace Website4.Models
{
    public class IndexVM
    {
        public List<Product> products { get; set; }
        public List<Brand> brands { get; set; }
        public List<Category> categories { get; set; }
    }
}