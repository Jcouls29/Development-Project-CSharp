using Sparcpoint.Inventory.Core.Entities;
using Sparcpoint.Inventory.Core.Requests;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sparcpoint.Inventory.Handler.Commands
{
    public class AddCategoryCommand : Category
    {
        public AddCategoryAttributesRequest CategoryAttributes { get; set; }
    }
}
