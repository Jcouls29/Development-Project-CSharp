using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Interview.Web.Models.Requests
{
    public class AdjustInventoryItem
    {
        [Required]
        public int ProductInstanceId { get; set; }

        /// <summary>Positive adds stock, negative removes it. Cannot be zero.</summary>
        [Required]
        public decimal Quantity { get; set; }

        [StringLength(32)]
        public string TypeCategory { get; set; }
    }

    public class AdjustInventoryRequest
    {
        public IList<AdjustInventoryItem> Adjustments { get; set; } = new List<AdjustInventoryItem>();
    }

    public class GetInventoryCountsRequest
    {
        public int? ProductInstanceId { get; set; }
        public IList<AttributePair> AttributeFilters { get; set; } = new List<AttributePair>();
        public IList<int> CategoryIds { get; set; } = new List<int>();
        public bool IncludeReverted { get; set; } = false;
    }
}
