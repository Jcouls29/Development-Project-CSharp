using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Interview.Web.Models;
using Sparcpoint.SqlServer.Abstractions;
using Interview.Web.Repositories.Interfaces;

namespace Interview.Web.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ISqlExecutor _sqlExecutor;

        public ProductRepository(ISqlExecutor sqlExecutor)
        {
            _sqlExecutor = sqlExecutor;
        }

        public async Task<int> AddProductAsync(CreateProductRequest request)
        {
            return await _sqlExecutor.ExecuteAsync(async (connection, transaction) =>
            {
                // 1. Insert Product
                var sqlProduct = @"
                    INSERT INTO [Instances].[Products] (Name, Description, ProductImageUris, ValidSkus)
                    VALUES (@Name, @Description, @ProductImageUris, @ValidSkus);
                    SELECT CAST(SCOPE_IDENTITY() as int);";

                var productId = await connection.ExecuteScalarAsync<int>(sqlProduct, new
                {
                    request.Name,
                    request.Description,
                    request.ProductImageUris,
                    request.ValidSkus
                }, transaction);

                // 2. Insert Categories
                if (request.CategoryIds != null && request.CategoryIds.Any())
                {
                    var sqlCategory = "INSERT INTO [Instances].[ProductCategories] (InstanceId, CategoryInstanceId) VALUES (@InstanceId, @CategoryInstanceId)";
                    var categoryData = request.CategoryIds.Select(cId => new { InstanceId = productId, CategoryInstanceId = cId });
                    await connection.ExecuteAsync(sqlCategory, categoryData, transaction);
                }

                // 3. Insert Attributes
                if (request.Attributes != null && request.Attributes.Any())
                {
                    var sqlAttr = "INSERT INTO [Instances].[ProductAttributes] (InstanceId, [Key], Value) VALUES (@InstanceId, @Key, @Value)";
                    var attrData = request.Attributes.Select(a => new { InstanceId = productId, Key = a.Key, Value = a.Value });
                    await connection.ExecuteAsync(sqlAttr, attrData, transaction);
                }

                return productId;
            });
        }

        public async Task<IEnumerable<ProductResponse>> SearchProductsAsync(string category = null, string attributeKey = null, string attributeValue = null)
        {
            return await _sqlExecutor.ExecuteAsync(async (connection, transaction) =>
            {
                var sql = @"
                    SELECT DISTINCT p.*
                    FROM [Instances].[Products] p
                    LEFT JOIN [Instances].[ProductCategories] pc ON p.InstanceId = pc.InstanceId
                    LEFT JOIN [Instances].[Categories] c ON pc.CategoryInstanceId = c.InstanceId
                    LEFT JOIN [Instances].[ProductAttributes] pa ON p.InstanceId = pa.InstanceId
                    WHERE 1=1";

                var parameters = new DynamicParameters();

                if (!string.IsNullOrEmpty(category))
                {
                    sql += " AND (c.Name LIKE @Category OR c.Description LIKE @Category)";
                    parameters.Add("Category", $"%{category}%");
                }

                if (!string.IsNullOrEmpty(attributeKey))
                {
                    sql += " AND pa.[Key] = @AttrKey";
                    parameters.Add("AttrKey", attributeKey);
                }

                if (!string.IsNullOrEmpty(attributeValue))
                {
                    sql += " AND pa.Value LIKE @AttrValue";
                    parameters.Add("AttrValue", $"%{attributeValue}%");
                }

                var products = await connection.QueryAsync<ProductResponse>(sql, parameters, transaction);

                return products;
            });
        }
    }
}
