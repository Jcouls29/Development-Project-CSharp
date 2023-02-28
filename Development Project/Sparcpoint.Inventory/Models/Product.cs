using System;
using System.Collections.Generic;

namespace Sparcpoint.Inventory.Models;

public partial class Product
{
    public int InstanceId { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string ProductImageUris { get; set; } = null!;

    public string ValidSkus { get; set; } = null!;

    public DateTime CreatedTimestamp { get; set; }

    public virtual ICollection<InventoryTransaction> InventoryTransactions { get; } = new List<InventoryTransaction>();

    public virtual ICollection<ProductAttribute> ProductAttributes { get; } = new List<ProductAttribute>();

    public virtual ICollection<Category> CategoryInstances { get; } = new List<Category>();
}
