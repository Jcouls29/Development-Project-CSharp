using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory.Models
{
    public class DapperProduct
    {
        public int InstanceId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ProductImageUris { get; set; }

        public string ValidSkus { get; set; }

        public DateTime CreatedTimeStamp { get; set; }

        public Dictionary<string, string> ProductAttributes { get; set; }

        public Dictionary<string, string> ProductCategories { get; set; }

        public IEnumerable<InventoryTransaction> InventoryTransactions { get; set; }
    }
}
