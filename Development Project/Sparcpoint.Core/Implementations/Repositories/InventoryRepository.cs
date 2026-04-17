using Dapper;
using Sparcpoint.Abstract.Repositories;
using Sparcpoint.DTOs;
using Sparcpoint.SqlServer.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sparcpoint.Implementations.Repositories
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly ISqlExecutor _executor;

        public InventoryRepository(ISqlExecutor executor)
        {
            _executor = executor;
        }

        public async Task UpdateInventoryAsync(List<UpdateInventoryRequestDto> request)
        {
            await _executor.ExecuteAsync(async (connection, transaction) =>
            {
                string sql = @"
                    INSERT INTO [Transactions].[InventoryTransactions] 
                    (ProductInstanceId, Quantity, CompletedTimestamp, TypeCategory)
                    VALUES (@ProductInstanceId, @Quantity, SYSUTCDATETIME(), @TypeCategory)";

                foreach (var update in request)
                {
                    if (update.Quantity == 0) continue;

                    await connection.ExecuteAsync(sql, new
                    {
                        ProductInstanceId = update.ProductId,
                        update.Quantity,
                        update.TypeCategory
                    }, transaction);
                }
            });
        }
    }
}
