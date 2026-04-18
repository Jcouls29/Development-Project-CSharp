using Interview.Web.Models.Products;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;

namespace Interview.Web.Repositories.Products
{
    public class ProductRepository : IProductRepository
    {
        private readonly ISqlExecutor _sqlExecutor;

        public ProductRepository(ISqlExecutor sqlExecutor)
        {
            _sqlExecutor = sqlExecutor ?? throw new ArgumentNullException(nameof(sqlExecutor));
        }

        public async Task<int> AddAsync(ProductCreateModel product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            return await _sqlExecutor.ExecuteAsync<int>(async (connection, transaction) =>
            {
                var dbConnection = connection as DbConnection;
                var dbTransaction = transaction as DbTransaction;
                if (dbConnection == null || dbTransaction == null)
                {
                    throw new InvalidOperationException("Expected database connection and transaction.");
                }

                await EnsureCategoriesExistAsync(dbConnection, dbTransaction, product.CategoryIds);

                int productId = await InsertProductAsync(dbConnection, dbTransaction, product);
                await InsertMetadataAsync(dbConnection, dbTransaction, productId, product.Metadata);
                await InsertCategoryLinksAsync(dbConnection, dbTransaction, productId, product.CategoryIds);

                return productId;
            });
        }

        public async Task<IReadOnlyList<ProductSearchResultModel>> SearchAsync(ProductSearchRequestModel request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return await _sqlExecutor.ExecuteAsync<IReadOnlyList<ProductSearchResultModel>>(async (connection, transaction) =>
            {
                var dbConnection = connection as DbConnection;
                var dbTransaction = transaction as DbTransaction;
                if (dbConnection == null || dbTransaction == null)
                {
                    throw new InvalidOperationException("Expected database connection and transaction.");
                }

                var products = await SearchProductsCoreAsync(dbConnection, dbTransaction, request);
                if (products.Count == 0)
                {
                    return products;
                }

                await PopulateMetadataAsync(dbConnection, dbTransaction, products);
                await PopulateCategoriesAsync(dbConnection, dbTransaction, products);
                return products;
            });
        }

