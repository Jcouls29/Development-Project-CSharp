using Dapper;
using Dapper.Contrib.Extensions;
using Sparcpoint.Application.Abstracts;
using Sparcpoint.Domain.Instance.Entities;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Application.Implementations
{
    public class InventoryService : IInventoryService
    {
        private readonly SqlServerQueryProvider _sqlServerQueryProvider;
        private readonly IQueryService _queryService;
        private readonly ISqlExecutor _sqlExecutor;
        private readonly string TableName = "[Transactions].[InventoryTransactions]";
        private readonly IProductService _productService;

        public InventoryService(SqlServerQueryProvider sqlServerQueryProvider, IQueryService queryService, ISqlExecutor sqlExecutor, IProductService productService)
        {
            _sqlServerQueryProvider = sqlServerQueryProvider;
            _queryService = queryService;
            _sqlExecutor = sqlExecutor;
            _productService = productService;
        }
        public Task<int> AddInventoryTransaction(InventoryTransaction transaction)
        {
            throw new NotImplementedException();
        }

        public  async Task<int> AddInventoryTransactionAsync(InventoryTransaction inventoryTransaction)
        {
            return await _sqlExecutor.ExecuteAsync(async (connection, transaction) => {
                inventoryTransaction.StartedTimestamp = DateTime.UtcNow;
                inventoryTransaction.CompletedTimestamp = DateTime.UtcNow;
                var inventoryTransactionId = await connection.InsertAsync(inventoryTransaction, transaction);
                return inventoryTransactionId;

            });
        }

        public Task<List<InventoryTransaction>> GetAllInventoryTransactions()
        {
            throw new NotImplementedException();
        }

        public Task<List<InventoryTransaction>> GetAllInventoryTransactionsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<int> GetInventoryForProduct(int productId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Product>> SearchInventoryAsync(string keyword)
        {
            List<Product> products = await _productService.SearchProductsAsync(keyword);
            foreach (var product in products)
            {
                product.Inventory = await GetInventoryForProductAsync(product.InstanceId);
            }
            return products;

        }

        public async Task<int> GetInventoryForProductAsync(int productId)
        {
            string sql = String.Format("SELECT Sum([Quantity]) FROM [Sparcpoint.Inventory.Database].[Transactions].[InventoryTransactions] where ProductInstanceId = {0}", productId);
            return await _queryService.ExecuteScalarAsync<int>(sql);
        }

        public Task<int> RollbackInventoryTransaction(int transactionId)
        {
            throw new NotImplementedException();
        }

        public async Task<int> RollbackInventoryTransactionAsync(int inventoryTransactionId)
        {
            return await _sqlExecutor.ExecuteAsync(async (connection, transaction) => {
                string sql = "DELETE FROM [Transactions].[InventoryTransactions] WHERE TransactionId = @TransactionId;";
                var affectedRows = connection.Execute(sql, new { TransactionId = inventoryTransactionId }, transaction);
                return affectedRows;

            });
        }
    }
}
