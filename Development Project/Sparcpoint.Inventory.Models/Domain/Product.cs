using System;
using System.Collections.Generic;

namespace Sparcpoint.Inventory.Models.Domain
{
    /// <summary>
    /// Represents a product in the inventory system with its associated metadata and categories.
    /// EVAL: This model maps to Instances.Products table and related attribute/category tables.
    /// </summary>
    public class Product
    {
        public int InstanceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        
        /// <summary>
        /// List of product image URIs. Stored as pipe-delimited string in database.
        /// </summary>
        public List<string> ProductImageUris { get; set; } = new List<string>();
        
        /// <summary>
        /// List of valid SKUs for this product. Stored as pipe-delimited string in database.
        /// </summary>
        public List<string> ValidSkus { get; set; } = new List<string>();
        
        /// <summary>
        /// Arbitrary metadata attributes (e.g., Color, Brand, Size, Weight).
        /// EVAL: Supports requirement for arbitrary metadata on products.
        /// </summary>
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
        
        /// <summary>
        /// Category IDs this product belongs to.
        /// EVAL: Supports hierarchical categorization requirement.
        /// </summary>
        public List<int> CategoryIds { get; set; } = new List<int>();
        
        public DateTime CreatedTimestamp { get; set; }
    }
}