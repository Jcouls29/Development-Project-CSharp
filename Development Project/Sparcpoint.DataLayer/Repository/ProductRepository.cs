using Microsoft.Extensions.Configuration;
using Sparcpoint.DataLayer.Context;
using Sparcpoint.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.DataLayer.Repository
{
    public class ProductRepository : IProductRepository
    {
        private IConfiguration _configuration;
        private IProductDBContext _context;
        public ProductRepository(
            IConfiguration configuration,IProductDBContext context)
        {

            _configuration = configuration;
            _context = context;
        }

        public async Task<Products> AddProduct(Products product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }
        public async Task<List<Products>> SearchProduct(FilterParam filterModel)
        {
            List<Products> final = null;

            _context = new ProductDBContext(_configuration);
            IQueryable<Products> c = _context.Products.AsQueryable();

            if (filterModel.Name != null)
            {
                c = _context.Products.Where(x => x.Name == filterModel.Name);
            }

            if (filterModel.Description != null)
            {
                c = c.Where(x => x.Description == filterModel.Description);
            }

            if (filterModel.ProductImageUri != null)
            {
                c = c.Where(x => x.ProductImageUri == filterModel.ProductImageUri);
            }

            if (filterModel.ValidSkus != null)
            {
                c = c.Where(x => x.ValidSkus == filterModel.ValidSkus);
            }
            if (filterModel.Category != null)
            {
                c = (from p in c
                     join pc in _context.ProductCategories on p.InstanceId equals pc.InstanceId
                     join ca in _context.Categories on pc.CategoryInstanceId equals ca.InstanceId
                     where ca.Name == filterModel.Category
                     select new Products()
                     {
                         Attributes = p.Attributes,
                         Description = p.Description,
                         Categories = p.Categories,
                         CreatedDate = p.CreatedDate,
                         InstanceId = p.InstanceId,
                         Name = p.Name,
                         ProductImageUri = p.ProductImageUri,
                         ValidSkus = p.ValidSkus
                     }
               );
            }
            final = c.ToList();
            return final;
        }
    }


}
