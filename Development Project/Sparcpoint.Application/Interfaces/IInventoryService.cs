using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Application.Interfaces
{
    public interface IInventoryService
    {
        Task AddStock(int productId, int quantity);
        Task RemoveStock(int productId, int quantity);
        Task<int> GetStock(int productId);
        Task UndoTransaction(int transactionId);
    }
}
