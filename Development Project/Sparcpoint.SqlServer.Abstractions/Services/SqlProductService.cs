using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Sparcpoint.SqlServer.Abstractions
{
    public class SqlProductService : IProductService
    {
        private readonly ISqlExecutor _SqlExecutor;

        public SqlProductService(ISqlExecutor sqlExecutor)
        {
            PreConditions.ParameterNotNull(sqlExecutor, nameof(sqlExecutor));
            _SqlExecutor = sqlExecutor;
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            PreConditions.ParameterNotNull(product, nameof(product));
            PreConditions.StringNotNullOrWhitespace(product.Name, nameof(product.Name));

            return await _SqlExecutor.ExecuteAsync(async (conn, trans) =>
            {
                var instanceId = await conn.ExecuteScalarAsync<int>(@"
                    INSERT INTO [Instances].[Products] ([Name], [Description], [ProductImageUris], [ValidSkus])
                    VALUES (@Name, @Description, @ProductImageUris, @ValidSkus);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);",
                    new
                    {
                        product.Name,
                        Description = product.Description ?? string.Empty,
                        ProductImageUris = product.ProductImageUris ?? string.Empty,
                        ValidSkus = product.ValidSkus ?? string.Empty
                    },
                    trans);

                product.InstanceId = instanceId;

                if (product.Metadata != null && product.Metadata.Any())
                {
                    var attributeTable = new DataTable();
                    attributeTable.Columns.Add("Key", typeof(string));
                    attributeTable.Columns.Add("Value", typeof(string));

                    foreach (var attr in product.Metadata)
                    {
                        attributeTable.Rows.Add(attr.Key, attr.Value);
                    }

                    await conn.ExecuteAsync(@"
                        INSERT INTO [Instances].[ProductAttributes] ([InstanceId], [Key], [Value])
                        SELECT @InstanceId, [Key], [Value]
                        FROM @Attributes;",
                        new
                        {
                            InstanceId = instanceId,
                            Attributes = attributeTable.AsTableValuedParameter("dbo.CustomAttributeList")
                        },
                        trans);
                }

                if (product.CategoryIds != null && product.CategoryIds.Any())
                {
                    var categoryTable = new DataTable();
                    categoryTable.Columns.Add("Value", typeof(int));

                    foreach (var categoryId in product.CategoryIds)
                    {
                        categoryTable.Rows.Add(categoryId);
                    }

                    await conn.ExecuteAsync(@"
                        INSERT INTO [Instances].[ProductCategories] ([InstanceId], [CategoryInstanceId])
                        SELECT @InstanceId, [Value]
                        FROM @CategoryIds;",
                        new
                        {
                            InstanceId = instanceId,
                            CategoryIds = categoryTable.AsTableValuedParameter("dbo.IntegerList")
                        },
                        trans);
                }

                product.CreatedTimestamp = await conn.ExecuteScalarAsync<DateTime>(@"
                    SELECT [CreatedTimestamp]
                    FROM [Instances].[Products]
                    WHERE [InstanceId] = @InstanceId;",
                    new { InstanceId = instanceId },
                    trans);

                return product;
            });
        }

        public async Task<Product> GetProductByIdAsync(int instanceId)
        {
            return await _SqlExecutor.ExecuteAsync(async (conn, trans) =>
            {
                var product = await conn.QuerySingleOrDefaultAsync<Product>(@"
                    SELECT [InstanceId], [Name], [Description], [ProductImageUris], [ValidSkus], [CreatedTimestamp]
                    FROM [Instances].[Products]
                    WHERE [InstanceId] = @InstanceId;",
                    new { InstanceId = instanceId },
                    trans);

                if (product == null)
                    return null;

                product.Metadata = (await conn.QueryAsync<ProductMetadata>(@"
                    SELECT [Key], [Value]
                    FROM [Instances].[ProductAttributes]
                    WHERE [InstanceId] = @InstanceId;",
                    new { InstanceId = instanceId },
                    trans)).ToList();

                product.CategoryIds = (await conn.QueryAsync<int>(@"
                    SELECT [CategoryInstanceId]
                    FROM [Instances].[ProductCategories]
                    WHERE [InstanceId] = @InstanceId;",
                    new { InstanceId = instanceId },
                    trans)).ToList();

                return product;
            });
        }

        public async Task<IEnumerable<Product>> SearchProductsAsync(ProductSearchCriteria criteria)
        {
            PreConditions.ParameterNotNull(criteria, nameof(criteria));

            return await _SqlExecutor.ExecuteAsync(async (conn, trans) =>
            {
                // EVAL: only non null criteria fields are added all conditions are AND combined
                var queryBuilder = new SqlServerQueryProvider();
                queryBuilder.SetTargetTableAlias("p");

                if (!string.IsNullOrWhiteSpace(criteria.NameContains))
                {
                    var param = queryBuilder.GetNextParameterName("@Name");
                    queryBuilder.Where($"p.[Name] LIKE {param}");
                    queryBuilder.AddParameter(param, $"%{criteria.NameContains}%");
                }

                if (!string.IsNullOrWhiteSpace(criteria.DescriptionContains))
                {
                    var param = queryBuilder.GetNextParameterName("@Desc");
                    queryBuilder.Where($"p.[Description] LIKE {param}");
                    queryBuilder.AddParameter(param, $"%{criteria.DescriptionContains}%");
                }

                if (!string.IsNullOrWhiteSpace(criteria.SkuContains))
                {
                    var param = queryBuilder.GetNextParameterName("@Sku");
                    queryBuilder.Where($"p.[ValidSkus] LIKE {param}");
                    queryBuilder.AddParameter(param, $"%{criteria.SkuContains}%");
                }

                if (criteria.CategoryIds != null && criteria.CategoryIds.Any())
                {
                    var paramNames = new List<string>();
                    foreach (var catId in criteria.CategoryIds)
                    {
                        var param = queryBuilder.GetNextParameterName("@CatId");
                        queryBuilder.AddParameter(param, catId);
                        paramNames.Add(param);
                    }

                    queryBuilder.Where(
                        $"EXISTS (SELECT 1 FROM [Instances].[ProductCategories] pc " +
                        $"WHERE pc.[InstanceId] = p.[InstanceId] " +
                        $"AND pc.[CategoryInstanceId] IN ({string.Join(", ", paramNames)}))");
                }

                if (criteria.Metadata != null && criteria.Metadata.Any())
                {
                    int attrIndex = 0;
                    foreach (var attr in criteria.Metadata)
                    {
                        var keyParam = queryBuilder.GetNextParameterName("@AttrKey");
                        var valParam = queryBuilder.GetNextParameterName("@AttrVal");

                        queryBuilder.Where(
                            $"EXISTS (SELECT 1 FROM [Instances].[ProductAttributes] pa{attrIndex} " +
                            $"WHERE pa{attrIndex}.[InstanceId] = p.[InstanceId] " +
                            $"AND pa{attrIndex}.[Key] = {keyParam} " +
                            $"AND pa{attrIndex}.[Value] = {valParam})");

                        queryBuilder.AddParameter(keyParam, attr.Key);
                        queryBuilder.AddParameter(valParam, attr.Value);
                        attrIndex++;
                    }
                }

                var whereClause = queryBuilder.WhereClause;
                var parameters = new DynamicParameters();
                foreach (var p in queryBuilder.Parameters)
                {
                    parameters.Add(p.Key, p.Value);
                }

                var sql = $@"
                    SELECT p.[InstanceId], p.[Name], p.[Description], p.[ProductImageUris], p.[ValidSkus], p.[CreatedTimestamp]
                    FROM [Instances].[Products] p
                    {whereClause}
                    ORDER BY p.[CreatedTimestamp] DESC;";

                var products = (await conn.QueryAsync<Product>(sql, parameters, trans)).ToList();

                if (!products.Any())
                    return Enumerable.Empty<Product>();

                var productIds = products.Select(pr => pr.InstanceId).ToList();
                var idList = string.Join(",", productIds);

                var allMetadata = (await conn.QueryAsync<dynamic>($@"
                    SELECT [InstanceId], [Key], [Value]
                    FROM [Instances].[ProductAttributes]
                    WHERE [InstanceId] IN ({idList});",
                    transaction: trans)).ToList();

                var allCategories = (await conn.QueryAsync<dynamic>($@"
                    SELECT [InstanceId], [CategoryInstanceId]
                    FROM [Instances].[ProductCategories]
                    WHERE [InstanceId] IN ({idList});",
                    transaction: trans)).ToList();

                var metadataLookup = allMetadata
                    .GroupBy(m => (int)m.InstanceId)
                    .ToDictionary(g => g.Key, g => g.Select(m => new ProductMetadata { Key = (string)m.Key, Value = (string)m.Value }).ToList());

                var categoryLookup = allCategories
                    .GroupBy(c => (int)c.InstanceId)
                    .ToDictionary(g => g.Key, g => g.Select(c => (int)c.CategoryInstanceId).ToList());

                foreach (var product in products)
                {
                    product.Metadata = metadataLookup.ContainsKey(product.InstanceId)
                        ? metadataLookup[product.InstanceId]
                        : new List<ProductMetadata>();
                    product.CategoryIds = categoryLookup.ContainsKey(product.InstanceId)
                        ? categoryLookup[product.InstanceId]
                        : new List<int>();
                }

                return products;
            });
        }
    }
}
