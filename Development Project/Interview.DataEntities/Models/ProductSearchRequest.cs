using System.Collections.Generic;

namespace Interview.DataEntities.Models
{
    public class ProductSearchRequest
    {
        public List<int> CategoryIds { get; set; }
        public Dictionary<string, string> MetadataFilters { get; set; }
    }
}
