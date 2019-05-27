using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Turing_Back_ED.Models
{
    public partial class ProductAttribute
    {
        public int ProductId { get; set; }
        public int AttributeValueId { get; set; }

        [ForeignKey(nameof(AttributeValueId))]
        public AttributeValue AttributeValue { get; set; }
    }
}
