using Dapper;
using Interview.Web.Helpers;
using Interview.Web.Interfaces;
using Interview.Web.Models;
using Microsoft.Extensions.Configuration;
using Sparcpoint.SqlServer.Abstractions;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Interview.Web.Repository
{
    public class ProductRepository : IProductRepository
    {
        private ISqlExecutor _sqlServer;
        private int _itemId;
        private Product _product;
        private Category _category;


        public ProductRepository(IConfiguration configuration)
        {
            _sqlServer = new SqlServerExecutor(configuration.GetConnectionString(Constants.DefaultConnection));
            _product = new Product();
            _category = new Category();

        }

        /// <summary>
        /// EVAL: Only retrieve all of the products that are not marked as deleted
        /// </summary>
        /// <returns></returns>
        public List<Product> GetAll()
        {
            return _sqlServer.Execute<List<Product>>(SqlServerExecutorGetAllProducts);
        }

        public List<Product> GetInventory(Product product)
        {
            _product = product;
            return _sqlServer.Execute<List<Product>>(SqlServerExecutorGetInventory);
        }

        public List<Product> GetInventory(string productname, string description, string validSkus, string categoryName)
        {
            _product.Name = productname;
            _product.Description = description;
            _product.ValidSkus = validSkus;
            _category.Name = categoryName;
            return _sqlServer.Execute<List<Product>>(SqlServerExecutorGetInventory);
        }



        public Product Add(Product product)
        {
            _product = product;
            _category = new Category();

            _category.Name = _product.Category.Name;
            _category.Description = _product.Category.Name;

            var id = _sqlServer.Execute<int>(SqlServerExecutorAddProducts);

            product.InstanceId = id;
            return product;
        }

        /// <summary>
        /// EVAL: Products can be added to the system but NEVER deleted.
        /// Instead of deleting, mark the item as "DELETED" in the ValidSkus field to represent a delete
        /// </summary>
        /// <param name="id"></param>
        public void Remove(int id)
        {
            _itemId = id;
            _sqlServer.Execute<int>(SqlServerExecutorRemoveProducts);
        }

        public int GetActiveProductCount()
        {
            return _sqlServer.Execute<int>(SqlServerExecutorProductCount);
        }


        #region Private methods

        private List<Product> SqlServerExecutorGetAllProducts(IDbConnection connection, IDbTransaction transaction)
        {
            var sql = Constants.ProductGetAllActive;
            return connection.Query<Product>(sql, null, transaction).ToList();
        }

        private List<Product> SqlServerExecutorGetInventory(IDbConnection connection, IDbTransaction transaction)
        {
            var parametersInventory = new { Name = _product.Name, Description = _product.Description, ValidSkus = _product.ValidSkus, CategoryName = _category.Name };

            var sql = Constants.ProductGetInventory;
            return connection.Query<Product>(sql, parametersInventory, transaction).ToList();
        }


        /// <summary>
        /// EVAL:  Add product/inventory - includes the addition of adding to the following table: Product, Categories, and ProductCategories
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private int SqlServerExecutorAddProducts(IDbConnection connection, IDbTransaction transaction)
        {
            int categoryId = 0;
            var parametersProduct = new { Name = _product.Name, Description = _product.Description, ProductImageUris = _product.ProductImageUris, ValidSkus = _product.ValidSkus };
            var parameterCategory = new { Name = _category.Name, Description = _category.Description };


            var sqlProduct = Constants.ProductInsert;
            int productId = connection.Query<int>(sqlProduct, parametersProduct, transaction).Single();


            var sqlCategory = Constants.CategoryInsertIfNotExist;
            var categoryItem = connection.Query<int>(sqlCategory, parameterCategory, transaction);
            if (categoryItem.Any())
            {
                categoryId = categoryItem.Single();
            }


            var sql = Constants.ProductCategoriesInsert;
            connection.Execute(sql, new { InstanceId = productId, CategoryInstanceId = categoryId }, transaction);

            return productId;
        }

        /// <summary>
        /// EVAL:  Mark the product as "DELETED" (according to requirements do not delete from table)
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private int SqlServerExecutorRemoveProducts(IDbConnection connection, IDbTransaction transaction)
        {
            var sql = Constants.ProductFlagAsDeleted;
            connection.Execute(sql, new { _itemId }, transaction);
            return 0;
        }

        private int SqlServerExecutorProductCount(IDbConnection connection, IDbTransaction transaction)
        {
            var sql = Constants.ProductCountAllActive;
            return connection.Query<int>(sql, null, transaction).Single();
        }

        #endregion
    }
}
