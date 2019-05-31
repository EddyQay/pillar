using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Turing_Back_ED.Workers;

namespace Turing_Back_ED.Models
{
    public partial class Tax : IEntity
    {
        public int TaxId { get; set; }
        public string TaxType { get; set; }
        public decimal TaxPercentage { get; set; }
        
    }
}
