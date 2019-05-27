using System;
using System.Collections.Generic;
using Turing_Back_ED.DAL;

namespace Turing_Back_ED.Tests.Models
{
    public partial class ProductTest : IEntity
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountedPrice { get; set; }
        public string Image { get; set; }
        public string Image2 { get; set; }
        public string Thumbnail { get; set; }
        public short Display { get; set; }
        public DateTimeOffset? Added { get; set; }
        public DateTimeOffset? Modified { get; set; }
    }
}
