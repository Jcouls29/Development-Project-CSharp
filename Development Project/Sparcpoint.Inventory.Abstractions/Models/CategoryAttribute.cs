namespace Sparcpoint.Inventory.Models
{
    /// <summary>
    /// EVAL: Parallel to <see cref="ProductAttribute"/> but for categories.
    /// Mirrors [Instances].[CategoryAttributes].
    /// </summary>
    public sealed class CategoryAttribute
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public CategoryAttribute() { }

        public CategoryAttribute(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}
