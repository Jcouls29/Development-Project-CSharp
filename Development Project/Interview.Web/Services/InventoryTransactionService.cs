using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Interview.Web.Data;
using Interview.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Interview.Web.Services
{
    public class InventoryTransactionService : IInventoryTransactionService
    {
        private readonly ApplicationDbContext _db;

        public InventoryTransactionService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<InventoryTransactionResponseDto?> CreateInventoryTransaction(InventoryTransactionCreateDto dto)
        {
            var productExists = await _db.Products.AnyAsync(p => p.Id == dto.ProductId);
            if (!productExists)
                return null;

            var transaction = new InventoryTransaction
            {
                ProductId = dto.ProductId,
                Quantity = dto.Quantity,
                Type = dto.Type
            };

            _db.InventoryTransactions.Add(transaction);
            await _db.SaveChangesAsync();

            return MapToDto(transaction);
        }

        public async Task<IEnumerable<InventoryTransactionResponseDto>> GetInventoryTransactionsByProductId(Guid productId)
        {
            var transactions = await _db.InventoryTransactions
                .Where(t => t.ProductId == productId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            return transactions.Select(MapToDto);
        }

        private static InventoryTransactionResponseDto MapToDto(InventoryTransaction transaction) =>
            new InventoryTransactionResponseDto
            {
                TransactionId = transaction.TransactionId,
                ProductId = transaction.ProductId,
                Quantity = transaction.Quantity,
                Type = transaction.Type,
                CreatedAt = transaction.CreatedAt
            };
    }
}
