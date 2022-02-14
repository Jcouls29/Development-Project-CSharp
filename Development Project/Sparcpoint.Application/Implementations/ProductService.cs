using Dapper.Contrib.Extensions;
using Sparcpoint.Application.Abstracts;
using Sparcpoint.Domain.Instance.Entities;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Application.Implementations
{
    public class ProductService : IProductService
    {
        private readonly SqlServerQueryProvider _sqlServerQueryProvider;
        private readonly IQueryService _queryService;
        private readonly ISqlExecutor _sqlExecutor;
        private readonly string TableName = "[Instances].[Products]";

        public ProductService(SqlServerQueryProvider sqlServerQueryProvider, IQueryService queryService, ISqlExecutor sqlExecutor)
        {
            _sqlServerQueryProvider = sqlServerQueryProvider;
            _queryService = queryService;
            _sqlExecutor = sqlExecutor;
        }

        public Task AddAttributesToProduct(int productId, List<ProductAttribute> productAttributes)
        {
            throw new NotImplementedException();
        }

        public async Task AddAttributesToProductAsync(int productId, List<ProductAttribute> attributes)
        {
            await _sqlExecutor.ExecuteAsync(async (connection, transaction) => {

                if (attributes.Count > 0)
                {
                    foreach (var item in attributes)
                    {
                        item.InstanceId = productId;
                        await connection.InsertAsync(item, transaction);
                    }
                }
            });
        }

        public Task AddProductToCategories(int productId, List<ProductCategory> productCategories)
        {
            throw new NotImplementedException();
        }

        public async Task AddProductToCategoriesAsync(int productId, List<ProductCategory> categories)
        {
            await _sqlExecutor.ExecuteAsync(async (connection, transaction) => {

                if (categories.Count > 0)
                {
                    foreach (var item in categories)
                    {
                        item.InstanceId = productId;
                        await connection.InsertAsync(item, transaction);
                    }
                }
            });
        }

        public Task<int> CreateProduct(Product product)
        {
            throw new NotImplementedException();
        }

        public async Task<int> CreateProductAsync(Product product)
        {
            return await _sqlExecutor.ExecuteAsync(async (connection, transaction) => {
                product.CreatedTimestamp = DateTime.Now;
                var instanceId = await connection.InsertAsync(product, transaction);
                if (product.ProductAttributes.Any())
                {
                    foreach (var item in product.ProductAttributes)
                    {
                        item.InstanceId = instanceId;
                        await connection.InsertAsync(item, transaction);
                    }
                }
                if (product.Categories.Any())
                {
                    foreach (var item in product.Categories)
                    {
                        item.InstanceId = instanceId;
                        await connection.InsertAsync(item, transaction);
                    }
                }
                return instanceId;

            });
        }

        public Task<List<ProductAttribute>> GetAttributesForProduct(int productId)
        {
            throw new NotImplementedException();
        }

        public Task<List<ProductAttribute>> GetAttributesForProductAsync(int productId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Product>> GetProducts()
        {
            string sql = "SELECT * FROM " + TableName;
            var products = await _queryService.QueryAsync<Product>(sql);
            return products.ToList();
            
        }

        public Task<List<Product>> GetProductsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<List<Product>> SearchProducts(string keyword)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Product>> SearchProductsAsync(string keyword)
        {
            string sql = "select distinct * from " +
                "(SELECT P.* FROM [Sparcpoint.Inventory.Database].[Instances].[Products] P " +
                "Left Join [Instances].[ProductAttributes] PA on PA.InstanceId = P.InstanceId " +
                "Left Join [Instances].[ProductCategories] PC on PC.InstanceId = P.InstanceId " +
                "Left Join [Instances].[Categories] C on C.InstanceId = PC.CategoryInstanceId" +
                " Where P.Name like '%{0}%' OR P.Description like '%{0}%' OR P.ValidSkus like '%{0}%' OR PA.[Key] like '%{0}%' " +
                "OR PA.Value like '%{0}%' OR C.Name like '%{0}%' OR C.Description like '%{0}%') products";
            sql = String.Format(sql, keyword);
            var products = await _queryService.QueryAsync<Product>(sql);
            return products.ToList();
        }
    }
}
