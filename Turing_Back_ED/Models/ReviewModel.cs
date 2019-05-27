using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Turing_Back_ED.Models
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public partial class ReviewModel
    {
        [JsonRequired]
        public string Review { get; set; }

        [JsonRequired]
        public short Rating { get; set; }
    }
}
