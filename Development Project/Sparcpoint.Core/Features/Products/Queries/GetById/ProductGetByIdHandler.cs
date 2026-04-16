using Dapper;
using MediatR;
using Sparcpoint.SqlServer.Abstractions;
using System.Threading;
using System.Threading.Tasks;

namespace Sparcpoint.Features.Products.Queries.GetById
{
    public class ProductGetByIdHandler : IRequestHandler<ProductGetByIdQuery, ProductGetByIdResponse>
    {
        private readonly ISqlExecutor _sqlExecutor;

        public ProductGetByIdHandler(ISqlExecutor sqlExecutor)
        {
            _sqlExecutor = sqlExecutor;
        }

        public async Task<ProductGetByIdResponse> Handle(ProductGetByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await _sqlExecutor.ExecuteAsync<ProductGetByIdResponse>(async (connection, transaction) =>
            {
                var sql = @"
                    SELECT InstanceId, Name, Description, ProductImageUris, ValidSkus, CreatedTimestamp
                    FROM Instances.Products
                    WHERE InstanceId = @Id;";

                return await connection.QueryFirstOrDefaultAsync<ProductGetByIdResponse>(sql, new { request.Id }, transaction);
            });

            return product;
        }
    }
}
