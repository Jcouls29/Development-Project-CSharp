namespace Sparcpoint.Inventory.Application.Interfaces
{
    public interface IProductRepository
    {
        Task<int> InsertProductAsync(string name, string description, string productImageUris, string validSkus);

        Task InsertAttributeAsync(int productId, string key, string value);

        Task InsertProductCategoryAsync(int productId, int categoryId);

        Task<List<int>> SearchProductIdsAsync(Dictionary<string, string> attributes, List<int> categoryIds);
    }
}