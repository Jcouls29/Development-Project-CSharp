using System;
using System.Collections.Generic;
using System.Text;

namespace Sparcpoint.Models.Requests
{
    public class ProductSearchRequest
    {
        public string Name { get; set; }
        public List<int> CategoryIds { get; set; } = new List<int>();
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
    }
}
