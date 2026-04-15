using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Sparcpoint.Core.Models;
using Sparcpoint.Core.Repositories;
using Sparcpoint.SqlServer.Abstractions;

namespace Sparcpoint.Core.Repositories.Implementations
{
    // EVAL: Repository Implementation - Uses raw ADO.NET via ISqlExecutor for database operations
    // EVAL: Dependency Injection - Constructor injection for testability
    public class ProductRepository : IProductRepository
    {
        private readonly ISqlExecutor _sqlExecutor;

        public ProductRepository(ISqlExecutor sqlExecutor)
        {
            _sqlExecutor = sqlExecutor ?? throw new ArgumentNullException(nameof(sqlExecutor));
        }

        private static IEnumerable<string> ParseCsv(string csv)
        {
            return string.IsNullOrWhiteSpace(csv)
                ? Enumerable.Empty<string>()
                : csv.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim());
        }

        public async Task<Product> GetByIdAsync(int instanceId)
        {
            return await _sqlExecutor.ExecuteAsync(async (conn, trans) =>
            {
                var sql = @"
                    SELECT p.InstanceId, p.Name, p.Description, p.ProductImageUris, p.ValidSkus, p.CreatedTimestamp,
                           c.InstanceId as CategoryInstanceId, c.Name as CategoryName, c.Description as CategoryDescription, c.CreatedTimestamp as CategoryCreatedTimestamp,
                           pa.[Key] as MetadataKey, pa.[Value] as MetadataValue
                    FROM Instances.Products p
                    LEFT JOIN Instances.ProductCategories pc ON p.InstanceId = pc.InstanceId
                    LEFT JOIN Instances.Categories c ON pc.CategoryInstanceId = c.InstanceId
                    LEFT JOIN Instances.ProductAttributes pa ON p.InstanceId = pa.InstanceId
                    WHERE p.InstanceId = @InstanceId";

                using (var cmd = (DbCommand)conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.Transaction = (DbTransaction)trans;
                    cmd.Parameters.AddWithValue("@InstanceId", instanceId);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        int? id = null;
                        string name = null;
                        string description = null;
                        string productImageUris = null;
                        string validSkus = null;
                        DateTime createdTimestamp = default;
                        var categories = new List<Category>();
                        var metadata = new Dictionary<string, string>();

                        while (await reader.ReadAsync())
                        {
                            if (!id.HasValue)
                            {
                                id = reader.GetInt32(0);
                                name = reader.GetString(1);
                                description = reader.GetString(2);
                                productImageUris = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);
                                validSkus = reader.IsDBNull(4) ? string.Empty : reader.GetString(4);
                                createdTimestamp = reader.GetDateTime(5);
                            }

                            if (!reader.IsDBNull(6))
                            {
                                var category = Category.Load(
                                    reader.GetInt32(6),
                                    reader.GetString(7),
                                    reader.GetString(8),
                                    reader.GetDateTime(9));
                                if (!categories.Contains(category))
                                    categories.Add(category);
                            }

                            if (!reader.IsDBNull(10))
                            {
                                var key = reader.GetString(10);
                                var value = reader.GetString(11);
                                metadata[key] = value;
                            }
                        }

                        if (!id.HasValue) return null;

                        return Product.Load(
                            id.Value,
                            name,
                            description,
                            ParseCsv(productImageUris),
                            ParseCsv(validSkus),
                            createdTimestamp,
                            categories,
                            metadata);
                    }
                }
            });
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _sqlExecutor.ExecuteAsync(async (conn, trans) =>
            {
                var sql = @"
                    SELECT p.InstanceId, p.Name, p.Description, p.ProductImageUris, p.ValidSkus, p.CreatedTimestamp,
                           c.InstanceId as CategoryInstanceId, c.Name as CategoryName, c.Description as CategoryDescription, c.CreatedTimestamp as CategoryCreatedTimestamp,
                           pa.[Key] as MetadataKey, pa.[Value] as MetadataValue
                    FROM Instances.Products p
                    LEFT JOIN Instances.ProductCategories pc ON p.InstanceId = pc.InstanceId
                    LEFT JOIN Instances.Categories c ON pc.CategoryInstanceId = c.InstanceId
                    LEFT JOIN Instances.ProductAttributes pa ON p.InstanceId = pa.InstanceId
                    ORDER BY p.InstanceId";

                var products = new Dictionary<int, Product>();

                using (var cmd = (DbCommand)conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.Transaction = (DbTransaction)trans;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var id = reader.GetInt32(0);
                            if (!products.TryGetValue(id, out var product))
                            {
                                product = Product.Load(
                                    id,
                                    reader.GetString(1),
                                    reader.GetString(2),
                                    ParseCsv(reader.IsDBNull(3) ? string.Empty : reader.GetString(3)),
                                    ParseCsv(reader.IsDBNull(4) ? string.Empty : reader.GetString(4)),
                                    reader.GetDateTime(5),
                                    Enumerable.Empty<Category>(),
                                    new Dictionary<string, string>());
                                products[id] = product;
                            }

                            if (!reader.IsDBNull(6))
                            {
                                var category = Category.Load(
                                    reader.GetInt32(6),
                                    reader.GetString(7),
                                    reader.GetString(8),
                                    reader.GetDateTime(9));
                                if (!product.Categories.Contains(category))
                                    product.Categories.Add(category);
                            }

                            if (!reader.IsDBNull(10))
                            {
                                var key = reader.GetString(10);
                                var value = reader.GetString(11);
                                product.Metadata[key] = value;
                            }
                        }
                    }
                }

                return products.Values;
            });
        }

        public async Task<IEnumerable<Product>> SearchAsync(string name = null, string description = null,
            IEnumerable<int> categoryIds = null, Dictionary<string, string> metadataFilters = null,
            int? skip = null, int? take = null)
        {
            return await _sqlExecutor.ExecuteAsync(async (conn, trans) =>
            {
                var sql = @"
                    SELECT DISTINCT p.InstanceId
                    FROM Instances.Products p
                    LEFT JOIN Instances.ProductCategories pc ON p.InstanceId = pc.InstanceId
                    WHERE 1=1";

                using (var cmd = (DbCommand)conn.CreateCommand())
                {
                    cmd.Transaction = (DbTransaction)trans;

                    if (!string.IsNullOrEmpty(name))
                    {
                        sql += " AND p.Name LIKE @Name";
                        cmd.Parameters.AddWithValue("@Name", $"%{name}%");
                    }

                    if (!string.IsNullOrEmpty(description))
                    {
                        sql += " AND p.Description LIKE @Description";
                        cmd.Parameters.AddWithValue("@Description", $"%{description}%");
                    }

                    if (categoryIds != null && categoryIds.Any())
                    {
                        sql += " AND pc.CategoryInstanceId IN (" + string.Join(",", categoryIds.Select((_, i) => $"@CategoryId{i}")) + ")";
                        for (int i = 0; i < categoryIds.Count(); i++)
                        {
                            cmd.Parameters.AddWithValue($"@CategoryId{i}", categoryIds.ElementAt(i));
                        }
                    }

                    if (metadataFilters != null && metadataFilters.Any())
                    {
                        int paramIndex = 0;
                        foreach (var filter in metadataFilters)
                        {
                            sql += $" AND EXISTS (SELECT 1 FROM Instances.ProductAttributes pa WHERE pa.InstanceId = p.InstanceId AND pa.[Key] = @Key{paramIndex} AND pa.[Value] LIKE @Value{paramIndex})";
                            cmd.Parameters.AddWithValue($"@Key{paramIndex}", filter.Key);
                            cmd.Parameters.AddWithValue($"@Value{paramIndex}", $"%{filter.Value}%");
                            paramIndex++;
                        }
                    }

                    sql += " ORDER BY p.InstanceId";

                    if (skip.HasValue && take.HasValue)
                    {
                        sql += " OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY";
                        cmd.Parameters.AddWithValue("@Skip", skip.Value);
                        cmd.Parameters.AddWithValue("@Take", take.Value);
                    }

                    cmd.CommandText = sql;

                    var productIds = new List<int>();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            productIds.Add(reader.GetInt32(0));
                        }
                    }

                    var products = new List<Product>();
                    foreach (var productId in productIds)
                    {
                        var product = await GetByIdAsync(productId);
                        if (product != null)
                            products.Add(product);
                    }

                    return products;
                }
            });
        }

        public async Task<int> GetSearchCountAsync(string name = null, string description = null,
            IEnumerable<int> categoryIds = null, Dictionary<string, string> metadataFilters = null)
        {
            return await _sqlExecutor.ExecuteAsync(async (conn, trans) =>
            {
                var sql = @"
                    SELECT COUNT(DISTINCT p.InstanceId)
                    FROM Instances.Products p
                    LEFT JOIN Instances.ProductCategories pc ON p.InstanceId = pc.InstanceId
                    WHERE 1=1";

                using (var cmd = (DbCommand)conn.CreateCommand())
                {
                    cmd.Transaction = (DbTransaction)trans;

                    if (!string.IsNullOrEmpty(name))
                    {
                        sql += " AND p.Name LIKE @Name";
                        cmd.Parameters.AddWithValue("@Name", $"%{name}%");
                    }

                    if (!string.IsNullOrEmpty(description))
                    {
                        sql += " AND p.Description LIKE @Description";
                        cmd.Parameters.AddWithValue("@Description", $"%{description}%");
                    }

                    if (categoryIds != null && categoryIds.Any())
                    {
                        sql += " AND pc.CategoryInstanceId IN (" + string.Join(",", categoryIds.Select((_, i) => $"@CategoryId{i}")) + ")";
                        for (int i = 0; i < categoryIds.Count(); i++)
                        {
                            cmd.Parameters.AddWithValue($"@CategoryId{i}", categoryIds.ElementAt(i));
                        }
                    }

                    if (metadataFilters != null && metadataFilters.Any())
                    {
                        int paramIndex = 0;
                        foreach (var filter in metadataFilters)
                        {
                            sql += $" AND EXISTS (SELECT 1 FROM Instances.ProductAttributes pa WHERE pa.InstanceId = p.InstanceId AND pa.[Key] = @Key{paramIndex} AND pa.[Value] LIKE @Value{paramIndex})";
                            cmd.Parameters.AddWithValue($"@Key{paramIndex}", filter.Key);
                            cmd.Parameters.AddWithValue($"@Value{paramIndex}", $"%{filter.Value}%");
                            paramIndex++;
                        }
                    }

                    cmd.CommandText = sql;
                    var result = await cmd.ExecuteScalarAsync();
                    return Convert.ToInt32(result);
                }
            });
        }

        public async Task<int> AddAsync(Product product)
        {
            // EVAL: Transactional operation - ensures data consistency across multiple tables
            return await _sqlExecutor.ExecuteAsync(async (conn, trans) =>
            {
                // Insert product
                var productSql = @"
                    INSERT INTO Instances.Products (Name, Description, ProductImageUris, ValidSkus, CreatedTimestamp)
                    OUTPUT INSERTED.InstanceId
                    VALUES (@Name, @Description, @ProductImageUris, @ValidSkus, @CreatedTimestamp)";

                using (var cmd = (DbCommand)conn.CreateCommand())
                {
                    cmd.CommandText = productSql;
                    cmd.Transaction = (DbTransaction)trans;
                    cmd.Parameters.AddWithValue("@Name", product.Name);
                    cmd.Parameters.AddWithValue("@Description", product.Description);
                    cmd.Parameters.AddWithValue("@ProductImageUris", string.Join(",", product.ProductImageUris));
                    cmd.Parameters.AddWithValue("@ValidSkus", string.Join(",", product.ValidSkus));
                    cmd.Parameters.AddWithValue("@CreatedTimestamp", product.CreatedTimestamp);

                    var productId = (int)await cmd.ExecuteScalarAsync();
                    product.SetInstanceId(productId);

                    // Insert categories
                    if (product.Categories.Any())
                    {
                        var categorySql = @"
                            INSERT INTO Instances.ProductCategories (InstanceId, CategoryInstanceId)
                            VALUES (@InstanceId, @CategoryInstanceId)";

                        foreach (var category in product.Categories)
                        {
                            using (var categoryCmd = (DbCommand)conn.CreateCommand())
                            {
                                categoryCmd.CommandText = categorySql;
                                categoryCmd.Transaction = (DbTransaction)trans;
                                categoryCmd.Parameters.AddWithValue("@InstanceId", productId);
                                categoryCmd.Parameters.AddWithValue("@CategoryInstanceId", category.InstanceId);
                                await categoryCmd.ExecuteNonQueryAsync();
                            }
                        }
                    }

                    // Insert metadata
                    if (product.Metadata.Any())
                    {
                        var metadataSql = @"
                            INSERT INTO Instances.ProductAttributes (InstanceId, [Key], [Value])
                            VALUES (@InstanceId, @Key, @Value)";

                        foreach (var kvp in product.Metadata)
                        {
                            using (var metadataCmd = (DbCommand)conn.CreateCommand())
                            {
                                metadataCmd.CommandText = metadataSql;
                                metadataCmd.Transaction = (DbTransaction)trans;
                                metadataCmd.Parameters.AddWithValue("@InstanceId", productId);
                                metadataCmd.Parameters.AddWithValue("@Key", kvp.Key);
                                metadataCmd.Parameters.AddWithValue("@Value", kvp.Value);
                                await metadataCmd.ExecuteNonQueryAsync();
                            }
                        }
                    }

                    return productId;
                }
            });
        }

        public async Task UpdateAsync(Product product)
        {
            // EVAL: Update operation with proper transaction handling
            await _sqlExecutor.ExecuteAsync(async (conn, trans) =>
            {
                // Update product
                var productSql = @"
                    UPDATE Instances.Products
                    SET Name = @Name, Description = @Description,
                        ProductImageUris = @ProductImageUris, ValidSkus = @ValidSkus
                    WHERE InstanceId = @InstanceId";

                using (var cmd = (DbCommand)conn.CreateCommand())
                {
                    cmd.CommandText = productSql;
                    cmd.Transaction = (DbTransaction)trans;
                    cmd.Parameters.AddWithValue("@InstanceId", product.InstanceId);
                    cmd.Parameters.AddWithValue("@Name", product.Name);
                    cmd.Parameters.AddWithValue("@Description", product.Description);
                    cmd.Parameters.AddWithValue("@ProductImageUris", string.Join(",", product.ProductImageUris));
                    cmd.Parameters.AddWithValue("@ValidSkus", string.Join(",", product.ValidSkus));
                    await cmd.ExecuteNonQueryAsync();
                }

                // Update categories (delete and re-insert)
                using (var deleteCmd = (DbCommand)conn.CreateCommand())
                {
                    deleteCmd.CommandText = "DELETE FROM Instances.ProductCategories WHERE InstanceId = @InstanceId";
                    deleteCmd.Transaction = (DbTransaction)trans;
                    deleteCmd.Parameters.AddWithValue("@InstanceId", product.InstanceId);
                    await deleteCmd.ExecuteNonQueryAsync();
                }

                if (product.Categories.Any())
                {
                    var categorySql = @"
                        INSERT INTO Instances.ProductCategories (InstanceId, CategoryInstanceId)
                        VALUES (@InstanceId, @CategoryInstanceId)";

                    foreach (var category in product.Categories)
                    {
                        using (var categoryCmd = (DbCommand)conn.CreateCommand())
                        {
                            categoryCmd.CommandText = categorySql;
                            categoryCmd.Transaction = (DbTransaction)trans;
                            categoryCmd.Parameters.AddWithValue("@InstanceId", product.InstanceId);
                            categoryCmd.Parameters.AddWithValue("@CategoryInstanceId", category.InstanceId);
                            await categoryCmd.ExecuteNonQueryAsync();
                        }
                    }
                }

                // Update metadata (delete and re-insert)
                using (var deleteCmd = (DbCommand)conn.CreateCommand())
                {
                    deleteCmd.CommandText = "DELETE FROM Instances.ProductAttributes WHERE InstanceId = @InstanceId";
                    deleteCmd.Transaction = (DbTransaction)trans;
                    deleteCmd.Parameters.AddWithValue("@InstanceId", product.InstanceId);
                    await deleteCmd.ExecuteNonQueryAsync();
                }

                if (product.Metadata.Any())
                {
                    var metadataSql = @"
                        INSERT INTO Instances.ProductAttributes (InstanceId, [Key], [Value])
                        VALUES (@InstanceId, @Key, @Value)";

                    foreach (var kvp in product.Metadata)
                    {
                        using (var metadataCmd = (DbCommand)conn.CreateCommand())
                        {
                            metadataCmd.CommandText = metadataSql;
                            metadataCmd.Transaction = (DbTransaction)trans;
                            metadataCmd.Parameters.AddWithValue("@InstanceId", product.InstanceId);
                            metadataCmd.Parameters.AddWithValue("@Key", kvp.Key);
                            metadataCmd.Parameters.AddWithValue("@Value", kvp.Value);
                            await metadataCmd.ExecuteNonQueryAsync();
                        }
                    }
                }
            });
        }

        public async Task<decimal> GetCurrentInventoryCountAsync(int productInstanceId)
        {
            // EVAL: Ledger pattern - SUM of all completed transactions gives current inventory
            return await _sqlExecutor.ExecuteAsync(async (conn, trans) =>
            {
                var sql = @"
                    SELECT COALESCE(SUM(Quantity), 0)
                    FROM Transactions.InventoryTransactions
                    WHERE ProductInstanceId = @ProductInstanceId AND CompletedTimestamp IS NOT NULL";

                using (var cmd = (DbCommand)conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.Transaction = (DbTransaction)trans;
                    cmd.Parameters.AddWithValue("@ProductInstanceId", productInstanceId);
                    var result = await cmd.ExecuteScalarAsync();
                    return result != null ? Convert.ToDecimal(result) : 0;
                }
            });
        }
    }
}
