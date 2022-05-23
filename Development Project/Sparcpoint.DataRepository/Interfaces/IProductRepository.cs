using Sparcpoint.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sparcpoint.DataRepository.Interfaces
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAll();

        Task<Product> GetById(int id);

        Task<int> Insert(Product product);
    }
}