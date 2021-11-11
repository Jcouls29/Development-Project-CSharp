using System;
namespace Interview.Web.Entities
{
    public class InventoryTransaction
    {
		public int TransactionId { get; set; }
		public int ProductInstanceId { get; set; }
		public decimal Quantity { get; set; }
		public DateTime StartedTimestamp { get; set; }
		public DateTime? CompletedTimestamp { get; set; }
		public string TypeCategory { get; set; }

		public virtual Products Product { get; set; }

	}
}
