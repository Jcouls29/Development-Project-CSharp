using System.Collections.Generic;

namespace Sparcpoint.Inventory.Abstractions
{
    public class CreateProductRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProductImageUris { get; set; } = string.Empty;
        public string ValidSkus { get; set; } = string.Empty;

        /// <summary>
        /// Arbitrary key/value metadata for this product (e.g. Color, Brand, SKU).
        /// </summary>
        public IDictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// IDs of categories this product belongs to.
        /// </summary>
        public IEnumerable<int> CategoryIds { get; set; } = new List<int>();
    }
}
