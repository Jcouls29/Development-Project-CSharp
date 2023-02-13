using AutoMapper;
using Sparcpoint.Inventory.Models;

namespace Interview.Web.Models
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<ProductAttributeRequest, ProductAttribute>();
            CreateMap<ProductCategoryRequest, ProductCategory>();
            CreateMap<CreateProductRequest, Product>();
        }
    }
}
