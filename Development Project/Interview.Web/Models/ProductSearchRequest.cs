using System.Collections.Generic;

namespace Interview.Web.Models
{
    public class ProductSearchRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ValidSkus { get; set; }
        public Dictionary<string, string> Attributes { get; set; }
    }
}
