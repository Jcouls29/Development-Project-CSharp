using Interview.Service.Models;
using Sparcpoint.SqlServer.Abstractions;
using System.Collections.Generic;

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
            // EVAL: Build SQL query based on parms values
            // EVAL: Execute SQL command using SqlExecutor
            // EVAL: Return result
            return new List<Product>();
        }

        public List<Product> AddProducts(List<Product> products)
        {
            // EVAL: For each product
            // EVAL:     Build an INSERT statement for Products table, save Id to sql variable
            // EVAL:     Check if category exists and add if new
            // EVAL:     Build an INSERT statement for ProductCategories table
            // EVAL:     For each product attribute, build an INSERT statement for ProductAttributes
            // EVAL:     Execute SQL command
            // EVAL:     Assign Id value to product model object
            return products;
        }
    }
}
