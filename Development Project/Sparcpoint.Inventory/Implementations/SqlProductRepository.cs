using Dapper;
using Newtonsoft.Json;
using Sparcpoint;
using Sparcpoint.Inventory.Abstract;
using Sparcpoint.Inventory.Models;
using Sparcpoint.Inventory.Requests;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory.Implementations
{
    // EVAL: All SQL is written inline with Dapper rather than stored procedures.
    // This keeps the data access logic visible and reviewable without requiring
    // DB deployments for query changes — acceptable for a project of this scope.
    public class SqlProductRepository : IProductRepository
    {
        private readonly ISqlExecutor _Executor;

        public SqlProductRepository(ISqlExecutor executor)
        {
            _Executor = executor ?? throw new ArgumentNullException(nameof(executor));
        }

        public async Task<Product?> GetByIdAsync(int instanceId)
        {
            return await _Executor.ExecuteAsync<Product?>(async (conn, trans) =>
            {
                const string sql = @"
                    SELECT p.InstanceId, p.Name, p.Description, p.ProductImageUris, p.ValidSkus, p.CreatedTimestamp
                    FROM [Instances].[Products] p
                    WHERE p.InstanceId = @InstanceId";

                var row = await conn.QuerySingleOrDefaultAsync(sql, new { InstanceId = instanceId }, trans);
                if (row == null) return null;

                var product = MapProduct(row);
                await HydrateAttributesAsync(conn, trans, product);
                await HydrateCategoriesAsync(conn, trans, product);
                return product;
            });
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _Executor.ExecuteAsync(async (conn, trans) =>
            {
                const string sql = @"
                    SELECT InstanceId, Name, Description, ProductImageUris, ValidSkus, CreatedTimestamp
                    FROM [Instances].[Products]
                    ORDER BY CreatedTimestamp DESC";

                var rows = await conn.QueryAsync(sql, transaction: trans);
                var products = rows.Select(r => MapProduct(r)).ToList();

                foreach (var product in products)
                {
                    await HydrateAttributesAsync(conn, trans, product);
                    await HydrateCategoriesAsync(conn, trans, product);
                }

                return (IEnumerable<Product>)products;
            });
        }

        public async Task<int> AddAsync(Product instance)
        {
            PreConditions.ParameterNotNull(instance, nameof(instance));
            PreConditions.StringNotNullOrWhitespace(instance.Name, nameof(instance.Name));

            // EVAL: CreateProductRequest is the preferred add path — this overload
            // exists to satisfy IInstanceRepository<T>. It maps to the request flow internally.
            var request = new CreateProductRequest
            {
                Name = instance.Name,
                Description = instance.Description,
                ProductImageUris = instance.ProductImageUris,
                ValidSkus = instance.ValidSkus,
                Attributes = instance.Attributes,
                CategoryInstanceIds = instance.CategoryInstanceIds
            };
            return await AddFromRequestAsync(request);
        }

        // EVAL: Primary add path used by the controller. Wraps the full insert
        // (product + attributes + categories) in a single transaction so partial
        // writes are never committed.
        public async Task<int> AddFromRequestAsync(CreateProductRequest request)
        {
            PreConditions.ParameterNotNull(request, nameof(request));
            PreConditions.StringNotNullOrWhitespace(request.Name, nameof(request.Name));

            return await _Executor.ExecuteAsync<int>(async (conn, trans) =>
            {
                // Insert product row
                const string insertProduct = @"
                    INSERT INTO [Instances].[Products] (Name, Description, ProductImageUris, ValidSkus)
                    OUTPUT INSERTED.InstanceId
                    VALUES (@Name, @Description, @ProductImageUris, @ValidSkus)";

                int newInstanceId = await conn.ExecuteScalarAsync<int>(insertProduct, new
                {
                    request.Name,
                    request.Description,
                    ProductImageUris = JsonConvert.SerializeObject(request.ProductImageUris),
                    ValidSkus = JsonConvert.SerializeObject(request.ValidSkus)
                }, trans);

                // Insert attributes
                if (request.Attributes.Any())
                {
                    const string insertAttr = @"
                        INSERT INTO [Instances].[ProductAttributes] (InstanceId, [Key], Value)
                        VALUES (@InstanceId, @Key, @Value)";

                    var attrParams = request.Attributes.Select(kvp => new
                    {
                        InstanceId = newInstanceId,
                        Key = kvp.Key,
                        Value = kvp.Value
                    });

                    await conn.ExecuteAsync(insertAttr, attrParams, trans);
                }

                // Insert category associations
                if (request.CategoryInstanceIds.Any())
                {
                    const string insertCat = @"
                        INSERT INTO [Instances].[ProductCategories] (InstanceId, CategoryInstanceId)
                        VALUES (@InstanceId, @CategoryInstanceId)";

                    var catParams = request.CategoryInstanceIds.Select(catId => new
                    {
                        InstanceId = newInstanceId,
                        CategoryInstanceId = catId
                    });

                    await conn.ExecuteAsync(insertCat, catParams, trans);
                }

                return newInstanceId;
            });
        }

        public async Task<IEnumerable<Product>> SearchAsync(ProductSearchRequest request)
        {
            PreConditions.ParameterNotNull(request, nameof(request));

            return await _Executor.ExecuteAsync(async (conn, trans) =>
            {
                // EVAL: SqlServerQueryProvider builds the WHERE clause dynamically
                // so we never manually concatenate SQL strings — prevents injection
                // and keeps each filter condition isolated.
                var query = SqlServerQueryProvider.Empty
                    .SetTargetTableAlias("p");

                if (!string.IsNullOrWhiteSpace(request.Name))
                    query.WhereEquals("Name", "Name", request.Name);

                // EVAL: Each attribute filter becomes a correlated EXISTS subquery.
                // AND semantics: product must match ALL supplied key/value pairs.
                // query.Where() is used directly here rather than WhereExists() because
                // WhereExists() requires a column name — EXISTS has no column, just a subquery.
                int paramIndex = 0;
                foreach (var attr in request.Attributes)
                {
                    string keyParam = $"AttrKey{paramIndex}";
                    string valParam = $"AttrVal{paramIndex}";
                    query.Where($@"EXISTS (
                        SELECT 1 FROM [Instances].[ProductAttributes] pa
                        WHERE pa.InstanceId = p.InstanceId
                          AND pa.[Key] = @{keyParam}
                          AND pa.[Value] = @{valParam}
                    )");
                    query.AddParameter(keyParam, attr.Key);
                    query.AddParameter(valParam, attr.Value);
                    paramIndex++;
                }

                // Category filter — product must belong to ALL supplied categories
                foreach (var catId in request.CategoryInstanceIds)
                {
                    string catParam = $"CatId{paramIndex}";
                    query.Where($@"EXISTS (
                        SELECT 1 FROM [Instances].[ProductCategories] pc
                        WHERE pc.InstanceId = p.InstanceId
                          AND pc.CategoryInstanceId = @{catParam}
                    )");
                    query.AddParameter(catParam, catId);
                    paramIndex++;
                }

                string sql = $@"
                    SELECT p.InstanceId, p.Name, p.Description, p.ProductImageUris, p.ValidSkus, p.CreatedTimestamp
                    FROM [Instances].[Products] p
                    {query.WhereClause}
                    ORDER BY p.CreatedTimestamp DESC";

                var rows = await conn.QueryAsync(sql, query.Parameters, trans);
                var products = rows.Select(r => MapProduct(r)).ToList();

                foreach (var product in products)
                {
                    await HydrateAttributesAsync(conn, trans, product);
                    await HydrateCategoriesAsync(conn, trans, product);
                }

                return (IEnumerable<Product>)products;
            });
        }

        // --- Private helpers ---

        private static Product MapProduct(dynamic row)
        {
            return new Product
            {
                InstanceId = row.InstanceId,
                Name = row.Name,
                Description = row.Description,
                CreatedTimestamp = row.CreatedTimestamp,
                ProductImageUris = JsonConvert.DeserializeObject<List<string>>(row.ProductImageUris) ?? new List<string>(),
                ValidSkus = JsonConvert.DeserializeObject<List<string>>(row.ValidSkus) ?? new List<string>()
            };
        }

        private static async Task HydrateAttributesAsync(System.Data.IDbConnection conn, System.Data.IDbTransaction trans, Product product)
        {
            const string sql = @"
                SELECT [Key], [Value]
                FROM [Instances].[ProductAttributes]
                WHERE InstanceId = @InstanceId";

            var attrs = await conn.QueryAsync(sql, new { product.InstanceId }, trans);
            product.Attributes = attrs.ToDictionary(a => (string)a.Key, a => (string)a.Value);
        }

        private static async Task HydrateCategoriesAsync(System.Data.IDbConnection conn, System.Data.IDbTransaction trans, Product product)
        {
            const string sql = @"
                SELECT CategoryInstanceId
                FROM [Instances].[ProductCategories]
                WHERE InstanceId = @InstanceId";

            var cats = await conn.QueryAsync<int>(sql, new { product.InstanceId }, trans);
            product.CategoryInstanceIds = cats.ToList();
        }
    }
}
