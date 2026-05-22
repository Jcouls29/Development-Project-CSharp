using System.Collections.Generic;
using System.Threading.Tasks;
using Sparcpoint.Inventory.Models.DTOs.Requests;
using Sparcpoint.Inventory.Models.DTOs.Responses;
using Sparcpoint.Inventory.Models.Search;

namespace Sparcpoint.Inventory.Services.Interfaces
{
    public interface IInventoryService
    {
        Task<List<InventoryTransactionResponse>> AddInventoryAsync(AddInventoryRequest request);
        Task<List<InventoryTransactionResponse>> RemoveInventoryAsync(RemoveInventoryRequest request);
        Task<bool> UndoTransactionAsync(int transactionId);
        Task<List<InventoryCountResponse>> GetInventoryCountAsync(InventoryCountCriteria criteria);
        Task<InventoryTransactionResponse?> GetTransactionByIdAsync(int transactionId);
    }
}