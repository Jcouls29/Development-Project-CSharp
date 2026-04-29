using Dapper;
using Sparcpoint.Abstract.Repositories;
using Sparcpoint.Constants;
using Sparcpoint.Models.DTOs;
using Sparcpoint.Models.Entity;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Sparcpoint.Implementations.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ISqlExecutor _executor;

        public ProductRepository(ISqlExecutor executor)
        {
            _executor = executor;
        }
        public async Task<int> AddProductAsync(CreateProductRequestDto request)
        {
            return await _executor.ExecuteAsync<int>(async(connection, transaction) =>
            {
                string addprodsql = @"
                    INSERT INTO [Instances].[Products] (Name, Description, ProductImageUris, ValidSkus)
                    VALUES (@Name, @Description, @ImageUris, @Skus);
                    SELECT SCOPE_IDENTITY();";

                var prodID = Convert.ToInt32(await connection.ExecuteScalarAsync(addprodsql, new
                {
                    request.Name,
                    request.Description,
                    request.ImageUris,
                    request.Skus
                }, transaction));

                //EVAL: using predefined set of additional attributes for products as per recommendation, can be extended to support dynamic attributes in the future
                //Using
                var metadata = new List<ProductAttribute>();

                if (!string.IsNullOrEmpty(request.AdditionalSku))
                {
                    metadata.Add(new ProductAttribute
                    {
                        Key = ProductAttributeKeys.AdditionalSku,
                        Value = request.AdditionalSku
                    });
                }
                if (!string.IsNullOrEmpty(request.Color))
                {
                    metadata.Add(new ProductAttribute
                    {
                        Key = ProductAttributeKeys.Color,
                        Value = request.Color
                    });
                }
                if (request.Length.HasValue)
                {
                    metadata.Add(new ProductAttribute
                    {
                        Key = ProductAttributeKeys.Length,
                        Value = request.Length.Value.ToString()
                    });
                }
                if (!string.IsNullOrEmpty(request.PackageUnit))
                {
                    metadata.Add(new ProductAttribute
                    {
                        Key = ProductAttributeKeys.PackageUnit,
                        Value = request.PackageUnit
                    });
                }

                if (metadata.Any())
                {
                    string addmetasql = @"
                        INSERT INTO [Instances].[ProductAttributes] (InstanceId, [Key], [Value])
                        VALUES (@InstanceId, @Key, @Value);";
                    foreach (var attr in metadata)
                    {
                        await connection.ExecuteAsync(addmetasql, new
                        {
                            InstanceId = prodID,
                            attr.Key,
                            attr.Value
                        }, transaction);
                    }
                }
                if (request.CategoryIds != null && request.CategoryIds.Any())
                {
                    string catSql = "INSERT INTO [Instances].[ProductCategories] (InstanceId, CategoryInstanceId) VALUES (@InstanceId, @CategoryId)";
                    foreach (var catId in request.CategoryIds)
                    {
                        await connection.ExecuteAsync(catSql, new { InstanceId = prodID, CategoryId = catId }, transaction);
                    }
                }

                return prodID;
            });

        }

        //EVAL: Getting product by product ID
        public async Task<Product> GetProductByIdAsync(int productId)
        {
            return await _executor.ExecuteAsync<Product>(async (connection, transaction) =>
            {
                var sql = @"
                    SELECT  p.*
                    FROM[Instances].[Products] p
                    WHERE p.InstanceId = @Id";
                var product = await connection.QueryFirstOrDefaultAsync<Product>(sql, new { Id = productId }, transaction);
                if (product != null)
                { 
                    //get attributes
                    var attrSql = @"
                        SELECT [Key], [Value]
                        FROM [Instances].[ProductAttributes]
                        WHERE InstanceId = @ProductId";
                    var attributes = await connection.QueryAsync<ProductAttribute>(attrSql, new { ProductId = productId }, transaction);
                    product.Metadata = attributes.ToList();
                }
                return product;
            });
        }

        //EVAL: Search product based on genral details, meta data and all combination. if no search parameter supplied all products will be returned
        public async Task<IEnumerable<Product>> SearchProductAsync(SearchProductRequestDto request)
        {
            return await _executor.ExecuteAsync<IEnumerable<Product>>(async (connection, transaction) =>
            {
                var sql = @"
                    SELECT  p.*
                    FROM[Instances].[Products] p";
                //if search criteria icludes category
                if(request.CategoryIds != null && request.CategoryIds.Any())
                {
                    sql += @" INNER JOIN [Instances].[ProductCategories] pc ON p.InstanceId = pc.InstanceId";
                }
                //if search criteria includes attributes
                if (request.MetaData != null && request.MetaData.Any())
                {
                    sql += @" INNER JOIN [Instances].[ProductAttributes] pc ON p.InstanceID = pc.InstanceId";
                }
                sql += " WHERE 1=1";
                var parameters = new DynamicParameters();
                if (!string.IsNullOrEmpty(request.Name))
                {
                    sql += " AND p.Name LIKE @Name";
                    parameters.Add("Name", $"%{request.Name}%");
                }
                if (request.CategoryIds?.Any() == true)
                {
                    sql += " AND pc.CategoryInstanceId IN @CategoryIds";
                    parameters.Add("CategoryIds", request.CategoryIds);
                }
                if (!string.IsNullOrEmpty(request.ValidSku))
                {
                    sql += " AND p.ValidSkus LIKE @ValidSKU";
                    parameters.Add("ValidSKU", $"%{request.ValidSku}%");
                }
                if (!string.IsNullOrEmpty(request.ProductImageUri))
                {
                    sql += " AND p.ProductImageUris LIKE @ProductImageUris";
                    parameters.Add("ProductImageUris", $"%{request.ProductImageUri}%");
                }
                if (request.MetaData != null && request.MetaData.Count > 0)
                {
                    int i = 0;
                    foreach (var attr in request.MetaData)
                    {
                        sql += $@" 
                                AND EXISTS (
                                SELECT 1 
                                FROM [Instances].[ProductAttributes] pa
                                WHERE pa.InstanceId = p.InstanceId
                                AND pa.[Key] = @AttrKey{i} 
                                AND pa.Value = @AttrVal{i}
                                )";
                        parameters.Add($"AttrKey{i}", attr.Key);
                        parameters.Add($"AttrVal{i}", attr.Value);
                        i++;
                    }
                }
                return await connection.QueryAsync<Product>(sql, parameters, transaction);
               
            });
        }
    }
}
