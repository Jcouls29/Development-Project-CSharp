using System;
using System.Collections.Generic;
using System.Text;

namespace Sparcpoint.Requests
{
    public class SearchProductRequest
    {
        public string Name { get; set; }
        public List<int> CategoryInstanceIds { get; set; } = new List<int>();                
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
        
    }
}
