using Sparcpoint.Models.DTOs;
using Sparcpoint.Models.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Abstract.Repositories
{
    //EVAL: Use of interfaces - Repository
    public interface IInventoryRepository
    {
        Task<int> AddProductToInventoryAsync(AddToInventoryRequestDto request);

        Task<int> RemoveInventoryTransactionAsync(int transactionId);

        Task<int> RemoveProductFromInventoryAsync(int productId);

        Task<decimal> GetProuctInventoryCountAsync(int productId);
    }
}
