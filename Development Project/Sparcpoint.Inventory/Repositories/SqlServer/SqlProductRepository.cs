using Dapper;
using Sparcpoint.Inventory.Models;
using Sparcpoint.SqlServer.Abstractions;
using System.Data;
using System.Text;

namespace Sparcpoint.Inventory.Repositories.SqlServer
{
    /// <summary>
    /// EVAL: SQL Server implementation of IProductRepository using ISqlExecutor and Dapper.
    /// All operations run within a transaction managed by ISqlExecutor for data consistency.
    /// Uses parameterized queries throughout to prevent SQL injection.
    /// </summary>
    public class SqlProductRepository : IProductRepository
    {
        private readonly ISqlExecutor _SqlExecutor;

        public SqlProductRepository(ISqlExecutor sqlExecutor)
        {
            _SqlExecutor = sqlExecutor ?? throw new ArgumentNullException(nameof(sqlExecutor));
        }

        public async Task<Product> CreateAsync(Product product)
        {
            return await _SqlExecutor.ExecuteAsync<Product>(async (conn, trans) =>
            {
                // EVAL: Insert product and retrieve generated identity in a single transaction
                const string insertProductSql = @"
                    INSERT INTO [Instances].[Products] ([Name], [Description], [ProductImageUris], [ValidSkus], [CreatedTimestamp])
                    VALUES (@Name, @Description, @ProductImageUris, @ValidSkus, @CreatedTimestamp);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                product.CreatedTimestamp = DateTime.UtcNow;

                var instanceId = await conn.QuerySingleAsync<int>(insertProductSql, new
                {
                    product.Name,
                    product.Description,
                    product.ProductImageUris,
                    product.ValidSkus,
                    product.CreatedTimestamp
                }, trans);

                product.InstanceId = instanceId;

                // EVAL: Bulk insert attributes as key-value pairs for flexible metadata
                if (product.Attributes?.Count > 0)
                {
                    const string insertAttrSql = @"
                        INSERT INTO [Instances].[ProductAttributes] ([InstanceId], [Key], [Value])
                        VALUES (@InstanceId, @Key, @Value);";

                    foreach (var attr in product.Attributes)
                    {
                        await conn.ExecuteAsync(insertAttrSql, new
                        {
                            InstanceId = instanceId,
                            Key = attr.Key,
                            Value = attr.Value
                        }, trans);
                    }
                }

                // EVAL: Link product to categories via junction table
                if (product.CategoryIds?.Count > 0)
                {
                    const string insertCatSql = @"
                        INSERT INTO [Instances].[ProductCategories] ([InstanceId], [CategoryInstanceId])
                        VALUES (@InstanceId, @CategoryInstanceId);";

                    foreach (var categoryId in product.CategoryIds)
                    {
                        await conn.ExecuteAsync(insertCatSql, new
                        {
                            InstanceId = instanceId,
                            CategoryInstanceId = categoryId
                        }, trans);
                    }
                }

                return product;
            });
        }

        public async Task<Product?> GetByIdAsync(int instanceId)
        {
            return await _SqlExecutor.ExecuteAsync<Product?>(async (conn, trans) =>
            {
                const string productSql = @"
                    SELECT [InstanceId], [Name], [Description], [ProductImageUris], [ValidSkus], [CreatedTimestamp]
                    FROM [Instances].[Products]
                    WHERE [InstanceId] = @InstanceId;";

                var product = await conn.QuerySingleOrDefaultAsync<Product>(productSql, new { InstanceId = instanceId }, trans);
                if (product == null) return null;

                // Load attributes
                const string attrSql = @"
                    SELECT [Key], [Value]
                    FROM [Instances].[ProductAttributes]
                    WHERE [InstanceId] = @InstanceId;";

                var attrs = await conn.QueryAsync(attrSql, new { InstanceId = instanceId }, trans);
                product.Attributes = new Dictionary<string, string>();
                foreach (var attr in attrs)
                {
                    product.Attributes[attr.Key] = attr.Value;
                }

                // Load category associations
                const string catSql = @"
                    SELECT [CategoryInstanceId]
                    FROM [Instances].[ProductCategories]
                    WHERE [InstanceId] = @InstanceId;";

                var catIds = await conn.QueryAsync<int>(catSql, new { InstanceId = instanceId }, trans);
                product.CategoryIds = catIds.ToList();

                return product;
            });
        }

