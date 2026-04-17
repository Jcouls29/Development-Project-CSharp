using System;
using System.Threading.Tasks;
using Sparcpoint.Core.Models;
using Microsoft.Extensions.Logging;
using Sparcpoint.SqlServer.Abstractions;
using Sparcpoint.Application.Repositories;

namespace Sparcpoint.Infrastructure.Data.Repositories
{
    public class SqlProductRepository : IProductRepository
    {
        private readonly ISqlExecutor _executor;
        private readonly ILogger<SqlProductRepository> _logger;

        public SqlProductRepository(ISqlExecutor executor, ILogger<SqlProductRepository> logger = null)
        {
            _executor = executor ?? throw new ArgumentNullException(nameof(executor));
            _logger = logger;
        }

        public async Task<System.Collections.Generic.IEnumerable<ProductResponseDto>> SearchAsync(
            string name,
            System.Collections.Generic.Dictionary<string, string> metadata = null,
            System.Collections.Generic.List<string> categories = null)
        {
            var results = new System.Collections.Generic.List<ProductResponseDto>();

            await _executor.ExecuteAsync(async (conn, tx) =>
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.Transaction = tx;

                    var sql = new System.Text.StringBuilder();
                    sql.Append("SELECT DISTINCT p.InstanceId, p.Name FROM [Instances].[Products] p WHERE 1=1 ");

                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        sql.Append(" AND p.Name LIKE @name");
                        var p = cmd.CreateParameter(); p.ParameterName = "@name"; p.Value = "%" + name + "%"; cmd.Parameters.Add(p);
                    }

                    if (metadata != null)
                    {
                        int i = 0;
                        foreach (var kv in metadata)
                        {
                            var keyParam = "@mkey" + i;
                            var valParam = "@mval" + i;
                            sql.Append($" AND EXISTS (SELECT 1 FROM [Instances].[ProductAttributes] pa WHERE pa.InstanceId = p.InstanceId AND pa.[Key] = {keyParam} AND pa.[Value] = {valParam})");
                            var pk = cmd.CreateParameter(); pk.ParameterName = keyParam; pk.Value = kv.Key ?? string.Empty; cmd.Parameters.Add(pk);
                            var pv = cmd.CreateParameter(); pv.ParameterName = valParam; pv.Value = kv.Value ?? (object)DBNull.Value; cmd.Parameters.Add(pv);
                            i++;
                        }
                    }

                    if (categories != null && categories.Count > 0)
                    {
                        // build list of params for IN clause
                        var catParams = new System.Collections.Generic.List<string>();
                        for (int i = 0; i < categories.Count; i++)
                        {
                            var pname = "@cat" + i;
                            catParams.Add(pname);
                            var p = cmd.CreateParameter(); p.ParameterName = pname; p.Value = categories[i] ?? string.Empty; cmd.Parameters.Add(p);
                        }

                        sql.Append(" AND EXISTS (SELECT 1 FROM [Instances].[ProductCategories] pc JOIN [Instances].[Categories] c ON pc.CategoryInstanceId = c.InstanceId WHERE pc.InstanceId = p.InstanceId AND c.Name IN (");
                        sql.Append(string.Join(",", catParams));
                        sql.Append(") )");
                    }

                    cmd.CommandText = sql.ToString();

                    using (var reader = await ((System.Data.Common.DbCommand)cmd).ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var id = reader.GetInt32(0);
                            var nm = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
                            results.Add(new ProductResponseDto { Id = id, Name = nm });
                        }
                    }
                }
            });

            return results;
        }

        public async Task<int> CreateProductAsync(ProductCreateDto dto)
        {
            int newInstanceId = 0;

            try
            {
                await _executor.ExecuteAsync(async (conn, tx) =>
                {
                // Insert into Instances.Products and return new InstanceId
                using (var cmd = conn.CreateCommand())
                {
                    cmd.Transaction = tx;
                    cmd.CommandText = "INSERT INTO [Instances].[Products] (Name, Description, ProductImageUris, ValidSkus) VALUES (@Name, @Description, @ImageUris, @ValidSkus); SELECT CAST(SCOPE_IDENTITY() AS INT);";

                    var pName = cmd.CreateParameter(); pName.ParameterName = "@Name"; pName.Value = dto.Name ?? string.Empty; cmd.Parameters.Add(pName);
                    var pDesc = cmd.CreateParameter(); pDesc.ParameterName = "@Description"; pDesc.Value = string.Empty; cmd.Parameters.Add(pDesc);
                    var pImgs = cmd.CreateParameter(); pImgs.ParameterName = "@ImageUris"; pImgs.Value = string.Empty; cmd.Parameters.Add(pImgs);

                    // Try to pick SKU from metadata if present
                    var sku = dto.Metadata != null && dto.Metadata.TryGetValue("SKU", out var s) ? s : string.Empty;
                    var pSkus = cmd.CreateParameter(); pSkus.ParameterName = "@ValidSkus"; pSkus.Value = sku ?? string.Empty; cmd.Parameters.Add(pSkus);

                    var result = await ((System.Data.Common.DbCommand)cmd).ExecuteScalarAsync();
                    newInstanceId = (result == null || result == DBNull.Value) ? 0 : Convert.ToInt32(result);
                }

                // Insert attributes
                if (dto.Metadata != null)
                {
                    foreach (var kv in dto.Metadata)
                    {
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.Transaction = tx;
                            cmd.CommandText = "INSERT INTO [Instances].[ProductAttributes] (InstanceId, [Key], [Value]) VALUES (@Id, @Key, @Value)";
                            var pId = cmd.CreateParameter(); pId.ParameterName = "@Id"; pId.Value = newInstanceId; cmd.Parameters.Add(pId);
                            var pKey = cmd.CreateParameter(); pKey.ParameterName = "@Key"; pKey.Value = kv.Key ?? string.Empty; cmd.Parameters.Add(pKey);
                            var pVal = cmd.CreateParameter(); pVal.ParameterName = "@Value"; pVal.Value = kv.Value ?? (object)DBNull.Value; cmd.Parameters.Add(pVal);
                            await ((System.Data.Common.DbCommand)cmd).ExecuteNonQueryAsync();
                        }
                    }
                }

                // Insert categories (ensure category exists, insert if not)
                if (dto.Categories != null)
                {
                    foreach (var cat in dto.Categories)
                    {
                        int categoryId = 0;
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.Transaction = tx;
                            cmd.CommandText = "SELECT InstanceId FROM [Instances].[Categories] WHERE [Name] = @Name";
                            var pName = cmd.CreateParameter(); pName.ParameterName = "@Name"; pName.Value = cat ?? string.Empty; cmd.Parameters.Add(pName);
                            var res = await ((System.Data.Common.DbCommand)cmd).ExecuteScalarAsync();
                            if (res != null && res != DBNull.Value)
                            {
                                categoryId = Convert.ToInt32(res);
                            }
                        }

                        if (categoryId == 0)
                        {
                            using (var cmd = conn.CreateCommand())
                            {
                                cmd.Transaction = tx;
                                cmd.CommandText = "INSERT INTO [Instances].[Categories] ([Name], [Description]) VALUES (@Name, @Desc); SELECT CAST(SCOPE_IDENTITY() AS INT);";
                                var pName = cmd.CreateParameter(); pName.ParameterName = "@Name"; pName.Value = cat ?? string.Empty; cmd.Parameters.Add(pName);
                                var pDesc = cmd.CreateParameter(); pDesc.ParameterName = "@Desc"; pDesc.Value = string.Empty; cmd.Parameters.Add(pDesc);
                                var res = await ((System.Data.Common.DbCommand)cmd).ExecuteScalarAsync();
                                categoryId = (res == null || res == DBNull.Value) ? 0 : Convert.ToInt32(res);
                            }
                        }

                        if (categoryId != 0)
                        {
                            using (var cmd = conn.CreateCommand())
                            {
                                cmd.Transaction = tx;
                                cmd.CommandText = "INSERT INTO [Instances].[ProductCategories] (InstanceId, CategoryInstanceId) VALUES (@InstanceId, @CategoryId)";
                                var pId = cmd.CreateParameter(); pId.ParameterName = "@InstanceId"; pId.Value = newInstanceId; cmd.Parameters.Add(pId);
                                var pCat = cmd.CreateParameter(); pCat.ParameterName = "@CategoryId"; pCat.Value = categoryId; cmd.Parameters.Add(pCat);
                                await ((System.Data.Common.DbCommand)cmd).ExecuteNonQueryAsync();
                            }
                        }
                    }
                }
                });
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to create product instance");
                throw;
            }

            return newInstanceId;
        }
    }
}
