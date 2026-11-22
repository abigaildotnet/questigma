using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Questigma.IntegrationTests;

public class HealthEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public HealthEndpointTests(WebApplicationFactory<Program> factory) => _factory = factory.WithWebHostBuilder(builder =>
        builder.UseSetting("ConnectionStrings:Questigma",
            "Host=127.0.0.1;Port=1;Database=questigma;Username=unused;Password=unused;Timeout=1"));

    [Fact]
    public async Task HealthReturnsOk()
    {
        using var client = _factory.CreateClient();
        var response = await client.GetAsync("/health");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task DatabaseHealthReturnsUnavailableWithoutConnection()
    {
        using var client = _factory.CreateClient();
        var response = await client.GetAsync("/health/db");
        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
        Assert.DoesNotContain("Password", await response.Content.ReadAsStringAsync());
    }
}
