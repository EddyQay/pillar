using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Turing_Back_ED.Models
{
    public partial class ProductCategory
    {
        public int ProductId { get; set; }
        public int CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; }
    }
}