        public async Task<ProductInventoryAdjustmentResultModel> AddInventoryTransactionAsync(ProductInventoryAdjustmentModel adjustment)
        {
            if (adjustment == null)
            {
                throw new ArgumentNullException(nameof(adjustment));
            }

            return await _sqlExecutor.ExecuteAsync<ProductInventoryAdjustmentResultModel>(async (connection, transaction) =>
            {
                var dbConnection = connection as DbConnection;
                var dbTransaction = transaction as DbTransaction;
                if (dbConnection == null || dbTransaction == null)
                {
                    throw new InvalidOperationException("Expected database connection and transaction.");
                }

                await EnsureProductsExistAsync(dbConnection, dbTransaction, new[] { adjustment.ProductId });

                int transactionId;
                using (var command = CreateCommand(dbConnection, dbTransaction, @"
INSERT INTO [Transactions].[InventoryTransactions] ([ProductInstanceId], [Quantity], [CompletedTimestamp], [TypeCategory])
VALUES (@ProductInstanceId, @Quantity, SYSUTCDATETIME(), @TypeCategory);
SELECT CAST(SCOPE_IDENTITY() AS INT);"))
                {
                    AddParameter(command, "@ProductInstanceId", DbType.Int32, adjustment.ProductId);
                    AddParameter(command, "@Quantity", DbType.Decimal, adjustment.Quantity);
                    AddParameter(command, "@TypeCategory", DbType.String, adjustment.TypeCategory);

                    object result = await command.ExecuteScalarAsync();
                    if (result == null || result == DBNull.Value)
                    {
                        throw new InvalidOperationException("Failed to create inventory transaction.");
                    }

                    transactionId = Convert.ToInt32(result);
                }

                decimal currentQuantity = await GetInventoryCountInternalAsync(dbConnection, dbTransaction, adjustment.ProductId);
                return new ProductInventoryAdjustmentResultModel
                {
                    TransactionId = transactionId,
                    CurrentQuantity = currentQuantity
                };
            });
        }

        public async Task<decimal> GetInventoryCountAsync(int productId)
        {
            if (productId <= 0)
            {
                throw new ArgumentException("productId must be a positive integer.", nameof(productId));
            }

            return await _sqlExecutor.ExecuteAsync<decimal>(async (connection, transaction) =>
            {
                var dbConnection = connection as DbConnection;
                var dbTransaction = transaction as DbTransaction;
                if (dbConnection == null || dbTransaction == null)
                {
                    throw new InvalidOperationException("Expected database connection and transaction.");
                }

                await EnsureProductsExistAsync(dbConnection, dbTransaction, new[] { productId });
                return await GetInventoryCountInternalAsync(dbConnection, dbTransaction, productId);
            });
        }

        private static async Task EnsureCategoriesExistAsync(DbConnection connection, DbTransaction transaction, IReadOnlyList<int> categoryIds)
        {
            foreach (int categoryId in categoryIds)
            {
                using var command = CreateCommand(connection, transaction,
                    "SELECT TOP 1 1 FROM [Instances].[Categories] WHERE [InstanceId] = @CategoryId;");
                AddParameter(command, "@CategoryId", DbType.Int32, categoryId);

                object exists = await command.ExecuteScalarAsync();
                if (exists == null)
                {
                    throw new ArgumentException($"Category id '{categoryId}' does not exist.", nameof(categoryIds));
                }
            }
        }

        private static async Task EnsureProductsExistAsync(DbConnection connection, DbTransaction transaction, IEnumerable<int> productIds)
        {
            foreach (int productId in productIds.Distinct())
            {
                using var command = CreateCommand(connection, transaction,
                    "SELECT TOP 1 1 FROM [Instances].[Products] WHERE [InstanceId] = @ProductId;");
                AddParameter(command, "@ProductId", DbType.Int32, productId);

                object exists = await command.ExecuteScalarAsync();
                if (exists == null)
                {
                    throw new ArgumentException($"Product id '{productId}' does not exist.", nameof(productIds));
                }
            }
        }

        private static async Task<int> InsertProductAsync(DbConnection connection, DbTransaction transaction, ProductCreateModel product)
        {
            using var command = CreateCommand(connection, transaction, @"
INSERT INTO [Instances].[Products] ([Name], [Description], [ProductImageUris], [ValidSkus])
VALUES (@Name, @Description, @ProductImageUris, @ValidSkus);
SELECT CAST(SCOPE_IDENTITY() AS INT);");

            AddParameter(command, "@Name", DbType.String, product.Name);
            AddParameter(command, "@Description", DbType.String, product.Description);
            AddParameter(command, "@ProductImageUris", DbType.String, SerializeStringCollection(product.ProductImageUris));
            AddParameter(command, "@ValidSkus", DbType.String, SerializeStringCollection(product.ValidSkus));

            object result = await command.ExecuteScalarAsync();
            if (result == null || result == DBNull.Value)
            {
                throw new InvalidOperationException("Failed to create product.");
            }

            return Convert.ToInt32(result);
        }

        private static async Task InsertMetadataAsync(DbConnection connection, DbTransaction transaction, int productId, IReadOnlyList<ProductMetadataModel> metadata)
        {
            foreach (var item in metadata)
            {
                using var command = CreateCommand(connection, transaction, @"
INSERT INTO [Instances].[ProductAttributes] ([InstanceId], [Key], [Value])
VALUES (@InstanceId, @Key, @Value);");

                AddParameter(command, "@InstanceId", DbType.Int32, productId);
                AddParameter(command, "@Key", DbType.String, item.Key);
                AddParameter(command, "@Value", DbType.String, item.Value);

                await command.ExecuteNonQueryAsync();
            }
        }

        private static async Task InsertCategoryLinksAsync(DbConnection connection, DbTransaction transaction, int productId, IReadOnlyList<int> categoryIds)
        {
            foreach (int categoryId in categoryIds)
            {
                using var command = CreateCommand(connection, transaction, @"
INSERT INTO [Instances].[ProductCategories] ([InstanceId], [CategoryInstanceId])
VALUES (@InstanceId, @CategoryInstanceId);");

                AddParameter(command, "@InstanceId", DbType.Int32, productId);
                AddParameter(command, "@CategoryInstanceId", DbType.Int32, categoryId);

                await command.ExecuteNonQueryAsync();
            }
        }

        private static DbCommand CreateCommand(DbConnection connection, DbTransaction transaction, string commandText)
        {
            var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandType = CommandType.Text;
            command.CommandText = commandText;
            return command;
        }

        private static void AddParameter(DbCommand command, string name, DbType dbType, object value)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.DbType = dbType;
            parameter.Value = value ?? DBNull.Value;
            command.Parameters.Add(parameter);
        }

        private static string SerializeStringCollection(IReadOnlyList<string> values)
        {
            return JsonSerializer.Serialize(values ?? Array.Empty<string>());
        }

        private static async Task<List<ProductSearchResultModel>> SearchProductsCoreAsync(DbConnection connection, DbTransaction transaction, ProductSearchRequestModel request)
        {
            var sql = new StringBuilder(@"
SELECT DISTINCT
    p.[InstanceId],
    p.[Name],
    p.[Description],
    p.[ProductImageUris],
    p.[ValidSkus],
    p.[CreatedTimestamp]
FROM [Instances].[Products] p
LEFT JOIN [Instances].[ProductAttributes] pa ON pa.[InstanceId] = p.[InstanceId]
LEFT JOIN [Instances].[ProductCategories] pc ON pc.[InstanceId] = p.[InstanceId]
WHERE 1 = 1");

            using var command = CreateCommand(connection, transaction, sql.ToString());

            if (!string.IsNullOrWhiteSpace(request.SearchText))
            {
                sql.Append(@"
AND (
    p.[Name] LIKE @SearchText
    OR p.[Description] LIKE @SearchText
    OR p.[ProductImageUris] LIKE @SearchText
    OR p.[ValidSkus] LIKE @SearchText
)");
                AddParameter(command, "@SearchText", DbType.String, $"%{request.SearchText}%");
            }

            if (!string.IsNullOrWhiteSpace(request.MetadataKey))
            {
                sql.Append("\nAND pa.[Key] = @MetadataKey");
                AddParameter(command, "@MetadataKey", DbType.String, request.MetadataKey);
            }

            if (!string.IsNullOrWhiteSpace(request.MetadataValue))
            {
                sql.Append("\nAND pa.[Value] = @MetadataValue");
                AddParameter(command, "@MetadataValue", DbType.String, request.MetadataValue);
            }

            if (request.CategoryIds != null && request.CategoryIds.Count > 0)
            {
                var categoryParamNames = new List<string>();
                for (int i = 0; i < request.CategoryIds.Count; i++)
                {
                    string paramName = $"@CategoryId{i}";
                    categoryParamNames.Add(paramName);
                    AddParameter(command, paramName, DbType.Int32, request.CategoryIds[i]);
                }

                sql.Append($"\nAND pc.[CategoryInstanceId] IN ({string.Join(", ", categoryParamNames)})");
            }

            sql.Append("\nORDER BY p.[InstanceId] DESC;");
            command.CommandText = sql.ToString();

            var results = new List<ProductSearchResultModel>();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                results.Add(new ProductSearchResultModel
                {
                    ProductId = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Description = reader.GetString(2),
                    ProductImageUris = DeserializeStringCollection(reader.GetString(3)),
                    ValidSkus = DeserializeStringCollection(reader.GetString(4)),
                    CreatedTimestamp = reader.GetDateTime(5)
                });
            }

            return results;
        }

        private static async Task PopulateMetadataAsync(DbConnection connection, DbTransaction transaction, List<ProductSearchResultModel> products)
        {
            var metadataMap = products.ToDictionary(x => x.ProductId, _ => new List<ProductMetadataModel>());
            using var command = CreateCommand(connection, transaction, @"
SELECT pa.[InstanceId], pa.[Key], pa.[Value]
FROM [Instances].[ProductAttributes] pa
WHERE pa.[InstanceId] IN ({PRODUCT_ID_PARAMS})
ORDER BY pa.[InstanceId];");

            AddIdListParameters(command, products.Select(x => x.ProductId), "@ProductId", "{PRODUCT_ID_PARAMS}");

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                int productId = reader.GetInt32(0);
                if (!metadataMap.TryGetValue(productId, out var metadataItems))
                {
                    continue;
                }

                metadataItems.Add(new ProductMetadataModel
                {
                    Key = reader.GetString(1),
                    Value = reader.GetString(2)
                });
            }

            foreach (var product in products)
            {
                product.Metadata = metadataMap[product.ProductId];
            }
        }

        private static async Task PopulateCategoriesAsync(DbConnection connection, DbTransaction transaction, List<ProductSearchResultModel> products)
        {
            using var command = CreateCommand(connection, transaction, @"
SELECT pc.[InstanceId], pc.[CategoryInstanceId]
FROM [Instances].[ProductCategories] pc
WHERE pc.[InstanceId] IN ({PRODUCT_ID_PARAMS})
ORDER BY pc.[InstanceId];");

            AddIdListParameters(command, products.Select(x => x.ProductId), "@ProductId", "{PRODUCT_ID_PARAMS}");

            var categoryMap = products.ToDictionary(x => x.ProductId, _ => new HashSet<int>());
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                int productId = reader.GetInt32(0);
                if (!categoryMap.TryGetValue(productId, out var categoryItems))
                {
                    continue;
                }

                categoryItems.Add(reader.GetInt32(1));
            }

            foreach (var product in products)
            {
                product.CategoryIds = categoryMap[product.ProductId].ToList();
            }
        }

        private static IReadOnlyList<string> DeserializeStringCollection(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return Array.Empty<string>();
            }

            try
            {
                return JsonSerializer.Deserialize<List<string>>(value) ?? new List<string>();
            }
            catch
            {
                return new List<string> { value };
            }
        }

        private static void AddIdListParameters(DbCommand command, IEnumerable<int> ids, string parameterPrefix, string placeholder)
        {
            var idList = ids.Distinct().ToList();
            var parameterNames = new List<string>(idList.Count);

            for (int i = 0; i < idList.Count; i++)
            {
                string parameterName = $"{parameterPrefix}{i}";
                parameterNames.Add(parameterName);
                AddParameter(command, parameterName, DbType.Int32, idList[i]);
            }

            command.CommandText = command.CommandText.Replace(placeholder, string.Join(", ", parameterNames));
        }

        private static async Task<decimal> GetInventoryCountInternalAsync(DbConnection connection, DbTransaction transaction, int productId)
        {
            using var command = CreateCommand(connection, transaction, @"
SELECT ISNULL(SUM([Quantity]), 0)
FROM [Transactions].[InventoryTransactions]
WHERE [ProductInstanceId] = @ProductInstanceId
    AND [CompletedTimestamp] IS NOT NULL;");

            AddParameter(command, "@ProductInstanceId", DbType.Int32, productId);
            object result = await command.ExecuteScalarAsync();
            if (result == null || result == DBNull.Value)
            {
                return 0m;
            }

            return Convert.ToDecimal(result);
        }
    }
}
