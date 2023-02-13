using Dapper;
using Dapper.Contrib.Extensions;
using Sparcpoint.Inventory.Abstract;
using Sparcpoint.Inventory.Models;
using Sparcpoint.SqlServer.Abstractions;

namespace Sparcpoint.Inventory.Implementations
{
    public class ProductService : IProductService
    {
        private readonly ISqlExecutor _sqlExecutor;

        public ProductService(ISqlExecutor sqlExecutor)
        {
            _sqlExecutor = sqlExecutor ?? throw new ArgumentNullException(nameof(sqlExecutor));
        }

        // EVAL: sync and async methods added to the interface since it was mentioned;
        // the "Flag argument hack" makes use of ConfigureAwait for performance and to avoid deadlocks,
        // but separate implementations should ideally be done (but reduce reuse of logic)
        // If not required for backwards-compatibility or are needed for some other reason, provide only async
        public void AddAttributesToProduct(int productId, IEnumerable<ProductAttribute> productAttributes)
        {
            throw new NotImplementedException();
        }

        public async Task AddAttributesToProductAsync(int productId, IEnumerable<ProductAttribute> productAttributes)
        {
            await _sqlExecutor.ExecuteAsync(async (connection, transaction) => {
                foreach (var item in productAttributes)
                {
                    item.InstanceId = productId;
                    await connection.InsertAsync(item, transaction);
                }
            });
        }

        public void AddProductToCategories(int productId, IEnumerable<ProductCategory> productCategories)
        {
            throw new NotImplementedException();
        }

        public async Task AddProductToCategoriesAsync(int productId, IEnumerable<ProductCategory> productCategories)
        {
            await _sqlExecutor.ExecuteAsync(async (connection, transaction) => {
                foreach (var item in productCategories)
                {
                    item.InstanceId = productId;
                    await connection.InsertAsync(item, transaction);
                }
            });
        }

        public int CreateProduct(Product product)
        {
            throw new NotImplementedException();
        }

        public async Task<int> CreateProductAsync(Product product)
        {
            // EVAL: validate the model first
            return await _sqlExecutor.ExecuteAsync(async (connection, transaction) => {
                product.CreatedTimestamp = DateTime.Now;
                var instanceId = await connection.InsertAsync(product, transaction);

                // EVAL: create missing attributes or throw an error?
                // same for categories
                foreach (var item in product.ProductAttributes)
                {
                    item.InstanceId = instanceId;
                    await connection.InsertAsync(item, transaction);
                }

                foreach (var item in product.Categories)
                {
                    item.InstanceId = instanceId;
                    await connection.InsertAsync(item, transaction);
                }

                return instanceId;
            });
        }

        public IEnumerable<Product> GetProducts()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            const string query = "SELECT * FROM [Instances].[Products]";
            return await _sqlExecutor.ExecuteAsync(async (connection, transaction) =>
            {
                return await connection.QueryAsync<Product>(query, transaction: transaction);
            });
        }

        public IEnumerable<Product> SearchProducts(string keyword, ProductSearchScope searchScope)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Product>> SearchProductsAsync(string keyword, ProductSearchScope searchScope)
        {
            // EVAL: modify this to return a parameterized query based on the given search type (not just any)
            //var query = GetSearchQuery(searchType); // use SqlServerQueryProvider
            //var products = await _sqlQueryService.QueryAsync<Product>(query, new { keyword });
            if (searchScope != ProductSearchScope.Any) { throw new NotImplementedException(); }

            const string sqlFmt = @"SELECT DISTINCT * FROM 
(SELECT P.* FROM [Sparcpoint.Inventory.Database].[Instances].[Products] P 
    LEFT JOIN [Instances].[ProductAttributes] PA ON PA.InstanceId = P.InstanceId 
    LEFT JOIN [Instances].[ProductCategories] PC ON PC.InstanceId = P.InstanceId 
    LEFT JOIN [Instances].[Categories] C ON C.InstanceId = PC.CategoryInstanceId
WHERE P.Name LIKE '%{0}%'
    OR P.Description LIKE '%{0}%'
    OR P.ValidSkus LIKE '%{0}%'
    OR PA.[Key] LIKE '%{0}%'
    OR PA.Value LIKE '%{0}%'
    OR C.Name LIKE '%{0}%'
    OR C.Description LIKE '%{0}%'
) products";
            var sql = string.Format(sqlFmt, keyword);
            return await _sqlExecutor.ExecuteAsync(async (connection, transaction) =>
            {
                return await connection.QueryAsync<Product>(sql, transaction: transaction);
            });
        }
    }
}
