using System;
using System.Collections.Generic;

namespace Sparcpoint.Inventory.Models;

public partial class Category
{
    public int InstanceId { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime CreatedTimestamp { get; set; }

    public virtual ICollection<CategoryAttribute> CategoryAttributes { get; } = new List<CategoryAttribute>();

    public virtual ICollection<Category> CategoryInstances { get; } = new List<Category>();

    public virtual ICollection<Category> Instances { get; } = new List<Category>();

    public virtual ICollection<Product> InstancesNavigation { get; } = new List<Product>();
}
