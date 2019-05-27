using System;
using System.Collections.Generic;
using Turing_Back_ED.DAL;

namespace Turing_Back_ED.Models
{
    public partial class Attribute : IEntity
    {
        public int AttributeId { get; set; }
        public string Name { get; set; }
        //public DateTimeOffset? Added { get; set; }
        //public DateTimeOffset? Modified { get; set; }
    }
}
