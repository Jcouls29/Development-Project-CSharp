namespace Inventory.BusinessServices
{
    public class Product
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string Type { get; set; } = string.Empty;
        public string? ProductImageUris { get; set; }
        public string ValidSkus { get; set; }
        public virtual Attribute? Attribute { get; set; }
        public virtual CategoryAttribute? CategoryAttribute { get; set; }
    }
}