using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Turing_Back_ED.Workers;

namespace Turing_Back_ED.Models
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public partial class Category : IEntity
    {
        public int CategoryId { get; set; }
        public int DepartmentId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTimeOffset? Added { get; set; }
        public DateTimeOffset? Modified { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(DepartmentId))]
        public Department Department { get; set; }
    }
}
