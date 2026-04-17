using Sparcpoint.Abstract.Repositories;
using Sparcpoint.Abstract.Services;
using Sparcpoint.Domain;
using Sparcpoint.DTOs;
using System.Collections.Generic;
using System.Linq;
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

        public async Task UpdateInventoryAsync(List<UpdateInventoryRequestDto> request)
        {
            await _inventoryRepository.UpdateInventoryAsync(request);
        }

        public decimal CalculateCurrentStock(IEnumerable<InventoryTransaction> transactions)
        {
            if (transactions == null) return 0;
            return transactions.Sum(t => t.Quantity);
        }

    }
}
