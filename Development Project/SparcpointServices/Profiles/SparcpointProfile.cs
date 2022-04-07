using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entities;
using SparcpointServices.Models;

namespace SparcpointServices.Profiles
{
    public class SparcpointProfile:Profile
    {
        public SparcpointProfile()
        {
            CreateMap<ProductModel, Product>()
                 .ForMember(dest => dest.ProductAttributes, act => act.MapFrom(src => src.ProductAttributeModel))
                 .ForMember(dest => dest.ProductCategories, act => act.MapFrom(src => src.ProductCategoryModel))
                .ReverseMap();
            CreateMap<ProductAttributeModel, ProductAttribute>()
                .ForMember(src => src.InstanceId, opts => opts.Ignore());
            CreateMap<ProductCategoryModel, ProductCategory>()
                .ForMember(src => src.InstanceId, opts => opts.Ignore());
        }
    }
}
