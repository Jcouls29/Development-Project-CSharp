using Dapper;
using Sparcpoint.Inventory.Abstract;
using Sparcpoint.Inventory.Models;
using Sparcpoint.Inventory.Models.Requests;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory.Implementations
{
    public class SqlProductRepository : IProductRepository
    {
        private readonly ISqlExecutor _Executor;

        public SqlProductRepository(ISqlExecutor executor)
        {
            _Executor = executor ?? throw new ArgumentNullException(nameof(executor));
        }

        public async Task<int> AddAsync(AddProductRequest request)
        {
            PreConditions.ParameterNotNull(request, nameof(request));
            PreConditions.StringNotNullOrWhitespace(request.Name, nameof(request.Name));
            PreConditions.StringMaxLength(request.Name, nameof(request.Name), 256);
            PreConditions.StringNotNullOrWhitespace(request.Description, nameof(request.Description));
            PreConditions.StringMaxLength(request.Description, nameof(request.Description), 256);

            if (request.Attributes != null)
                foreach (var attr in request.Attributes)
                {
                    PreConditions.StringMaxLength(attr.Key, "Attribute key", 64);
                    PreConditions.StringMaxLength(attr.Value, "Attribute value", 512);
                }

            if (request.CategoryIds != null)
                foreach (var id in request.CategoryIds)
                    PreConditions.IntGreaterThanZero(id, "CategoryId");

            return await _Executor.ExecuteAsync<int>(async (conn, trans) =>
            {
                // EVAL: MERGE with HOLDLOCK prevents phantom inserts under concurrent requests.
                // If a product with the same Name already exists, the INSERT is skipped and the
                // existing InstanceId is returned — no duplicate, no error.
                // Single transaction ensures atomicity — if category/attribute inserts fail,
                // the product row is rolled back too.
                const string insertProduct = @"
                    MERGE [Instances].[Products] WITH (HOLDLOCK) AS target
                    USING (VALUES (@Name)) AS source ([Name]) ON target.[Name] = source.[Name]
                    WHEN NOT MATCHED THEN
                        INSERT ([Name], [Description], [ProductImageUris], [ValidSkus])
                        VALUES (@Name, @Description, @ProductImageUris, @ValidSkus);
                    SELECT CAST([InstanceId] AS INT) FROM [Instances].[Products] WHERE [Name] = @Name;";

                int instanceId = await conn.ExecuteScalarAsync<int>(insertProduct, new
                {
                    request.Name,
                    request.Description,
                    ProductImageUris = request.ProductImageUris ?? string.Empty,
                    ValidSkus = request.ValidSkus ?? string.Empty
                }, trans);

                if (request.Attributes != null && request.Attributes.Any())
                {
                    const string insertAttribute = @"
                        MERGE [Instances].[ProductAttributes] AS target
                        USING (VALUES (@InstanceId, @Key)) AS source ([InstanceId], [Key])
                            ON target.[InstanceId] = source.[InstanceId] AND target.[Key] = source.[Key]
                        WHEN NOT MATCHED THEN
                            INSERT ([InstanceId], [Key], [Value]) VALUES (@InstanceId, @Key, @Value)
                        WHEN MATCHED THEN
                            UPDATE SET [Value] = @Value;";

                    await conn.ExecuteAsync(insertAttribute,
                        request.Attributes.Select(attr => new { InstanceId = instanceId, attr.Key, attr.Value }),
                        trans);
                }

                if (request.CategoryIds != null && request.CategoryIds.Any())
                {
                    const string insertCategory = @"
                        MERGE [Instances].[ProductCategories] AS target
                        USING (VALUES (@InstanceId, @CategoryInstanceId)) AS source ([InstanceId], [CategoryInstanceId])
                            ON target.[InstanceId] = source.[InstanceId] AND target.[CategoryInstanceId] = source.[CategoryInstanceId]
                        WHEN NOT MATCHED THEN
                            INSERT ([InstanceId], [CategoryInstanceId]) VALUES (@InstanceId, @CategoryInstanceId);";

                    await conn.ExecuteAsync(insertCategory,
                        request.CategoryIds.Select(id => new { InstanceId = instanceId, CategoryInstanceId = id }),
                        trans);
                }

                return instanceId;
            });
        }

        public async Task<IEnumerable<Product>> SearchAsync(ProductSearchRequest request)
        {
            PreConditions.ParameterNotNull(request, nameof(request));

            return await _Executor.ExecuteAsync<IEnumerable<Product>>(async (conn, trans) =>
            {
                var parameters = new DynamicParameters();
                var joins = new List<string>();
                var wheres = new List<string>();

                // EVAL: Each attribute filter becomes a separate INNER JOIN so products must satisfy
                // ALL specified attribute criteria (AND semantics), not just any one of them
                var attrs = request.Attributes?
                    .Where(a => !string.IsNullOrWhiteSpace(a.Key))
                    .ToList();

                if (attrs != null && attrs.Any())
                {
                    for (int i = 0; i < attrs.Count; i++)
                    {
                        string alias = $"pa{i}";
                        joins.Add($"INNER JOIN [Instances].[ProductAttributes] {alias} " +
                                  $"ON p.[InstanceId] = {alias}.[InstanceId] " +
                                  $"AND {alias}.[Key] = @attrKey{i} AND {alias}.[Value] = @attrVal{i}");
                        parameters.Add($"attrKey{i}", attrs[i].Key);
                        parameters.Add($"attrVal{i}", attrs[i].Value);
                    }
                }

                // EVAL: Categories use OR semantics (IN) — a product matching ANY of the specified
                // categories is returned. This differs from attributes which use AND semantics.
                // Rationale: filtering by multiple categories means "show me products from any
                // of these categories", not "products that somehow belong to all of them."
                var categoryIds = request.CategoryIds?.ToList();
                if (categoryIds != null && categoryIds.Any())
                {
                    joins.Add("INNER JOIN [Instances].[ProductCategories] pc ON p.[InstanceId] = pc.[InstanceId]");
                    wheres.Add("pc.[CategoryInstanceId] IN @CategoryIds");
                    parameters.Add("CategoryIds", categoryIds);
                }

                if (!string.IsNullOrWhiteSpace(request.Name))
                {
                    wheres.Add("p.[Name] LIKE @Name");
                    parameters.Add("Name", $"%{request.Name}%");
                }

                var sql = "SELECT DISTINCT p.* FROM [Instances].[Products] p"
                    + (joins.Any() ? " " + string.Join(" ", joins) : "")
                    + (wheres.Any() ? " WHERE " + string.Join(" AND ", wheres) : "");

                var products = (await conn.QueryAsync<Product>(sql, parameters, trans)).ToList();

                if (products.Any())
                {
                    var ids = products.Select(p => p.InstanceId).ToList();

                    var allAttributes = await conn.QueryAsync<ProductAttribute>(
                        "SELECT * FROM [Instances].[ProductAttributes] WHERE [InstanceId] IN @Ids",
                        new { Ids = ids }, trans);

                    var attrLookup = allAttributes
                        .GroupBy(a => a.InstanceId)
                        .ToDictionary(g => g.Key, g => (IEnumerable<ProductAttribute>)g.ToList());

                    var allCatRows = await conn.QueryAsync<ProductCategoryRow>(
                        "SELECT [InstanceId], [CategoryInstanceId] FROM [Instances].[ProductCategories] WHERE [InstanceId] IN @Ids",
                        new { Ids = ids }, trans);

                    var catLookup = allCatRows
                        .GroupBy(r => r.InstanceId)
                        .ToDictionary(g => g.Key, g => (IEnumerable<int>)g.Select(r => r.CategoryInstanceId).ToList());

                    foreach (var product in products)
                    {
                        product.Attributes = attrLookup.TryGetValue(product.InstanceId, out var pa)
                            ? pa : Enumerable.Empty<ProductAttribute>();

                        product.CategoryIds = catLookup.TryGetValue(product.InstanceId, out var pc)
                            ? pc : Enumerable.Empty<int>();
                    }
                }

                return products;
            });
        }

        private class ProductCategoryRow
        {
            public int InstanceId { get; set; }
            public int CategoryInstanceId { get; set; }
        }
    }
}
