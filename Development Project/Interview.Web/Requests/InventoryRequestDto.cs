using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Web.Requests
{/// <summary>
 /// Represents list of products to be added to the inventory with Quantity
 /// </summary>
    public class InventoryAddProductsRequestDto
    {
        public IDictionary<int, int> Products_Quanities { get; set; }

    }
}