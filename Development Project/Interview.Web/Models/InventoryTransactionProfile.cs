using AutoMapper;
using Sparcpoint.Inventory.Models;

namespace Interview.Web.Models
{
    public class InventoryTransactionProfile : Profile
    {
        public InventoryTransactionProfile()
        {
            CreateMap<InventoryTransactionRequest, InventoryTransaction>();
        }
    }
}
