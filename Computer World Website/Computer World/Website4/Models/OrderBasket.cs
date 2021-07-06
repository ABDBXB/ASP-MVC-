using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Website4.Models;

namespace Website4.Models
{
    public class OrderBasket
    {
        public Order order  { get; set; }
        public Basket basketslist { get; set; }

    }
}