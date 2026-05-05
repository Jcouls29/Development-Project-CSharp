using Dapper;
using Newtonsoft.Json;
using Sparcpoint.Inventory.Models;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory.SqlServer
{
    public class SqlProductRepository : IProductRepository
    {
        private readonly ISqlExecutor _Executor;

        public SqlProductRepository(ISqlExecutor executor)
        {
            _Executor = executor ?? throw new ArgumentNullException(nameof(executor));
        }

        public async Task<int> AddAsync(ProductEntry product)
        {
            if (product == null) throw new ArgumentNullException(nameof(product));
            if (string.IsNullOrWhiteSpace(product.Name))
                throw new ArgumentException("Name is required.", nameof(product.Name));

            return await _Executor.ExecuteAsync(async (conn, trans) =>
            {
                // EVAL: OUTPUT INSERTED avoids a separate SELECT @@IDENTITY call and is safe under concurrency
                int productId = await conn.QuerySingleAsync<int>(@"
                    INSERT INTO [Instances].[Products] ([Name], [Description], [ProductImageUris], [ValidSkus])
                    OUTPUT INSERTED.[InstanceId]
                    VALUES (@Name, @Description, @ProductImageUris, @ValidSkus)",
                    new
                    {
                        product.Name,
                        Description = product.Description ?? string.Empty,
                        ProductImageUris = SerializeArray(product.ProductImageUris),
                        ValidSkus = SerializeArray(product.ValidSkus)
                    }, trans);

                await InsertAttributesAsync(conn, trans, productId, product.Attributes);
                await InsertCategoryLinksAsync(conn, trans, productId, product.CategoryIds);

                return productId;
            });
        }

        public async Task<ProductEntry> GetByIdAsync(int instanceId)
        {
            return await _Executor.ExecuteAsync(async (conn, trans) =>
            {
                var record = await conn.QuerySingleOrDefaultAsync<ProductDbRecord>(@"
                    SELECT [InstanceId], [Name], [Description], [ProductImageUris], [ValidSkus], [CreatedTimestamp]
                    FROM [Instances].[Products]
                    WHERE [InstanceId] = @InstanceId",
                    new { InstanceId = instanceId }, trans);

                if (record == null) return null;

                return await LoadSingleEntryAsync(conn, trans, record);
            });
        }

        public async Task<IEnumerable<ProductEntry>> SearchAsync(ProductSearchFilter filter = null)
        {
            return await _Executor.ExecuteAsync(async (conn, trans) =>
            {
                var parameters = new DynamicParameters();
                var whereConditions = new List<string>();

                if (!string.IsNullOrWhiteSpace(filter?.Name))
                {
                    whereConditions.Add("p.[Name] LIKE @Name");
                    parameters.Add("Name", $"%{filter.Name}%");
                }

                // EVAL: Each attribute gets its own correlated EXISTS subquery so all conditions
                // must be satisfied simultaneously (AND semantics). This hits the
                // IX_ProductAttributes_Key_Value index on every probe.
                int attrIdx = 0;
                foreach (var attr in filter?.Attributes ?? new Dictionary<string, string>())
                {
                    string keyParam = $"attrKey{attrIdx}";
                    string valParam = $"attrVal{attrIdx}";
                    whereConditions.Add($@"EXISTS (
                        SELECT 1 FROM [Instances].[ProductAttributes] pa{attrIdx}
                        WHERE pa{attrIdx}.[InstanceId] = p.[InstanceId]
                          AND pa{attrIdx}.[Key]   = @{keyParam}
                          AND pa{attrIdx}.[Value] = @{valParam})");
                    parameters.Add(keyParam, attr.Key);
                    parameters.Add(valParam, attr.Value);
                    attrIdx++;
                }

                if (filter?.CategoryIds?.Length > 0)
                {
                    // EVAL: Dapper expands IN @CategoryIds to IN (1,2,...) automatically
                    whereConditions.Add(@"EXISTS (
                        SELECT 1 FROM [Instances].[ProductCategories] pc
                        WHERE pc.[InstanceId] = p.[InstanceId]
                          AND pc.[CategoryInstanceId] IN @CategoryIds)");
                    parameters.Add("CategoryIds", filter.CategoryIds);
                }

                string whereClause = whereConditions.Any()
                    ? "WHERE " + string.Join("\n      AND ", whereConditions)
                    : string.Empty;

                string sql = $@"
                    SELECT DISTINCT
                        p.[InstanceId], p.[Name], p.[Description],
                        p.[ProductImageUris], p.[ValidSkus], p.[CreatedTimestamp]
                    FROM [Instances].[Products] p
                    {whereClause}
                    ORDER BY p.[Name] ASC";

                var products = (await conn.QueryAsync<ProductDbRecord>(sql, parameters, trans)).ToList();

                if (!products.Any())
                    return Enumerable.Empty<ProductEntry>();

                return await LoadManyEntriesAsync(conn, trans, products);
            });
        }

        // EVAL: Batch-load attributes and categories for all matched products in two queries
        // rather than one query per product, avoiding the N+1 problem.
        private async Task<IEnumerable<ProductEntry>> LoadManyEntriesAsync(
            IDbConnection conn, IDbTransaction trans, List<ProductDbRecord> products)
        {
            int[] ids = products.Select(p => p.InstanceId).ToArray();

            var attributes = await conn.QueryAsync<AttributeRecord>(@"
                SELECT [InstanceId], [Key], [Value]
                FROM [Instances].[ProductAttributes]
                WHERE [InstanceId] IN @Ids",
                new { Ids = ids }, trans);

            var categoryLinks = await conn.QueryAsync<CategoryLinkRecord>(@"
                SELECT [InstanceId], [CategoryInstanceId]
                FROM [Instances].[ProductCategories]
                WHERE [InstanceId] IN @Ids",
                new { Ids = ids }, trans);

            var attrLookup = attributes
                .GroupBy(a => a.InstanceId)
                .ToDictionary(g => g.Key, g => g.ToDictionary(a => a.Key, a => a.Value));

            var catLookup = categoryLinks
                .GroupBy(c => c.InstanceId)
                .ToDictionary(g => g.Key, g => g.Select(c => c.CategoryInstanceId).ToArray());

            return products.Select(p => MapToEntry(p, attrLookup, catLookup));
        }

        private async Task<ProductEntry> LoadSingleEntryAsync(
            IDbConnection conn, IDbTransaction trans, ProductDbRecord record)
        {
            var attributes = await conn.QueryAsync<AttributeRecord>(@"
                SELECT [InstanceId], [Key], [Value]
                FROM [Instances].[ProductAttributes]
                WHERE [InstanceId] = @InstanceId",
                new { record.InstanceId }, trans);

            var categoryIds = await conn.QueryAsync<int>(@"
                SELECT [CategoryInstanceId]
                FROM [Instances].[ProductCategories]
                WHERE [InstanceId] = @InstanceId",
                new { record.InstanceId }, trans);

            return new ProductEntry
            {
                InstanceId = record.InstanceId,
                Name = record.Name,
                Description = record.Description,
                ProductImageUris = DeserializeArray(record.ProductImageUris),
                ValidSkus = DeserializeArray(record.ValidSkus),
                CreatedTimestamp = record.CreatedTimestamp,
                Attributes = attributes.ToDictionary(a => a.Key, a => a.Value),
                CategoryIds = categoryIds.ToArray()
            };
        }

        private static ProductEntry MapToEntry(
            ProductDbRecord record,
            Dictionary<int, Dictionary<string, string>> attrLookup,
            Dictionary<int, int[]> catLookup)
        {
            return new ProductEntry
            {
                InstanceId = record.InstanceId,
                Name = record.Name,
                Description = record.Description,
                ProductImageUris = DeserializeArray(record.ProductImageUris),
                ValidSkus = DeserializeArray(record.ValidSkus),
                CreatedTimestamp = record.CreatedTimestamp,
                Attributes = attrLookup.TryGetValue(record.InstanceId, out var attrs)
                    ? attrs
                    : new Dictionary<string, string>(),
                CategoryIds = catLookup.TryGetValue(record.InstanceId, out var cats)
                    ? cats
                    : Array.Empty<int>()
            };
        }

        private static async Task InsertAttributesAsync(
            IDbConnection conn, IDbTransaction trans, int productId, IDictionary<string, string> attributes)
        {
            if (attributes == null || !attributes.Any()) return;

            foreach (var attr in attributes)
            {
                await conn.ExecuteAsync(@"
                    INSERT INTO [Instances].[ProductAttributes] ([InstanceId], [Key], [Value])
                    VALUES (@InstanceId, @Key, @Value)",
                    new { InstanceId = productId, attr.Key, attr.Value }, trans);
            }
        }

        private static async Task InsertCategoryLinksAsync(
            IDbConnection conn, IDbTransaction trans, int productId, int[] categoryIds)
        {
            if (categoryIds == null || !categoryIds.Any()) return;

            foreach (int catId in categoryIds)
            {
                await conn.ExecuteAsync(@"
                    INSERT INTO [Instances].[ProductCategories] ([InstanceId], [CategoryInstanceId])
                    VALUES (@InstanceId, @CategoryInstanceId)",
                    new { InstanceId = productId, CategoryInstanceId = catId }, trans);
            }
        }

        private static string SerializeArray(string[] values)
            => JsonConvert.SerializeObject(values ?? Array.Empty<string>());

        private static string[] DeserializeArray(string json)
            => string.IsNullOrEmpty(json)
                ? Array.Empty<string>()
                : JsonConvert.DeserializeObject<string[]>(json) ?? Array.Empty<string>();

        private class ProductDbRecord
        {
            public int InstanceId { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string ProductImageUris { get; set; }
            public string ValidSkus { get; set; }
            public DateTime CreatedTimestamp { get; set; }
        }

        private class AttributeRecord
        {
            public int InstanceId { get; set; }
            public string Key { get; set; }
            public string Value { get; set; }
        }

        private class CategoryLinkRecord
        {
            public int InstanceId { get; set; }
            public int CategoryInstanceId { get; set; }
        }
    }
}
