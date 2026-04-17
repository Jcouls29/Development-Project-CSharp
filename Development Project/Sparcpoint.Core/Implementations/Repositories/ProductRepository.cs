using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sparcpoint.Abstract.Repositories;
using Sparcpoint.Core.Models;
using Sparcpoint.DTOs;
using Sparcpoint.SqlServer.Abstractions;

namespace Sparcpoint.Implementations.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ISqlExecutor _sqlExecutor;

        public ProductRepository(ISqlExecutor sqlExecutor)
        {
            _sqlExecutor = sqlExecutor; 
        }

        /*
         * Creates new product
         */
        public async Task<Product> Create(ProductDTO request)
        {
            return await _sqlExecutor.ExecuteAsync(async (connection, transaction) =>
            {
                var imageUris = request.ProductImageUris != null
                    ? string.Join(",", request.ProductImageUris)
                    : null;

                var skus = request.ValidSkus != null
                    ? string.Join(",", request.ValidSkus)
                    : null;

                int productId;

                using (var command = connection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandText = @"
                        INSERT INTO Products (Name, Description, ProductImageUris, ValidSkus, CreatedTimestamp)
                        VALUES (@Name, @Description, @ProductImageUris, @ValidSkus, GETDATE());

                        SELECT CAST(SCOPE_IDENTITY() as int);
                    ";

                    var nameParameter = command.CreateParameter();
                    nameParameter.ParameterName = "@Name";
                    nameParameter.Value = (object)request.Name ?? DBNull.Value;
                    command.Parameters.Add(nameParameter);

                    var descriptionParameter = command.CreateParameter();
                    descriptionParameter.ParameterName = "@Description";
                    descriptionParameter.Value = (object)request.Description ?? DBNull.Value;
                    command.Parameters.Add(descriptionParameter);

                    var productImageUrisParameter = command.CreateParameter();
                    productImageUrisParameter.ParameterName = "@ProductImageUris";
                    productImageUrisParameter.Value = (object)imageUris ?? DBNull.Value;
                    command.Parameters.Add(productImageUrisParameter);

                    var validSkusParameter = command.CreateParameter();
                    validSkusParameter.ParameterName = "@ValidSkus";
                    validSkusParameter.Value = (object)skus ?? DBNull.Value;
                    command.Parameters.Add(validSkusParameter);

                    var result = await ((dynamic)command).ExecuteScalarAsync();
                    productId = Convert.ToInt32(result);
                }

                if (request.Categories != null)
                {
                    foreach (var category in request.Categories)
                    {
                        using (var categoryCommand = connection.CreateCommand())
                        {
                            categoryCommand.Transaction = transaction;
                            categoryCommand.CommandText = @"
                                INSERT INTO ProductCategories (ProductId, CategoryId)
                                VALUES (@ProductId, @CategoryId);
                            ";

                            var productIdParameter = categoryCommand.CreateParameter();
                            productIdParameter.ParameterName = "@ProductId";
                            productIdParameter.Value = productId;
                            categoryCommand.Parameters.Add(productIdParameter);

                            var categoryIdParameter = categoryCommand.CreateParameter();
                            categoryIdParameter.ParameterName = "@CategoryId";
                            categoryIdParameter.Value = category.CategoryId;
                            categoryCommand.Parameters.Add(categoryIdParameter);

                            await ((dynamic)categoryCommand).ExecuteNonQueryAsync();
                        }
                    }
                }

                if (request.Metadata != null)
                {
                    foreach (var metadata in request.Metadata)
                    {
                        using (var metadataCommand = connection.CreateCommand())
                        {
                            metadataCommand.Transaction = transaction;
                            metadataCommand.CommandText = @"
                        INSERT INTO ProductMetadata (ProductId, [Key], [Value])
                        VALUES (@ProductId, @Key, @Value);
                    ";

                            var productIdParameter = metadataCommand.CreateParameter();
                            productIdParameter.ParameterName = "@ProductId";
                            productIdParameter.Value = productId;
                            metadataCommand.Parameters.Add(productIdParameter);

                            var keyParameter = metadataCommand.CreateParameter();
                            keyParameter.ParameterName = "@Key";
                            keyParameter.Value = (object)metadata.Key ?? DBNull.Value;
                            metadataCommand.Parameters.Add(keyParameter);

                            var valueParameter = metadataCommand.CreateParameter();
                            valueParameter.ParameterName = "@Value";
                            valueParameter.Value = (object)metadata.Value ?? DBNull.Value;
                            metadataCommand.Parameters.Add(valueParameter);

                            await ((dynamic)metadataCommand).ExecuteNonQueryAsync();
                        }
                    }
                }

                return new Product
                {
                    InstanceId = productId,
                    Name = request.Name,
                    Description = request.Description,
                    ProductImageUris = request.ProductImageUris,
                    ValidSkus = request.ValidSkus,
                    CreatedTimestamp = DateTime.UtcNow,
                    Categories = request.Categories,
                    Metadata = request.Metadata
                };
            });
        }


        /*
         * Gets all products
         */
        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _sqlExecutor.ExecuteAsync(async (connection, transaction) =>
            {
                var products = new List<Product>();

                using (var command = connection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandText = @"
                        SELECT
                            InstanceId,
                            Name,
                            Description,
                            ProductImageUris,
                            ValidSkus,
                            CreatedTimestamp
                        FROM Products";

                    using (var reader = await ((dynamic)command).ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            var product = new Product
                            {
                                InstanceId = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Description = reader.GetString(2),
                                ProductImageUris = reader.IsDBNull(3)
                                    ? null
                                    : reader.GetString(3).Split(',', StringSplitOptions.RemoveEmptyEntries),
                                ValidSkus = reader.IsDBNull(4)
                                    ? null
                                    : reader.GetString(4).Split(',', StringSplitOptions.RemoveEmptyEntries),
                                CreatedTimestamp = reader.GetDateTime(5)
                            };

                            products.Add(product);
                        }
                    }
                }

                return products;
            });
        }

        public async Task<IEnumerable<Product>> SearchAsync(ProductDTO request)
        {
            return await _sqlExecutor.ExecuteAsync(async (connection, transaction) =>
            {
                var products = new List<Product>();
                var sqlBuilder = new System.Text.StringBuilder();

                sqlBuilder.AppendLine(@"
                    SELECT DISTINCT
                        p.InstanceId,
                        p.Name,
                        p.Description,
                        p.ProductImageUris,
                        p.ValidSkus,
                        p.CreatedTimestamp
                    FROM Products p");

                using (var command = connection.CreateCommand())
                {
                    command.Transaction = transaction;

                    if (!string.IsNullOrWhiteSpace(request.Name))
                    {
                        sqlBuilder.AppendLine("AND p.Name LIKE @Name");

                        var parameter = command.CreateParameter();
                        parameter.ParameterName = "@Name";
                        parameter.Value = $"%{request.Name}%";
                        command.Parameters.Add(parameter);
                    }

                    if (!string.IsNullOrWhiteSpace(request.Description))
                    {
                        sqlBuilder.AppendLine("AND p.Description LIKE @Description");

                        var parameter = command.CreateParameter();
                        parameter.ParameterName = "@Description";
                        parameter.Value = $"%{request.Description}%";
                        command.Parameters.Add(parameter);
                    }

                    if (request.Categories != null)
                    {
                        var categories = request.Categories;

                        if (categories.Count() > 0)
                        {
                            for (int i = 0; i < categories.Count(); i++)
                            {
                                var parameterName = $"@CategoryId{i}";
                                if (i == 0)
                                {
                                    sqlBuilder.AppendLine("AND EXISTS (");
                                    sqlBuilder.AppendLine("    SELECT 1");
                                    sqlBuilder.AppendLine("    FROM ProductCategories pc");
                                    sqlBuilder.AppendLine("    WHERE pc.ProductId = p.InstanceId");
                                    sqlBuilder.Append($"      AND pc.CategoryId IN ({parameterName}");
                                }
                                else
                                {
                                    sqlBuilder.Append($", {parameterName}");
                                }

                                var parameter = command.CreateParameter();
                                parameter.ParameterName = parameterName;
                                parameter.Value = categories.ElementAt(i);
                                command.Parameters.Add(parameter);
                            }

                            sqlBuilder.AppendLine("))");
                        }
                    }

                    if (request.Metadata != null)
                    {
                        var metadataList = request.Metadata.ToList();

                        for (int i = 0; i < metadataList.Count; i++)
                        {
                            var keyParameterName = $"@MetaKey{i}";
                            var valueParameterName = $"@MetaValue{i}";

                            sqlBuilder.AppendLine($@"
                        AND EXISTS (
                            SELECT 1
                            FROM ProductMetadata pm
                            WHERE pm.ProductId = p.InstanceId
                              AND pm.[Key] = {keyParameterName}
                              AND pm.[Value] = {valueParameterName}
                        )");

                            var keyParameter = command.CreateParameter();
                            keyParameter.ParameterName = keyParameterName;
                            keyParameter.Value = (object)metadataList[i].Key ?? DBNull.Value;
                            command.Parameters.Add(keyParameter);

                            var valueParameter = command.CreateParameter();
                            valueParameter.ParameterName = valueParameterName;
                            valueParameter.Value = (object)metadataList[i].Value ?? DBNull.Value;
                            command.Parameters.Add(valueParameter);
                        }
                    }

                    command.CommandText = sqlBuilder.ToString();

                    using (var reader = await ((dynamic)command).ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            var product = new Product
                            {
                                InstanceId = reader.GetInt32(0),
                                Name = reader.IsDBNull(1) ? null : reader.GetString(1),
                                Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                                ProductImageUris = reader.IsDBNull(3)
                                    ? null
                                    : reader.GetString(3).Split(',', StringSplitOptions.RemoveEmptyEntries),
                                ValidSkus = reader.IsDBNull(4)
                                    ? null
                                    : reader.GetString(4).Split(',', StringSplitOptions.RemoveEmptyEntries),
                                CreatedTimestamp = reader.GetDateTime(5)
                            };

                            products.Add(product);
                        }
                    }
                }

                return products;
            });
        }
    }
}
