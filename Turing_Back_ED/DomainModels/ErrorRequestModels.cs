using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Turing_Back_ED.DomainModels
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public partial class BadRequestModel
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public string Field { get; set; }
        public int Status { get; set; }
        
        public IEnumerable<JToken> Errors { get; set; }

        public override string ToString()
        {
            return "model";
        }
    }

    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public partial class ErrorRequestModel
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public int Status { get; set; }
    }

    public partial class Fields
    {
        public string Field { get; set; }
        public string Message { get; set; }
    }
}
