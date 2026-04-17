namespace Sparcpoint.Inventory.Models
{
    /// <summary>
    /// EVAL: Free key/value pair for arbitrary metadata (SKU, Color, Brand, etc.).
    /// Keeps the design open for extension without altering the Products table.
    /// Mirrors [Instances].[ProductAttributes].
    /// </summary>
    public sealed class ProductAttribute
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public ProductAttribute() { }

        public ProductAttribute(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}
