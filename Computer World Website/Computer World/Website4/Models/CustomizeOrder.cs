using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Website4.Models
{
    [MetadataType(typeof(OrderMetaData))]
    public partial class Order
    {
        
    }
    public class OrderMetaData
    {
        [Display(Name ="Sipariş ID")]
        public int id { get; set; }
        [Display(Name = "Kullanıcı Adı")]
        public Nullable<int> MemberId { get; set; }
        [Display(Name = "Toplam Fiyat")]
        public Nullable<double> TotalPrice { get; set; }
        [Display(Name = "Alış Tarihi")]
        public Nullable<System.DateTime> BuyingDate { get; set; }
        [Display(Name = "Teslim Adresi")]
        public string DeliveryAdrees { get; set; }
        [Display(Name = "Sipariş Durumu")]
        public Nullable<bool> Status_ { get; set; }
        public Nullable<bool> Active { get; set; }
    }
}