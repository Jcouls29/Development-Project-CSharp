using Microsoft.Extensions.DependencyInjection;
using Sparcpoint.Inventory.Abstractions;
using Sparcpoint.Inventory.SqlServer;
using System;
using Xunit;

namespace Sparcpoint.Inventory.Tests
{
    public class ServiceCollectionExtensionsTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void AddSqlInventoryServices_ThrowsOnBlankConnectionString(string cs)
        {
            var services = new ServiceCollection();
            Assert.Throws<ArgumentException>(() => services.AddSqlInventoryServices(cs));
        }

        [Fact]
        public void AddSqlInventoryServices_RegistersAllRepositories()
        {
            var services = new ServiceCollection();
            services.AddSqlInventoryServices("Server=.;Database=Test;Trusted_Connection=True;TrustServerCertificate=True;");
            var provider = services.BuildServiceProvider();
            Assert.NotNull(provider.GetService<IProductRepository>());
            Assert.NotNull(provider.GetService<ICategoryRepository>());
            Assert.NotNull(provider.GetService<IInventoryRepository>());
        }
    }
}
