using System.Collections.Generic;

namespace Sparcpoint.Core.Models
{
    public class Product
    {
        public string Name { get; set; }
        public string Description { get; set; }

        // EVAL: Categories can be an arbitratry number
        public List<string> Categories { get; set; }

        // Eval: Metadata can be an arbitrary number of KV entries
        public Dictionary<string, string> Metadata { get; set; }
    }
}
