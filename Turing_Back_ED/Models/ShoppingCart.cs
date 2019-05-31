using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Turing_Back_ED.Workers;

namespace Turing_Back_ED.Models
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public partial class ShoppingCart : IEntity
    {
        public int ItemId { get; set; } = 0;

        [Required]
        [JsonIgnore]
        public Guid CartId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public string Attributes { get; set; }

        public int Quantity { get; set; }

        [JsonIgnore]
        public bool BuyNow { get; set; }
        public DateTimeOffset? Added { get; set; }
        public DateTimeOffset? Modified { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }

        public static explicit operator ShoppingCart(ShoppingCartForInput v)
        {
            ShoppingCart cart = new ShoppingCart()
            {
                ItemId = v.ItemId,
                CartId = v.CartId,
                ProductId = v.ProductId,
                Quantity = v.Quantity,
                Attributes = v.Attributes,
                BuyNow = v.BuyNow,
                Added = v.Added,
                Modified = v.Modified
            };

            return cart;
        }
    }
}
