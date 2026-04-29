using Dapper;
using Sparcpoint.Inventory.Abstractions;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory.SqlServer
{
    /// <summary>
    /// EVAL: went with Dapper instead of EF Core because the EAV schema and dynamic WHERE building
    /// would get messy with an ORM - easier to just write the SQL directly
    /// </summary>
    public class SqlProductRepository : IProductRepository
    {
        private readonly ISqlExecutor _Executor;

        public SqlProductRepository(ISqlExecutor executor)
        {
            _Executor = executor ?? throw new ArgumentNullException(nameof(executor));
        }

        public async Task<int> AddAsync(CreateProductRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            PreConditions.StringNotNullOrWhitespace(request.Name, nameof(request.Name));
            PreConditions.StringNotNullOrWhitespace(request.Description, nameof(request.Description));

            return await _Executor.ExecuteAsync(async (conn, tx) =>
            {
                // Insert the base product record
                var productId = await conn.ExecuteScalarAsync<int>(@"
                    INSERT INTO [Instances].[Products] ([Name], [Description], [ProductImageUris], [ValidSkus])
                    VALUES (@Name, @Description, @ProductImageUris, @ValidSkus);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);",
                    new
                    {
                        request.Name,
                        request.Description,
                        ProductImageUris = request.ProductImageUris ?? string.Empty,
                        ValidSkus = request.ValidSkus ?? string.Empty
                    }, tx);

                // Insert arbitrary metadata attributes
                // EVAL: row-by-row inserts are fine for normal writes - for bulk imports a TVP would be faster
                if (request.Attributes != null)
                {
                    foreach (var attr in request.Attributes)
                    {
                        await conn.ExecuteAsync(@"
                            INSERT INTO [Instances].[ProductAttributes] ([InstanceId], [Key], [Value])
                            VALUES (@InstanceId, @Key, @Value)",
                            new { InstanceId = productId, attr.Key, attr.Value }, tx);
                    }
                }

                // Link product to categories
                if (request.CategoryIds != null)
                {
                    var categoryIds = request.CategoryIds.ToList();
                    if (categoryIds.Count > 0)
                    {
                        // EVAL: validate category IDs before inserting so we return a 400 instead of
                        // letting the FK violation (FK_ProductCategories_Categories) blow up as a 500
                        var existingCount = await conn.ExecuteScalarAsync<int>(
                            "SELECT COUNT(*) FROM [Instances].[Categories] WHERE [InstanceId] IN @Ids",
                            new { Ids = categoryIds }, tx);

                        if (existingCount != categoryIds.Count)
                            throw new ArgumentException(
                                "One or more CategoryIds do not reference existing categories.",
                                nameof(request.CategoryIds));

                        foreach (var categoryId in categoryIds)
                        {
                            await conn.ExecuteAsync(@"
                                INSERT INTO [Instances].[ProductCategories] ([InstanceId], [CategoryInstanceId])
                                VALUES (@InstanceId, @CategoryInstanceId)",
                                new { InstanceId = productId, CategoryInstanceId = categoryId }, tx);
                        }
                    }
                }

                return productId;
            });
        }

        public async Task<IEnumerable<Product>> SearchAsync(ProductSearchFilter filter)
        {
            return await _Executor.ExecuteAsync(async (conn, tx) =>
            {
                // EVAL: SqlServerQueryProvider handles WHERE and ORDER BY - SELECT/FROM is hardcoded
                // because the provider has no API for those clauses, it just adds to a base query.
                // Name LIKE uses Where() + AddParameter() because WhereEquals generates "=" not LIKE.
                // CategoryIds and EAV filters use raw Where() with composed subquery strings because
                // the provider's WhereExists formats as "{col} EXISTS {expr}" which isn't valid SQL here.
                // GetNextParameterName() keeps parameter names unique across the whole query and
                // AddParameter() registers each value so provider.Parameters has the full bag for Dapper.
                var provider = SqlServerQueryProvider.Empty;

                // Name filter: partial match (LIKE, not equality WhereEquals does not apply)
                if (!string.IsNullOrWhiteSpace(filter?.Name))
                {
                    provider.Where("p.[Name] LIKE @Name");
                    provider.AddParameter("Name", $"%{filter.Name}%");
                }

                // EVAL: each attribute filter is an EXISTS subquery, not a JOIN - joining on EAV tables
                // causes row multiplication (N attributes * M filter criteria before DISTINCT), EXISTS
                // is cleaner and has a more predictable query plan.
                // Key and val get different numeric suffixes from GetNextParameterName() - that's on
                // purpose so every param name is globally unique. The table alias uses just the key's
                // suffix (via Substring) as the per-iteration discriminator, val suffix only shows up
                // as a param name so there's no conflict.
                if (filter?.Attributes != null)
                {
                    foreach (var attr in filter.Attributes)
                    {
                        var keyParam = provider.GetNextParameterName("AttrKey");  // e.g. "AttrKey0"
                        var valParam = provider.GetNextParameterName("AttrVal");  // e.g. "AttrVal1"
                        var alias = keyParam.Substring("AttrKey".Length);         // e.g. "0" - unique per iteration
                        provider.Where($@"EXISTS (
                            SELECT 1 FROM [Instances].[ProductAttributes] pa{alias}
                            WHERE pa{alias}.[InstanceId] = p.[InstanceId]
                              AND pa{alias}.[Key] = @{keyParam}
                              AND pa{alias}.[Value] = @{valParam})");
                        provider.AddParameter(keyParam, attr.Key);
                        provider.AddParameter(valParam, attr.Value);
                    }
                }

                // Category filter: product must belong to at least one of the specified categories.
                // The IN list is passed as a parameter; Dapper expands int[] to an IN clause natively.
                if (filter?.CategoryIds != null && filter.CategoryIds.Any())
                {
                    provider.Where(@"EXISTS (
                        SELECT 1 FROM [Instances].[ProductCategories] pc
                        WHERE pc.[InstanceId] = p.[InstanceId]
                          AND pc.[CategoryInstanceId] IN @CategoryIds)");
                    provider.AddParameter("CategoryIds", filter.CategoryIds);
                }

                // ORDER BY via the provider so the clause is managed consistently
                provider.OrderByAscending("[Name]");

                var sqlBase = @"
                    SELECT p.[InstanceId], p.[Name], p.[Description], p.[ProductImageUris], p.[ValidSkus], p.[CreatedTimestamp]
                    FROM [Instances].[Products] p";

                var sqlBuilder = new StringBuilder(sqlBase);
                sqlBuilder.Append($" {provider.WhereClause}");
                sqlBuilder.Append($" {provider.OrderByClause}");

                // EVAL: OFFSET-FETCH needs ORDER BY which is guaranteed above. Pagination only kicks in
                // when both Page and PageSize are supplied - if only one is given we ignore it and return
                // everything. PageSize is clamped to 200 instead of throwing because that's friendlier
                // for clients that pass a huge value - request stays valid and the DB is protected.
                if (filter?.Page != null && filter?.PageSize != null)
                {
                    // EVAL: Page < 1 produces a negative OFFSET which is a SQL error, so we throw
                    // instead of clamping - this is always a caller bug, not just an oversized value
                    if (filter.Page < 1)
                        throw new ArgumentOutOfRangeException(nameof(filter.Page), "Page must be >= 1.");

                    var pageSize = Math.Min(filter.PageSize.Value, 200);
                    provider.AddParameter("Page", filter.Page.Value);
                    provider.AddParameter("PageSize", pageSize);
                    sqlBuilder.Append(" OFFSET (@Page - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY");
                }

                // provider.Parameters returns a defensive copy (Dictionary<string, object>).
                // Dapper accepts IDictionary<string, object> directly - no DynamicParameters wrapper needed.
                var products = (await conn.QueryAsync<Product>(sqlBuilder.ToString(), provider.Parameters, tx)).ToList();

                // Load attributes and category links for all returned products in two bulk queries
                // to avoid N+1 queries
                if (products.Any())
                {
                    var ids = products.Select(p => p.InstanceId).ToArray();

                    var attributes = await conn.QueryAsync<(int InstanceId, string Key, string Value)>(
                        "SELECT [InstanceId], [Key], [Value] FROM [Instances].[ProductAttributes] WHERE [InstanceId] IN @Ids",
                        new { Ids = ids }, tx);

                    var categoryLinks = await conn.QueryAsync<(int InstanceId, int CategoryInstanceId)>(
                        "SELECT [InstanceId], [CategoryInstanceId] FROM [Instances].[ProductCategories] WHERE [InstanceId] IN @Ids",
                        new { Ids = ids }, tx);

                    var attrLookup = attributes
                        .GroupBy(a => a.InstanceId)
                        .ToDictionary(g => g.Key, g => g.Select(a => new ProductAttribute { Key = a.Key, Value = a.Value }).ToList());

                    var catLookup = categoryLinks
                        .GroupBy(c => c.InstanceId)
                        .ToDictionary(g => g.Key, g => g.Select(c => c.CategoryInstanceId).ToList());

                    foreach (var product in products)
                    {
                        product.Attributes = attrLookup.TryGetValue(product.InstanceId, out var attrs)
                            ? attrs : new List<ProductAttribute>();
                        product.CategoryIds = catLookup.TryGetValue(product.InstanceId, out var cats)
                            ? cats : new List<int>();
                    }
                }

                return products;
            });
        }
    }
}
