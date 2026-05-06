namespace Sparcpoint.Inventory.Abstractions
{
    /// <summary>
    /// A single key/value metadata entry attached to a product.
    /// Maps to Instances.ProductAttributes table.
    /// </summary>
    public class ProductAttribute
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
