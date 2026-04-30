using System.Collections.Generic;

namespace Sparcpoint.Inventory.Requests
{
    // EVAL: All fields are optional — any combination can be used to filter.
    // An empty request returns all products, which is intentional for list endpoints.
    public class ProductSearchRequest
    {
        public string? Name { get; set; }
        public List<int> CategoryInstanceIds { get; set; } = new();

        // EVAL: Attribute filters are AND'd together — a product must match
        // ALL supplied key/value pairs to be returned. This can be relaxed
        // to OR logic per customer requirement without changing the interface.
        public Dictionary<string, string> Attributes { get; set; } = new();
    }
}
