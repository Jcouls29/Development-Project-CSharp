using Domain.Entity;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repository.Repositories
{
    public class ProductRepository : IProductRepository
    {
        public void DeleteProduct(int productId)
        {
            throw new NotImplementedException();
        }

        public Product GetProductById(int productId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Product> GetProudcts()
        {
            throw new NotImplementedException();
        }

        public Product InsertProduct(Product product)
        {
            throw new NotImplementedException();
        }

        public async void SaveChangesAsync()
        {
            
        }

        public void UpdateCustomer(Product product)
        {
            throw new NotImplementedException();
        }
    }
}
