using System.Collections.Generic;

namespace Sparcpoint
{
    public class ProductSearchCriteria
    {
        public string NameContains { get; set; }
        public string DescriptionContains { get; set; }
        public string SkuContains { get; set; }
        public List<int> CategoryIds { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
    }
}
