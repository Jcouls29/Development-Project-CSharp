using Dapper;
using Sparcpoint;
using Sparcpoint.Inventory.Abstract;
using Sparcpoint.Inventory.Extensions;
using Sparcpoint.Inventory.Models;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory.Implementations
{
    // EVAL: Implements IProductRepository against the provided [Instances] schema. Uses ISqlExecutor
    // from Sparcpoint.SqlServer.Abstractions for connection/transaction management and IDataSerializer
    // from Sparcpoint.Core for the VARCHAR(MAX) JSON fields on Products — both per the recommendation
    // to re-use existing system code rather than introduce new dependencies.
    public class SqlProductRepository : IProductRepository
    {
        private readonly ISqlExecutor _Executor;
        private readonly IDataSerializer _Serializer;

        public SqlProductRepository(ISqlExecutor executor, IDataSerializer serializer)
        {
            _Executor = executor ?? throw new ArgumentNullException(nameof(executor));
            _Serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        // EVAL: Product creation is one logical operation but three SQL statements (product row +
        // attributes TVP + categories TVP). Wrapping the lambda in ISqlExecutor.ExecuteAsync gives us
        // a single transaction — if any step fails, none commit. Returns the new product id.
        public Task<int> CreateAsync(CreateProductRequest request)
        {
            PreConditions.ParameterNotNull(request, nameof(request));
            PreConditions.StringNotNullOrWhitespace(request.Name, nameof(request.Name));

            return _Executor.ExecuteAsync<int>(async (conn, trans) =>
            {
                // EVAL: Truncate() defensively caps strings at the column's VARCHAR length so a long
                // input never produces a SqlException (covers the "string overruns" edge case in the spec).
                // ImageUris and ValidSkus are serialized to JSON because the provided schema stores them
                // as VARCHAR(MAX) — re-using IDataSerializer keeps the choice of serializer swappable.
                var productId = await conn.QuerySingleAsync<int>(@"
                    INSERT INTO [Instances].[Products] ([Name], [Description], [ProductImageUris], [ValidSkus])
                    VALUES (@Name, @Description, @ProductImageUris, @ValidSkus);
                    SELECT SCOPE_IDENTITY();",
                    new
                    {
                        Name = request.Name.Truncate(256),
                        Description = (request.Description ?? string.Empty).Truncate(256),
                        ProductImageUris = _Serializer.Serialize(request.ImageUris ?? Array.Empty<string>()),
                        ValidSkus = _Serializer.Serialize(request.ValidSkus ?? Array.Empty<string>())
                    },
                    trans);

                // EVAL: Uses the existing dbo.CustomAttributeList table-valued parameter rather than
                // emitting N individual INSERTs. One round-trip regardless of attribute count, and the
                // attribute key set is open — any client-supplied (Key, Value) pair is persisted
                // verbatim, satisfying requirement #2 (arbitrary metadata).
                if (request.Attributes != null && request.Attributes.Count > 0)
                {
                    var attrTable = new DataTable();
                    attrTable.Columns.Add("Key", typeof(string));
                    attrTable.Columns.Add("Value", typeof(string));

                    foreach (var kvp in request.Attributes)
                        attrTable.Rows.Add(kvp.Key.Truncate(64), kvp.Value.Truncate(512));

                    await conn.ExecuteAsync(@"
                        INSERT INTO [Instances].[ProductAttributes] ([InstanceId], [Key], [Value])
                        SELECT @ProductId, [Key], [Value] FROM @Attributes",
                        new { ProductId = productId, Attributes = attrTable.AsTableValuedParameter("dbo.CustomAttributeList") },
                        trans);
                }

                if (request.CategoryIds != null && request.CategoryIds.Count > 0)
                {
                    var catTable = new DataTable();
                    catTable.Columns.Add("Value", typeof(int));

                    foreach (var catId in request.CategoryIds)
                        catTable.Rows.Add(catId);

                    await conn.ExecuteAsync(@"
                        INSERT INTO [Instances].[ProductCategories] ([InstanceId], [CategoryInstanceId])
                        SELECT @ProductId, [Value] FROM @CategoryIds",
                        new { ProductId = productId, CategoryIds = catTable.AsTableValuedParameter("dbo.IntegerList") },
                        trans);
                }

                return productId;
            });
        }

        public Task<Product> GetByIdAsync(int productId)
        {
            return _Executor.ExecuteAsync<Product>(async (conn, trans) =>
            {
                var row = await conn.QuerySingleOrDefaultAsync<ProductRow>(@"
                    SELECT [InstanceId], [Name], [Description], [ProductImageUris], [ValidSkus], [CreatedTimestamp]
                    FROM [Instances].[Products]
                    WHERE [InstanceId] = @ProductId",
                    new { ProductId = productId }, trans);

                if (row == null) return null;

                var results = await HydrateAsync(conn, trans, new[] { row });
                return results.FirstOrDefault();
            });
        }

        // EVAL: Search composes a dynamic WHERE clause using SqlServerQueryProvider from
        // Sparcpoint.SqlServer.Abstractions. Any combination of name/category/attribute filters is
        // supported (requirement #3), and the provider sanitizes column/parameter names to prevent
        // SQL injection. EXISTS subqueries are used per category/attribute rather than JOINs so the
        // result set isn't duplicated when a product matches multiple categories or attributes.
        public Task<IEnumerable<Product>> SearchAsync(ProductSearchCriteria criteria)
        {
            PreConditions.ParameterNotNull(criteria, nameof(criteria));

            return _Executor.ExecuteAsync<IEnumerable<Product>>(async (conn, trans) =>
            {
                var query = new SqlServerQueryProvider();

                if (!string.IsNullOrWhiteSpace(criteria.NameContains))
                {
                    var nameParam = query.GetNextParameterName("@NameFilter");
                    query.Where($"p.[Name] LIKE {nameParam}")
                         .AddParameter(nameParam, $"%{criteria.NameContains}%");
                }

                if (criteria.CategoryIds != null && criteria.CategoryIds.Count > 0)
                {
                    foreach (var catId in criteria.CategoryIds)
                    {
                        var paramName = query.GetNextParameterName("@CatId");
                        query.Where($"EXISTS (SELECT 1 FROM [Instances].[ProductCategories] pc WHERE pc.[InstanceId] = p.[InstanceId] AND pc.[CategoryInstanceId] = {paramName})")
                             .AddParameter(paramName, catId);
                    }
                }

                if (criteria.AttributeFilters != null)
                {
                    foreach (var kvp in criteria.AttributeFilters)
                    {
                        var keyParam = query.GetNextParameterName("@AttrKey");
                        var valParam = query.GetNextParameterName("@AttrVal");
                        query.Where($"EXISTS (SELECT 1 FROM [Instances].[ProductAttributes] pa WHERE pa.[InstanceId] = p.[InstanceId] AND pa.[Key] = {keyParam} AND pa.[Value] = {valParam})")
                             .AddParameter(keyParam, kvp.Key)
                             .AddParameter(valParam, kvp.Value);
                    }
                }

                var sql = $@"
                    SELECT p.[InstanceId], p.[Name], p.[Description], p.[ProductImageUris], p.[ValidSkus], p.[CreatedTimestamp]
                    FROM [Instances].[Products] p
                    {query.WhereClause}
                    ORDER BY p.[CreatedTimestamp] DESC";

                var rows = (await conn.QueryAsync<ProductRow>(sql, query.Parameters, trans)).ToList();
                return await HydrateAsync(conn, trans, rows);
            });
        }

        // EVAL: Hydration is the N+1-avoidance step: one batch query loads all attributes for all
        // matched product ids, one batch query loads all category links, and both are looked up by
        // id in memory. Without this, a search that returns 100 products would issue 200 extra
        // round-trips.
        private async Task<IEnumerable<Product>> HydrateAsync(IDbConnection conn, IDbTransaction trans, IList<ProductRow> rows)
        {
            if (rows.Count == 0) return Enumerable.Empty<Product>();

            var idTable = new DataTable();
            idTable.Columns.Add("Value", typeof(int));
            foreach (var r in rows) idTable.Rows.Add(r.InstanceId);

            var attributes = (await conn.QueryAsync<ProductAttributeRow>(@"
                SELECT [InstanceId], [Key], [Value]
                FROM [Instances].[ProductAttributes]
                WHERE [InstanceId] IN (SELECT [Value] FROM @Ids)",
                new { Ids = idTable.AsTableValuedParameter("dbo.IntegerList") }, trans)).ToLookup(a => a.InstanceId);

            var categories = (await conn.QueryAsync<ProductCategoryRow>(@"
                SELECT [InstanceId], [CategoryInstanceId]
                FROM [Instances].[ProductCategories]
                WHERE [InstanceId] IN (SELECT [Value] FROM @Ids)",
                new { Ids = idTable.AsTableValuedParameter("dbo.IntegerList") }, trans)).ToLookup(c => c.InstanceId);

            return rows.Select(r => new Product
            {
                ProductId = r.InstanceId,
                Name = r.Name,
                Description = r.Description,
                ImageUris = SafeDeserializeList(r.ProductImageUris),
                ValidSkus = SafeDeserializeList(r.ValidSkus),
                CreatedTimestamp = r.CreatedTimestamp,
                Attributes = attributes[r.InstanceId].ToDictionary(a => a.Key, a => a.Value),
                CategoryIds = categories[r.InstanceId].Select(c => c.CategoryInstanceId).ToList()
            }).ToList();
        }

        private IReadOnlyList<string> SafeDeserializeList(string json)
        {
            if (string.IsNullOrEmpty(json)) return Array.Empty<string>();
            try { return _Serializer.Deserialize<List<string>>(json) ?? (IReadOnlyList<string>)Array.Empty<string>(); }
            catch { return Array.Empty<string>(); }
        }

        private class ProductRow
        {
            public int InstanceId { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string ProductImageUris { get; set; }
            public string ValidSkus { get; set; }
            public DateTime CreatedTimestamp { get; set; }
        }

        private class ProductAttributeRow
        {
            public int InstanceId { get; set; }
            public string Key { get; set; }
            public string Value { get; set; }
        }

        private class ProductCategoryRow
        {
            public int InstanceId { get; set; }
            public int CategoryInstanceId { get; set; }
        }
    }
}
