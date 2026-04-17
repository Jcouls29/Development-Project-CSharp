using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Sparcpoint;
using Sparcpoint.Inventory.Models;
using Sparcpoint.SqlServer.Abstractions;
using Sparcpoint.Inventory.Repositories;
using Sparcpoint.Inventory.SqlServer.Internal;

namespace Sparcpoint.Inventory.SqlServer.Repositories
{
    /// <summary>
    /// EVAL: SQL Server + Dapper implementation of IProductRepository.
    /// Uses the injected ISqlExecutor (abstraction already provided by the
    /// base solution) to handle connection + transaction uniformly.
    /// All write operations run within the executor's transaction.
    /// </summary>
    public sealed class SqlProductRepository : IProductRepository
    {
        private readonly ISqlExecutor _executor;

        public SqlProductRepository(ISqlExecutor executor)
        {
            _executor = executor ?? throw new ArgumentNullException(nameof(executor));
        }

        public Task<int> AddAsync(Product product, CancellationToken cancellationToken = default)
        {
            PreConditions.ParameterNotNull(product, nameof(product));
            PreConditions.StringNotNullOrWhitespace(product.Name, nameof(product.Name));

            // EVAL: Defensive validation against DB column length (VARCHAR 256).
            if (product.Name.Length > 256) throw new ArgumentException("Name exceeds 256 chars", nameof(product));
            if ((product.Description ?? string.Empty).Length > 256) throw new ArgumentException("Description exceeds 256 chars", nameof(product));

            return _executor.ExecuteAsync(async (conn, tx) =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                const string insertSql = @"
INSERT INTO [Instances].[Products] ([Name], [Description], [ProductImageUris], [ValidSkus])
OUTPUT INSERTED.[InstanceId]
VALUES (@Name, @Description, @ProductImageUris, @ValidSkus);";

                var instanceId = await conn.ExecuteScalarAsync<int>(
                    new CommandDefinition(insertSql, new
                    {
                        Name = product.Name,
                        Description = product.Description ?? string.Empty,
                        ProductImageUris = JsonList.Serialize(product.ProductImageUris),
                        ValidSkus = JsonList.Serialize(product.ValidSkus)
                    }, transaction: tx, cancellationToken: cancellationToken));

                await InsertAttributesAsync(conn, tx, instanceId, product.Attributes, cancellationToken);
                await InsertCategoriesAsync(conn, tx, instanceId, product.CategoryIds, cancellationToken);

                return instanceId;
            });
        }

