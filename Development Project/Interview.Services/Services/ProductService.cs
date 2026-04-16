using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interview.DataEntities.Models;
using Sparcpoint.SqlServer.Abstractions;

namespace Interview.Services
{
    public class ProductService : IProductService
    {
        private readonly ISqlExecutor _SqlExecutor;

        public ProductService(ISqlExecutor sqlExecutor)
        {
            _SqlExecutor = sqlExecutor ?? throw new ArgumentNullException(nameof(sqlExecutor));
        }

        public async Task<int> CreateProductAsync(ProductRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (string.IsNullOrWhiteSpace(request.Name)) throw new ArgumentException("Product Name is required.", nameof(request.Name));
            if (string.IsNullOrWhiteSpace(request.Description)) throw new ArgumentException("Product Description is required.", nameof(request.Description));
            if (string.IsNullOrWhiteSpace(request.ProductImageUris)) throw new ArgumentException("Product Image URIs are required.", nameof(request.ProductImageUris));
            if (string.IsNullOrWhiteSpace(request.ValidSkus)) throw new ArgumentException("Valid SKUs are required.", nameof(request.ValidSkus));

            ValidateMetadata(request.Metadata);

            return await _SqlExecutor.ExecuteAsync(async (connection, transaction) =>
            {
                int productId;
                const string productSql = @"
                    INSERT INTO [Instances].[Products] (Name, Description, ProductImageUris, ValidSkus)
                    VALUES (@Name, @Description, @ProductImageUris, @ValidSkus);
                    SELECT CAST(SCOPE_IDENTITY() as int);";

                using (var cmd = (SqlCommand)connection.CreateCommand())
                {
                    cmd.CommandText = productSql;
                    cmd.Transaction = (SqlTransaction)transaction;
                    cmd.Parameters.AddWithValue("@Name", request.Name);
                    cmd.Parameters.AddWithValue("@Description", request.Description);
                    cmd.Parameters.AddWithValue("@ProductImageUris", request.ProductImageUris);
                    cmd.Parameters.AddWithValue("@ValidSkus", request.ValidSkus);

                    productId = (int)await cmd.ExecuteScalarAsync();
                }

                if (request.Metadata != null && request.Metadata.Any())
                {
                    foreach (var kvp in request.Metadata)
                    {
                        const string attrSql = @"
                            INSERT INTO [Instances].[ProductAttributes] ([InstanceId], [Key], [Value])
                            VALUES (@InstanceId, @Key, @Value)";
                        
                        using (var cmd = (SqlCommand)connection.CreateCommand())
                        {
                            cmd.CommandText = attrSql;
                            cmd.Transaction = (SqlTransaction)transaction;
                            cmd.Parameters.AddWithValue("@InstanceId", productId);
                            cmd.Parameters.AddWithValue("@Key", kvp.Key);
                            cmd.Parameters.AddWithValue("@Value", kvp.Value);
                            await cmd.ExecuteNonQueryAsync();
                        }
                    }
                }

                return productId;
            });
        }

        public async Task<IEnumerable<ProductResponse>> SearchProductsAsync(ProductSearchRequest request)
        {
            return await _SqlExecutor.ExecuteAsync(async (connection, transaction) =>
            {
                var sql = new StringBuilder(@"
                    SELECT DISTINCT p.InstanceId, p.Name, p.Description, p.ProductImageUris, p.ValidSkus, p.CreatedTimestamp
                    FROM [Instances].[Products] p");

                if (request.CategoryIds != null && request.CategoryIds.Any())
                {
                    sql.Append(" JOIN [Instances].[ProductCategories] pc ON p.InstanceId = pc.InstanceId");
                }

                var whereClauses = new List<string>();
                var parameters = new List<SqlParameter>();

                if (request.CategoryIds != null && request.CategoryIds.Any())
                {
                    whereClauses.Add($"pc.CategoryId IN ({string.Join(",", request.CategoryIds)})");
                }

                if (request.MetadataFilters != null && request.MetadataFilters.Any())
                {
                    int i = 0;
                    foreach (var filter in request.MetadataFilters)
                    {
                        string alias = $"pa{i}";
                        sql.Append($" JOIN [Instances].[ProductAttributes] {alias} ON p.InstanceId = {alias}.InstanceId");
                        whereClauses.Add($"{alias}.[Key] = @Key{i} AND {alias}.[Value] = @Value{i}");
                        parameters.Add(new SqlParameter($"@Key{i}", filter.Key));
                        parameters.Add(new SqlParameter($"@Value{i}", filter.Value));
                        i++;
                    }
                }

                if (whereClauses.Any())
                {
                    sql.Append(" WHERE " + string.Join(" AND ", whereClauses));
                }

                var products = new List<ProductResponse>();
                using (var cmd = (SqlCommand)connection.CreateCommand())
                {
                    cmd.CommandText = sql.ToString();
                    cmd.Transaction = (SqlTransaction)transaction;
                    cmd.Parameters.AddRange(parameters.ToArray());

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            products.Add(new ProductResponse
                            {
                                InstanceId = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Description = reader.GetString(2),
                                ProductImageUris = reader.GetString(3),
                                ValidSkus = reader.GetString(4),
                                CreatedTimestamp = reader.GetDateTime(5),
                                Metadata = new Dictionary<string, string>()
                            });
                        }
                    }
                }

                // Hydrate Metadata for each product
                foreach (var product in products)
                {
                    using (var cmd = (SqlCommand)connection.CreateCommand())
                    {
                        cmd.CommandText = "SELECT [Key], [Value] FROM [Instances].[ProductAttributes] WHERE InstanceId = @Id";
                        cmd.Transaction = (SqlTransaction)transaction;
                        cmd.Parameters.AddWithValue("@Id", product.InstanceId);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                product.Metadata[reader.GetString(0)] = reader.GetString(1);
                            }
                        }
                    }
                }

                return (IEnumerable<ProductResponse>)products;
            });
        }


        private void ValidateMetadata(Dictionary<string, string> metadata)
        {
            if (metadata == null) return;
            foreach (var kvp in metadata)
            {
                if (!IsAlphanumeric(kvp.Key) || !IsAlphanumeric(kvp.Value))
                    throw new ArgumentException($"Metadata key or value '{kvp.Key}:{kvp.Value}' contains special characters. Only alphanumeric characters allowed.");
            }
        }

        private bool IsAlphanumeric(string str)
        {
            if (string.IsNullOrEmpty(str)) return true;
            foreach (char c in str)
            {
                if (!char.IsLetterOrDigit(c)) return false;
            }
            return true;
        }
    }
}
