using Sparcpoint.Requests;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Abstract
{
    public interface IInventoryRespository
    {
        Task<int> AddInventoryAsync(AddInventoryRequest request);
        Task<int> RemoveInventoryAsync(RemoveInventoryRequest request);
        Task RemoveTransactionAsync(int transId);
        Task<decimal> GetInventoryCountAsync(int id);
    }
}
