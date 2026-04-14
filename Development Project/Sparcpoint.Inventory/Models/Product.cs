using System;
using System.Collections.Generic;

namespace Sparcpoint.Inventory.Models
{
    public sealed class Product
    {
        public int InstanceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public IReadOnlyList<string> ProductImageUris { get; set; } = Array.Empty<string>();
        public IReadOnlyList<string> ValidSkus { get; set; } = Array.Empty<string>();
        public DateTime CreatedTimestamp { get; set; }

        public IReadOnlyList<ProductAttribute> Attributes { get; set; } = Array.Empty<ProductAttribute>();
        public IReadOnlyList<int> CategoryIds { get; set; } = Array.Empty<int>();
    }
}
