using Dapper;
using Interview.Web.Models.Product;
using Interview.Web.Services;
using Microsoft.Extensions.Logging;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Web.Services
{
    public class ProductService : IProductService
    {
        private readonly ISqlExecutor _executor;
        private readonly IProductRule _rule;

        public ProductService(ISqlExecutor executor, IProductRule rule)
        {
            _executor = executor ?? throw new ArgumentNullException(nameof(executor));
            _rule = rule ?? throw new ArgumentNullException(nameof(rule));
        }

        public async Task<int> CreateProductAsync(CreateProductRequest request)
        {
            _rule.ValidateCreate(request);

            return await _executor.ExecuteAsync(async (conn, trans) =>
            {
                var productId = await conn.ExecuteScalarAsync<int>(
                    @"INSERT INTO Products (Name, Description)
              VALUES (@Name, @Description);
              SELECT CAST(SCOPE_IDENTITY() as int);",
                    new { request.Name, request.Description }, trans);

                if (request.Metadata?.Any() == true)
                {
                    var attrSql = @"INSERT INTO ProductAttributes (ProductId, [Key], [Value])
                            VALUES (@ProductId, @Key, @Value);";

                    var attrData = request.Metadata.Select(a => new
                    {
                        ProductId = productId,
                        Key = a.Key,
                        Value = a.Value
                    });

                    await conn.ExecuteAsync(attrSql, attrData, trans);
                }

                if (request.CategoryIds?.Any() == true)
                {
                    var catSql = @"INSERT INTO ProductCategories (ProductId, CategoryId)
                           VALUES (@ProductId, @CategoryId);";

                    var catData = request.CategoryIds.Select(c => new
                    {
                        ProductId = productId,
                        CategoryId = c
                    });

                    await conn.ExecuteAsync(catSql, catData, trans);
                }

                return productId;
            });
        }

        public async Task<IEnumerable<ProductDto>> SearchProductsAsync(ProductSearchRequest request)
        {
            _rule.ValidateSearch(request);

            return await _executor.ExecuteAsync(async (conn, trans) =>
            {
                var sql = @"
            SELECT DISTINCT p.Id, p.Name, p.Description
            FROM Products p
            LEFT JOIN ProductCategories pc ON p.Id = pc.ProductId
            LEFT JOIN ProductAttributes pa ON p.Id = pa.ProductId
            WHERE 1=1";

                var parameters = new DynamicParameters();

                if (!string.IsNullOrWhiteSpace(request?.Name))
                {
                    sql += " AND p.Name LIKE @Name";
                    parameters.Add("Name", $"%{request.Name}%");
                }

                if (request?.CategoryId != null)
                {
                    sql += " AND pc.CategoryId = @CategoryId";
                    parameters.Add("CategoryId", request.CategoryId);
                }

                if (!string.IsNullOrWhiteSpace(request?.AttributeKey))
                {
                    sql += " AND pa.[Key] = @Key";
                    parameters.Add("Key", request.AttributeKey);
                }

                if (!string.IsNullOrWhiteSpace(request?.AttributeValue))
                {
                    sql += " AND pa.[Value] = @Value";
                    parameters.Add("Value", request.AttributeValue);
                }

                return await conn.QueryAsync<ProductDto>(sql, parameters, trans);
            });
        }
    }
}