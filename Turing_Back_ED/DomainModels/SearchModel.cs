using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Turing_Back_ED.Models;

namespace Turing_Back_ED.DomainModels
{
    public partial class SearchModel
    {
        [Required (ErrorMessage = "'query_string' field is required, and thus must not be empty")]
        public string Query_String { get; set; }
        public string All_Words { get; set; } = "on";
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 20;
        public int Description_Length { get; set; } = 200;
    }

    public partial class SearchResponseModel
    {
        public int Count { get; set; }
        public IEnumerable<object> Rows { get; set; }
    }

    public partial class ProductLocationsModel
    {
        public int Count { get; set; }
        public IEnumerable<ProductLocation> Rows { get; set; }
    }

    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public partial class GeneralQueryModel
    {
        public int Page { get; set; } = 1;
        
        public int Limit { get; set; } = 20;
        
        public int Description_Length { get; set; } = 200;
    }

    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public partial class CategoryQueryModel
    {
        public int Page { get; set; } = 1;

        public int Limit { get; set; } = 20;

        public string Order { get; set; } = Utilities.Constants.DefaultSortOrder;
    }

}
