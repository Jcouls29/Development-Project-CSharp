using Dapper;
using Interview.Web.Models;
using Microsoft.Extensions.Logging;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Web.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly ISqlExecutor _executor;
        private readonly IInventoryRule _rule;

        public InventoryService(ISqlExecutor executor, IInventoryRule rule)
        {
            _executor = executor ?? throw new ArgumentNullException(nameof(executor));
            _rule = rule ?? throw new ArgumentNullException(nameof(rule));
        }

        public async Task AddInventoryAsync(AddInventoryRequest request)
        {
            _rule.ValidateAdd(request);
            await _executor.ExecuteAsync(async (conn, trans) =>
            {
                // EVAL: Inventory stored as positive transaction for auditability
                await conn.ExecuteAsync(
                    @"INSERT INTO InventoryTransactions (ProductId, Quantity)
                  VALUES (@ProductId, @Quantity);",
                    new
                    {
                        request.ProductId,
                        request.Quantity
                    },
                    trans);
            });
        }

        public async Task RemoveInventoryAsync(RemoveInventoryRequest request)
        {
            await _executor.ExecuteAsync(async (conn, trans) =>
            {
                var currentInventory = await conn.ExecuteScalarAsync<int>(
                    @"SELECT ISNULL(SUM(Quantity), 0)
              FROM InventoryTransactions
              WHERE ProductId = @ProductId",
                    new { request.ProductId }, trans);

                _rule.ValidateRemove(request, currentInventory);

                await conn.ExecuteAsync(
                    @"INSERT INTO InventoryTransactions (ProductId, Quantity)
              VALUES (@ProductId, @Quantity);",
                    new
                    {
                        request.ProductId,
                        Quantity = -request.Quantity
                    }, trans);
            });
        }

        public async Task<InventoryCountResponse> GetInventoryCountAsync(int productId)
        {
            return await _executor.ExecuteAsync(async (conn, trans) =>
            {
                // EVAL: Aggregate transactions for inventory
                var total = await conn.ExecuteScalarAsync<int>(
                    @"SELECT ISNULL(SUM(Quantity), 0)
                  FROM InventoryTransactions
                  WHERE ProductId = @ProductId;",
                    new { ProductId = productId }, trans);

                return new InventoryCountResponse
                {
                    ProductId = productId,
                    TotalQuantity = total
                };
            });
        }
    }
}
