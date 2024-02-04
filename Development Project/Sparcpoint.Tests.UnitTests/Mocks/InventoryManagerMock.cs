using Sparcpoint.Inventory.Data;
using Sparcpoint.Product.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Tests.UnitTests.Mocks
{
    internal class InventoryManagerMock: InventoryManager
    {
        public InventoryManagerMock(string connString):base(connString) { }

        protected override Task<ProductItem> QueryProductById(int productId)
        {
            return null;
        }
    }
}
