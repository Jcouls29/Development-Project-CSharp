using Dapper;
using Interview.Web.Context;
using Interview.Web.Contracts;
using Interview.Web.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

//EVAL: I created this first just to be sure I could pull data from the database using Dapper and see it in the browser.

namespace Interview.Web.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly DapperContext _context;

        public ProductRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetProducts()
        {
            var query = "SELECT * FROM Instances.Products";

            using (var connection = _context.CreateConnection())
            {
                var products = await connection.QueryAsync<Product>(query);
                return products.ToList();
            }
        }
    }
}
