using System;
using System.Collections.Generic;

namespace Sparcpoint
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> ProductImageUris { get; set; } = new List<string>();
        public List<string> ValidSkus { get; set; } = new List<string>();
        public DateTime CreatedTimestamp { get; set; }
        public List<int> CategoryInstanceIds { get; set; } = new List<int>();
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
    }
}