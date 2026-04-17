using System.Collections.Generic;

namespace Interview.Web.Models.Requests
{
    /// <summary>
    /// EVAL: Request with all criteria as optional. Designed additively
    /// — a new client can add fields without breaking old ones.
    /// </summary>
    public class SearchProductsRequest
    {
        public string NameContains { get; set; }
        public IList<AttributePair> AttributeFilters { get; set; } = new List<AttributePair>();
        public IList<int> CategoryIds { get; set; } = new List<int>();
        public int Skip { get; set; } = 0;
        public int Take { get; set; } = 100;
    }
}
