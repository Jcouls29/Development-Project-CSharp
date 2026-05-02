using System;
using System.Collections.Generic;

namespace Interview.Web.Contracts
{
    public class InventoryAdjustmentRequest
    {
        public string TypeCategory { get; set; }
        public List<InventoryAdjustmentItemRequest> Items { get; set; } = new List<InventoryAdjustmentItemRequest>();
    }

    public class InventoryAdjustmentItemRequest
    {
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
    }

    public class InventoryTransactionResponse
    {
        public int TransactionId { get; set; }
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
        public DateTime CompletedTimestamp { get; set; }
        public string TypeCategory { get; set; }
    }

    public class InventoryCountRequest
    {
        public int? ProductId { get; set; }
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
        public List<int> CategoryIds { get; set; } = new List<int>();
        public bool MatchAllCategories { get; set; }
        public bool IncludeDescendantCategories { get; set; } = true;
    }

    public class InventoryCountItemResponse
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Quantity { get; set; }
    }

    public class InventoryCountResponse
    {
        public decimal TotalQuantity { get; set; }
        public List<InventoryCountItemResponse> Products { get; set; } = new List<InventoryCountItemResponse>();
    }
}
