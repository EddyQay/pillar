using System;
using System.Collections.Generic;

namespace Turing_Back_ED.Models
{
    public partial class ProductLocation
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
    }
}
