
using System.Collections.Generic;

namespace Sparcpoint.Core.Models
{
    public class ProductSearchResponse
    {
        public IList<ProductSearchResult> Items { get; set; } = new List<ProductSearchResult>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}