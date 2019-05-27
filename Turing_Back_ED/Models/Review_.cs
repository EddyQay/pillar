using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Turing_Back_ED.Models
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public partial class Review_
    {
        [Key]
        public int ReviewId { get; set; }
        public int CustomerId { get; set; }
        public int ProductId { get; set; }
        public string Review { get; set; }
        public short Rating { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTimeOffset? Modified { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public Customer Customer { get; set; }
    }
}
