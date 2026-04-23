using Interview.Web.Models;

namespace Interview.Web.Services
{
    public interface IInventoryRule
    {
        void ValidateAdd(AddInventoryRequest request);
        void ValidateRemove(RemoveInventoryRequest request, int currentInventory);
    }
}
