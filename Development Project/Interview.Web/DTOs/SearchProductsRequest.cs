namespace Interview.Web.DTOs
{
    /// <summary>
    /// EVAL: Query parameters for product search.
    /// All fields are optional — omitting all returns all products.
    /// Attributes are passed as a JSON string and deserialized in the controller.
    /// </summary>
    public class SearchProductsRequest
    {
        public string? NameContains { get; set; }
        public string? DescriptionContains { get; set; }
        public string? SkuContains { get; set; }

        /// <summary>
        /// Comma-separated category IDs to filter by.
        /// </summary>
        public string? CategoryIds { get; set; }

        /// <summary>
        /// Attribute key to filter by (used with AttributeValue).
        /// </summary>
        public string? AttributeKey { get; set; }

        /// <summary>
        /// Attribute value to filter by (used with AttributeKey).
        /// </summary>
        public string? AttributeValue { get; set; }
    }
}
