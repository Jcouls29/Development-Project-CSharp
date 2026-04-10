namespace Sparcpoint.Inventory.Models
{
    /// <summary>
    /// EVAL: Product entity maps to [Instances].[Products] table.
    /// Products can be added to the system but never deleted, per business requirements.
    /// Attributes are stored as key-value pairs in [Instances].[ProductAttributes] for arbitrary metadata support.
    /// </summary>
    public class Product
    {
        public int InstanceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ProductImageUris { get; set; } = string.Empty;
        public string ValidSkus { get; set; } = string.Empty;
        public DateTime CreatedTimestamp { get; set; }

        /// <summary>
        /// Arbitrary key-value metadata for this product (e.g., Color, Brand, SKU, Length).
        /// Stored in [Instances].[ProductAttributes].
        /// </summary>
        public Dictionary<string, string> Attributes { get; set; } = new();

        /// <summary>
        /// Category IDs this product belongs to.
        /// Stored in [Instances].[ProductCategories].
        /// </summary>
        public List<int> CategoryIds { get; set; } = new();
    }
}
