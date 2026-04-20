using Sparcpoint.Domain;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Sparcpoint.Contracts
{
    /** IProductRepository defines the contract for product-related data access operations.*/
    public interface IProductRepository
    {
        /** Add method takes a Product object and adds it to the repository, returning the ID of the newly added product.*/
        Task<int> Add(Product product);
        /** Get method takes a query function as a parameter and returns a list of Product objects that match the query criteria.*/
        Task<IEnumerable<Product>> Get(string query);
    }
}
