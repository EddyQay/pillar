using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Turing_Back_ED.DomainModels
{
    public class PaymentsInputModel
    {
        [Required]
        public string StripeToken { get; set; }

        [Required]
        public int OrderId { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public long Amount { get; set; }

        public string Currency { get; set; } = "USD";

        public KeyValuePair<string, int> MetaData { get; set; }
    }
}
