using Sparcpoint.Abstract.Services;
using Sparcpoint.Domain;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Sparcpoint.Implementations.Services
{
    public class ProductService : IProductService
    {
        private readonly ISqlExecutor _sqlExecutor;

        public ProductService(ISqlExecutor sqlExecutor)
        {
            _sqlExecutor = sqlExecutor;
        }

        // TODO: change paramneter to ProductRequest
        public async Task<int> AddProductAsync(Product request)
        {
            // TODO: insert product
            // TODO insert metada
            // TODO: return productid

            

        }
    }
}
