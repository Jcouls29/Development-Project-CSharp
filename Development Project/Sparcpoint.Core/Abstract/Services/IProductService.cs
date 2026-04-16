using Interview.Web.Models;
using Sparcpoint.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sparcpoint.Abstract.Services
{
    public interface IProductService
    {
        Task<Product> AddAsync(Product product, CancellationToken ct = default);

        Task<Product> GetByIdAsync(int id, CancellationToken ct = default);

        Task<List<Product>> SearchAsync(SearchRequest search, CancellationToken ct = default);
    }
}
