using AutoMapper;
using Domain.Entities;
using Repository;
using Repository.Interface;
using SparcpointServices.Interface;
using SparcpointServices.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SparcpointServices
{
    public class ProductService : IProduct
    {
        private readonly IProductRepo _productRepo;
        private IMapper _mapper;

        public ProductService(IProductRepo productRepo, IMapper mapper)
        {
            _productRepo = productRepo;
            _mapper = mapper;
        }

        public List<ProductModel> GetAllProducts()
        {
            //add
            // _productRepo.add(_mapper.Map<Product>(ProductModel));

            return _mapper.Map<List<ProductModel>>(_productRepo.GetAllProducts());
        }

        public void AddProduct(Product product)
        {
            _productRepo.AddProduct(product);
        }

       public List<ProductModel> SearchProduct(string keyword)
        {
            return _mapper.Map<List<ProductModel>>(_productRepo.SearchProduct(keyword));
        }
    }
}
