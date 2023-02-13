using AutoMapper;
using Sparcpoint.Inventory.Models;

namespace Interview.Web.Models
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<CreateCategoryRequest, Category>();
            CreateMap<CategoryAttributeRequest, CategoryAttribute>();
            CreateMap<CategoryOfCategoryRequest, CategoryOfCategory>();
        }
    }
}
