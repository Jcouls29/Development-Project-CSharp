using System.Collections.Generic;

namespace Sparcpoint.Models
{
    public class ProductSearchRequest
    {
        public List<int> CategoryIds { get; set; } = new List<int>();
        public List<AttributeFilter> Attributes { get; set; } = new List<AttributeFilter>();
    }

    public class AttributeFilter
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
