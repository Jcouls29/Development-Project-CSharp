using System.Collections.Generic;

namespace Interview.Web.Models
{
    public class SearchProductsRequest
    {
        public string NameContains { get; set; }
        public string DescriptionContains { get; set; }
        public string SkuContains { get; set; }

        /// <summary>Comma separated list of category IDs (e.g. "1,3,5")</summary>
        public string CategoryIds { get; set; }

        /// <summary>Comma separated keyvalue(e.g. "Color:Red,Brand:Nike")</summary>
        public string Metadata { get; set; }
    }
}
