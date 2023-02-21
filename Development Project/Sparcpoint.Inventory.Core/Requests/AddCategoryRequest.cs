using Sparcpoint.Inventory.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sparcpoint.Inventory.Core.Requests
{
    public class AddCategoryRequest : Category
    {
        public AddCategoryAttributesRequest CategoryAttributes { get; set; }
    }
}
