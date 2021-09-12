using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Interview.Web.models;

namespace Interview.Web.Repository
{
    public class ProductRepository : IProduct
    {
        public bool SaveProducts(ProductDetailsModel pod)
        {
            return true;
        }
        private List<ProductDetailsModel> products;
        public ProductRepository()
        {
            products = new List<ProductDetailsModel>();
        }
        public void Delete(int id)
        {
            var product = FindById(id);
            if (product is null)
            {
                // implement custome exception
                throw new Exception("Product Not found");
            }
            product.Active = false;
        }
        /// <summary>
        /// return all product details
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ProductDetailsModel> GetAllProducts()
        {
            return products.AsEnumerable();
                
        }
        /// <summary>
        /// return all product details based on id 
        /// </summary>
        /// <returns></returns>
        public ProductDetailsModel GetProductById(int id)
        {
            return FindById(id);
        }
        /// <summary>
        /// Add new Product
        /// </summary>
        /// <param name="entity"></param>
        public void AddProduct(ProductDetailsModel entity)
        {
            if (entity is null)
            {
                throw new Exception("Invalid entity");
            }
            products.Add(entity);
        }

        /// <summary>
        /// Update Product Details
        /// </summary>
        /// <param name="entity"></param>
        public void UpdateProduct(ProductDetailsModel entity)
        {
            if (entity is null)
            {
                throw new Exception("Invalid entity");
            }

            var product = FindById(entity.ProductId);
            products.Remove(product);
            AddProduct(entity);
        }

        /// <summary>
        /// finding products based on productid
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private ProductDetailsModel FindById(int id)
        {
            return products.Find(p => p.ProductId == id);
        }
    }
}

