using System.Collections.Generic;

namespace Sparcpoint.Core.Models
{
    public enum MetadataMatchMode
    {
        Exact = 0,
        Contains = 1
    }

    public class ProductSearchCriteria
    {
        // General fields (string matching)
        public string Name { get; set; }
        public string Description { get; set; }

        // SKU filtering
        public IEnumerable<string> Skus { get; set; }

        // Category filtering
        public IEnumerable<int> CategoryIds { get; set; }
        // If true, include descendant categories when resolving category filters
        public bool IncludeCategoryDescendants { get; set; }

        // Metadata key/value filters
        public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();
        // Matching behavior for metadata values (Exact vs substring Contains)
        public MetadataMatchMode MetadataMatchMode { get; set; } = MetadataMatchMode.Exact;
        // If true, all metadata pairs must match (AND). If false, any matching pair satisfies (OR).
        public bool MatchAllMetadata { get; set; } = true;

        // Paging & sorting
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        // Column name to order by (validate/whitelist server-side)
        public string OrderBy { get; set; }
        public bool OrderDescending { get; set; }
    }
}