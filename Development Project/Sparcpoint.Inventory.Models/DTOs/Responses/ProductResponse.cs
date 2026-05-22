using System;
using System.Collections.Generic;

namespace Sparcpoint.Inventory.Models.DTOs.Responses
{
    /// <summary>
    /// Response model for product data.
    /// </summary>
    public class ProductResponse
    {
        public int InstanceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<string> ProductImageUris { get; set; } = new List<string>();
        public List<string> ValidSkus { get; set; } = new List<string>();
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
        public List<int> CategoryIds { get; set; } = new List<int>();
        public DateTime CreatedTimestamp { get; set; }
    }
}