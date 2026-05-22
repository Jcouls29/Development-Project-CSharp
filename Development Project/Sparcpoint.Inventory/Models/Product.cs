using System;
using System.Collections.Generic;

namespace Sparcpoint.Inventory.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public IReadOnlyList<string> ImageUris { get; set; } = Array.Empty<string>();
        public IReadOnlyList<string> ValidSkus { get; set; } = Array.Empty<string>();
        public IReadOnlyDictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
        public IReadOnlyList<int> CategoryIds { get; set; } = Array.Empty<int>();
        public DateTime CreatedTimestamp { get; set; }
    }
}
