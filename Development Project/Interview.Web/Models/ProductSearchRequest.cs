using Sparcpoint.Inventory.Models;

namespace Interview.Web.Models
{
    public class ProductSearchRequest
    {
        public ProductSearchScope Scope { get; set; }
        public string Keyword { get; set; }
    }
}
