namespace Sparcpoint.Inventory.Models
{
    public sealed class ProductAttribute
    {
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;

        public ProductAttribute() { }

        public ProductAttribute(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }

    // EVAL: Well-known attribute keys exposed as constants so callers get
    // IntelliSense while the underlying storage remains flexible key/value.
    public static class ProductAttributeKeys
    {
        public const string Sku = "SKU";
        public const string Color = "Color";
        public const string Brand = "Brand";
        public const string Length = "Length";
        public const string PackageUnit = "PackageUnit";
    }
}
