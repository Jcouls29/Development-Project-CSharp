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

        // EVAL: ProductInstanceId must be a positive integer — zero or negative values
        // cannot reference a valid auto-incremented database identity.
        if (request.ProductInstanceId <= 0)
            throw new System.ArgumentException("ProductInstanceId must be a positive integer.", nameof(request.ProductInstanceId));

        // EVAL: Quantity of zero has no effect on inventory and is most likely a caller error.
        // Negative quantities are valid for removals so we only reject zero.
        if (request.Quantity == 0)
            throw new System.ArgumentException("Quantity cannot be zero.", nameof(request.Quantity));

        return await _inventoryRepository.AddAsync(request);
    }

    public async Task RemoveTransactionAsync(int transactionId)
    {
        // EVAL: TransactionId must be positive — same reasoning as ProductInstanceId above.
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
