using Interview.Web.Models;
using Interview.Web.Data;
using System.Collections.Generic;

namespace Interview.Web.Services
{
    public class ProductService : IProductService
    {
        private readonly InventoryContext _context;
        public ProductService(InventoryContext context)
        {
            _context = context;
        }
        public IEnumerable<Product> GetProducts()
        {
            return _context.Products.AsQueryable();
        }
        public void AddProduct(Product product)
        {
            _context.Add(product);
            _context.SaveChangesAsync();
        }

        public int UpdateInventory(Transaction transaction)
        {
            return 0;
        }
    }
}
