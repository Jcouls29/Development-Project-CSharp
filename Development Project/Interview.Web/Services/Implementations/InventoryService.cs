using System.Threading.Tasks;
using Interview.Web.Models;
using Interview.Web.Repositories.Interfaces;
using Interview.Web.Services.Interfaces;
using Sparcpoint;

namespace Interview.Web.Services.Implementations;

public class InventoryService : IInventoryService
{
    private readonly IInventoryRepository _inventoryRepository;

    public InventoryService(IInventoryRepository inventoryRepository)
    {
        _inventoryRepository = inventoryRepository;
    }

    public async Task<int> AddAsync(AddInventoryRequest request)
    {
        PreConditions.ParameterNotNull(request, nameof(request));

        // EVAL: Identity columns start at 1 — zero or negative can't reference a real row.
        if (request.ProductInstanceId <= 0)
            throw new System.ArgumentException("ProductInstanceId must be a positive integer.", nameof(request.ProductInstanceId));

        // EVAL: Zero quantity has no effect and is almost always a caller mistake.
        // Negatives are allowed — that's how stock removal is recorded.
        if (request.Quantity == 0)
            throw new System.ArgumentException("Quantity cannot be zero.", nameof(request.Quantity));

        return await _inventoryRepository.AddAsync(request);
    }

    public async Task RemoveTransactionAsync(int transactionId)
    {
        // EVAL: Same positive-integer rule as ProductInstanceId.
        if (transactionId <= 0)
            throw new System.ArgumentException("TransactionId must be a positive integer.", nameof(transactionId));

        await _inventoryRepository.RemoveTransactionAsync(transactionId);
    }

    public async Task<decimal> GetCountAsync(InventoryCountRequest request)
    {
        PreConditions.ParameterNotNull(request, nameof(request));

        return await _inventoryRepository.GetCountAsync(request);
    }
}
