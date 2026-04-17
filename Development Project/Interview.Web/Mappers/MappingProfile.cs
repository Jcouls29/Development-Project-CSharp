using AutoMapper;
using Interview.Web.DTOs;
using Sparcpoint.DTOs;

namespace Interview.Web.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ProductAttributeDto, ProductAttributeRequestDto>();
            CreateMap<CreateProductDto, CreateProductRequestDto>();
        }
    }
}
