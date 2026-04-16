using System.Collections.Generic;

namespace Sparcpoint.Inventory.Models
{
    public class ProductDetailModel : ProductModel
    {
        public Dictionary<string, string> Attributes { get; set; }
        public List<CategoryModel> Categories { get; set; }
    }
}
