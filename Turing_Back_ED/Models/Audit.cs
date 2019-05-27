using System;
using System.Collections.Generic;
using Turing_Back_ED.DAL;

namespace Turing_Back_ED.Models
{
    public partial class Audit : IEntity
    {
        public int AuditId { get; set; }
        public int OrderId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Message { get; set; }
        public int Code { get; set; }
        public DateTimeOffset? Added { get; set; }
        public DateTimeOffset? Modified { get; set; }
    }
}
