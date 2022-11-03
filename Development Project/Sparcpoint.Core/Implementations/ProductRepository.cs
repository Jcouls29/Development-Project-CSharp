using Dapper;
using Sparcpoint.Entities;
using Sparcpoint.Repositories;
using Sparcpoint.SqlServer.Abstractions.Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Implementations
{
   public  class ProductRepository : IProdcutRepository
    {
        private readonly DapperContext _context;

        public ProductRepository(DapperContext context)
        {
            _context = context;
        }
      
        public async Task<IEnumerable<Product>> GetProducts()
        {
            var query = "SELECT * FROM INSTANCES.PRODUCTS";

            using (var connection= _context.CreateConnection())
            {
                var products = await connection.QueryAsync<Product>(query);
                return products.ToList();
            }

        }

        public async Task<Product> CreateProduct(ProductForCreationDto productForCreation)
        {
            var query = "INSERT INTO INSTANCES.PRODUCTS ([Name] ,[Description],[ProductImageUris],[ValidSkus]) VALUES (@Name,@Description,@ProductImageUris,@ValidSkus)" +
                         "SELECT CAST(SCOPE_IDENTITY() as int)";
            var parameters = new DynamicParameters();
            parameters.Add("Name", productForCreation.Name, DbType.String);
            parameters.Add("Description", productForCreation.Description, DbType.String);
            parameters.Add("ProductImageUris", productForCreation.ProductImageUris, DbType.String);
            parameters.Add("ValidSkus", productForCreation.ValidSkus, DbType.String);

            using (var connection = _context.CreateConnection())
            {
                var instanceId = await connection.QuerySingleAsync<int>(query,parameters);

                var CreatedProduct = new Product
                {
                    InstanceId = instanceId,
                    Name = productForCreation.Name,
                    Description = productForCreation.Description,
                    ProductImageUris = productForCreation.ProductImageUris,
                    ValidSkus = productForCreation.ValidSkus,
                };

                return CreatedProduct;
            }

        }

    }
}
