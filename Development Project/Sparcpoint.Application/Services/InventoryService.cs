using Sparcpoint.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Application.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _repository;

        public InventoryService(IInventoryRepository repository)
        {
            _repository = repository;
        }

        public async Task AddStock(int productId, int quantity)
        {
            PreConditions.ParameterNotNull(productId, nameof(productId));

            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than 0");

            await _repository.InsertTransaction(productId, quantity, "ADD");
        }

        public async Task RemoveStock(int productId, int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than 0");

            await _repository.InsertTransaction(productId, -quantity, "REMOVE");
        }

        public async Task<int> GetStock(int productId)
        {
            return await _repository.GetStock(productId);
        }

        public async Task UndoTransaction(int transactionId)
        {
  
            await _repository.DeleteTransaction(transactionId);

        }
    }
}
