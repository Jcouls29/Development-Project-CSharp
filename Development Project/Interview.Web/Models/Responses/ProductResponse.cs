using System;
using System.Collections.Generic;

namespace Interview.Web.Models.Responses
{
    /// <summary>
    /// API response representing a product with all its associated data.
    /// </summary>
    // EVAL: Response DTOs decouple the API contract from the domain model.
    // This allows the API shape to differ from the DB schema (e.g., deserializing
    // JSON strings into proper lists, omitting internal fields).
    public class ProductResponse
    {
        /// <summary>
        /// Unique product identifier.
        /// </summary>
        public int InstanceId { get; set; }

        /// <summary>
        /// Product name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Product description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Product image URIs.
        /// </summary>
        // EVAL: Deserialized from VARCHAR(MAX) JSON string into a proper list
        // so API consumers get a native array, not a raw JSON string.
        public List<string> ImageUris { get; set; } = new List<string>();

        /// <summary>
        /// Valid SKUs for this product.
        /// </summary>
        public List<string> Skus { get; set; } = new List<string>();

        /// <summary>
        /// Arbitrary metadata key-value pairs.
        /// </summary>
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Categories this product belongs to.
        /// </summary>
        public List<CategorySummary> Categories { get; set; } = new List<CategorySummary>();

        /// <summary>
        /// When the product was created (UTC).
        /// </summary>
        public DateTime CreatedTimestamp { get; set; }
    }

    /// <summary>
    /// Lightweight category reference included in product responses.
    /// </summary>
    // RV: Embedding full category trees in every product response would be expensive.
    // Summary with just Id + Name is sufficient for most UI use cases.
    public class CategorySummary
    {
        public int InstanceId { get; set; }
        public string Name { get; set; }
    }
}
