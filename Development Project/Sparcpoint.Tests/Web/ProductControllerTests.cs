using System.Collections.Generic;
using System.Threading.Tasks;
using Interview.Web.Controllers;
using Interview.Web.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Sparcpoint.Inventory;
using Sparcpoint.Inventory.Models;
using Xunit;

namespace Sparcpoint.Tests.Web
{
    public class ProductControllerTests
    {
        private static ProductController Build(IProductRepository repo)
            => new ProductController(repo);

        private static Mock<IProductRepository> MockRepo() => new Mock<IProductRepository>();

        // ── GetById ───────────────────────────────────────────────────────────

        [Fact]
        public async Task GetById_ProductExists_ReturnsOkWithProduct()
        {
            var product = new ProductEntry { InstanceId = 1, Name = "Widget" };
            var repo = MockRepo();
            repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);

            var result = await Build(repo.Object).GetById(1);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Same(product, ok.Value);
        }

        [Fact]
        public async Task GetById_ProductNotFound_ReturnsNotFound()
        {
            var repo = MockRepo();
            repo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((ProductEntry?)null);

            var result = await Build(repo.Object).GetById(99);

            Assert.IsType<NotFoundResult>(result);
        }

        // ── Search (GET) ──────────────────────────────────────────────────────

        [Fact]
        public async Task Search_ReturnsOkWithResults()
        {
            var products = new List<ProductEntry>
            {
                new ProductEntry { InstanceId = 1, Name = "Widget" },
                new ProductEntry { InstanceId = 2, Name = "Gadget" },
            };
            var repo = MockRepo();
            repo.Setup(r => r.SearchAsync(It.IsAny<ProductSearchFilter>())).ReturnsAsync(products);

            var result = await Build(repo.Object).Search("Widget", System.Array.Empty<int>());

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Same(products, ok.Value);
        }

        [Fact]
        public async Task Search_PassesNameFilterToRepository()
        {
            var repo = MockRepo();
            repo.Setup(r => r.SearchAsync(It.IsAny<ProductSearchFilter>()))
                .ReturnsAsync(new List<ProductEntry>());

            await Build(repo.Object).Search("Widget", System.Array.Empty<int>());

            repo.Verify(r => r.SearchAsync(
                It.Is<ProductSearchFilter>(f => f.Name == "Widget")), Times.Once);
        }

        // ── AdvancedSearch (POST) ─────────────────────────────────────────────

        [Fact]
        public async Task AdvancedSearch_WithAttributes_PassesFilterToRepository()
        {
            var repo = MockRepo();
            repo.Setup(r => r.SearchAsync(It.IsAny<ProductSearchFilter>()))
                .ReturnsAsync(new List<ProductEntry>());

            var request = new SearchProductsRequest
            {
                Name       = "Widget",
                CategoryIds = new[] { 1, 2 },
                Attributes = new Dictionary<string, string> { ["Color"] = "Blue" },
            };

            await Build(repo.Object).AdvancedSearch(request);

            repo.Verify(r => r.SearchAsync(It.Is<ProductSearchFilter>(f =>
                f.Name == "Widget" &&
                f.CategoryIds!.Length == 2 &&
                f.Attributes!["Color"] == "Blue")), Times.Once);
        }

        // ── Create ────────────────────────────────────────────────────────────

        [Fact]
        public async Task Create_ValidRequest_ReturnsCreatedAtAction()
        {
            var repo = MockRepo();
            repo.Setup(r => r.AddAsync(It.IsAny<ProductEntry>())).ReturnsAsync(7);

            var result = await Build(repo.Object).Create(new CreateProductRequest { Name = "Widget" });

            var created = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(ProductController.GetById), created.ActionName);
        }

        [Fact]
        public async Task Create_ValidRequest_CallsRepositoryWithCorrectName()
        {
            var repo = MockRepo();
            repo.Setup(r => r.AddAsync(It.IsAny<ProductEntry>())).ReturnsAsync(1);

            await Build(repo.Object).Create(new CreateProductRequest
            {
                Name        = "Widget",
                Description = "A fine widget",
            });

            repo.Verify(r => r.AddAsync(It.Is<ProductEntry>(p =>
                p.Name == "Widget" && p.Description == "A fine widget")), Times.Once);
        }

        [Fact]
        public async Task Create_NullRequest_ReturnsBadRequest()
        {
            var result = await Build(MockRepo().Object).Create(null!);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Create_EmptyName_ReturnsBadRequest()
        {
            var result = await Build(MockRepo().Object).Create(new CreateProductRequest { Name = "" });

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Create_WhitespaceName_ReturnsBadRequest()
        {
            var result = await Build(MockRepo().Object).Create(new CreateProductRequest { Name = "   " });

            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}
