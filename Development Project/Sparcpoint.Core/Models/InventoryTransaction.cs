using System;

namespace Sparcpoint.Core.Models
{
    // EVAL: Domain Entity - Represents inventory transactions in the ledger system
    // EVAL: Ledger pattern - immutable transaction records for audit trail and undo capability
    public class InventoryTransaction
    {
        public int TransactionId { get; private set; }
        public int ProductInstanceId { get; private set; }
        public decimal Quantity { get; private set; } // Positive for additions, negative for removals
        public DateTime StartedTimestamp { get; private set; }
        public DateTime? CompletedTimestamp { get; private set; }
        public string TypeCategory { get; private set; } // "ADD", "REMOVE", "UNDO"

        private InventoryTransaction() { }

        // EVAL: Factory method for creating new transactions
        public static InventoryTransaction Create(int productInstanceId, decimal quantity, string typeCategory = null)
        {
            // EVAL: Business rule - quantity cannot be zero
            if (quantity == 0) throw new ArgumentException("Transaction quantity cannot be zero", nameof(quantity));

            // EVAL: Business rule - reasonable type category length
            if (!string.IsNullOrEmpty(typeCategory) && typeCategory.Length > 32)
                throw new ArgumentException("Type category cannot exceed 32 characters", nameof(typeCategory));

            return new InventoryTransaction
            {
                ProductInstanceId = productInstanceId,
                Quantity = quantity,
                TypeCategory = typeCategory,
                StartedTimestamp = DateTime.UtcNow
            };
        }

        internal static InventoryTransaction Load(int transactionId, int productInstanceId, decimal quantity, DateTime startedTimestamp, DateTime? completedTimestamp, string typeCategory)
        {
            return new InventoryTransaction
            {
                TransactionId = transactionId,
                ProductInstanceId = productInstanceId,
                Quantity = quantity,
                StartedTimestamp = startedTimestamp,
                CompletedTimestamp = completedTimestamp,
                TypeCategory = typeCategory
            };
        }

        // EVAL: Method to mark transaction as completed
        public void Complete()
        {
            if (CompletedTimestamp.HasValue)
                throw new InvalidOperationException("Transaction is already completed");

            CompletedTimestamp = DateTime.UtcNow;
        }

        // EVAL: Internal methods for repository hydration
        internal void SetTransactionId(int transactionId) => TransactionId = transactionId;
        internal void SetCompletedTimestamp(DateTime? timestamp) => CompletedTimestamp = timestamp;
    }
}