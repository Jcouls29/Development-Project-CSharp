using Dapper;
using Microsoft.Data.SqlClient;
using Sparcpoint.Data.Resources;
using Sparcpoint.Dto;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sparcpoint.Data
{
    /// <summary>
    /// If this were a real project, this class should have
    /// logging, and depending on the application, a retry process,
    /// and possibly an interface if there was a need to define a contract.
    /// </summary>
    public sealed class ProductsRepository
    {
        private readonly string _connectionString;

        public ProductsRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            IEnumerable<Product> products = new List<Product>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                products = await conn.QueryAsync<Product>(Queries.GetAllProducts);
            }

            return products.ToList();
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            Product product = new Product();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@id", id);

                product = await conn.QuerySingleOrDefaultAsync<Product>(Queries.GetProductById, parameters);
            }

            return product;
        }

        public async Task<int> GetProductCountAsync()
        {
            int count = 0;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                count = await conn.QuerySingleOrDefaultAsync<int>(Queries.GetProductCount);
            }

            return count;
        }

        public async Task<Product> AddProductAsync(Product product)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@name", product.Name);
                parameters.Add("@description", product.Description);
                parameters.Add("@productImageUris", product.ProductImageUris);
                parameters.Add("@validSkus", product.ValidSkus);

                product.InstanceId = await conn.ExecuteScalarAsync<int>(Queries.AddProduct, parameters);
            }

            return product;
        }

    }
}
