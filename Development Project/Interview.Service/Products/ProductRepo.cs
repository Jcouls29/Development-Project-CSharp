using Interview.Service.Models;
using Sparcpoint.SqlServer.Abstractions;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Interview.Service.Products
{
    public class ProductRepo : IProductRepo
    {
        private readonly ISqlExecutor _sqlExecutor;

        public ProductRepo(ISqlExecutor sqlExecutor)
        {
            _sqlExecutor = sqlExecutor;
        }

        public List<Product> RetreiveProducts(ProductFilterParams parms)
        {
            var result = new List<Product>();
            // EVAL: Build SQL query based on parms values
            // EVAL: result.Add(Execute<List<Product>>(FilterProductCommand))
            return result;
        }

        public List<Product> AddProducts(List<Product> products)
        {
            var result = new List<Product>();
            foreach (var product in products)
            {
                // EVAL:  Check to see if product already exists (can use RetrieveProducts method), skip item if it does
                // EVAL:  Build an INSERT statement for Products table, save Id to sql variable
                // EVAL:  Check if category exists and add if new
                // EVAL:  Build an INSERT statement for ProductCategories table using Id variable
                // EVAL:  For each product attribute, build an INSERT statement for ProductAttributes using Id variable
            }
            // EVAL: result.Add(await ExecuteAsync<List<Product>>(AddProductCommand)

            return result;
        }

        //public Task FilterProductCommand(IDbConnection conn, IDbTransaction trans)
        //{
            // Not familiar with how to interact with SQLServerExecutor
        //}

        //public Task AddProductCommand(IDbConnection conn, IDbTransaction trans)
        //{
            // Not familiar with how to interact with SQLServerExecutor
        //}
    }
}
