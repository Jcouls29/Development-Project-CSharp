using Interview.Web.Repository;
using Interview.Web.models;

namespace Interview.Web.Services
{
    /// <summary>
    /// class for product service
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="repository"></param>
        public ProductService(IProductRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// get all products
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ProductModel> GetAllProducts()
        {
            return _repository.GetAll();
        }

        /// <summary>
        /// get product by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ProductModel GetProductById(int id)
        {
            return _repository.GetById(id);
        }

        /// <summary>
        /// Update entire product
        /// </summary>
        /// <param name="entity"></param>
        public void UpdateProduct(ProductModel entity)
        {
            // In future add transaction update and return true or false or return product id if there is a requirement
            _repository.Update(entity);
        }

        /// <summary>
        /// Add products
        /// </summary>
        /// <param name="product"></param>
        public void AddProduct(ProductModel product)
        {
            // In future add transaction insert and return true or false
            _repository.Add(entity);
        }

        /// <summary>
        /// Delete products
        /// </summary>
        /// <param name="product"></param>
        public void DeleteProduct(ProductModel product)
        {
            // In future add transaction delete and return true or false
            _repository.Delete(entity);
        }
    }
}
