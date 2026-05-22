using System;
using System.Collections.Generic;

namespace Interview.Web.Models
{
    public class Product
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Flexible metadata stored as key/value pairs
        public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();

        // Many-to-many categories
        public ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();

        public ICollection<InventoryTransaction> InventoryTransactions { get; set; } = new List<InventoryTransaction>();
    }
}
