using Interview.Web.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interview.Web.Repositories
{
    public interface IProductRepository
    {
        Task<Product> AddAsync(Product product);
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product> GetByIdAsync(Guid id);
        Task<IEnumerable<Product>> SearchByMetadataAsync(Dictionary<string, string> metadataCriteria);
        Task<IEnumerable<Product>> SearchAsync(string name, List<string> categories, Dictionary<string, string> metadataCriteria);
    }
}
