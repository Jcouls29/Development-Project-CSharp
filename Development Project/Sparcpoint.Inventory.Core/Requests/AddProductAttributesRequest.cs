using Sparcpoint.Inventory.Core.Entities.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sparcpoint.Inventory.Core.Requests
{
    public class AddProductAttributesRequest : InventoryAttributes
    {
        public int InstanceId { get; set; }
    }
}
