using System;
using System.Collections.Generic;
using System.Text;

namespace Sparcpoint.Requests
{
    public class CreateProductRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<string> ProductImageUris { get; set; } = new List<string>();
        public List<string> ValidSkus { get; set; } = new List<string>();
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();        
        public List<int> CategoryInstanceIds { get; set; } = new List<int>();
    }
}
