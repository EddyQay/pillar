using System;
using System.Collections.Generic;
using Turing_Back_ED.DAL;

namespace Turing_Back_ED.Models
{
    public partial class Shipping : IEntity
    {
        public int ShippingId { get; set; }
        public string ShippingType { get; set; }
        public decimal ShippingCost { get; set; }
        public int ShippingRegionId { get; set; }
    }
}
