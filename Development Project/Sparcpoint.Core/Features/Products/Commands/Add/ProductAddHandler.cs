using Dapper;
using MediatR;
using Sparcpoint.Models;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sparcpoint.Features.Products.Commands.Add
{
    public class ProductAddHandler : IRequestHandler<ProductAddCommand, ProductAddResponse>
    {
        private readonly ISqlExecutor _sqlExecutor;

        public ProductAddHandler(ISqlExecutor sqlExecutor)
        {
            _sqlExecutor = sqlExecutor;
        }

        public async Task<ProductAddResponse> Handle(ProductAddCommand dto, CancellationToken cancellationToken)
        {
            var product = await _sqlExecutor.ExecuteAsync<Product>(async (connection, transaction) =>
            {
                try
                {
                    var sql = @"
                    INSERT INTO Instances.Products (Name, Description, ProductImageUris, ValidSkus)
                    VALUES (@Name, @Description, @ProductImageUris, @ValidSkus);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                    var id = await connection.ExecuteScalarAsync<int>(
                        sql,
                        new
                        {
                            dto.Name,
                            dto.Description,
                            dto.ProductImageUris,
                            dto.ValidSkus
                        },
                        transaction: transaction
                    );

                    var result = await connection.QueryFirstAsync<Product>(
                        "SELECT InstanceId, Name, Description, ProductImageUris, ValidSkus, CreatedTimestamp FROM Instances.Products WHERE InstanceId = @Id",
                        new { Id = id },
                        transaction: transaction
                    );

                    // ProductCategories
                    await connection.ExecuteAsync(
                        "INSERT INTO Instances.ProductCategories (InstanceId, CategoryInstanceId) VALUES (@InstanceId, @CategoryInstanceId)",
                        new ProductCategory { InstanceId = id, CategoryInstanceId = dto.CategoryId },
                        transaction: transaction
                    );

                    // ProductAttributes
                    if (dto.Attributes != null)
                    {
                        foreach (var attr in dto.Attributes)
                        {
                            await connection.ExecuteAsync(
                                "INSERT INTO Instances.ProductAttributes (InstanceId, [Key], [Value]) VALUES (@InstanceId, @Key, @Value)",
                                new ProductAttribute { InstanceId = id, Key = attr.Key, Value = attr.Value },
                                transaction: transaction
                            );
                        }
                    }

                    return result;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    connection.Close();
                    throw;
                }
                
            });

            return new ProductAddResponse()
            {
                InstanceId = product.InstanceId,
                Name = product.Name,
                Description = product.Description,
                ProductImageUris = product.ProductImageUris,
                ValidSkus = product.ValidSkus,
                CreatedTimestamp = product.CreatedTimestamp
            };
        }
    }
}
