using System;
using System.Linq;
using System.Threading.Tasks;
using Interview.Web.Data;
using Interview.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Interview.Web.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly ApplicationDbContext _db;

        public InventoryService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Inventory?> GetInventoryByProductId(Guid productId)
        {
            var productExists = await _db.Products.AnyAsync(p => p.Id == productId);
            if (!productExists)
                return null;

            var onHandQuantity = await _db.InventoryTransactions
                .Where(t => t.ProductId == productId)
                .SumAsync(t =>
                    t.Type == InventoryTransactionType.Adjustment ? t.Quantity :
                    t.Type == InventoryTransactionType.Purchase ? t.Quantity :
                    t.Type == InventoryTransactionType.Sale ? -t.Quantity : 0);

            return new Inventory
            {
                ProductId = productId,
                OnHandQuantity = onHandQuantity
            };
        }
    }
}
