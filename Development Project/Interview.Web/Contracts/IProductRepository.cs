using Interview.Web.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

//EVAL: I created this first just to be sure I could pull data from the database using Dapper and see it in the browser.

namespace Interview.Web.Contracts
{
    public interface IProductRepository
    {
        public Task<IEnumerable<Product>> GetProducts();
    }
}
