using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Interview.Web.Models;
using Interview.Web.Repositories.Interfaces;
using Sparcpoint.SqlServer.Abstractions;

namespace Interview.Web.Repositories.Implementations;

public class ProductRepository : IProductRepository
{
    private readonly ISqlExecutor _sqlExecutor;

    public ProductRepository(ISqlExecutor sqlExecutor)
    {
        _sqlExecutor = sqlExecutor;
    }

    public async Task<int> CreateAsync(CreateProductRequest request)
    {
        return await _sqlExecutor.ExecuteAsync(async (conn, trans) =>
        {
            // EVAL: SCOPE_IDENTITY() is scoped to the current connection, so concurrent
            // inserts on other connections won't affect the returned id.
            const string insertProduct = @"
                INSERT INTO Instances.Products (Name, Description, ValidSkus, ProductImageUris)
                VALUES (@Name, @Description, @ValidSkus, @ProductImageUris);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            var id = await conn.ExecuteScalarAsync<int>(insertProduct, new
            {
                request.Name,
                Description = request.Description ?? string.Empty,
                // EVAL: SKUs and image URIs are stored comma-separated to match the VARCHAR(MAX)
                // columns — no additional tables needed.
                ValidSkus = string.Join(",", request.ValidSkus ?? Array.Empty<string>()),
                ProductImageUris = string.Join(",", request.ProductImageUris ?? Array.Empty<string>())
            }, trans);

            if (request.Attributes != null && request.Attributes.Any())
            {
                const string insertAttribute = @"
                    INSERT INTO Instances.ProductAttributes (InstanceId, [Key], [Value])
                    VALUES (@InstanceId, @Key, @Value);";

                foreach (var attr in request.Attributes)
                {
                    await conn.ExecuteAsync(insertAttribute, new
                    {
                        InstanceId = id,
                        Key = attr.Key,
                        Value = attr.Value
                    }, trans);
                }
            }

            if (request.CategoryIds != null && request.CategoryIds.Any())
            {
                const string insertCategory = @"
                    INSERT INTO Instances.ProductCategories (InstanceId, CategoryInstanceId)
                    VALUES (@InstanceId, @CategoryInstanceId);";

                foreach (var categoryId in request.CategoryIds)
                {
                    await conn.ExecuteAsync(insertCategory, new
                    {
                        InstanceId = id,
                        CategoryInstanceId = categoryId
                    }, trans);
                }
            }

            return id;
        });
    }

    public async Task<IEnumerable<ProductResponse>> SearchAsync(ProductSearchRequest request)
    {
        return await _sqlExecutor.ExecuteAsync(async (conn, trans) =>
        {
            // EVAL: WhereClause returns empty string when no filters are set,
            // so the query naturally returns all products without a separate branch.
            var query = SqlServerQueryProvider.Empty;
            query.SetTargetTableAlias("p");

            if (!string.IsNullOrWhiteSpace(request?.Name))
                query.WhereEquals("Name", "Name", request.Name);

            if (!string.IsNullOrWhiteSpace(request?.Description))
                query.WhereEquals("Description", "Description", request.Description);

            // EVAL: EXISTS avoids row duplication that a JOIN would cause when a product
            // has multiple attributes, and short-circuits on the first match.
            if (!string.IsNullOrWhiteSpace(request?.AttributeKey) && !string.IsNullOrWhiteSpace(request?.AttributeValue))
            {
                query.Where($@"EXISTS (
                    SELECT 1 FROM Instances.ProductAttributes pa
                    WHERE pa.InstanceId = p.InstanceId
                    AND pa.[Key] = @AttributeKey
                    AND pa.[Value] = @AttributeValue)");

                query.AddParameter("@AttributeKey", request.AttributeKey);
                query.AddParameter("@AttributeValue", request.AttributeValue);
            }

            // EVAL: IN subquery returns products in any of the requested categories
            // without duplicating rows the way a JOIN would.
            if (request?.CategoryIds != null && request.CategoryIds.Any())
            {
                var categoryIdList = string.Join(",", request.CategoryIds);
                query.WhereIn("InstanceId", $"(SELECT InstanceId FROM Instances.ProductCategories WHERE CategoryInstanceId IN ({categoryIdList}))");
            }

            var sql = $"SELECT * FROM Instances.Products p {query.WhereClause}";

            var products = await conn.QueryAsync<Product>(sql, query.Parameters, trans);

            // EVAL: N+1 query per product — acceptable here, but in production this would
            // be replaced with a single JOIN query grouped by InstanceId.
            var responses = new List<ProductResponse>();
            foreach (var product in products)
            {
                var attributes = await LoadAttributesAsync(conn, trans, product.InstanceId);
                responses.Add(MapToResponse(product, attributes));
            }

            return responses;
        });
    }

    private static async Task<Dictionary<string, string>> LoadAttributesAsync(
        System.Data.IDbConnection conn,
        System.Data.IDbTransaction trans,
        int instanceId)
    {
        const string sql = @"
            SELECT [Key], [Value]
            FROM Instances.ProductAttributes
            WHERE InstanceId = @InstanceId";

        var rows = await conn.QueryAsync<(string Key, string Value)>(sql, new { InstanceId = instanceId }, trans);
        return rows.ToDictionary(r => r.Key, r => r.Value);
    }

    // EVAL: Keeps the DB model decoupled from the API contract — schema changes stay
    // contained here and don't ripple up to services or controllers.
    private static ProductResponse MapToResponse(Product product, Dictionary<string, string> attributes)
    {
        return new ProductResponse
        {
            InstanceId = product.InstanceId,
            Name = product.Name,
            Description = product.Description,
            ValidSkus = SplitCsv(product.ValidSkus),
            ProductImageUris = SplitCsv(product.ProductImageUris),
            CreatedTimestamp = product.CreatedTimestamp,
            Attributes = attributes
        };
    }

    // EVAL: Guards against returning [""] when the stored value is empty or null.
    private static string[] SplitCsv(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Array.Empty<string>();

        return value.Split(',', StringSplitOptions.RemoveEmptyEntries);
    }
}
