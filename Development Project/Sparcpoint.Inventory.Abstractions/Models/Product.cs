using System;
using System.Collections.Generic;

namespace Sparcpoint.Inventory.Abstractions
{
    /// <summary>
    /// Represents a product instance in the inventory system.
    /// </summary>
    public class Product
    {
        public int InstanceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        // EVAL: ProductImageUris and ValidSkus stored as raw strings (JSON/CSV).
        // A future improvement would normalize these into child tables for indexability.
        public string ProductImageUris { get; set; }
        public string ValidSkus { get; set; }
        public DateTime CreatedTimestamp { get; set; }

        // EVAL: Arbitrary metadata - loaded separately after the main product query
        // to avoid a Cartesian join explosion on multi-attribute products.
        public List<ProductAttribute> Attributes { get; set; } = new List<ProductAttribute>();
        public List<int> CategoryIds { get; set; } = new List<int>();
    }
}
