using AutoMapper;
using Sparcpoint.Domain.Instance.Entities;
using Sparcpoint.Domain.Requestes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Domain.Profiles
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
