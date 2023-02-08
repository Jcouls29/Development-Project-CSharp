using AutoMapper;
using Interview.Web.Dtos;
using Sparcpoint.Models;

namespace Interview.Web.Mapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Product, ProductDto>().ReverseMap();
        }
    }
}
