using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Turing_Back_ED.DAL;

namespace Turing_Back_ED.Models
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public partial class ShoppingCartForInput : IEntity
    {
        public int ItemId { get; set; }

        [Required]
        public Guid CartId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public string Attributes { get; set; }

        public int Quantity { get; set; }
        
        public bool BuyNow { get; set; }
        public DateTimeOffset? Added { get; set; }
        public DateTimeOffset? Modified { get; set; }


    }
}
