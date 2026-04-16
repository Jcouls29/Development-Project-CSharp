using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Application.Interfaces
{
    public interface IInventoryRepository
    {
        Task InsertTransaction(int productId, int quantity, string type);
        Task<int> GetStock(int productId);
        Task DeleteTransaction(int transactionId);
    }
}
