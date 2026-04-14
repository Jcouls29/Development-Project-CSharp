using Dapper;
using Sparcpoint.Inventory.Abstractions;
using Sparcpoint.Inventory.Models;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory.Repositories.Sql
{
    public sealed class SqlProductRepository : IProductRepository
    {
        private readonly ISqlExecutor _executor;

        public SqlProductRepository(ISqlExecutor executor)
        {
            _executor = executor ?? throw new ArgumentNullException(nameof(executor));
        }

        public Task<int> AddAsync(Product product, CancellationToken cancellationToken = default)
        {
            return _executor.ExecuteAsync(async (conn, tx) =>
            {
                var id = await conn.ExecuteScalarAsync<int>(Queries.InsertProduct, new
                {
                    product.Name,
                    product.Description,
                    // EVAL: Simple JSON-ish serialization for list columns; the schema stores VARCHAR(MAX).
                    ProductImageUris = string.Join("|", product.ProductImageUris ?? Array.Empty<string>()),
                    ValidSkus = string.Join("|", product.ValidSkus ?? Array.Empty<string>()),
                }, tx);

                foreach (var attribute in product.Attributes ?? Array.Empty<ProductAttribute>())
                {
                    await conn.ExecuteAsync(Queries.InsertProductAttribute,
                        new { InstanceId = id, attribute.Key, attribute.Value }, tx);
                }

                foreach (var categoryId in product.CategoryIds ?? Array.Empty<int>())
                {
                    await conn.ExecuteAsync(Queries.InsertProductCategory,
                        new { InstanceId = id, CategoryInstanceId = categoryId }, tx);
                }

                return id;
            });
        }

        public Task<Product?> GetByIdAsync(int instanceId, CancellationToken cancellationToken = default)
        {
            return _executor.ExecuteAsync<Product?>(async (conn, tx) =>
            {
                var row = await conn.QuerySingleOrDefaultAsync<ProductRow>(Queries.SelectProductById, new { InstanceId = instanceId }, tx);
                if (row is null) return null;

                var attributes = (await conn.QueryAsync<ProductAttribute>(Queries.SelectProductAttributes, new { InstanceId = instanceId }, tx)).ToArray();
                var categoryIds = (await conn.QueryAsync<int>(Queries.SelectProductCategories, new { InstanceId = instanceId }, tx)).ToArray();

                return Materialize(row, attributes, categoryIds);
            });
        }

        public Task<IReadOnlyList<Product>> SearchAsync(ProductSearchCriteria criteria, CancellationToken cancellationToken = default)
        {
            if (criteria is null) throw new ArgumentNullException(nameof(criteria));

            // EVAL: Dynamic WHERE built with parameterized fragments — resists injection
            // while allowing any combination of filters (open-closed).
            var sql = new StringBuilder(@"
SELECT DISTINCT p.[InstanceId], p.[Name], p.[Description], p.[ProductImageUris], p.[ValidSkus], p.[CreatedTimestamp]
FROM [Instances].[Products] p ");

            var parameters = new DynamicParameters();
            var filters = new List<string>();

            if (criteria.AttributeMatches.Count > 0)
            {
                for (int i = 0; i < criteria.AttributeMatches.Count; i++)
                {
                    var alias = $"a{i}";
                    sql.Append($"INNER JOIN [Instances].[ProductAttributes] {alias} ON {alias}.[InstanceId] = p.[InstanceId] AND {alias}.[Key] = @Key{i} AND {alias}.[Value] = @Value{i} ");
                    parameters.Add($"Key{i}", criteria.AttributeMatches[i].Key);
                    parameters.Add($"Value{i}", criteria.AttributeMatches[i].Value);
                }
            }

            if (criteria.CategoryIds.Count > 0)
            {
                sql.Append(@"INNER JOIN [Instances].[ProductCategories] pc ON pc.[InstanceId] = p.[InstanceId] ");
                filters.Add("pc.[CategoryInstanceId] IN @CategoryIds");
                parameters.Add("CategoryIds", criteria.CategoryIds);
            }

            if (!string.IsNullOrWhiteSpace(criteria.NameContains))
            {
                filters.Add("p.[Name] LIKE @NameContains");
                parameters.Add("NameContains", $"%{criteria.NameContains}%");
            }

            if (!string.IsNullOrWhiteSpace(criteria.DescriptionContains))
            {
                filters.Add("p.[Description] LIKE @DescriptionContains");
                parameters.Add("DescriptionContains", $"%{criteria.DescriptionContains}%");
            }

            if (!string.IsNullOrWhiteSpace(criteria.Sku))
            {
                filters.Add("p.[ValidSkus] LIKE @SkuLike");
                parameters.Add("SkuLike", $"%{criteria.Sku}%");
            }

            if (filters.Count > 0)
                sql.Append("WHERE ").Append(string.Join(" AND ", filters)).Append(' ');

            sql.Append("ORDER BY p.[InstanceId] OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;");
            parameters.Add("Skip", criteria.Skip);
            parameters.Add("Take", criteria.Take);

            return _executor.ExecuteAsync<IReadOnlyList<Product>>(async (conn, tx) =>
            {
                var rows = (await conn.QueryAsync<ProductRow>(sql.ToString(), parameters, tx)).ToArray();
                return rows.Select(r => Materialize(r, Array.Empty<ProductAttribute>(), Array.Empty<int>())).ToArray();
            });
        }

        private static Product Materialize(ProductRow row, IReadOnlyList<ProductAttribute> attributes, IReadOnlyList<int> categoryIds)
            => new()
            {
                InstanceId = row.InstanceId,
                Name = row.Name,
                Description = row.Description,
                ProductImageUris = string.IsNullOrEmpty(row.ProductImageUris) ? Array.Empty<string>() : row.ProductImageUris.Split('|'),
                ValidSkus = string.IsNullOrEmpty(row.ValidSkus) ? Array.Empty<string>() : row.ValidSkus.Split('|'),
                CreatedTimestamp = row.CreatedTimestamp,
                Attributes = attributes,
                CategoryIds = categoryIds,
            };

        private sealed class ProductRow
        {
            public int InstanceId { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string ProductImageUris { get; set; } = string.Empty;
            public string ValidSkus { get; set; } = string.Empty;
            public DateTime CreatedTimestamp { get; set; }
        }
    }
}
