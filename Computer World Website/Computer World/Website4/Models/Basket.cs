//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Website4.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Basket
    {
        public int id { get; set; }
        public Nullable<int> BasketNo { get; set; }
        public Nullable<int> ProductId { get; set; }
        public Nullable<int> MemberId { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<bool> Active { get; set; }
    
        public virtual Member Member { get; set; }
        public virtual Product Product { get; set; }
    }
}
