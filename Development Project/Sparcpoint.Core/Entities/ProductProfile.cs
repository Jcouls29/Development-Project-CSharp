using AutoMapper;
using Sparcpoint.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Core.Entities
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<CreateProductRequest, Product>();
            CreateMap<ProductAttributeRequest, ProductAttribute>();
            CreateMap<ProductCategoryRequest, ProductCategory>();
        }
    }
}