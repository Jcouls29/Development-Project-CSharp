using Sparcpoint.Inventory.Domain.Entities.Instances;

namespace Sparcpoint.Inventory.Application.Repositories
{
    public interface IInstanceRepository<T> : IRepository<T> { }
    public interface IProductRepository : IInstanceRepository<Product> { }
    public interface IProductAttributeRepository : IInstanceRepository<ProductAttribute> { }
    public interface IProductCategoryRepository : IInstanceRepository<ProductCategory> { }
    public interface ICategoryRepository : IInstanceRepository<Category> { }
    public interface ICategoryAttributeRepository : IInstanceRepository<CategoryAttribute> { }
    public interface ICategoryCategoryRepository : IInstanceRepository<CategoryCategory> { }
}
