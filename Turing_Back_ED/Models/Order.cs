using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Turing_Back_ED.Workers;

namespace Turing_Back_ED.Models
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public partial class Order : IEntity
    {
        public int OrderId { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ShippedOn { get; set; }
        public int Status { get; set; }
        public string Comments { get; set; }
        public int? CustomerId { get; set; }

        [JsonIgnore]
        public string AuthCode { get; set; }

        public string Reference { get; set; }
        public int? ShippingId { get; set; }
        public int? TaxId { get; set; }
        
        public List<OrderDetail> OrderDetails { get; set; }
    }
}
