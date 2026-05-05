// EVAL: This is the core data access implementation for products.
// It uses ISqlExecutor (provided by the framework) for connection/transaction management
// and Dapper for lightweight ORM mapping. No Entity Framework -- Dapper gives full SQL
// control with minimal overhead, which is important for an inventory system where
// query performance matters.

using Dapper;
using Newtonsoft.Json;
using Sparcpoint.Inventory.Abstract;
using Sparcpoint.Inventory.Models;
using Sparcpoint.SqlServer.Abstractions;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory.SqlServer
{
    /// <summary>
    /// SQL Server implementation of <see cref="IProductRepository"/>.
    /// Uses ISqlExecutor for connection management and Dapper for mapping.
    /// </summary>
    public class SqlServerProductRepository : IProductRepository
    {
        private readonly ISqlExecutor _executor;

        public SqlServerProductRepository(ISqlExecutor executor)
        {
            // EVAL: Constructor injection of ISqlExecutor follows the DI pattern
            // recommended by the project framework.
            PreConditions.ParameterNotNull(executor, nameof(executor));
            _executor = executor;
        }

        /// <inheritdoc />
        public async Task<Product> CreateAsync(Product product, CancellationToken cancellationToken = default)
        {
            PreConditions.ParameterNotNull(product, nameof(product));
            PreConditions.StringNotNullOrWhitespace(product.Name, nameof(product.Name));
            PreConditions.StringNotNullOrWhitespace(product.Description, nameof(product.Description));

            return await _executor.ExecuteAsync<Product>(async (conn, trans) =>
            {
                // EVAL: All three inserts (Product, Attributes, Categories) happen within
                // the same transaction provided by ISqlExecutor. If any step fails,
                // the entire operation is rolled back automatically.

                // 1. Insert the product and retrieve the auto-generated InstanceId
                const string insertProductSql = @"
                    INSERT INTO [Instances].[Products] ([Name], [Description], [ProductImageUris], [ValidSkus])
                    VALUES (@Name, @Description, @ProductImageUris, @ValidSkus);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                // EVAL: Raw SQL is preferred over Dapper.Contrib's Insert here because:
                // 1. Table uses schema prefix [Instances].[Products] which requires [Table] attribute config
                // 2. We need SCOPE_IDENTITY() in the same command for the generated ID
                // 3. Raw SQL gives full visibility into what's executed -- important for a data access layer
                var serializedImageUris = JsonConvert.SerializeObject(product.ProductImageUris ?? new List<string>());
                var serializedSkus = JsonConvert.SerializeObject(product.ValidSkus ?? new List<string>());

                var newId = await conn.QuerySingleAsync<int>(
                    insertProductSql,
                    new
                    {
                        product.Name,
                        product.Description,
                        ProductImageUris = serializedImageUris,
                        ValidSkus = serializedSkus
                    },
                    trans);

                product.InstanceId = newId;

                // 2. Insert product attributes (arbitrary metadata)
                if (product.Attributes != null && product.Attributes.Count > 0)
                {
                    const string insertAttributeSql = @"
                        INSERT INTO [Instances].[ProductAttributes] ([InstanceId], [Key], [Value])
                        VALUES (@InstanceId, @Key, @Value);";

                    // RV: Inserting attributes one-by-one within a transaction.
                    // For very large attribute sets, consider using a table-valued parameter
                    // with the existing CustomAttributeList type for batch insert.
                    foreach (var attr in product.Attributes)
                    {
                        await conn.ExecuteAsync(
                            insertAttributeSql,
                            new { InstanceId = newId, Key = attr.Key, Value = attr.Value },
                            trans);
                    }
                }

                // 3. Insert product-category associations
                if (product.CategoryIds != null && product.CategoryIds.Count > 0)
                {
                    const string insertCategorySql = @"
                        INSERT INTO [Instances].[ProductCategories] ([InstanceId], [CategoryInstanceId])
                        VALUES (@InstanceId, @CategoryInstanceId);";

                    foreach (var categoryId in product.CategoryIds)
                    {
                        await conn.ExecuteAsync(
                            insertCategorySql,
                            new { InstanceId = newId, CategoryInstanceId = categoryId },
                            trans);
                    }
                }

                // 4. Read back DB-generated CreatedTimestamp and resolve Category names
                // so the response contains a fully hydrated Product.
                const string timestampSql = @"
                    SELECT [CreatedTimestamp]
                    FROM [Instances].[Products]
                    WHERE [InstanceId] = @InstanceId;";

                product.CreatedTimestamp = await conn.QuerySingleAsync<System.DateTime>(
                    timestampSql,
                    new { InstanceId = newId },
                    trans);

                await PopulateCategoryIdsAsync(conn, trans, product);

                return product;
            });
        }

        /// <inheritdoc />
        public async Task<Product> GetByIdAsync(int instanceId, CancellationToken cancellationToken = default)
        {
            return await _executor.ExecuteAsync<Product>(async (conn, trans) =>
            {
                // 1. Fetch the product
                const string productSql = @"
                    SELECT [InstanceId], [Name], [Description], [ProductImageUris], [ValidSkus], [CreatedTimestamp]
                    FROM [Instances].[Products]
                    WHERE [InstanceId] = @InstanceId;";

                var row = await conn.QuerySingleOrDefaultAsync<ProductRow>(
                    productSql,
                    new { InstanceId = instanceId },
                    trans);

                if (row == null)
                    return null;

                var product = MapRowToProduct(row);

                // 2. Fetch attributes
                await PopulateAttributesAsync(conn, trans, product);

                // 3. Fetch category associations
                await PopulateCategoryIdsAsync(conn, trans, product);

                return product;
            });
        }

        /// <inheritdoc />
        public async Task<IDictionary<int, Product>> GetByIdsAsync(IEnumerable<int> instanceIds, CancellationToken cancellationToken = default)
        {
            var idList = instanceIds?.Distinct().ToList() ?? new List<int>();
            if (idList.Count == 0)
                return new Dictionary<int, Product>();

            return await _executor.ExecuteAsync<IDictionary<int, Product>>(async (conn, trans) =>
            {
                const string sql = @"
                    SELECT [InstanceId], [Name], [Description], [ProductImageUris], [ValidSkus], [CreatedTimestamp]
                    FROM [Instances].[Products]
                    WHERE [InstanceId] IN @Ids;";

                var rows = await conn.QueryAsync<ProductRow>(sql, new { Ids = idList }, trans);
                var products = rows.Select(MapRowToProduct).ToList();

                if (products.Count > 0)
                {
                    var foundIds = products.Select(p => p.InstanceId).ToList();
                    await PopulateAttributesBulkAsync(conn, trans, products, foundIds);
                    await PopulateCategoryIdsBulkAsync(conn, trans, products, foundIds);
                }

                return products.ToDictionary(p => p.InstanceId);
            });
        }

        /// <inheritdoc />
        public async Task<HashSet<int>> GetExistingCategoryIdsAsync(IEnumerable<int> categoryIds, CancellationToken cancellationToken = default)
        {
            var idList = categoryIds?.Distinct().ToList() ?? new List<int>();
            if (idList.Count == 0)
                return new HashSet<int>();

            return await _executor.ExecuteAsync<HashSet<int>>(async (conn, trans) =>
            {
                const string sql = @"
                    SELECT [InstanceId]
                    FROM [Instances].[Categories]
                    WHERE [InstanceId] IN @Ids;";

                var rows = await conn.QueryAsync<int>(sql, new { Ids = idList }, trans);
                return new HashSet<int>(rows);
            });
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Product>> SearchAsync(ProductSearchCriteria criteria, CancellationToken cancellationToken = default)
        {
            criteria = criteria ?? new ProductSearchCriteria();

            return await _executor.ExecuteAsync<IEnumerable<Product>>(async (conn, trans) =>
            {
                // EVAL: Dynamic query building using StringBuilder and parameterized queries.
                // The SqlServerQueryProvider from the framework could also be used here,
                // but for complex JOINs with conditional logic, explicit SQL is more readable
                // and easier to debug.
                var sql = new StringBuilder();
                var parameters = new DynamicParameters();

                sql.Append(@"
                    SELECT DISTINCT p.[InstanceId], p.[Name], p.[Description],
                           p.[ProductImageUris], p.[ValidSkus], p.[CreatedTimestamp]
                    FROM [Instances].[Products] p");

                // RV: JOIN to ProductCategories only when filtering by category.
                // This avoids unnecessary joins when categories aren't in the search criteria.
                if (criteria.CategoryIds != null && criteria.CategoryIds.Count > 0)
                {
                    sql.Append(@"
                    INNER JOIN [Instances].[ProductCategories] pc ON p.[InstanceId] = pc.[InstanceId]");
                }

                // EVAL: For attribute filtering, each key-value pair requires a separate JOIN
                // to ensure AND logic (product must have ALL specified attributes).
                // This is a well-known pattern for EAV (Entity-Attribute-Value) queries.
                if (criteria.Attributes != null && criteria.Attributes.Count > 0)
                {
                    int attrIndex = 0;
                    foreach (var attr in criteria.Attributes)
                    {
                        var alias = $"pa{attrIndex}";
                        sql.Append($@"
                    INNER JOIN [Instances].[ProductAttributes] {alias}
                        ON p.[InstanceId] = {alias}.[InstanceId]
                        AND {alias}.[Key] = @AttrKey{attrIndex}
                        AND {alias}.[Value] = @AttrValue{attrIndex}");

                        parameters.Add($"AttrKey{attrIndex}", attr.Key);
                        parameters.Add($"AttrValue{attrIndex}", attr.Value);
                        attrIndex++;
                    }
                }

                // Build WHERE clause
                var whereAdded = false;

                if (!string.IsNullOrWhiteSpace(criteria.Name))
                {
                    sql.Append(whereAdded ? " AND" : " WHERE");
                    sql.Append(" p.[Name] LIKE @Name");
                    parameters.Add("Name", $"%{criteria.Name}%");
                    whereAdded = true;
                }

                if (!string.IsNullOrWhiteSpace(criteria.Description))
                {
                    sql.Append(whereAdded ? " AND" : " WHERE");
                    sql.Append(" p.[Description] LIKE @Description");
                    parameters.Add("Description", $"%{criteria.Description}%");
                    whereAdded = true;
                }

                if (criteria.CategoryIds != null && criteria.CategoryIds.Count > 0)
                {
                    sql.Append(whereAdded ? " AND" : " WHERE");
                    sql.Append(" pc.[CategoryInstanceId] IN @CategoryIds");
                    parameters.Add("CategoryIds", criteria.CategoryIds);
                    whereAdded = true;
                }

                // Ordering and pagination
                sql.Append(@"
                    ORDER BY p.[Name] ASC
                    OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;");

                parameters.Add("Skip", criteria.Skip);
                parameters.Add("Take", criteria.Take);

                // Execute the search query
                var rows = await conn.QueryAsync<ProductRow>(sql.ToString(), parameters, trans);

                var products = rows.Select(MapRowToProduct).ToList();

                // EVAL: Bulk-fetch attributes and categories for all products in two queries
                // instead of 2N+1 queries (N+1 pattern). For a page of 50 products, this runs
                // 3 total queries instead of 101. Scales well regardless of page size.
                if (products.Count > 0)
                {
                    var productIds = products.Select(p => p.InstanceId).ToList();
                    await PopulateAttributesBulkAsync(conn, trans, products, productIds);
                    await PopulateCategoryIdsBulkAsync(conn, trans, products, productIds);
                }

                return products;
            });
        }

        #region Private Helpers

        /// <summary>
        /// Populates the Attributes dictionary on a product from the ProductAttributes table.
        /// </summary>
        private async Task PopulateAttributesAsync(IDbConnection conn, IDbTransaction trans, Product product)
        {
            const string attributesSql = @"
                SELECT [Key], [Value]
                FROM [Instances].[ProductAttributes]
                WHERE [InstanceId] = @InstanceId;";

            var attributes = await conn.QueryAsync<KeyValueRow>(
                attributesSql,
                new { product.InstanceId },
                trans);

            product.Attributes = attributes.ToDictionary(a => a.Key, a => a.Value);
        }

        /// <summary>
        /// Populates the CategoryIds list on a product from the ProductCategories table.
        /// </summary>
        private async Task PopulateCategoryIdsAsync(IDbConnection conn, IDbTransaction trans, Product product)
        {
            const string categoriesSql = @"
                SELECT pc.[CategoryInstanceId], c.[Name]
                FROM [Instances].[ProductCategories] pc
                INNER JOIN [Instances].[Categories] c ON pc.[CategoryInstanceId] = c.[InstanceId]
                WHERE pc.[InstanceId] = @InstanceId;";

            var rows = await conn.QueryAsync<BulkCategoryRow>(
                categoriesSql,
                new { product.InstanceId },
                trans);

            product.CategoryIds = rows.Select(r => r.CategoryInstanceId).ToList();
            product.Categories = rows.ToDictionary(r => r.CategoryInstanceId, r => r.Name);
        }

        /// <summary>
        /// Bulk-populates Attributes for a list of products in a single query.
        /// </summary>
        private async Task PopulateAttributesBulkAsync(IDbConnection conn, IDbTransaction trans, List<Product> products, List<int> productIds)
        {
            const string sql = @"
                SELECT [InstanceId], [Key], [Value]
                FROM [Instances].[ProductAttributes]
                WHERE [InstanceId] IN @ProductIds;";

            var rows = await conn.QueryAsync<BulkKeyValueRow>(sql, new { ProductIds = productIds }, trans);

            var grouped = rows.GroupBy(r => r.InstanceId)
                .ToDictionary(g => g.Key, g => g.ToDictionary(r => r.Key, r => r.Value));

            foreach (var product in products)
            {
                product.Attributes = grouped.ContainsKey(product.InstanceId)
                    ? grouped[product.InstanceId]
                    : new Dictionary<string, string>();
            }
        }

        /// <summary>
        /// Bulk-populates CategoryIds for a list of products in a single query.
        /// </summary>
        private async Task PopulateCategoryIdsBulkAsync(IDbConnection conn, IDbTransaction trans, List<Product> products, List<int> productIds)
        {
            const string sql = @"
                SELECT pc.[InstanceId], pc.[CategoryInstanceId], c.[Name]
                FROM [Instances].[ProductCategories] pc
                INNER JOIN [Instances].[Categories] c ON pc.[CategoryInstanceId] = c.[InstanceId]
                WHERE pc.[InstanceId] IN @ProductIds;";

            var rows = await conn.QueryAsync<BulkCategoryRow>(sql, new { ProductIds = productIds }, trans);

            var grouped = rows.GroupBy(r => r.InstanceId);

            foreach (var product in products)
            {
                var productRows = grouped.FirstOrDefault(g => g.Key == product.InstanceId);
                if (productRows != null)
                {
                    product.CategoryIds = productRows.Select(r => r.CategoryInstanceId).ToList();
                    product.Categories = productRows.ToDictionary(r => r.CategoryInstanceId, r => r.Name);
                }
                else
                {
                    product.CategoryIds = new List<int>();
                    product.Categories = new Dictionary<int, string>();
                }
            }
        }

        /// <summary>
        /// Maps a raw database row to a Product domain model.
        /// </summary>
        private Product MapRowToProduct(ProductRow row)
        {
            return new Product
            {
                InstanceId = row.InstanceId,
                Name = row.Name,
                Description = row.Description,
                // EVAL: Deserialize JSON strings from DB into native lists at the repository layer.
                // This keeps the storage format (VARCHAR(MAX) JSON) as an infrastructure concern.
                ProductImageUris = SafeDeserializeList(row.ProductImageUris),
                ValidSkus = SafeDeserializeList(row.ValidSkus),
                CreatedTimestamp = row.CreatedTimestamp
            };
        }

        /// <summary>
        /// Safely deserializes a JSON string to a list of strings.
        /// Returns empty list on null or invalid JSON.
        /// </summary>
        private List<string> SafeDeserializeList(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return new List<string>();

            try
            {
                return JsonConvert.DeserializeObject<List<string>>(json) ?? new List<string>();
            }
            catch (JsonException)
            {
                return new List<string>();
            }
        }

        #endregion

        #region Internal Row Types

        // EVAL: Internal row types map directly to database columns for Dapper.
        // Domain models may have a different shape (e.g., deserialized lists),
        // so we map through these intermediaries.

        private class ProductRow
        {
            public int InstanceId { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string ProductImageUris { get; set; }
            public string ValidSkus { get; set; }
            public System.DateTime CreatedTimestamp { get; set; }
        }

        private class KeyValueRow
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }

        private class BulkKeyValueRow
        {
            public int InstanceId { get; set; }
            public string Key { get; set; }
            public string Value { get; set; }
        }

        private class BulkCategoryRow
        {
            public int InstanceId { get; set; }
            public int CategoryInstanceId { get; set; }
            public string Name { get; set; }
        }

        #endregion
    }
}
