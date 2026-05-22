using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Interview.Web.Models;

namespace Interview.Web.Services
{
    public interface IInventoryTransactionService
    {
        Task<InventoryTransactionResponseDto?> CreateInventoryTransaction(InventoryTransactionCreateDto dto);
        Task<IEnumerable<InventoryTransactionResponseDto>> GetInventoryTransactionsByProductId(Guid productId);
    }
}
