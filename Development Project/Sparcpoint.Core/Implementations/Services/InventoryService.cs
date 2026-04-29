using Sparcpoint.Abstract.Repositories;
using Sparcpoint.Abstract.Services;
using Sparcpoint.Models.DTOs;
using Sparcpoint.Models.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Implementations.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepository;

        public InventoryService(IInventoryRepository inventoryRepository)
        {
            _inventoryRepository = inventoryRepository;

        }
        public async Task<int> AddProductToInventoryAsync(AddToInventoryRequestDto request)
        {
            var transID = await _inventoryRepository.AddProductToInventoryAsync(request);
            if (transID <= 0)
            {
                throw new InvalidOperationException("Unable to Add Product inventory - No Trans id returned");
            }
            return transID;
        }

        public async Task<int> RemoveInventoryTransactionAsync(int transactionId)
        {
            return await _inventoryRepository.RemoveInventoryTransactionAsync(transactionId);
        }

        public async Task<int> RemoveProductFromInventoryAsync(int productId)
        {
            return await _inventoryRepository.RemoveProductFromInventoryAsync(productId);
        }

        public async Task<decimal> GetProuctInventoryCountAsync(int productId)
        {
            return await _inventoryRepository.GetProuctInventoryCountAsync(productId);
        }
    }
}