        public async Task<IEnumerable<Product>> SearchAsync(ProductSearchCriteria criteria)
        {
            return await _SqlExecutor.ExecuteAsync<IEnumerable<Product>>(async (conn, trans) =>
            {
                // EVAL: Dynamic query building using SqlServerQueryProvider for safe, parameterized queries.
                // Each filter criterion is optional — only active filters are applied (AND combined).
                var queryBuilder = new SqlServerQueryProvider();
                queryBuilder.SetTargetTableAlias("p");

                var parameters = new DynamicParameters();

                // EVAL: Using SqlServerQueryProvider for WHERE clause generation and
                // Dapper DynamicParameters for parameterized query execution.
                if (!string.IsNullOrWhiteSpace(criteria.NameContains))
                {
                    var paramName = queryBuilder.GetNextParameterName("@Name");
                    queryBuilder.Where($"p.[Name] LIKE {paramName}");
                    parameters.Add(paramName, $"%{criteria.NameContains}%");
                }

                if (!string.IsNullOrWhiteSpace(criteria.DescriptionContains))
                {
                    var paramName = queryBuilder.GetNextParameterName("@Desc");
                    queryBuilder.Where($"p.[Description] LIKE {paramName}");
                    parameters.Add(paramName, $"%{criteria.DescriptionContains}%");
                }

                if (!string.IsNullOrWhiteSpace(criteria.SkuContains))
                {
                    var paramName = queryBuilder.GetNextParameterName("@Sku");
                    queryBuilder.Where($"p.[ValidSkus] LIKE {paramName}");
                    parameters.Add(paramName, $"%{criteria.SkuContains}%");
                }

                // EVAL: Category filtering uses EXISTS subquery to avoid duplicate rows from JOINs
                if (criteria.CategoryIds?.Count > 0)
                {
                    var paramName = queryBuilder.GetNextParameterName("@CatIds");
                    queryBuilder.Where($"EXISTS (SELECT 1 FROM [Instances].[ProductCategories] pc WHERE pc.[InstanceId] = p.[InstanceId] AND pc.[CategoryInstanceId] IN {paramName})");
                    parameters.Add(paramName, criteria.CategoryIds);
                }

                // EVAL: Attribute filtering supports multiple key-value pairs.
                // Each attribute pair adds a separate EXISTS subquery so all must match (AND logic).
                if (criteria.Attributes?.Count > 0)
                {
                    int attrIndex = 0;
                    foreach (var attr in criteria.Attributes)
                    {
                        var keyParam = $"@AttrKey{attrIndex}";
                        var valParam = $"@AttrVal{attrIndex}";
                        queryBuilder.Where($"EXISTS (SELECT 1 FROM [Instances].[ProductAttributes] pa{attrIndex} WHERE pa{attrIndex}.[InstanceId] = p.[InstanceId] AND pa{attrIndex}.[Key] = {keyParam} AND pa{attrIndex}.[Value] = {valParam})");
                        parameters.Add(keyParam, attr.Key);
                        parameters.Add(valParam, attr.Value);
                        attrIndex++;
                    }
                }

                var whereClause = queryBuilder.WhereClause;
                var sql = new StringBuilder();
                sql.Append(@"SELECT p.[InstanceId], p.[Name], p.[Description], p.[ProductImageUris], p.[ValidSkus], p.[CreatedTimestamp]
                    FROM [Instances].[Products] p");

                if (!string.IsNullOrWhiteSpace(whereClause))
                {
                    sql.Append($" WHERE {whereClause}");
                }

                sql.Append(" ORDER BY p.[CreatedTimestamp] DESC");

                var products = (await conn.QueryAsync<Product>(sql.ToString(), parameters, trans)).ToList();

                // Load attributes and categories for each product
                if (products.Count > 0)
                {
                    var productIds = products.Select(p => p.InstanceId).ToList();

                    const string attrSql = @"
                        SELECT [InstanceId], [Key], [Value]
                        FROM [Instances].[ProductAttributes]
                        WHERE [InstanceId] IN @ProductIds;";

                    var allAttrs = await conn.QueryAsync(attrSql, new { ProductIds = productIds }, trans);
                    var attrLookup = allAttrs.GroupBy(a => (int)a.InstanceId)
                        .ToDictionary(g => g.Key, g => g.ToDictionary(a => (string)a.Key, a => (string)a.Value));

                    const string catSql = @"
                        SELECT [InstanceId], [CategoryInstanceId]
                        FROM [Instances].[ProductCategories]
                        WHERE [InstanceId] IN @ProductIds;";

                    var allCats = await conn.QueryAsync(catSql, new { ProductIds = productIds }, trans);
                    var catLookup = allCats.GroupBy(c => (int)c.InstanceId)
                        .ToDictionary(g => g.Key, g => g.Select(c => (int)c.CategoryInstanceId).ToList());

                    foreach (var product in products)
                    {
                        product.Attributes = attrLookup.TryGetValue(product.InstanceId, out var attrs) ? attrs : new Dictionary<string, string>();
                        product.CategoryIds = catLookup.TryGetValue(product.InstanceId, out var cats) ? cats : new List<int>();
                    }
                }

                return products;
            });
        }
    }
}
