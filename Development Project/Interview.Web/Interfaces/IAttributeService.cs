using Sparkpoint.Data;
using System.Collections.Generic;

namespace Interview.Web.Interfaces
{
    public interface IProductAttributeService
    {
        IEnumerable<ProductAttribute> GetAllProductAttributes();
        ProductAttribute GetAttributeById(int id);
        ProductAttribute CreateAttribute(ProductAttribute attribute);
        ProductAttribute UpdateAttribute(ProductAttribute attribute);
        ProductAttribute DeleteAttribute(int id);
    }
}