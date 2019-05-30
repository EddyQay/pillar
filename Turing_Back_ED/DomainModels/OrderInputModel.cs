using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Turing_Back_ED.DomainModels
{
    public class OrderInputModel
    {
        [Required]
        public Guid CartId { get; set; }

        [Required]
        public int ShippingId { get; set; }

        [Required]
        public int TaxId { get; set; }

        public string Comments { get; set; }
        
        public int CustomerId { get; set; }
    }
}
