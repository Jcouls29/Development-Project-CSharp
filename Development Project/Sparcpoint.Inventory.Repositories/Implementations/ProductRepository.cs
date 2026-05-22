using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;
using Sparcpoint.Inventory.Models.Domain;
using Sparcpoint.Inventory.Models.Search;
using Sparcpoint.Inventory.Repositories.Interfaces;
using Sparcpoint.SqlServer.Abstractions;

namespace Sparcpoint.Inventory.Repositories.Implementations
{
    /// <summary>
    /// Product repository implementation using Dapper and ISqlExecutor.
    /// EVAL: Combines Dapper's mapping capabilities with ISqlExecutor's transaction management.
    /// </summary>
    public class ProductRepository : IProductRepository
    {
        private readonly ISqlExecutor _sqlExecutor;
        private readonly ILogger<ProductRepository> _logger;

        public ProductRepository(ISqlExecutor sqlExecutor, ILogger<ProductRepository> logger)
        {
            _sqlExecutor = sqlExecutor ?? throw new ArgumentNullException(nameof(sqlExecutor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            // EVAL: Using table-valued parameters for efficient bulk insert of attributes and categories
            return await _sqlExecutor.ExecuteAsync(async (conn, trans) =>
            {
                // Insert main product record
                const string insertProductSql = @"
                    INSERT INTO [Instances].[Products] 
                    ([Name], [Description], [ProductImageUris], [ValidSkus])
                    VALUES 
                    (@Name, @Description, @ProductImageUris, @ValidSkus);
                    
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                var productId = await conn.QuerySingleAsync<int>(
                    insertProductSql,
                    new
                    {
                        product.Name,
                        product.Description,
                        ProductImageUris = string.Join("|", product.ProductImageUris ?? new List<string>()),
                        ValidSkus = string.Join("|", product.ValidSkus ?? new List<string>())
                    },
                    trans);

                product.InstanceId = productId;

                // Insert attributes using table-valued parameter
                if (product.Attributes != null && product.Attributes.Any())
                {
                    var attributeTable = CreateAttributeTable(product.Attributes);

                    const string insertAttributesSql = @"
                        INSERT INTO [Instances].[ProductAttributes] ([InstanceId], [Key], [Value])
                        SELECT @ProductId, [Key], [Value]
                        FROM @Attributes";

                    await conn.ExecuteAsync(
                        insertAttributesSql,
                        new { ProductId = productId, Attributes = attributeTable.AsTableValuedParameter("[dbo].[CustomAttributeList]") },
                        trans);
                }

                // Insert category associations
                if (product.CategoryIds != null && product.CategoryIds.Any())
                {
                    var categoryTable = CreateIntegerTable(product.CategoryIds);

                    const string insertCategoriesSql = @"
                        INSERT INTO [Instances].[ProductCategories] ([InstanceId], [CategoryInstanceId])
                        SELECT @ProductId, [Value]
                        FROM @Categories";

                    await conn.ExecuteAsync(
                        insertCategoriesSql,
                        new { ProductId = productId, Categories = categoryTable.AsTableValuedParameter("[dbo].[IntegerList]") },
                        trans);
                }

                _logger.LogInformation("Created product {ProductId} with {AttributeCount} attributes and {CategoryCount} categories",
                    productId, product.Attributes?.Count ?? 0, product.CategoryIds?.Count ?? 0);

                return product;
            });
        }

        public async Task<Product?> GetProductByIdAsync(int instanceId)
        {
            return await _sqlExecutor.ExecuteAsync(async (conn, trans) =>
            {
                // Get product with attributes and categories in parallel queries
                const string productSql = "SELECT * FROM [Instances].[Products] WHERE [InstanceId] = @InstanceId";
                const string attributesSql = "SELECT [Key], [Value] FROM [Instances].[ProductAttributes] WHERE [InstanceId] = @InstanceId";
                const string categoriesSql = "SELECT [CategoryInstanceId] FROM [Instances].[ProductCategories] WHERE [InstanceId] = @InstanceId";

                var product = await conn.QuerySingleOrDefaultAsync<ProductDto>(productSql, new { InstanceId = instanceId }, trans);
                if (product == null) return null;

                var attributes = await conn.QueryAsync<AttributeDto>(attributesSql, new { InstanceId = instanceId }, trans);
                var categories = await conn.QueryAsync<int>(categoriesSql, new { InstanceId = instanceId }, trans);

                return MapToProduct(product, attributes, categories);
            });
        }

        public async Task<List<Product>> SearchProductsAsync(ProductSearchCriteria criteria)
        {
            // EVAL: Dynamic SQL generation based on provided criteria (all optional, combined with AND)
            return await _sqlExecutor.ExecuteAsync(async (conn, trans) =>
            {
                var sqlBuilder = new System.Text.StringBuilder();
                var parameters = new DynamicParameters();

                sqlBuilder.AppendLine(@"
                    SELECT DISTINCT p.*
                    FROM [Instances].[Products] p");

                var whereClauses = new List<string>();

                // Filter by name
                if (!string.IsNullOrWhiteSpace(criteria.Name))
                {
                    whereClauses.Add("p.[Name] LIKE @Name");
                    parameters.Add("Name", $"%{criteria.Name}%");
                }

                // Filter by description
                if (!string.IsNullOrWhiteSpace(criteria.Description))
                {
                    whereClauses.Add("p.[Description] LIKE @Description");
                    parameters.Add("Description", $"%{criteria.Description}%");
                }

                // Filter by SKU
                if (!string.IsNullOrWhiteSpace(criteria.Sku))
                {
                    whereClauses.Add("p.[ValidSkus] LIKE @Sku");
                    parameters.Add("Sku", $"%{criteria.Sku}%");
                }

                // Filter by attributes
                if (criteria.Attributes != null && criteria.Attributes.Any())
                {
                    var attrIndex = 0;
                    foreach (var attr in criteria.Attributes)
                    {
                        sqlBuilder.AppendLine($@"
                            INNER JOIN [Instances].[ProductAttributes] pa{attrIndex}
                                ON p.[InstanceId] = pa{attrIndex}.[InstanceId]
                                AND pa{attrIndex}.[Key] = @AttrKey{attrIndex}
                                AND pa{attrIndex}.[Value] = @AttrValue{attrIndex}");

                        parameters.Add($"AttrKey{attrIndex}", attr.Key);
                        parameters.Add($"AttrValue{attrIndex}", attr.Value);
                        attrIndex++;
                    }
                }

                // Filter by categories
                if (criteria.CategoryIds != null && criteria.CategoryIds.Any())
                {
                    var catIndex = 0;
                    foreach (var categoryId in criteria.CategoryIds)
                    {
                        sqlBuilder.AppendLine($@"
                            INNER JOIN [Instances].[ProductCategories] pc{catIndex}
                                ON p.[InstanceId] = pc{catIndex}.[InstanceId]
                                AND pc{catIndex}.[CategoryInstanceId] = @CategoryId{catIndex}");

                        parameters.Add($"CategoryId{catIndex}", categoryId);
                        catIndex++;
                    }
                }

                if (whereClauses.Any())
                {
                    sqlBuilder.AppendLine("WHERE " + string.Join(" AND ", whereClauses));
                }

                sqlBuilder.AppendLine("ORDER BY p.[CreatedTimestamp] DESC");

                if (criteria.Limit.HasValue)
                {
                    sqlBuilder.AppendLine($"OFFSET {criteria.Offset ?? 0} ROWS FETCH NEXT @Limit ROWS ONLY");
                    parameters.Add("Limit", criteria.Limit.Value);
                }

                var sql = sqlBuilder.ToString();
                _logger.LogDebug("Executing product search: {Sql}", sql);

                var productDtos = await conn.QueryAsync<ProductDto>(sql, parameters, trans);

                // Load attributes and categories for each product
                var products = new List<Product>();
                foreach (var dto in productDtos)
                {
                    var attributes = await conn.QueryAsync<AttributeDto>(
                        "SELECT [Key], [Value] FROM [Instances].[ProductAttributes] WHERE [InstanceId] = @InstanceId",
                        new { dto.InstanceId },
                        trans);

                    var categories = await conn.QueryAsync<int>(
                        "SELECT [CategoryInstanceId] FROM [Instances].[ProductCategories] WHERE [InstanceId] = @InstanceId",
                        new { dto.InstanceId },
                        trans);

                    products.Add(MapToProduct(dto, attributes, categories));
                }

                return products;
            });
        }

        public async Task<bool> UpdateProductAsync(Product product)
        {
            return await _sqlExecutor.ExecuteAsync(async (conn, trans) =>
            {
                const string sql = @"
                    UPDATE [Instances].[Products]
                    SET [Name] = @Name,
                        [Description] = @Description,
                        [ProductImageUris] = @ProductImageUris,
                        [ValidSkus] = @ValidSkus
                    WHERE [InstanceId] = @InstanceId";

                var rowsAffected = await conn.ExecuteAsync(
                    sql,
                    new
                    {
                        product.InstanceId,
                        product.Name,
                        product.Description,
                        ProductImageUris = string.Join("|", product.ProductImageUris ?? new List<string>()),
                        ValidSkus = string.Join("|", product.ValidSkus ?? new List<string>())
                    },
                    trans);

                return rowsAffected > 0;
            });
        }

        #region Helper Methods

        // EVAL: Helper methods to create table-valued parameters for bulk operations
        private DataTable CreateAttributeTable(Dictionary<string, string> attributes)
        {
            var table = new DataTable();
            table.Columns.Add("Key", typeof(string));
            table.Columns.Add("Value", typeof(string));

            foreach (var attr in attributes)
            {
                table.Rows.Add(attr.Key, attr.Value);
            }

            return table;
        }

        private DataTable CreateIntegerTable(List<int> values)
        {
            var table = new DataTable();
            table.Columns.Add("Value", typeof(int));

            foreach (var value in values)
            {
                table.Rows.Add(value);
            }

            return table;
        }

        private Product MapToProduct(ProductDto dto, IEnumerable<AttributeDto> attributes, IEnumerable<int> categoryIds)
        {
            return new Product
            {
                InstanceId = dto.InstanceId,
                Name = dto.Name,
                Description = dto.Description,
                ProductImageUris = string.IsNullOrWhiteSpace(dto.ProductImageUris)
                    ? new List<string>()
                    : dto.ProductImageUris.Split('|', StringSplitOptions.RemoveEmptyEntries).ToList(),
                ValidSkus = string.IsNullOrWhiteSpace(dto.ValidSkus)
                    ? new List<string>()
                    : dto.ValidSkus.Split('|', StringSplitOptions.RemoveEmptyEntries).ToList(),
                Attributes = attributes?.ToDictionary(a => a.Key, a => a.Value) ?? new Dictionary<string, string>(),
                CategoryIds = categoryIds?.ToList() ?? new List<int>(),
                CreatedTimestamp = dto.CreatedTimestamp
            };
        }

        #endregion

        #region DTOs for Dapper Mapping

        private class ProductDto
        {
            public int InstanceId { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string ProductImageUris { get; set; } = string.Empty;
            public string ValidSkus { get; set; } = string.Empty;
            public DateTime CreatedTimestamp { get; set; }
        }

        private class AttributeDto
        {
            public string Key { get; set; } = string.Empty;
            public string Value { get; set; } = string.Empty;
        }

        #endregion
    }
}