using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sparcpoint
{
    public interface IInventoryRepository
    {
        Task SaveTransactionAsync(List<InventoryUpdate> updates);
        Task UndoTransactionAsync(Guid transactionId);
        Task<int> GetStockCountAsync(int productInstanceId);
    }
}