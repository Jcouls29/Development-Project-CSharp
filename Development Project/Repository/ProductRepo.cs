using DBCore;
using Domain.Entities;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repository
{
    public class ProductRepo : IProductRepo
    {
        private readonly SparcpointDbContext _sparcpointDbContext;
        public ProductRepo(SparcpointDbContext sparcpointDbContext)
        {
            _sparcpointDbContext = sparcpointDbContext;
        }
        public List<Product> GetAllProducts()
        {
            return _sparcpointDbContext.Products.ToList();
        }

        public void AddProduct(Product product)
        {
            //add in product, productattribute and productcategory
            product.CreatedTimestamp = DateTime.Now;
            _sparcpointDbContext.Products.AddRange(product);
            _sparcpointDbContext.SaveChanges();
        }

        public List<Product> SearchProduct(string keyword)
        {
            return _sparcpointDbContext.Products.Where(x => x.Name.Contains(keyword) || x.Description.Contains(keyword)).ToList();
        }
    }
}
