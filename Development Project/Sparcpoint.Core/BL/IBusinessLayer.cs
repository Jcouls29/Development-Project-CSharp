using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SparcPoint.Inventory.DataModels;

namespace Sparcpoint.BL
{
    public interface IBusinessLayer
    {
        /// <summary>
        /// Returns the list of all products available
        /// </summary>
        /// <returns></returns>
        Task<List<Product>> GetAllProducts();

        /// <summary>
        /// Add a new product to the Inventory
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        Task AddProduct(Product item);

        /// <summary>
        /// Removes the product specified from inventory
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        Task<bool> RemoveProduct(Product item);

        /// <summary>
        /// Returns the Number of Products available in inventory
        /// </summary>
        /// <returns></returns>
        Task<int> ProductCount();

        /// <summary>
        /// Finds Product with a fuzzy search with the search parameters
        /// </summary>
        /// <param name="searchParameters">Comma separated search parameters</param>
        /// <returns>List of Products that match the search paramerts</returns>
        Task<List<Product>> FindProduct(string searchParameters);

        /// <summary>
        /// Returns List of Products of a particular category
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        Task<List<Product>> FindProductsOfCategory(Category category);
        Task<int> ProductCount(string searchparameter);
    }
}
