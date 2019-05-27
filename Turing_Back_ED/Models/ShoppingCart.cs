using System;
using System.Collections.Generic;
using Turing_Back_ED.DAL;

namespace Turing_Back_ED.Models
{
    public partial class ShoppingCart : IEntity
    {
        public int ItemId { get; set; }
        public string CartId { get; set; }
        public int ProductId { get; set; }
        public string Attributes { get; set; }
        public int Quantity { get; set; }
        public byte BuyNow { get; set; }
        public DateTimeOffset? Added { get; set; }
        public DateTimeOffset? Modified { get; set; }
    }
}
