using Sparcpoint.Products.Data;
using Sparcpoint.Products.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Tests.UnitTests.Mocks
{
    internal class ProductManagerMock: ProductManager
    {
        public ProductManagerMock(string connString):base(connString) { }

        protected override Task<Product> QueryProductById(int productId)
        {
            return null;
        }

        protected override Task QueryAddNewProduct(Product product)
        {
            return null;
        }
    }
}
