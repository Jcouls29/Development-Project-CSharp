using System.Collections.Generic;

namespace Interview.Web.Models.Requests
{
    public class SearchProductsRequest
    {
        public string Name { get; set; }
        public int[] CategoryIds { get; set; }
        public Dictionary<string, string> Attributes { get; set; }
    }
}
