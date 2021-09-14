using Dapper;
using Interview.Web.Model;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Interview.Web.Repository
{
    public class InventoryTransactionsRepository : IInventoryTransactionsRepository
    {
        private readonly ISqlExecutor _sqlExecutor;
        private string _tableName = "Transactions.InventoryTransactions";

        public InventoryTransactionsRepository(ISqlExecutor sqlExecutor)
        {
            _sqlExecutor = sqlExecutor;
        }




        #region "Product Inventory"

        /// <summary>
        /// get the  count of the products or type catgory from the Inventory
        /// </summary>
        /// <param name="inventoryTransactions"></param>
        /// <returns></returns>

        public Task<int> GetProductInventoryCount(InventoryTransactions inventoryTransactions)
        {
            var query = new StringBuilder($"SELECT sum(QUANTITY) FROM {_tableName} where ");

            //dynamically forming query based on filter criteria
            if (inventoryTransactions.ProductInstanceId > 0)
                query.Append($"ProductInstanceId =@ProductInstanceId");

            if (!string.IsNullOrEmpty(inventoryTransactions.TypeCategory))
                query.Append($"TypeCategory =@TypeCategory");

            return _sqlExecutor.ExecuteAsync(async (conn, trn) =>
            {

                var result = (await conn.QueryFirstAsync<int>(query.ToString(), inventoryTransactions, trn));
                return result;
            });
        }

        /// <summary>
        /// Add product to the inventory
        /// </summary>Add product to the inventory
        /// <param name="item"></param>
        /// <returns>Void</returns>
        public Task Add(InventoryTransactions item)
        {

            var query = $"insert into {_tableName} values (@ProductInstanceId,@Quantity,@StartedTimestamp,@CompletedTimestamp,@TypeCategory)";

            return _sqlExecutor.ExecuteAsync(async (conn, trn) =>
            {

                var result = (await conn.ExecuteAsync(query, item, trn));
                return result;
            });
        }


        /// <summary>
        /// Get the count 
        /// </summary>
        /// <returns></returns>
        public Task<int> GetCount()
        {
            return _sqlExecutor.ExecuteAsync(async (conn, trn) =>
            {
                var result = (await conn.QueryFirstAsync<int>($"SELECT Count(1) FROM {_tableName}", null, trn));
                return result;
            });
        }

        /// <summary>
        /// Remove products froom Inventory
        /// </summary>remove product form the inventory
        /// <param name="item"></param>
        /// <returns>true-Success false - failure</returns>

        public Task<bool> Remove(InventoryTransactions item)
        {
            var query = $"DELETE {_tableName} WHERE ProductInstanceId = @ProductInstanceId";
            return _sqlExecutor.ExecuteAsync(async (conn, trn) =>
            {
                var result = (await conn.ExecuteAsync(query, item, trn));
                return result > 0;
            });
        }


        public IEnumerator<InventoryTransactions> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public Task Clear()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        internal virtual dynamic Mapping(InventoryTransactions item)
        {
            return item;
        }

        #endregion
    }
}
