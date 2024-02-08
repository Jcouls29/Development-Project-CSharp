using Microsoft.AspNetCore.Mvc;
using Sparcpoint.SqlServer.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sparcpoint.SqlServer.Abstractions.Data;
using System;
using System.Data.SqlClient;
using System.Data;
using Dapper;


namespace Interview.Web.Controllers
{
    /// <summary>
    /// Handles CRUD operations for "transaction" items and inventory.
    /// </summary>
    [ApiController]
    [Route("api/v1/transactions")]
    public class TransactionsController : Controller
    {
        // I am not really sure how Inventory works. Where is the table for [Product, Quantity]?
        // Is this like a ledger only? We don't keep track of the value, just the transactions, so it's more complex to ensure we do not subtract more inventory than exists?
        // I'm not sure how this would work unless ProductInstanceId is primary or we could use negative quantities.

        private readonly ISqlExecutor _sqlExecutor;
        public TransactionsController(ISqlExecutor sqlExecutor)
        {
            _sqlExecutor = sqlExecutor ?? throw new ArgumentNullException(nameof(sqlExecutor));
        }

        [HttpGet("/inventory/create")]
        public IActionResult CreateInventoryTransaction(InventoryTransaction inventoryTransaction)
        {
            try
            {
                int numberOfRowsAffected = _sqlExecutor.Execute<int>(
                    (connection, transaction) =>
                    {
                        string query = @"INSERT INTO [master].[Transactions].[InventoryTransactions] 
                             ([ProductInstanceId], [Quantity], [StartedTimestamp], [CompletedTimestamp], [TypeCategory])
                             VALUES (@ProductInstanceId, @Quantity, @StartedTimestamp, @CompletedTimestamp, @TypeCategory)";

                        return connection.Execute(query, inventoryTransaction, transaction);
                    }
                );

                if (numberOfRowsAffected > 0)
                {
                    // If rows were affected, return Ok
                    return Ok();
                }
                else
                {
                    // If no rows were affected, return an appropriate response (e.g., NotFound or BadRequest)
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                return StatusCode(500, $"An error occurred while adding the product to inventory: {ex.Message}");
            }
        }

        [HttpGet("/inventory/add")]
        public IActionResult AddInventory(string productInstanceId)
        {
            try
            {
                int numberOfRowsAffected = _sqlExecutor.Execute<int>(
                    (connection, transaction) =>
                    {
                        string query = @"UPDATE [master].[Transactions].[InventoryTransactions]
                             SET Quantity=(Quantity+1)
                             WHERE ProductInstanceId=@ProductInstanceId";

                        return connection.Execute(query, new { ProductInstanceId = productInstanceId }, transaction);
                    }
                );

                if (numberOfRowsAffected > 0)
                {
                    // If rows were affected, return Ok
                    return Ok();
                }
                else
                {
                    // If no rows were affected, return an appropriate response (e.g., NotFound or BadRequest)
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                return StatusCode(500, $"An error occurred while adding the product to inventory: {ex.Message}");
            }
        }

        [HttpGet("/inventory/count")]
        public IActionResult GetInventoryCount(string productInstanceId)
        {
            try
            {
                var quantityResult = _sqlExecutor.Execute<decimal>(
                    (connection, transaction) =>
                    {
                        string query = @"SELECT top 1 [Quantity]
                             FROM [master].[Transactions].[InventoryTransactions]
                             WHERE ProductInstanceId=@ProductInstanceId";

                        //return connection.Execute(query, new { ProductInstanceId = productInstanceId }, transaction);
                        return connection.QueryFirstOrDefault<decimal>(query, new { ProductInstanceId = productInstanceId }, transaction);
                    }
                );

                return Ok(quantityResult);
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                return StatusCode(500, $"An error occurred while adding the product to inventory: {ex.Message}");
            }
        }

        [HttpGet("/inventory/remove")]
        public IActionResult RemoveInventory(string productInstanceId)
        {
            try
            {
                int numberOfRowsAffected = _sqlExecutor.Execute<int>(
                    (connection, transaction) =>
                    {
                        string query = @"UPDATE [master].[Transactions].[InventoryTransactions]
                             SET Quantity=(Quantity-1)
                             WHERE ProductInstanceId=@ProductInstanceId";

                        return connection.Execute(query, new { ProductInstanceId = productInstanceId }, transaction);
                    }
                );

                if (numberOfRowsAffected > 0)
                {
                    // If rows were affected, return Ok
                    return Ok();
                }
                else
                {
                    // If no rows were affected, return an appropriate response (e.g., NotFound or BadRequest)
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                return StatusCode(500, $"An error occurred while adding the product to inventory: {ex.Message}");
            }
        }
    }
}
