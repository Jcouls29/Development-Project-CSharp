using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparcpoint.Products.Data
{
    public class ProductSearch
    {
        public string Manufacturer { get; set; }
        public string ModelName { get; set; }
        public string Attribute { get; set; }
        public string Category { get; set; }
        public ProductSearch() 
        {
            Manufacturer = null;
            ModelName = null;
            Attribute = null;
            Category = null;
        }

        public string CreateQueryString()
        {
            StringBuilder sql = new StringBuilder("select  distinct " +
                "p.ProductId, " +
                "Manufacturer, " +
                "ModelName, " +
                "Description, " +
                "CAtegoriesJson, " +
                "sku, " +
                "AttributesJson, " +
                "QuantityOnHand " +
                "from Instances.Product p " +
                "inner join Instances.InventoryItem ii on p.ProductId = ii.ProductId " +
                "outer apply OPENJSON(CategoriesJson) AS cat " +
                "outer apply OPENJSON(AttributesJson) as att");
            List<string> whereClause = new List<string>();

            if(Manufacturer !=  null)
            {
                whereClause.Add("Manufacturer = @Manufacturer");
            }

            if (ModelName != null)
            {
                whereClause.Add("ModelName = @ModelName");
            }

            if(Category != null)
            {
                whereClause.Add($"cat.[value] = @Category");
            }

            if (Attribute != null)
            {
                whereClause.Add($"att.[value] = @Attribute");
            }

            return whereClause.Any() ? sql + " WHERE "  + string.Join(" AND ", whereClause) : sql.ToString();
        }
    }
}
