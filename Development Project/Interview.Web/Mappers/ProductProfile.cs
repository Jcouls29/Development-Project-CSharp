using AutoMapper;
using Sparcpoint.Entities;
using Sparcpoint.Models;

namespace Interview.Web.Mappers
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, ProductModel>().ReverseMap();
        }
    }
}
