using System.Threading.Tasks;
using Xunit;
using Interview.Web.Controllers;
using Sparcpoint.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Sparcpoint.Application.Repositories;

namespace Sparcpoint.Tests.Controllers
{
    public class InventoryControllerTests
    {
        class FakeInventoryRepo : IInventoryRepository
        {
            private readonly int _undoId;
            public FakeInventoryRepo(int undoId) { _undoId = undoId; }
            public Task<int> RecordTransactionAsync(int productInstanceId, decimal quantity, string transactionType, int? relatedTransactionId = null)
                => Task.FromResult(1);
            public Task<bool> MarkAsReversedAsync(int transactionId) => Task.FromResult(true);
            public Task<int> UndoTransactionAsync(int transactionId) => Task.FromResult(_undoId);
        }

        [Fact]
        public async Task Undo_ReturnsCompensatingTransactionId()
        {
            var repo = new FakeInventoryRepo(123);
            var controller = new InventoryController(repo);

            var result = await controller.Undo(10) as OkObjectResult;

            Assert.NotNull(result);
            var value = result.Value;
            var compProp = value.GetType().GetProperty("Compensating");
            Assert.NotNull(compProp);
            var compVal = compProp.GetValue(value);
            Assert.IsType<InventoryTransactionResponseDto>(compVal);
            var dto = (InventoryTransactionResponseDto)compVal;
            Assert.Equal(123, dto.TransactionId);
        }

        [Fact]
        public async Task Adjust_ReturnsTransactionDto()
        {
            var repo = new FakeInventoryRepo(0);
            var controller = new InventoryController(repo);

            var dto = new InventoryAdjustDto { ProductId = 5, Quantity = 2.5m, TransactionType = "Add" };
            var result = await controller.Adjust(dto) as OkObjectResult;

            Assert.NotNull(result);
            Assert.IsType<InventoryTransactionResponseDto>(result.Value);
            var resp = (InventoryTransactionResponseDto)result.Value;
            Assert.Equal(1, resp.TransactionId);
            Assert.Equal(5, resp.ProductInstanceId);
        }
    }
}
