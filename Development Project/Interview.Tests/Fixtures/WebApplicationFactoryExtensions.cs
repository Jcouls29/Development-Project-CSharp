using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

namespace Interview.Tests
{
    internal static class WebApplicationFactoryExtensions
    {
        public static HttpClient CreateHttpsClient<TStartup>(
            this TestWebApplicationFactory<TStartup> factory,
            Action<IServiceCollection> configureServices) where TStartup : class
        {
            var configuredFactory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(configureServices);
            });

            return configuredFactory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
                BaseAddress = new Uri("https://localhost"),
            });
        }
    }
}
