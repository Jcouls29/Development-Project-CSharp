using System.ComponentModel;

namespace Inventory.Data
{
    public class Product
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string Type { get; set; } = string.Empty;
        public string? ProductImageUris { get; set; }
        public string? ValidSkus { get; set; }
        public virtual ICollection<Attribute>? Attribute { get; set; } = new HashSet<Attribute>();
        public virtual ICollection<CategoryAttribute>? CategoryAttribute { get; set; } = new HashSet<CategoryAttribute>();
    }
}