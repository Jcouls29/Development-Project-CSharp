using Microsoft.AspNetCore.Mvc.Testing;

namespace Sparcpoint.Tests;

public class ProductUnitTest(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task GetProducts()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/products");

        response.EnsureSuccessStatusCode();
    }
}
