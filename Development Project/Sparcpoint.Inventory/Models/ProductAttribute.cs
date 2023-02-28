using System;
using System.Collections.Generic;

namespace Sparcpoint.Inventory.Models;

public partial class ProductAttribute
{
    public int InstanceId { get; set; }

    public string Key { get; set; } = null!;

    public string Value { get; set; } = null!;

    public virtual Product Instance { get; set; } = null!;
}