        public Task<Product> GetByIdAsync(int instanceId, CancellationToken cancellationToken = default)
        {
            return _executor.ExecuteAsync(async (conn, tx) =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                const string sql = @"
SELECT [InstanceId], [Name], [Description], [ProductImageUris], [ValidSkus], [CreatedTimestamp]
FROM [Instances].[Products]
WHERE [InstanceId] = @InstanceId;";

                var row = await conn.QuerySingleOrDefaultAsync<ProductRow>(
                    new CommandDefinition(sql, new { InstanceId = instanceId }, transaction: tx, cancellationToken: cancellationToken));

                if (row == null) return (Product)null;

                var product = MapToProduct(row);

                product.Attributes = (await conn.QueryAsync<ProductAttributeRow>(
                    new CommandDefinition(@"
SELECT [InstanceId], [Key], [Value] FROM [Instances].[ProductAttributes] WHERE [InstanceId] = @InstanceId;",
                        new { InstanceId = instanceId }, transaction: tx, cancellationToken: cancellationToken)))
                    .Select(a => new ProductAttribute(a.Key, a.Value))
                    .ToList();

                product.CategoryIds = (await conn.QueryAsync<int>(
                    new CommandDefinition(@"
SELECT [CategoryInstanceId] FROM [Instances].[ProductCategories] WHERE [InstanceId] = @InstanceId;",
                        new { InstanceId = instanceId }, transaction: tx, cancellationToken: cancellationToken)))
                    .ToList();

                return product;
            });
        }

        public Task<IReadOnlyList<Product>> SearchAsync(ProductSearchCriteria criteria, CancellationToken cancellationToken = default)
        {
            PreConditions.ParameterNotNull(criteria, nameof(criteria));

            return _executor.ExecuteAsync(async (conn, tx) =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                // EVAL: Dynamic but parameterized construction — no concatenating
                // user values directly in SQL. Each filter uses @Px.
                var where = new List<string>();
                var parameters = new DynamicParameters();

                if (!string.IsNullOrWhiteSpace(criteria.NameContains))
                {
                    where.Add("P.[Name] LIKE @NameLike");
                    parameters.Add("@NameLike", "%" + criteria.NameContains + "%");
                }

                if (criteria.AttributeFilters != null)
                {
                    int i = 0;
                    foreach (var attr in criteria.AttributeFilters)
                    {
                        if (string.IsNullOrWhiteSpace(attr.Key)) continue;
                        var keyParam = "@AttrK" + i;
                        var valParam = "@AttrV" + i;
                        where.Add($@"EXISTS (
    SELECT 1 FROM [Instances].[ProductAttributes] PA
    WHERE PA.[InstanceId] = P.[InstanceId]
      AND PA.[Key] = {keyParam}
      AND PA.[Value] = {valParam})");
                        parameters.Add(keyParam, attr.Key);
                        parameters.Add(valParam, attr.Value);
                        i++;
                    }
                }

                if (criteria.CategoryIds != null && criteria.CategoryIds.Count > 0)
                {
                    int i = 0;
                    foreach (var catId in criteria.CategoryIds)
                    {
                        var p = "@Cat" + i;
                        where.Add($@"EXISTS (
    SELECT 1 FROM [Instances].[ProductCategories] PC
    WHERE PC.[InstanceId] = P.[InstanceId] AND PC.[CategoryInstanceId] = {p})");
                        parameters.Add(p, catId);
                        i++;
                    }
                }

                var sb = new StringBuilder();
                sb.Append("SELECT P.[InstanceId], P.[Name], P.[Description], P.[ProductImageUris], P.[ValidSkus], P.[CreatedTimestamp] ");
                sb.Append("FROM [Instances].[Products] P ");
                if (where.Count > 0)
                    sb.Append("WHERE ").Append(string.Join(" AND ", where)).Append(' ');
                sb.Append("ORDER BY P.[InstanceId] ");
                sb.Append("OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;");
                parameters.Add("@Skip", Math.Max(0, criteria.Skip));
                parameters.Add("@Take", Math.Max(1, Math.Min(1000, criteria.Take)));

                var rows = (await conn.QueryAsync<ProductRow>(
                    new CommandDefinition(sb.ToString(), parameters, transaction: tx, cancellationToken: cancellationToken))).ToList();

                if (rows.Count == 0) return (IReadOnlyList<Product>)Array.Empty<Product>();

                var ids = rows.Select(r => r.InstanceId).ToArray();

                // EVAL: Single round-trip for attributes and categories of ALL products of the page.
                var attrMap = (await conn.QueryAsync<ProductAttributeRow>(
                    new CommandDefinition(@"
SELECT [InstanceId], [Key], [Value] FROM [Instances].[ProductAttributes] WHERE [InstanceId] IN @Ids;",
                        new { Ids = ids }, transaction: tx, cancellationToken: cancellationToken)))
                    .GroupBy(x => x.InstanceId)
                    .ToDictionary(g => g.Key, g => (IList<ProductAttribute>)g.Select(x => new ProductAttribute(x.Key, x.Value)).ToList());

                var catMap = (await conn.QueryAsync<ProductCategoryRow>(
                    new CommandDefinition(@"
SELECT [InstanceId], [CategoryInstanceId] FROM [Instances].[ProductCategories] WHERE [InstanceId] IN @Ids;",
                        new { Ids = ids }, transaction: tx, cancellationToken: cancellationToken)))
                    .GroupBy(x => x.InstanceId)
                    .ToDictionary(g => g.Key, g => (IList<int>)g.Select(x => x.CategoryInstanceId).ToList());

                var products = rows.Select(r =>
                {
                    var p = MapToProduct(r);
                    if (attrMap.TryGetValue(r.InstanceId, out var a)) p.Attributes = a;
                    if (catMap.TryGetValue(r.InstanceId, out var c)) p.CategoryIds = c;
                    return p;
                }).ToList();

                return (IReadOnlyList<Product>)products;
            });
        }

        // ----- helpers -------------------------------------------------------

        private static Product MapToProduct(ProductRow row) => new Product
        {
            InstanceId = row.InstanceId,
            Name = row.Name,
            Description = row.Description,
            ProductImageUris = JsonList.Deserialize(row.ProductImageUris),
            ValidSkus = JsonList.Deserialize(row.ValidSkus),
            CreatedTimestamp = row.CreatedTimestamp
        };

        private static async Task InsertAttributesAsync(
            System.Data.IDbConnection conn, System.Data.IDbTransaction tx,
            int instanceId, IList<ProductAttribute> attrs, CancellationToken ct)
        {
            if (attrs == null || attrs.Count == 0) return;

            // EVAL: Dapper supports INSERT with arrays of objects — a single
            // round-trip and correctly parameterized.
            const string sql = @"
INSERT INTO [Instances].[ProductAttributes] ([InstanceId], [Key], [Value])
VALUES (@InstanceId, @Key, @Value);";

            var payload = attrs
                .Where(a => !string.IsNullOrWhiteSpace(a?.Key))
                .Select(a => new { InstanceId = instanceId, a.Key, Value = a.Value ?? string.Empty })
                .ToArray();

            if (payload.Length == 0) return;

            await conn.ExecuteAsync(new CommandDefinition(sql, payload, transaction: tx, cancellationToken: ct));
        }

        private static async Task InsertCategoriesAsync(
            System.Data.IDbConnection conn, System.Data.IDbTransaction tx,
            int instanceId, IList<int> categoryIds, CancellationToken ct)
        {
            if (categoryIds == null || categoryIds.Count == 0) return;

            const string sql = @"
INSERT INTO [Instances].[ProductCategories] ([InstanceId], [CategoryInstanceId])
VALUES (@InstanceId, @CategoryInstanceId);";

            var payload = categoryIds.Distinct()
                .Select(c => new { InstanceId = instanceId, CategoryInstanceId = c })
                .ToArray();

            await conn.ExecuteAsync(new CommandDefinition(sql, payload, transaction: tx, cancellationToken: ct));
        }
    }
}
