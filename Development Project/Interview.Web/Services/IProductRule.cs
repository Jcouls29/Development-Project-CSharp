using Interview.Web.Models.Product;

namespace Interview.Web.Services
{
    public interface IProductRule
    {
        void ValidateCreate(CreateProductRequest request);
        void ValidateSearch(ProductSearchRequest request);
    }
}
