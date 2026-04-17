using Dapper;
using Sparcpoint.Abstract.Repositories;
using Sparcpoint.DTOs;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Linq;
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
            return await _executor.ExecuteAsync<int>(async (connection, transaction) =>
            {
                string prodSql = @"
                    INSERT INTO [Instances].[Products] (Name, Description, ProductImageUris, ValidSkus)
                    VALUES (@Name, @Description, @ImageUris, @Skus);
                    SELECT SCOPE_IDENTITY();";

                var productId = Convert.ToInt32(await connection.ExecuteScalarAsync(prodSql, new
                {
                    request.Name,
                    request.Description,
                    request.ImageUris,
                    request.Skus
                }, transaction));

                if (request.Attributes != null && request.Attributes.Any())
                {
                    string attrSql = "INSERT INTO [Instances].[ProductAttributes] (InstanceId, [Key], [Value]) VALUES (@InstanceId, @Key, @Value)";
                    foreach (var attr in request.Attributes)
                    {
                        await connection.ExecuteAsync(attrSql, new { InstanceId = productId, attr.Key, attr.Value }, transaction);
                    }
                }

                if (request.CategoryIds != null && request.CategoryIds.Any())
                {
                    string catSql = "INSERT INTO [Instances].[ProductCategories] (InstanceId, CategoryId) VALUES (@InstanceId, @CategoryId)";
                    foreach (var catId in request.CategoryIds)
                    {
                        await connection.ExecuteAsync(catSql, new { InstanceId = productId, CategoryId = catId }, transaction);
                    }
                }

                return productId;
            });
        }

    }
}
