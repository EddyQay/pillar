using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Turing_Back_ED.DAL;

namespace Turing_Back_ED.Models
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public partial class AttributeValue
    {
        public int AttributeValueId { get; set; }
        public int AttributeId { get; set; }
        public string Value { get; set; }

        [ForeignKey(nameof(AttributeId))]
        public Attribute Attribute { get; set; }
    }
}
