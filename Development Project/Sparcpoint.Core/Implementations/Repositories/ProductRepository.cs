using Dapper;
using Sparcpoint.Abstract.Repositories;
using Sparcpoint.DTOs;
using Sparcpoint.SqlServer.Abstractions;
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
            int productId = 0;

            // EVAL: Aseguramos atomicidad
            await _executor.ExecuteAsync(async (connection, transaction) =>
            {
                // 1. Insert Product
                string sql = @"
                INSERT INTO [Instances].[Products] (Name, Description, ProductImageUris, ValidSkus) 
                VALUES (@Name, @Description, @ImageUris, @Skus); 
                SELECT CAST(SCOPE_IDENTITY() as int);";

                productId = await connection.QuerySingleAsync<int>(sql,
                    new
                    {
                        request.Name,
                        request.Description,
                        request.ImageUris,
                        request.Skus
                    }, transaction);

                // 2. Insert Attributes (Metadata)
                if (request.Attributes != null && request.Attributes.Any())
                {
                    string attrSql = @"
                    INSERT INTO [Instances].[ProductAttributes] (InstanceId, [Key], [Value]) 
                    VALUES (@productId, @Key, @Value)";

                    foreach (var attr in request.Attributes)
                    {
                        await connection.ExecuteAsync(attrSql,
                            new { productId, attr.Key, attr.Value }, transaction);
                    }
                }

                // 3. Insert Categories
                if (request.CategoryIds != null && request.CategoryIds.Any())
                {
                    string catSql = @"
                    INSERT INTO [Instances].[ProductCategories] (InstanceId, CategoryInstanceId) 
                    VALUES (@productId, @CategoryInstanceId)";

                    foreach (var catId in request.CategoryIds)
                    {
                        await connection.ExecuteAsync(catSql,
                            new { productId, CategoryInstanceId = catId }, transaction);
                    }
                }
            });

            return productId;
        }
    }
}
