using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Turing_Back_ED.DAL;

namespace Turing_Back_ED.Models
{
    public partial class OrderDetailItem : IEntity
    {
        public int OrderId { get; set; }
        public int ItemId { get; set; }
        public int ProductId { get; set; }
        public string Attributes { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal SubTotal { get; set; }
        public string Status { get; set; } 

        //[ForeignKey(nameof(OrderId))]
        //public Order Order { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }

        public OrderDetailItem From(OrderDetail orderItem)
        {
            Product = orderItem.Product;
            //Order = orderItem.Order;
            
            OrderId = orderItem.OrderId;
            ProductId = orderItem.ProductId;
            Attributes = orderItem.Attributes;
            ProductName = orderItem.ProductName;
            Quantity = orderItem.Quantity;
            UnitCost = Product.Price;
            SubTotal = Product.Price * Quantity;

            return this;
        }
    }
}
