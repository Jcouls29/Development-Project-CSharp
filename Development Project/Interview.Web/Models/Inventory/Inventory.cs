using System;

namespace Interview.Web.Models
{
    /// <summary>
    /// Read model for current on-hand quantity (computed from transactions, not stored in DB).
    /// </summary>
    public class Inventory
    {
        public Guid ProductId { get; set; }
        public decimal OnHandQuantity { get; set; }
    }
}
