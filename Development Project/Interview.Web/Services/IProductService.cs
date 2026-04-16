using Interview.Web.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interview.Web.Services
{
    public interface IProductService
    {
        Task<Product> CreateProductAsync(Product product);
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product> GetByIdAsync(Guid id);
        Task<IEnumerable<Product>> SearchByMetadataAsync(Dictionary<string, string> metadataCriteria);

        // - TODO: categorize and create hierarchies of products for simple sorting on various UI
        //  frontends
        // - add and remove products from inventory, with the ability to “undo” transactions easily.
    }
}
