using System;
using Sparcpoint.Inventory.Models;

namespace Interview.Web.DTOs
{
    /// <summary>
    /// EVAL: Response DTO for inventory transactions.
    /// Separates the API contract from the internal domain model,
    /// consistent with the ProductResponse pattern.
    /// </summary>
    public class InventoryTransactionResponse
    {
        public int TransactionId { get; set; }
        public int ProductInstanceId { get; set; }
        public decimal Quantity { get; set; }
        public DateTime StartedTimestamp { get; set; }
        public DateTime? CompletedTimestamp { get; set; }
        public string TypeCategory { get; set; }

        public static InventoryTransactionResponse FromTransaction(InventoryTransaction transaction)
        {
            return new InventoryTransactionResponse
            {
                TransactionId = transaction.TransactionId,
                ProductInstanceId = transaction.ProductInstanceId,
                Quantity = transaction.Quantity,
                StartedTimestamp = transaction.StartedTimestamp,
                CompletedTimestamp = transaction.CompletedTimestamp,
                TypeCategory = transaction.TypeCategory
            };
        }
    }
}
