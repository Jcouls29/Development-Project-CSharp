using System.Collections.Generic;
using System.Linq;

namespace Inventory.BusinessServices
{
    public class ProductSearch
    {
        public List<Product>? Products { get; set; }
        public int TotalCount { get; set; }
    }
}