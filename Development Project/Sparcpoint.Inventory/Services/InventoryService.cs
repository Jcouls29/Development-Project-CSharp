using Sparcpoint;
using Sparcpoint.Inventory.Interfaces;
using Sparcpoint.Inventory.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepository;

        public InventoryService(IInventoryRepository inventoryRepository)
        {
            _inventoryRepository = inventoryRepository ?? throw new ArgumentNullException(nameof(inventoryRepository));
        }

        public async Task<InventoryTransactionModel> AddInventoryAsync(InventoryTransactionRequest request)
        {
            var normalizedRequest = ValidateAndNormalizeRequest(request, isRemoval: false);
            var transactionId = await _inventoryRepository.AddTransactionAsync(normalizedRequest);
            return await _inventoryRepository.GetTransactionByIdAsync(transactionId);
        }

        public async Task<InventoryTransactionModel> RemoveInventoryAsync(InventoryTransactionRequest request)
        {
            var normalizedRequest = ValidateAndNormalizeRequest(request, isRemoval: true);
            var transactionId = await _inventoryRepository.AddTransactionAsync(normalizedRequest);
            return await _inventoryRepository.GetTransactionByIdAsync(transactionId);
        }

        public async Task<BulkInventoryResult> AddInventoryBulkAsync(BulkInventoryRequest request)
        {
            return await ProcessBulkAsync(request, isRemoval: false);
        }

        public async Task<BulkInventoryResult> RemoveInventoryBulkAsync(BulkInventoryRequest request)
        {
            return await ProcessBulkAsync(request, isRemoval: true);
        }

        public Task<InventoryCountModel> GetInventoryCountAsync(int productInstanceId)
        {
            if (productInstanceId <= 0)
                throw new ArgumentOutOfRangeException(nameof(productInstanceId));

            return _inventoryRepository.GetTotalCountByProductAsync(productInstanceId);
        }

        public async Task<InventoryTransactionModel> UndoTransactionAsync(int transactionId)
        {
            if (transactionId <= 0)
                throw new ArgumentOutOfRangeException(nameof(transactionId));

            await _inventoryRepository.CompleteTransactionAsync(transactionId);
            return await _inventoryRepository.GetTransactionByIdAsync(transactionId);
        }

        public async Task<InventoryCountByMetadataModel> GetInventoryCountByMetadataAsync(Dictionary<string, string> attributes)
        {
            PreConditions.ParameterNotNull(attributes, nameof(attributes));

            if (attributes.Count == 0)
                throw new ArgumentException("At least one attribute filter is required.", nameof(attributes));

            var items = await _inventoryRepository.GetCountByMetadataAsync(attributes);
            return new InventoryCountByMetadataModel
            {
                Items = items,
                TotalQuantity = items.Sum(i => i.Quantity),
            };
        }

        public Task<PaginatedResult<InventoryTransactionModel>> GetTransactionHistoryAsync(int productInstanceId, int page, int pageSize)
        {
            if (productInstanceId <= 0)
                throw new ArgumentOutOfRangeException(nameof(productInstanceId));
            if (page < 1)
                throw new ArgumentOutOfRangeException(nameof(page));
            if (pageSize < 1 || pageSize > 100)
                throw new ArgumentOutOfRangeException(nameof(pageSize));

            return _inventoryRepository.GetTransactionsByProductAsync(productInstanceId, page, pageSize);
        }

        private async Task<BulkInventoryResult> ProcessBulkAsync(BulkInventoryRequest request, bool isRemoval)
        {
            PreConditions.ParameterNotNull(request, nameof(request));
            PreConditions.ParameterNotNull(request.Items, nameof(request.Items));

            if (request.Items.Count == 0)
                throw new ArgumentException("Items list cannot be empty.", nameof(request.Items));

            if (request.Items.Count > 100)
                throw new ArgumentException("Items list cannot exceed 100 items.", nameof(request.Items));

            var normalizedItems = request.Items
                .Select(item => ValidateAndNormalizeRequest(item, isRemoval))
                .ToList();

            var transactionIds = await _inventoryRepository.AddBulkTransactionsAsync(normalizedItems);

            var results = new List<InventoryTransactionModel>();
            foreach (var id in transactionIds)
            {
                results.Add(await _inventoryRepository.GetTransactionByIdAsync(id));
            }

            return new BulkInventoryResult
            {
                Results = results,
                Errors = new List<string>(),
            };
        }

        private InventoryTransactionRequest ValidateAndNormalizeRequest(InventoryTransactionRequest request, bool isRemoval)
        {
            PreConditions.ParameterNotNull(request, nameof(request));

            if (request.ProductInstanceId <= 0)
                throw new ArgumentOutOfRangeException(nameof(request.ProductInstanceId));

            if (request.Quantity == 0)
                throw new ArgumentOutOfRangeException(nameof(request.Quantity));

            if (request.TypeCategory != null && request.TypeCategory.Length > 32)
                throw new ArgumentException("TypeCategory must be 32 characters or less.", nameof(request.TypeCategory));

            return new InventoryTransactionRequest
            {
                ProductInstanceId = request.ProductInstanceId,
                Quantity = isRemoval ? -Math.Abs(request.Quantity) : Math.Abs(request.Quantity),
                TypeCategory = request.TypeCategory,
            };
        }
    }
}
