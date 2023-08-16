using Interview.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Interview.Service.Inventory
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepo _repo;

        public InventoryService(IInventoryRepo repo)
        {
            _repo = repo;
        }

        public List<InventoryTransaction> AddInventory(List<Product> products)
        {
            var inventoryTransactions = products.Select(p => new InventoryTransaction
            {
                ProductInstanceId = p.InstanceId,
                Quantity = p.Qty ?? 0,
                StartedTimestamp = DateTime.Now,
                TypeCategory = "ADD"
            }).ToList();

            return _repo.AddInventory(inventoryTransactions);
        }

        public void DeleteInventory(List<int> productIds)
        {
            var transactions = new List<InventoryTransaction>();
            foreach (var id in productIds)
            {
                InventoryTransaction trans = new InventoryTransaction
                {
                    ProductInstanceId = id,
                    Quantity = GetInventoryCount(new ProductFilterParams { Id = id }) * -1,
                    StartedTimestamp = DateTime.Now,
                    CompletedTimestamp = DateTime.Now,
                    TypeCategory = "REMOVE"
                };
                transactions.Add(trans);
            }
            var result = _repo.AddInventory(transactions);
        }

        public int GetInventoryCount(ProductFilterParams parms)
        {
            return _repo.GetProductInventory(parms).FirstOrDefault().Qty ?? 0;
        }
    }
}
