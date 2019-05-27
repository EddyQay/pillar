using System;
using System.Collections.Generic;
using Turing_Back_ED.DAL;

namespace Turing_Back_ED.Models
{
    public partial class Orders : IEntity
    {
        public int OrderId { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ShippedOn { get; set; }
        public int Status { get; set; }
        public string Comments { get; set; }
        public int? CustomerId { get; set; }
        public string AuthCode { get; set; }
        public string Reference { get; set; }
        public int? ShippingId { get; set; }
        public int? TaxId { get; set; }
        public DateTimeOffset? Added { get; set; }
        public DateTimeOffset? Modified { get; set; }
    }
}
