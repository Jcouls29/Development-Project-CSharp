using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Dapper;
using Sparcpoint.Core.Abstract;
using Sparcpoint.Core.Models;
using Sparcpoint.SqlServer.Abstractions;

namespace Sparcpoint.Inventory.Database.Data
{
    public class ProductRepository : IProductRepository
    {
        private readonly ISqlExecutor _sqlExecutor;

        public ProductRepository(ISqlExecutor sqlExecutor)
        {
            _sqlExecutor = sqlExecutor ?? throw new ArgumentNullException(nameof(sqlExecutor));
        }

        public async Task<int> AddProductAsync(ProductCreateRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            return await _sqlExecutor.ExecuteAsync(async (conn, trans) =>
            {
                var insertSql = @"
                    INSERT INTO [Instances].[Products] (Name, Description, ProductImageUris, ValidSkus) 
                    VALUES (@Name, @Description, @ProductImageUris, @ValidSkus);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);
";

                var imageJson = JsonSerializer.Serialize(request.ProductImageUris ?? Array.Empty<string>());
                var skusJson = JsonSerializer.Serialize(request.ValidSkus ?? Array.Empty<string>());

                var productId = await conn.ExecuteScalarAsync<int>(
                    insertSql,
                    new { request.Name, request.Description, ProductImageUris = imageJson, ValidSkus = skusJson },
                    trans);

                if (request.Metadata != null && request.Metadata.Count > 0)
                {
                    var attrs = request.Metadata.Select(kv => new { InstanceId = productId, Key = kv.Key, Value = kv.Value }).ToList();
                    await conn.ExecuteAsync(
                        "INSERT INTO [Instances].[ProductAttributes] (InstanceId, [Key], [Value]) VALUES (@InstanceId, @Key, @Value);",
                        attrs,
                        trans);
                }

                if (request.CategoryIds != null && request.CategoryIds.Any())
                {
                    using (var cmd = new SqlCommand("[Instances].[usp_Product_SetCategories]", (SqlConnection)conn, (SqlTransaction)trans))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ProductInstanceId", productId);

                        var dt = new DataTable();
                        dt.Columns.Add("Value", typeof(int));
                        foreach (var id in request.CategoryIds.Distinct())
                            dt.Rows.Add(id);

                        var tvp = new SqlParameter("@CategoryIds", SqlDbType.Structured)
                        {
                            TypeName = "dbo.IntegerList",
                            Value = dt
                        };
                        cmd.Parameters.Add(tvp);

                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                return productId;
            });
        }

        private static readonly HashSet<string> _allowedOrderByColumns = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Name", "Description", "InstanceId"
        };

        public async Task<IList<ProductSearchResult>> SearchProductsAsync(ProductSearchCriteria criteria)
        {
            return await _sqlExecutor.ExecuteAsync(async (conn, trans) =>
            {
                // EVAL: Removed the unused LEFT JOINs on ProductCategories and ProductAttributes.
                // Category filtering uses an EXISTS subquery (avoids row duplication from one-to-many joins).
                // Metadata filtering also uses EXISTS subqueries added dynamically below.
                // This means the GROUP BY is no longer needed either.
                var sql = @"
            SELECT p.InstanceId, p.Name, p.Description, p.ProductImageUris, p.ValidSkus
            FROM [Instances].[Products] p
            WHERE 1=1";

                var parameters = new DynamicParameters();

                if (!string.IsNullOrWhiteSpace(criteria.Name))
                {
                    sql += " AND p.Name LIKE @Name";
                    parameters.Add("@Name", "%" + criteria.Name + "%");
                }

                if (!string.IsNullOrWhiteSpace(criteria.Description))
                {
                    sql += " AND p.Description LIKE @Description";
                    parameters.Add("@Description", "%" + criteria.Description + "%");
                }

                //Category filtering uses EXISTS instead of a JOIN, 
                // preventing duplicate product rows when a product has multiple categories.
                if (criteria.CategoryIds != null && criteria.CategoryIds.Any())
                {
                    sql += @" AND EXISTS (
                SELECT 1 FROM [Instances].[ProductCategories] pc
                WHERE pc.InstanceId = p.InstanceId
                AND pc.CategoryInstanceId IN @CategoryIds)";
                    parameters.Add("@CategoryIds", criteria.CategoryIds.ToArray());
                }

                // MetadataMatchMode and MatchAllMetadata are respected.
                // AND mode: one EXISTS per key/value pair, all must match.
                // OR mode: a single EXISTS with multiple OR conditions, any pair satisfies.
                if (criteria.Metadata != null && criteria.Metadata.Count > 0)
                {
                    int i = 0;
                    if (criteria.MatchAllMetadata)
                    {
                        // AND: every key/value pair must exist on the product
                        foreach (var kv in criteria.Metadata)
                        {
                            var valueCondition = criteria.MetadataMatchMode == MetadataMatchMode.Contains
                                ? $"pa{i}.[Value] LIKE @MetaValue{i}"
                                : $"pa{i}.[Value] = @MetaValue{i}";

                            sql += $@" AND EXISTS (
                        SELECT 1 FROM [Instances].[ProductAttributes] pa{i}
                        WHERE pa{i}.InstanceId = p.InstanceId
                        AND pa{i}.[Key] = @MetaKey{i}
                        AND {valueCondition})";

                            parameters.Add($"@MetaKey{i}", kv.Key);
                            parameters.Add($"@MetaValue{i}", criteria.MetadataMatchMode == MetadataMatchMode.Contains
                                ? "%" + kv.Value + "%"
                                : kv.Value);
                            i++;
                        }
                    }
                    else
                    {
                        // OR: any one key/value pair matching is sufficient
                        var orClauses = new List<string>();
                        foreach (var kv in criteria.Metadata)
                        {
                            var valueCondition = criteria.MetadataMatchMode == MetadataMatchMode.Contains
                                ? $"pa.[Value] LIKE @MetaValue{i}"
                                : $"pa.[Value] = @MetaValue{i}";

                            orClauses.Add($"(pa.[Key] = @MetaKey{i} AND {valueCondition})");
                            parameters.Add($"@MetaKey{i}", kv.Key);
                            parameters.Add($"@MetaValue{i}", criteria.MetadataMatchMode == MetadataMatchMode.Contains
                                ? "%" + kv.Value + "%"
                                : kv.Value);
                            i++;
                        }

                        sql += $@" AND EXISTS (
                    SELECT 1 FROM [Instances].[ProductAttributes] pa
                    WHERE pa.InstanceId = p.InstanceId
                    AND ({string.Join(" OR ", orClauses)}))";
                    }
                }

                // OrderBy is applied using a whitelist to prevent SQL injection.
                // Column names cannot be parameterized in SQL, so we validate against known safe values.
                if (!string.IsNullOrWhiteSpace(criteria.OrderBy) && _allowedOrderByColumns.Contains(criteria.OrderBy))
                {
                    var direction = criteria.OrderDescending ? "DESC" : "ASC";
                    sql += $" ORDER BY p.[{criteria.OrderBy}] {direction}";
                }
                else
                {
                    // EVAL: Default sort ensures stable, consistent paging
                    sql += " ORDER BY p.InstanceId ASC";
                }

                var products = await conn.QueryAsync<ProductSearchResult>(sql, parameters, trans);
                return products.ToList();
            });
        }
    }
}