using Sparcpoint.Inventory.Core.Entities;
using Sparcpoint.Inventory.Core.Entities.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sparcpoint.Inventory.Core.Requests
{
    public class AddProductRequest : BaseInventory
    {
        public string ProductImageUris { get; set; }
        public string ValidSkus { get; set; }
    }
}
