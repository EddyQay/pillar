using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Turing_Back_ED.Models
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public partial class OrderDetail
    {
        public int ItemId { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public string Attributes { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitCost { get; set; }

        //[ForeignKey(nameof(OrderId))]
        //public Order Order { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }

        public OrderDetail From(int orderId, ShoppingCartProductItem cartItem)
        {
            Product = cartItem.Product;
            ItemId = cartItem.ItemId;
            OrderId = orderId;
            ProductId = cartItem.ProductId;
            Attributes = cartItem.Attributes;
            Quantity = cartItem.Quantity;
            UnitCost = cartItem.Price;
            ProductName = Product.Name;

            return this;
        }
    }
}
