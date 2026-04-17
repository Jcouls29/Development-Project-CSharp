using System;
using System.Collections.Generic;

namespace Interview.Web.Models
{
    public class Product
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        public List<string> Categories { get; set; } = new List<string>();
        public int Quantity { get; set; } = 0;
    }

    public class InventoryTransaction
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ProductId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public int Delta { get; set; }
        public string Note { get; set; }
    }
}