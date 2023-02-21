using System;
using System.Collections.Generic;
using System.Text;

namespace Sparcpoint.Inventory.Core.Entities.Shared
{
    public class BaseInventory : Audit
    {
        public int InstanceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
