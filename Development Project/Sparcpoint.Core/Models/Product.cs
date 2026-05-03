using System;
using System.Collections.Generic;
using System.Text;

namespace Sparcpoint.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ProductImageUris { get; set; } = string.Empty;
        public string ValidSkus { get; set; } = string.Empty;
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
        public List<int> CategoryInstanceIds { get; set; } = new List<int>();
        public DateTime CreatedTimestamp { get; set; }
    }   
}
