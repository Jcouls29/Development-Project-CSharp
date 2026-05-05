using System;
using System.Collections.Generic;

namespace Sparcpoint.Core.Models
{
    public class ProductSearchResult
    {
        public int InstanceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        // JSON columns deserialized by repository
        public IEnumerable<string> ProductImageUris { get; set; }
        public IEnumerable<string> ValidSkus { get; set; }

        // Key/value metadata from ProductAttributes
        public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();

        // Assigned category ids (many-to-many)
        public IEnumerable<int> CategoryIds { get; set; }

        // Product created timestamp if available in schema
        public DateTime? CreatedTimestamp { get; set; }
    }
}

