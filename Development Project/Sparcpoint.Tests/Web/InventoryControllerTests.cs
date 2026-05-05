using System.Collections.Generic;
using System.Threading.Tasks;
using Interview.Web.Controllers;
using Interview.Web.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Sparcpoint.Inventory;
using Xunit;

namespace Sparcpoint.Tests.Web
{
    public class InventoryControllerTests
    {
        private static InventoryController Build(IInventoryRepository repo)
            => new InventoryController(repo);

        private static Mock<IInventoryRepository> MockRepo() => new Mock<IInventoryRepository>();

        // ── Add ───────────────────────────────────────────────────────────────

        [Fact]
        public async Task Add_ValidRequest_ReturnsOkWithTransactionId()
        {
            var repo = MockRepo();
            repo.Setup(r => r.AddAsync(5, 10m, null)).ReturnsAsync(42);

            var result = await Build(repo.Object).Add(new ModifyInventoryRequest
            {
                ProductInstanceId = 5,
                Quantity          = 10,
            });

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Add_NullRequest_ReturnsBadRequest()
        {
            var result = await Build(MockRepo().Object).Add(null!);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Add_ZeroQuantity_ReturnsBadRequest()
        {
            var result = await Build(MockRepo().Object).Add(
                new ModifyInventoryRequest { ProductInstanceId = 1, Quantity = 0 });

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Add_NegativeQuantity_ReturnsBadRequest()
        {
            var result = await Build(MockRepo().Object).Add(
                new ModifyInventoryRequest { ProductInstanceId = 1, Quantity = -5 });

            Assert.IsType<BadRequestObjectResult>(result);
        }

        // ── AddBulk ───────────────────────────────────────────────────────────

        [Fact]
        public async Task AddBulk_ValidRequest_ReturnsOk()
        {
            var repo = MockRepo();
            repo.Setup(r => r.AddBulkAsync(It.IsAny<IEnumerable<(int, decimal, string)>>()))
                .Returns(Task.CompletedTask);

            var result = await Build(repo.Object).AddBulk(new BulkModifyInventoryRequest
            {
                Items = new[]
                {
                    new ModifyInventoryRequest { ProductInstanceId = 1, Quantity = 5 },
                    new ModifyInventoryRequest { ProductInstanceId = 2, Quantity = 3 },
                }
            });

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task AddBulk_NullItems_ReturnsBadRequest()
        {
            var result = await Build(MockRepo().Object).AddBulk(
                new BulkModifyInventoryRequest { Items = null! });

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task AddBulk_AnyItemWithZeroQuantity_ReturnsBadRequest()
        {
            var result = await Build(MockRepo().Object).AddBulk(new BulkModifyInventoryRequest
            {
                Items = new[]
                {
                    new ModifyInventoryRequest { ProductInstanceId = 1, Quantity = 5 },
                    new ModifyInventoryRequest { ProductInstanceId = 2, Quantity = 0 }, // invalid
                }
            });

            Assert.IsType<BadRequestObjectResult>(result);
        }

        // ── Remove ────────────────────────────────────────────────────────────

        [Fact]
        public async Task Remove_ValidRequest_ReturnsOk()
        {
            var repo = MockRepo();
            repo.Setup(r => r.RemoveAsync(1, 5m, null)).Returns(Task.CompletedTask);

            var result = await Build(repo.Object).Remove(
                new ModifyInventoryRequest { ProductInstanceId = 1, Quantity = 5 });

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task Remove_NegativeQuantity_ReturnsBadRequest()
        {
            var result = await Build(MockRepo().Object).Remove(
                new ModifyInventoryRequest { ProductInstanceId = 1, Quantity = -1 });

            Assert.IsType<BadRequestObjectResult>(result);
        }

        // ── DeleteTransaction ─────────────────────────────────────────────────

        [Fact]
        public async Task DeleteTransaction_ValidId_ReturnsNoContent()
        {
            var repo = MockRepo();
            repo.Setup(r => r.DeleteTransactionAsync(99)).Returns(Task.CompletedTask);

            var result = await Build(repo.Object).DeleteTransaction(99);

            Assert.IsType<NoContentResult>(result);
            repo.Verify(r => r.DeleteTransactionAsync(99), Times.Once);
        }

        // ── GetCount ──────────────────────────────────────────────────────────

        [Fact]
        public async Task GetCount_ReturnsOkWithCount()
        {
            var repo = MockRepo();
            repo.Setup(r => r.GetCountAsync(3)).ReturnsAsync(150m);

            var result = await Build(repo.Object).GetCount(3);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(ok.Value);
        }

        // ── GetCountByAttributes ──────────────────────────────────────────────

        [Fact]
        public async Task GetCountByAttributes_ValidRequest_ReturnsOk()
        {
            var repo = MockRepo();
            repo.Setup(r => r.GetCountByAttributesAsync(It.IsAny<IDictionary<string, string>>()))
                .ReturnsAsync(75m);

            var result = await Build(repo.Object).GetCountByAttributes(new GetCountByAttributesRequest
            {
                Attributes = new Dictionary<string, string> { ["Color"] = "Blue" },
            });

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetCountByAttributes_NullAttributes_ReturnsBadRequest()
        {
            var result = await Build(MockRepo().Object).GetCountByAttributes(
                new GetCountByAttributesRequest { Attributes = null! });

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetCountByAttributes_EmptyAttributes_ReturnsBadRequest()
        {
            var result = await Build(MockRepo().Object).GetCountByAttributes(
                new GetCountByAttributesRequest { Attributes = new Dictionary<string, string>() });

            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}
