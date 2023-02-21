using Sparcpoint.Inventory.Core.Entities;
using Sparcpoint.Inventory.Core.Requests;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sparcpoint.Inventory.Core.Response
{
    public class AddProductResponse
    {
        public int InstanceId { get; set; }
        //public Dictionary<int, List<int>> CategoriesWithAttributeInstanceIds { get; set; }
        public List<int> ProductAttributeInstanceIds { get; set; }

    }
}
