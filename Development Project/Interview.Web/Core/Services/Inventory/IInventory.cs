namespace Interview.Web.Core.Services
{
    public interface IInventory
    {
        void AddProductToInventory();
        void RemoveProductFromInventory();
        void RetriveProductCountFromInventory();
    }
}
