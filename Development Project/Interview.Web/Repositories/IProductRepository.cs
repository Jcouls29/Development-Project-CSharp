using System.Collections.Generic;
using System.Threading.Tasks;
using Interview.Web.Models;
namespace Interview.Web.Repositories;

public interface IProductRepository
{
    Task<IEnumerable<Product>> Get();
    Task<Product> Get(int id);
    Task<Product> Create(Product product);
}