using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interview.Web.Mappings
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Data.ViewModels.ProductViewModel, Data.Models.Product>()
                .ForMember(
                     dest => dest.CreatedTimestamp,
                     opt => opt.MapFrom(src => DateTime.Now)
                ).ForMember(
                    dest => dest.ProductAttributes,
                    opt => opt.MapFrom(src => src.ProductAttributes)
                ).ForMember(
                    dest => dest.Categories,
                    opt => opt.MapFrom(src => src.Categories)
                );
        }
    }
}
