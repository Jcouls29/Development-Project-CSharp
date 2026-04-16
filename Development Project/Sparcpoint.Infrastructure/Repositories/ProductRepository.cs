using Dapper;
using Sparcpoint.Application.DTOs;
using Sparcpoint.Application.Interfaces;
using Sparcpoint.Domain.Models;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ISqlExecutor _sqlExecutor;

        public ProductRepository(ISqlExecutor sqlExecutor)
        {
            _sqlExecutor = sqlExecutor;
        }

        public async Task<int> InsertProduct(CreateProductRequest request)
        {
            return await _sqlExecutor.ExecuteAsync(async (conn, tx) =>
            {


                //insert prod
                var insertProductSql = @"
                    INSERT INTO instances.Products 
                    (name, description, productimageuris, validskus, createdtimestamp)
                    OUTPUT INSERTED.instanceid
                    VALUES (@Name, @Description, @ImageUris, @ValidSkus, GETUTCDATE());
                ";

                var productId = await conn.ExecuteScalarAsync<int>(
                    insertProductSql,
                    new
                    {
                        request.Name,
                        request.Description,
                        ImageUris = "",   
                        ValidSkus = ""
                    },
                    tx
                );

                //insert attributes
                if (request.Attributes != null && request.Attributes.Any())
                {
                    var insertAttributesSql = @"
                        INSERT INTO instances.ProductAttributes (instanceid, [key], [value])
                        VALUES (@InstanceId, @Key, @Value);
                    ";

                    foreach (var attr in request.Attributes)
                    {
                        await conn.ExecuteAsync(insertAttributesSql, new
                        {
                            InstanceId = productId,
                            Key = attr.Key,
                            Value = attr.Value
                        }, tx);
                    }


                }

                //insert categories
                if (request.CategoryIds != null && request.CategoryIds.Any())
                {
                    var insertCategorySql = @"
                        INSERT INTO instances.ProductCategories (instanceid, categoryinstanceid)
                        VALUES (@InstanceId, @CategoryId);
                    ";

                    foreach (var categoryId in request.CategoryIds)
                    {
                        await conn.ExecuteAsync(insertCategorySql, new
                        {
                            InstanceId = productId,
                            CategoryId = categoryId
                        }, tx);
                    }
                }

                return productId;
            });
        }

        public async Task<IEnumerable<Product>> SearchProducts(ProductSearchRequest request)
        {
            return await _sqlExecutor.ExecuteAsync(async (conn, tx) =>
            {
                var sql = @"
                    SELECT DISTINCT p.*
                    FROM instances.Products p
                ";

                var whereClauses = new List<string>();
                var parameters = new DynamicParameters();

                if (request.CategoryIds != null && request.CategoryIds.Any())
                {
                    sql += @"
                        JOIN instances.ProductCategories pc 
                        ON p.instanceid = pc.instanceid
                     ";

                    whereClauses.Add("pc.categoryinstanceid IN @CategoryIds");
                    parameters.Add("CategoryIds", request.CategoryIds);
                }

                if (request.Attributes != null && request.Attributes.Any())
                {
                    int i = 0;

                    foreach (var attr in request.Attributes)
                    {
                        var keyParam = $"Key{i}";
                        var valueParam = $"Value{i}";

                        whereClauses.Add($@"
                            EXISTS (
                                SELECT 1 
                                FROM instances.ProductAttributes pa
                                WHERE pa.instanceid = p.instanceid
                                AND pa.[key] = @{keyParam}
                                AND pa.[value] = @{valueParam}
                            )
                        ");

                        parameters.Add(keyParam, attr.Key);
                        parameters.Add(valueParam, attr.Value);

                        i++;
                    }

                    
                }

                if (whereClauses.Any())
                {
                    sql += " WHERE " + string.Join(" AND ", whereClauses);
                }

                var result = await conn.QueryAsync<Product>(sql, parameters, tx);

                return result;
            });
        }
    }
}
