using Interview.Service.Models;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Collections.Generic;

namespace Interview.Service.Inventory
{
    public class InventoryRepo : IInventoryRepo
    {
        private readonly ISqlExecutor _sqlExecutor;

        public InventoryRepo(ISqlExecutor sqlExecutor)
        {
            _sqlExecutor = sqlExecutor;
        }

        public List<InventoryTransaction> AddInventory(List<Product> products)
        {
            // EVAL: For each product in the list
            // EVAL:    Create an INSERT statement
            // EVAL:    Execute command
            // EVAL:    Create InventoryTransaction object to be returned
            return new List<InventoryTransaction>();
        }

        public void DeleteInventory(List<int> productId)
        {
            // EVAL: For each id int the list
            // EVAL:     Create a DELETE statement
            // EVAL:     Execute Command
            // EVAL:     Log to db that inventory id was successfully deleted??
        }

        public int GetInventoryCount(ProductFilterParams parms)
        {
            // EVAL: Build SQL query based on parms values
            // EVAL: Execute SQL command using SqlExecutor
            // EVAL: Map result (if necessary)
            // EVAL: Return result
            throw new NotImplementedException();
        }
    }
}
