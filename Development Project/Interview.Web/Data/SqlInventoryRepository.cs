using Interview.Web.Contracts;
using Interview.Web.Infrastructure;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Interview.Web.Data
{
    public class SqlInventoryRepository : IInventoryRepository
    {
        private readonly ISqlExecutor _sqlExecutor;

        public SqlInventoryRepository(ISqlExecutor sqlExecutor)
        {
            _sqlExecutor = sqlExecutor;
        }

        public Task<ProductResponse> CreateProductAsync(CreateProductRequest request)
        {
            return _sqlExecutor.ExecuteAsync(async (connection, transaction) =>
            {
                var dbConnection = (DbConnection)connection;
                var dbTransaction = (DbTransaction)transaction;

                await EnsureCategoryIdsExistAsync(dbConnection, dbTransaction, request.CategoryIds);

                var productId = await InsertProductAsync(dbConnection, dbTransaction, request);
                await InsertProductAttributesAsync(dbConnection, dbTransaction, productId, request.Attributes);
                await InsertProductCategoriesAsync(dbConnection, dbTransaction, productId, request.CategoryIds);

                return await GetRequiredProductAsync(dbConnection, dbTransaction, productId);
            });
        }

        public Task<ProductResponse> GetProductAsync(int productId)
        {
            return _sqlExecutor.ExecuteAsync(async (connection, transaction) =>
            {
                var dbConnection = (DbConnection)connection;
                var dbTransaction = (DbTransaction)transaction;

                var products = await GetProductsByIdsAsync(dbConnection, dbTransaction, new[] { productId });
                return products.FirstOrDefault();
            });
        }

        public Task<ProductResponse> UpdateProductAsync(int productId, UpdateProductRequest request)
        {
            return _sqlExecutor.ExecuteAsync(async (connection, transaction) =>
            {
                var dbConnection = (DbConnection)connection;
                var dbTransaction = (DbTransaction)transaction;

                // Ensure the product exists before updating
                var existing = await GetProductsByIdsAsync(dbConnection, dbTransaction, new[] { productId });
                if (existing.Count == 0)
                    throw new ResourceNotFoundException($"Product {productId} was not found.");

                // Ensure all requested category ids exist
                await EnsureCategoryIdsExistAsync(dbConnection, dbTransaction, request.CategoryIds);

                // Update the core product row
                using (var command = dbConnection.CreateCommand())
                {
                    command.Transaction = dbTransaction;
                    command.CommandText = @"
                    UPDATE [Instances].[Products]
                    SET [Name] = @Name,
                        [Description] = @Description,
                        [ProductImageUris] = @ProductImageUris,
                        [ValidSkus] = @ValidSkus
                    WHERE InstanceId = @ProductId;";

                    AddParameter(command, "@ProductId", productId);
                    AddParameter(command, "@Name", request.Name);
                    AddParameter(command, "@Description", request.Description);
                    AddParameter(command, "@ProductImageUris", JsonSerializer.Serialize(request.ProductImageUris));
                    AddParameter(command, "@ValidSkus", JsonSerializer.Serialize(request.ValidSkus));
                    await command.ExecuteNonQueryAsync();
                }

                // Replace attributes: delete old, insert new
                using (var command = dbConnection.CreateCommand())
                {
                    command.Transaction = dbTransaction;
                    command.CommandText = "DELETE FROM [Instances].[ProductAttributes] WHERE InstanceId = @ProductId;";
                    AddParameter(command, "@ProductId", productId);
                    await command.ExecuteNonQueryAsync();
                }

                await InsertProductAttributesAsync(dbConnection, dbTransaction, productId, request.Attributes);

                // Replace category links: delete old, insert new
                using (var command = dbConnection.CreateCommand())
                {
                    command.Transaction = dbTransaction;
                    command.CommandText = "DELETE FROM [Instances].[ProductCategories] WHERE InstanceId = @ProductId;";
                    AddParameter(command, "@ProductId", productId);
                    await command.ExecuteNonQueryAsync();
                }

                await InsertProductCategoriesAsync(dbConnection, dbTransaction, productId, request.CategoryIds);

                return await GetRequiredProductAsync(dbConnection, dbTransaction, productId);
            });
        }

        public Task<IReadOnlyList<ProductResponse>> SearchProductsAsync(SearchProductsRequest request)
        {
            return _sqlExecutor.ExecuteAsync(async (connection, transaction) =>
            {
                var dbConnection = (DbConnection)connection;
                var dbTransaction = (DbTransaction)transaction;

                using var command = dbConnection.CreateCommand();
                command.Transaction = dbTransaction;

                var sql = new StringBuilder();
                var includeDescendantCategories = request.CategoryIds.Count > 0 && request.IncludeDescendantCategories;
                if (includeDescendantCategories)
                    AppendCategoryCte(sql, command, request.CategoryIds);

                sql.AppendLine("SELECT p.InstanceId");
                sql.AppendLine("FROM [Instances].[Products] p");
                sql.AppendLine("WHERE 1 = 1");

                if (!string.IsNullOrWhiteSpace(request.NameContains))
                {
                    sql.AppendLine("  AND p.[Name] LIKE @NameContains");
                    AddParameter(command, "@NameContains", "%" + request.NameContains + "%");
                }

                if (!string.IsNullOrWhiteSpace(request.DescriptionContains))
                {
                    sql.AppendLine("  AND p.[Description] LIKE @DescriptionContains");
                    AddParameter(command, "@DescriptionContains", "%" + request.DescriptionContains + "%");
                }

                AppendSkuFilter(sql, command, request.Skus);
                AppendAttributeFilters(sql, command, request.Attributes);
                AppendCategoryFilter(sql, command, request.CategoryIds, request.MatchAllCategories, includeDescendantCategories, "p");
                sql.AppendLine("ORDER BY p.CreatedTimestamp DESC");

                if (includeDescendantCategories)
                    sql.AppendLine("OPTION (MAXRECURSION 100)");

                command.CommandText = sql.ToString();

                var productIds = await ReadSingleColumnIntegersAsync(command);
                return await GetProductsByIdsAsync(dbConnection, dbTransaction, productIds);
            });
        }

        public Task<CategoryResponse> CreateCategoryAsync(CreateCategoryRequest request)
        {
            return _sqlExecutor.ExecuteAsync(async (connection, transaction) =>
            {
                var dbConnection = (DbConnection)connection;
                var dbTransaction = (DbTransaction)transaction;

                await EnsureCategoryIdsExistAsync(dbConnection, dbTransaction, request.ParentCategoryIds);

                var categoryId = await InsertCategoryAsync(dbConnection, dbTransaction, request);
                await InsertCategoryAttributesAsync(dbConnection, dbTransaction, categoryId, request.Attributes);
                await InsertCategoryParentsAsync(dbConnection, dbTransaction, categoryId, request.ParentCategoryIds);

                return await GetRequiredCategoryAsync(dbConnection, dbTransaction, categoryId);
            });
        }

        public Task<IReadOnlyList<CategoryResponse>> GetCategoriesAsync()
        {
            return _sqlExecutor.ExecuteAsync(async (connection, transaction) =>
            {
                var dbConnection = (DbConnection)connection;
                var dbTransaction = (DbTransaction)transaction;

                var categoryIds = new List<int>();
                var categoryRows = new List<CategoryRow>();

                using (var command = dbConnection.CreateCommand())
                {
                    command.Transaction = dbTransaction;
                    command.CommandText = @"
                    SELECT c.InstanceId, c.[Name], c.[Description], c.CreatedTimestamp
                    FROM [Instances].[Categories] c
                    ORDER BY c.[Name];";

                    using var reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        var row = new CategoryRow
                        {
                            CategoryId = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Description = reader.GetString(2),
                            CreatedTimestamp = reader.GetDateTime(3)
                        };

                        categoryRows.Add(row);
                        categoryIds.Add(row.CategoryId);
                    }
                }

                return await BuildCategoriesAsync(dbConnection, dbTransaction, categoryRows, categoryIds);
            });
        }

        public Task<IReadOnlyList<InventoryTransactionResponse>> AddInventoryAsync(InventoryAdjustmentRequest request)
            => CreateInventoryTransactionsAsync(request, false);

        public Task<IReadOnlyList<InventoryTransactionResponse>> RemoveInventoryAsync(InventoryAdjustmentRequest request)
            => CreateInventoryTransactionsAsync(request, true);

        public Task<bool> DeleteInventoryTransactionAsync(int transactionId)
        {
            return _sqlExecutor.ExecuteAsync(async (connection, transaction) =>
            {
                var dbConnection = (DbConnection)connection;
                var dbTransaction = (DbTransaction)transaction;

                using var command = dbConnection.CreateCommand();
                command.Transaction = dbTransaction;
                // EVAL: The exercise asks for transaction removal, so delete acts as a lightweight
                // undo operation without inventing a second reversal table or workflow.
                command.CommandText = "DELETE FROM [Transactions].[InventoryTransactions] WHERE TransactionId = @TransactionId;";
                AddParameter(command, "@TransactionId", transactionId);

                return await command.ExecuteNonQueryAsync() > 0;
            });
        }

        public Task<InventoryCountResponse> GetInventoryCountsAsync(InventoryCountRequest request)
        {
            return _sqlExecutor.ExecuteAsync(async (connection, transaction) =>
            {
                var dbConnection = (DbConnection)connection;
                var dbTransaction = (DbTransaction)transaction;

                using var command = dbConnection.CreateCommand();
                command.Transaction = dbTransaction;

                var sql = new StringBuilder();
                var includeDescendantCategories = request.CategoryIds.Count > 0 && request.IncludeDescendantCategories;
                if (includeDescendantCategories)
                    AppendCategoryCte(sql, command, request.CategoryIds);

                sql.AppendLine("SELECT p.InstanceId, p.[Name], COALESCE(SUM(it.Quantity), 0)");
                sql.AppendLine("FROM [Instances].[Products] p");
                sql.AppendLine("LEFT JOIN [Transactions].[InventoryTransactions] it ON it.ProductInstanceId = p.InstanceId");
                sql.AppendLine("WHERE 1 = 1");

                if (request.ProductId.HasValue)
                {
                    sql.AppendLine("  AND p.InstanceId = @ProductId");
                    AddParameter(command, "@ProductId", request.ProductId.Value);
                }

                AppendAttributeFilters(sql, command, request.Attributes);
                AppendCategoryFilter(sql, command, request.CategoryIds, request.MatchAllCategories, includeDescendantCategories, "p");

                sql.AppendLine("GROUP BY p.InstanceId, p.[Name]");
                sql.AppendLine("ORDER BY p.[Name]");

                if (includeDescendantCategories)
                    sql.AppendLine("OPTION (MAXRECURSION 100)");

                command.CommandText = sql.ToString();

                var response = new InventoryCountResponse();
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    var item = new InventoryCountItemResponse
                    {
                        ProductId = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Quantity = reader.GetDecimal(2)
                    };

                    response.Products.Add(item);
                    response.TotalQuantity += item.Quantity;
                }

                if (request.ProductId.HasValue && response.Products.Count == 0)
                    throw new ResourceNotFoundException($"Product {request.ProductId.Value} was not found.");

                return response;
            });
        }

        private Task<IReadOnlyList<InventoryTransactionResponse>> CreateInventoryTransactionsAsync(InventoryAdjustmentRequest request, bool isRemoval)
        {
            return _sqlExecutor.ExecuteAsync(async (connection, transaction) =>
            {
                var dbConnection = (DbConnection)connection;
                var dbTransaction = (DbTransaction)transaction;
                var productIds = request.Items.Select(item => item.ProductId).Distinct().ToList();

                await EnsureProductIdsExistAsync(dbConnection, dbTransaction, productIds);

                if (isRemoval)
                {
                    var currentInventory = await GetCurrentInventoryAsync(dbConnection, dbTransaction, productIds);
                    foreach (var item in request.Items)
                    {
                        currentInventory.TryGetValue(item.ProductId, out var currentQuantity);
                        if (currentQuantity < item.Quantity)
                            throw new ApiValidationException($"Removing {item.Quantity} units from product {item.ProductId} would make inventory negative.");
                    }
                }

                var results = new List<InventoryTransactionResponse>();
                foreach (var item in request.Items)
                {
                    using var command = dbConnection.CreateCommand();
                    command.Transaction = dbTransaction;
                    command.CommandText = @"
                    INSERT INTO [Transactions].[InventoryTransactions]
                        ([ProductInstanceId], [Quantity], [CompletedTimestamp], [TypeCategory])
                    OUTPUT
                        INSERTED.TransactionId,
                        INSERTED.ProductInstanceId,
                        INSERTED.Quantity,
                        INSERTED.CompletedTimestamp,
                        INSERTED.TypeCategory
                    VALUES
                        (@ProductInstanceId, @Quantity, SYSUTCDATETIME(), @TypeCategory);";

                    AddParameter(command, "@ProductInstanceId", item.ProductId);
                    AddParameter(command, "@Quantity", isRemoval ? -item.Quantity : item.Quantity);
                    AddParameter(command, "@TypeCategory", request.TypeCategory);

                    using var reader = await command.ExecuteReaderAsync();
                    if (!await reader.ReadAsync())
                        throw new InvalidOperationException("Inventory transaction insert did not return the created row.");

                    results.Add(new InventoryTransactionResponse
                    {
                        TransactionId = reader.GetInt32(0),
                        ProductId = reader.GetInt32(1),
                        Quantity = reader.GetDecimal(2),
                        CompletedTimestamp = reader.GetDateTime(3),
                        TypeCategory = reader.IsDBNull(4) ? null : reader.GetString(4)
                    });
                }

                return (IReadOnlyList<InventoryTransactionResponse>)results;
            });
        }

        private static async Task<int> InsertProductAsync(DbConnection dbConnection, DbTransaction dbTransaction, CreateProductRequest request)
        {
            using var command = dbConnection.CreateCommand();
            command.Transaction = dbTransaction;
            command.CommandText = @"
            INSERT INTO [Instances].[Products]
                ([Name], [Description], [ProductImageUris], [ValidSkus])
            OUTPUT INSERTED.InstanceId
            VALUES
                (@Name, @Description, @ProductImageUris, @ValidSkus);";

            AddParameter(command, "@Name", request.Name);
            AddParameter(command, "@Description", request.Description);
            // EVAL: JSON keeps these multi-value fields in the current schema without introducing more tables.
            AddParameter(command, "@ProductImageUris", JsonSerializer.Serialize(request.ProductImageUris));
            AddParameter(command, "@ValidSkus", JsonSerializer.Serialize(request.ValidSkus));

            return Convert.ToInt32(await command.ExecuteScalarAsync());
        }

        private static async Task<int> InsertCategoryAsync(DbConnection dbConnection, DbTransaction dbTransaction, CreateCategoryRequest request)
        {
            using var command = dbConnection.CreateCommand();
            command.Transaction = dbTransaction;
            command.CommandText = @"
            INSERT INTO [Instances].[Categories]
                ([Name], [Description])
            OUTPUT INSERTED.InstanceId
            VALUES
                (@Name, @Description);";

            AddParameter(command, "@Name", request.Name);
            AddParameter(command, "@Description", request.Description);

            return Convert.ToInt32(await command.ExecuteScalarAsync());
        }

        private static async Task InsertProductAttributesAsync(DbConnection dbConnection, DbTransaction dbTransaction, int productId, IReadOnlyDictionary<string, string> attributes)
        {
            foreach (var pair in attributes)
            {
                using var command = dbConnection.CreateCommand();
                command.Transaction = dbTransaction;
                command.CommandText = @"
                INSERT INTO [Instances].[ProductAttributes]
                    ([InstanceId], [Key], [Value])
                VALUES
                    (@InstanceId, @Key, @Value);";

                AddParameter(command, "@InstanceId", productId);
                AddParameter(command, "@Key", pair.Key);
                AddParameter(command, "@Value", pair.Value);
                await command.ExecuteNonQueryAsync();
            }
        }

        private static async Task InsertProductCategoriesAsync(DbConnection dbConnection, DbTransaction dbTransaction, int productId, IReadOnlyCollection<int> categoryIds)
        {
            foreach (var categoryId in categoryIds)
            {
                using var command = dbConnection.CreateCommand();
                command.Transaction = dbTransaction;
                command.CommandText = @"
                INSERT INTO [Instances].[ProductCategories]
                    ([InstanceId], [CategoryInstanceId])
                VALUES
                    (@InstanceId, @CategoryInstanceId);";

                AddParameter(command, "@InstanceId", productId);
                AddParameter(command, "@CategoryInstanceId", categoryId);
                await command.ExecuteNonQueryAsync();
            }
        }

        private static async Task InsertCategoryAttributesAsync(DbConnection dbConnection, DbTransaction dbTransaction, int categoryId, IReadOnlyDictionary<string, string> attributes)
        {
            foreach (var pair in attributes)
            {
                using var command = dbConnection.CreateCommand();
                command.Transaction = dbTransaction;
                command.CommandText = @"
                INSERT INTO [Instances].[CategoryAttributes]
                    ([InstanceId], [Key], [Value])
                VALUES
                    (@InstanceId, @Key, @Value);";

                AddParameter(command, "@InstanceId", categoryId);
                AddParameter(command, "@Key", pair.Key);
                AddParameter(command, "@Value", pair.Value);
                await command.ExecuteNonQueryAsync();
            }
        }

        private static async Task InsertCategoryParentsAsync(DbConnection dbConnection, DbTransaction dbTransaction, int categoryId, IReadOnlyCollection<int> parentCategoryIds)
        {
            foreach (var parentCategoryId in parentCategoryIds)
            {
                using var command = dbConnection.CreateCommand();
                command.Transaction = dbTransaction;
                command.CommandText = @"
                INSERT INTO [Instances].[CategoryCategories]
                    ([InstanceId], [CategoryInstanceId])
                VALUES
                    (@InstanceId, @CategoryInstanceId);";

                AddParameter(command, "@InstanceId", categoryId);
                AddParameter(command, "@CategoryInstanceId", parentCategoryId);
                await command.ExecuteNonQueryAsync();
            }
        }

        private static async Task<ProductResponse> GetRequiredProductAsync(DbConnection dbConnection, DbTransaction dbTransaction, int productId)
        {
            var products = await GetProductsByIdsAsync(dbConnection, dbTransaction, new[] { productId });
            var product = products.FirstOrDefault();
            if (product == null)
                throw new ResourceNotFoundException($"Product {productId} was not found.");

            return product;
        }

        private static async Task<IReadOnlyList<ProductResponse>> GetProductsByIdsAsync(DbConnection dbConnection, DbTransaction dbTransaction, IReadOnlyCollection<int> productIds)
        {
            if (productIds == null || productIds.Count == 0)
                return Array.Empty<ProductResponse>();

            var productRows = new List<ProductRow>();
            using (var command = dbConnection.CreateCommand())
            {
                command.Transaction = dbTransaction;
                var parameterNames = AddIntegerParameters(command, "ProductId", productIds);
                command.CommandText = $@"
                SELECT p.InstanceId, p.[Name], p.[Description], p.ProductImageUris, p.ValidSkus, p.CreatedTimestamp
                FROM [Instances].[Products] p
                WHERE p.InstanceId IN ({string.Join(", ", parameterNames)})
                ORDER BY p.CreatedTimestamp DESC;";

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    productRows.Add(new ProductRow
                    {
                        ProductId = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Description = reader.GetString(2),
                        ProductImageUris = reader.GetString(3),
                        ValidSkus = reader.GetString(4),
                        CreatedTimestamp = reader.GetDateTime(5)
                    });
                }
            }

            if (productRows.Count == 0)
                return Array.Empty<ProductResponse>();

            var idList = productRows.Select(row => row.ProductId).ToList();
            var attributes = await ReadAttributesAsync(
                dbConnection,
                dbTransaction,
                "[Instances].[ProductAttributes]",
                idList);

            var categories = await ReadCategoryLinksAsync(
                dbConnection,
                dbTransaction,
                "[Instances].[ProductCategories]",
                idList);

            return productRows
                .Select(row => new ProductResponse
                {
                    ProductId = row.ProductId,
                    Name = row.Name,
                    Description = row.Description,
                    ProductImageUris = DeserializeStringList(row.ProductImageUris),
                    ValidSkus = DeserializeStringList(row.ValidSkus),
                    Attributes = attributes.TryGetValue(row.ProductId, out var attributeMap)
                        ? attributeMap
                        : new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase),
                    CategoryIds = categories.TryGetValue(row.ProductId, out var categoryIds)
                        ? categoryIds
                        : new List<int>(),
                    CreatedTimestamp = row.CreatedTimestamp
                })
                .ToList();
        }

        private static async Task<CategoryResponse> GetRequiredCategoryAsync(DbConnection dbConnection, DbTransaction dbTransaction, int categoryId)
        {
            var categories = await GetCategoriesByIdsAsync(dbConnection, dbTransaction, new[] { categoryId });
            var category = categories.FirstOrDefault();
            if (category == null)
                throw new ResourceNotFoundException($"Category {categoryId} was not found.");

            return category;
        }

        private static async Task<IReadOnlyList<CategoryResponse>> GetCategoriesByIdsAsync(DbConnection dbConnection, DbTransaction dbTransaction, IReadOnlyCollection<int> categoryIds)
        {
            if (categoryIds == null || categoryIds.Count == 0)
                return Array.Empty<CategoryResponse>();

            var categoryRows = new List<CategoryRow>();
            using (var command = dbConnection.CreateCommand())
            {
                command.Transaction = dbTransaction;
                var parameterNames = AddIntegerParameters(command, "CategoryId", categoryIds);
                command.CommandText = $@"
                SELECT c.InstanceId, c.[Name], c.[Description], c.CreatedTimestamp
                FROM [Instances].[Categories] c
                WHERE c.InstanceId IN ({string.Join(", ", parameterNames)})
                ORDER BY c.[Name];";

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    categoryRows.Add(new CategoryRow
                    {
                        CategoryId = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Description = reader.GetString(2),
                        CreatedTimestamp = reader.GetDateTime(3)
                    });
                }
            }

            return await BuildCategoriesAsync(
                dbConnection,
                dbTransaction,
                categoryRows,
                categoryRows.Select(row => row.CategoryId).ToList());
        }

        private static async Task<IReadOnlyList<CategoryResponse>> BuildCategoriesAsync(
            DbConnection dbConnection,
            DbTransaction dbTransaction,
            IReadOnlyList<CategoryRow> categoryRows,
            IReadOnlyCollection<int> categoryIds)
        {
            if (categoryRows.Count == 0)
                return Array.Empty<CategoryResponse>();

            var attributes = await ReadAttributesAsync(
                dbConnection,
                dbTransaction,
                "[Instances].[CategoryAttributes]",
                categoryIds);

            var parentCategories = await ReadCategoryLinksAsync(
                dbConnection,
                dbTransaction,
                "[Instances].[CategoryCategories]",
                categoryIds);

            return categoryRows
                .Select(row => new CategoryResponse
                {
                    CategoryId = row.CategoryId,
                    Name = row.Name,
                    Description = row.Description,
                    Attributes = attributes.TryGetValue(row.CategoryId, out var attributeMap)
                        ? attributeMap
                        : new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase),
                    ParentCategoryIds = parentCategories.TryGetValue(row.CategoryId, out var parentIds)
                        ? parentIds
                        : new List<int>(),
                    CreatedTimestamp = row.CreatedTimestamp
                })
                .ToList();
        }

        private static async Task<Dictionary<int, Dictionary<string, string>>> ReadAttributesAsync(
            DbConnection dbConnection,
            DbTransaction dbTransaction,
            string tableName,
            IReadOnlyCollection<int> instanceIds)
        {
            var result = new Dictionary<int, Dictionary<string, string>>();
            if (instanceIds == null || instanceIds.Count == 0)
                return result;

            using var command = dbConnection.CreateCommand();
            command.Transaction = dbTransaction;
            var parameterNames = AddIntegerParameters(command, "InstanceId", instanceIds);
            command.CommandText = $@"
            SELECT [InstanceId], [Key], [Value]
            FROM {tableName}
            WHERE [InstanceId] IN ({string.Join(", ", parameterNames)});";

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var instanceId = reader.GetInt32(0);
                if (!result.TryGetValue(instanceId, out var attributes))
                {
                    attributes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    result[instanceId] = attributes;
                }

                attributes[reader.GetString(1)] = reader.GetString(2);
            }

            return result;
        }

        private static async Task<Dictionary<int, List<int>>> ReadCategoryLinksAsync(
            DbConnection dbConnection,
            DbTransaction dbTransaction,
            string tableName,
            IReadOnlyCollection<int> instanceIds)
        {
            var result = new Dictionary<int, List<int>>();
            if (instanceIds == null || instanceIds.Count == 0)
                return result;

            using var command = dbConnection.CreateCommand();
            command.Transaction = dbTransaction;
            var parameterNames = AddIntegerParameters(command, "InstanceId", instanceIds);
            command.CommandText = $@"
            SELECT [InstanceId], [CategoryInstanceId]
            FROM {tableName}
            WHERE [InstanceId] IN ({string.Join(", ", parameterNames)});";

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var instanceId = reader.GetInt32(0);
                if (!result.TryGetValue(instanceId, out var categories))
                {
                    categories = new List<int>();
                    result[instanceId] = categories;
                }

                categories.Add(reader.GetInt32(1));
            }

            return result;
        }

        private static async Task<Dictionary<int, decimal>> GetCurrentInventoryAsync(
            DbConnection dbConnection,
            DbTransaction dbTransaction,
            IReadOnlyCollection<int> productIds)
        {
            var result = new Dictionary<int, decimal>();
            if (productIds == null || productIds.Count == 0)
                return result;

            using var command = dbConnection.CreateCommand();
            command.Transaction = dbTransaction;
            var parameterNames = AddIntegerParameters(command, "ProductId", productIds);
            command.CommandText = $@"
            SELECT it.ProductInstanceId, COALESCE(SUM(it.Quantity), 0)
            FROM [Transactions].[InventoryTransactions] it
            WHERE it.ProductInstanceId IN ({string.Join(", ", parameterNames)})
            GROUP BY it.ProductInstanceId;";

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result[reader.GetInt32(0)] = reader.GetDecimal(1);
            }

            return result;
        }

        private static async Task EnsureCategoryIdsExistAsync(DbConnection dbConnection, DbTransaction dbTransaction, IReadOnlyCollection<int> categoryIds)
        {
            if (categoryIds == null || categoryIds.Count == 0)
                return;

            var count = await CountMatchingIdsAsync(dbConnection, dbTransaction, "[Instances].[Categories]", categoryIds);
            if (count != categoryIds.Count)
                throw new ApiValidationException("One or more categoryIds do not exist.");
        }

        private static async Task EnsureProductIdsExistAsync(DbConnection dbConnection, DbTransaction dbTransaction, IReadOnlyCollection<int> productIds)
        {
            if (productIds == null || productIds.Count == 0)
                return;

            var count = await CountMatchingIdsAsync(dbConnection, dbTransaction, "[Instances].[Products]", productIds);
            if (count != productIds.Count)
                throw new ApiValidationException("One or more productIds do not exist.");
        }

        private static async Task<int> CountMatchingIdsAsync(
            DbConnection dbConnection,
            DbTransaction dbTransaction,
            string tableName,
            IReadOnlyCollection<int> ids)
        {
            using var command = dbConnection.CreateCommand();
            command.Transaction = dbTransaction;
            var parameterNames = AddIntegerParameters(command, "Id", ids);
            command.CommandText = $@"SELECT COUNT(*) FROM {tableName} WHERE InstanceId IN ({string.Join(", ", parameterNames)});";
            return Convert.ToInt32(await command.ExecuteScalarAsync());
        }

        private static async Task<List<int>> ReadSingleColumnIntegersAsync(DbCommand command)
        {
            var results = new List<int>();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                results.Add(reader.GetInt32(0));

            return results;
        }

        private static void AppendSkuFilter(StringBuilder sql, DbCommand command, IReadOnlyCollection<string> skus)
        {
            if (skus == null || skus.Count == 0)
                return;

            var clauses = new List<string>();
            var index = 0;
            foreach (var sku in skus)
            {
                var parameterName = "@SkuPattern" + index++;
                clauses.Add("p.ValidSkus LIKE " + parameterName);
                AddParameter(command, parameterName, "%\"" + sku + "\"%");
            }

            sql.AppendLine("  AND (" + string.Join(" OR ", clauses) + ")");
        }

        private static void AppendAttributeFilters(StringBuilder sql, DbCommand command, IReadOnlyDictionary<string, string> attributes)
        {
            if (attributes == null || attributes.Count == 0)
                return;

            var index = 0;
            foreach (var pair in attributes)
            {
                var keyParameter = "@AttributeKey" + index;
                var valueParameter = "@AttributeValue" + index;

                sql.AppendLine($@"  AND EXISTS (
                        SELECT 1
                        FROM [Instances].[ProductAttributes] pa
                        WHERE pa.InstanceId = p.InstanceId
                        AND pa.[Key] = {keyParameter}
                        AND pa.[Value] = {valueParameter}
                    )");

                AddParameter(command, keyParameter, pair.Key);
                AddParameter(command, valueParameter, pair.Value);
                index++;
            }
        }

        private static void AppendCategoryCte(StringBuilder sql, DbCommand command, IReadOnlyList<int> categoryIds)
        {
            // EVAL: CategoryCategories is interpreted as child -> parent, so the recursive CTE starts
            // from requested parents and walks down to include descendant categories in searches/counts.
            sql.AppendLine("WITH RequestedCategories AS (");
            for (var index = 0; index < categoryIds.Count; index++)
            {
                var parameterName = "@CategoryRoot" + index;
                var selectKeyword = index == 0 ? "    SELECT" : "    UNION ALL SELECT";
                sql.AppendLine($"{selectKeyword} {parameterName} AS RootCategoryId, {parameterName} AS CategoryInstanceId");
                AddParameter(command, parameterName, categoryIds[index]);
            }

            sql.AppendLine("), ExpandedCategories AS (");
            sql.AppendLine("    SELECT RootCategoryId, CategoryInstanceId");
            sql.AppendLine("    FROM RequestedCategories");
            sql.AppendLine("    UNION ALL");
            sql.AppendLine("    SELECT ec.RootCategoryId, cc.InstanceId");
            sql.AppendLine("    FROM ExpandedCategories ec");
            sql.AppendLine("    INNER JOIN [Instances].[CategoryCategories] cc ON cc.CategoryInstanceId = ec.CategoryInstanceId");
            sql.AppendLine(")");
        }

        private static void AppendCategoryFilter(
            StringBuilder sql,
            DbCommand command,
            IReadOnlyList<int> categoryIds,
            bool matchAllCategories,
            bool includeDescendantCategories,
            string productAlias)
        {
            if (categoryIds == null || categoryIds.Count == 0)
                return;

            AddParameter(command, "@RequestedCategoryCount", categoryIds.Count);

            if (includeDescendantCategories)
            {
                if (matchAllCategories)
                {
                    sql.AppendLine($@"  AND (
                    SELECT COUNT(DISTINCT ec.RootCategoryId)
                    FROM [Instances].[ProductCategories] pc
                    INNER JOIN ExpandedCategories ec ON ec.CategoryInstanceId = pc.CategoryInstanceId
                    WHERE pc.InstanceId = {productAlias}.InstanceId
                ) = @RequestedCategoryCount");
                }
                else
                {
                    sql.AppendLine($@"  AND EXISTS (
                    SELECT 1
                    FROM [Instances].[ProductCategories] pc
                    INNER JOIN ExpandedCategories ec ON ec.CategoryInstanceId = pc.CategoryInstanceId
                    WHERE pc.InstanceId = {productAlias}.InstanceId
                )");
                }

                return;
            }

            var parameterNames = AddIntegerParameters(command, "DirectCategory", categoryIds);
            var categoryList = string.Join(", ", parameterNames);

            if (matchAllCategories)
            {
                sql.AppendLine($@"  AND (
                SELECT COUNT(DISTINCT pc.CategoryInstanceId)
                FROM [Instances].[ProductCategories] pc
                WHERE pc.InstanceId = {productAlias}.InstanceId
                AND pc.CategoryInstanceId IN ({categoryList})
            ) = @RequestedCategoryCount");
            }
            else
            {
                sql.AppendLine($@"  AND EXISTS (
                SELECT 1
                FROM [Instances].[ProductCategories] pc
                WHERE pc.InstanceId = {productAlias}.InstanceId
                AND pc.CategoryInstanceId IN ({categoryList})
            )");
            }
        }

        private static List<string> AddIntegerParameters(DbCommand command, string prefix, IEnumerable<int> values)
        {
            var parameterNames = new List<string>();
            var index = 0;
            foreach (var value in values)
            {
                var parameterName = "@" + prefix + index++;
                parameterNames.Add(parameterName);
                AddParameter(command, parameterName, value);
            }

            return parameterNames;
        }

        private static void AddParameter(DbCommand command, string parameterName, object value)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = parameterName;
            parameter.Value = value ?? DBNull.Value;
            command.Parameters.Add(parameter);
        }

        private static List<string> DeserializeStringList(string serialized)
        {
            if (string.IsNullOrWhiteSpace(serialized))
                return new List<string>();

            try
            {
                return JsonSerializer.Deserialize<List<string>>(serialized) ?? new List<string>();
            }
            catch (JsonException)
            {
                return serialized
                    .Split(new[] { ',', ';', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(value => value.Trim())
                    .Where(value => value.Length > 0)
                    .ToList();
            }
        }

        private sealed class ProductRow
        {
            public int ProductId { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string ProductImageUris { get; set; }
            public string ValidSkus { get; set; }
            public DateTime CreatedTimestamp { get; set; }
        }

        private sealed class CategoryRow
        {
            public int CategoryId { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public DateTime CreatedTimestamp { get; set; }
        }
    }
}
