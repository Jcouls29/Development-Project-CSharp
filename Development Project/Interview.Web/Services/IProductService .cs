using Interview.Web.models;
namespace Interview.Web.Services
{
    /// <summary>
    /// Interface for Product Service
    /// </summary>
    public interface IProductService
    {
        /// <summary>
        /// Add products
        /// </summary>
        /// <param name="pod"></param>
        void AddProduct(ProductModel product);

        /// <summary>
        /// get all products
        /// </summary>
        /// <returns></returns>
        IEnumerable<ProductModel> GetAllProducts();

        /// <summary>
        /// get product by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ProductModel GetProductById(int id);

        /// <summary>
        /// Update entire product
        /// </summary>
        /// <param name="entity"></param>
       void UpdateProduct(ProductModel entity);

        /// <summary>
        /// delete product
        /// </summary>
        /// <param name="entity"></param>
        void DeleteProduct(ProductModel entity);
    }
}
