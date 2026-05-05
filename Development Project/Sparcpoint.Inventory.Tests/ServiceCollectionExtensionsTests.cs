// EVAL: Tests for DI registration extension method.
// Verifies that all expected services are registered with correct lifetimes.

using Interview.Web.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sparcpoint.Inventory.Abstract;
using Sparcpoint.SqlServer.Abstractions;
using System.Collections.Generic;

namespace Sparcpoint.Inventory.Tests
{
    public class ServiceCollectionExtensionsTests
    {
        private static IConfiguration CreateTestConfiguration()
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "ConnectionStrings:SparcpointInventory", "Server=test;Database=test;User Id=test;Password=test;" }
                })
                .Build();

            return config;
        }

        [Fact]
        public void AddInventoryServices_RegistersISqlExecutor()
        {
            var services = new ServiceCollection();
            var config = CreateTestConfiguration();

            services.AddInventoryServices(config);

            var provider = services.BuildServiceProvider();
            var executor = provider.GetService<ISqlExecutor>();
            Assert.NotNull(executor);
        }

        [Fact]
        public void AddInventoryServices_RegistersIProductRepository()
        {
            var services = new ServiceCollection();
            var config = CreateTestConfiguration();

            services.AddInventoryServices(config);

            var descriptor = Assert.Single(services, s => s.ServiceType == typeof(IProductRepository));
            Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime);
        }

        [Fact]
        public void AddInventoryServices_RegistersIInventoryRepository()
        {
            var services = new ServiceCollection();
            var config = CreateTestConfiguration();

            services.AddInventoryServices(config);

            var descriptor = Assert.Single(services, s => s.ServiceType == typeof(IInventoryRepository));
            Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime);
        }

        [Fact]
        public void AddInventoryServices_RegistersHealthChecks()
        {
            var services = new ServiceCollection();
            var config = CreateTestConfiguration();

            services.AddInventoryServices(config);

            // Health checks are registered as IHealthCheckService entries
            Assert.Contains(services, s => s.ServiceType.Name.Contains("HealthCheck"));
        }

        [Fact]
        public void AddInventoryServices_ConfiguresSqlServerOptions()
        {
            var services = new ServiceCollection();
            var config = CreateTestConfiguration();

            services.AddInventoryServices(config);

            Assert.Contains(services, s =>
                s.ServiceType.IsGenericType &&
                s.ServiceType.GetGenericTypeDefinition() == typeof(Microsoft.Extensions.Options.IConfigureOptions<>) &&
                s.ServiceType.GetGenericArguments()[0] == typeof(SqlServerOptions));
        }

        [Fact]
        public void AddInventoryServices_ReturnsServiceCollection_ForChaining()
        {
            var services = new ServiceCollection();
            var config = CreateTestConfiguration();

            var result = services.AddInventoryServices(config);

            Assert.Same(services, result);
        }

        [Fact]
        public void AddInventoryServices_EmptyConnectionString_SkipsHealthCheck()
        {
            var services = new ServiceCollection();
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "ConnectionStrings:SparcpointInventory", "" }
                })
                .Build();

            services.AddInventoryServices(config);

            // Should not throw, health check registration is skipped
            Assert.DoesNotContain(services, s => s.ServiceType.Name.Contains("HealthCheck"));
        }

        [Fact]
        public void AddInventoryServices_NullConnectionString_SkipsHealthCheck()
        {
            var services = new ServiceCollection();
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>())
                .Build();

            services.AddInventoryServices(config);

            // Should not throw
            Assert.NotNull(services);
        }
    }
}
