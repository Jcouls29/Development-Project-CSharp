using System;
using System.Collections.Generic;

namespace Interview.Web.Models.Responses
{
    /// <summary>
    /// API response for inventory transaction operations.
    /// </summary>
    public class InventoryTransactionResponse
    {
        /// <summary>
        /// The transactions that were created or modified.
        /// </summary>
        public List<TransactionDetail> Transactions { get; set; } = new List<TransactionDetail>();
    }

    /// <summary>
    /// Details of a single inventory transaction.
    /// </summary>
    // EVAL: Returning transaction IDs in the response enables the "undo" workflow.
    // Client adds inventory -> gets back TransactionId -> can later DELETE that transaction.
    public class TransactionDetail
    {
        /// <summary>
        /// Unique transaction ID. Use this to undo the transaction.
        /// </summary>
        public int TransactionId { get; set; }

        /// <summary>
        /// The product this transaction applies to.
        /// </summary>
        public int ProductInstanceId { get; set; }

        /// <summary>
        /// Quantity added (positive) or removed (negative).
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// Transaction type classification.
        /// </summary>
        public string TypeCategory { get; set; }

        /// <summary>
        /// When the transaction was created.
        /// </summary>
        public DateTime StartedTimestamp { get; set; }

        /// <summary>
        /// Whether this transaction is still active (not undone).
        /// </summary>
        public bool IsActive { get; set; }
    }
}
