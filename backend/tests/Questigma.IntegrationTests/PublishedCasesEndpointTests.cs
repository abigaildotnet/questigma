using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Questigma.Infrastructure.Persistence;
using Questigma.Domain.Investigation;
using Testcontainers.PostgreSql;

namespace Questigma.IntegrationTests;

public sealed class PublishedCasesEndpointTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:17").Build();
    private WebApplicationFactory<Program> _factory = null!;
    private readonly Guid _publishedId = Guid.NewGuid();
    private readonly Guid _draftId = Guid.NewGuid();

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            builder.UseSetting("ConnectionStrings:Questigma", _postgres.GetConnectionString()));
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<QuestigmaDbContext>();
        await db.Database.MigrateAsync();
        db.Cases.AddRange(
            new CatalogCase(_publishedId, "Caso público", "Sinopse pública", "Introdução pública", true),
            new CatalogCase(_draftId, "Rascunho secreto", "Sinopse secreta", "Introdução secreta", false));
        await db.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        await _factory.DisposeAsync();
        await _postgres.DisposeAsync();
    }

    [Fact]
    public async Task ListReturnsOnlyPublishedCasesAndPublicFields()
    {
        using var client = _factory.CreateClient();
        using var response = await client.GetAsync("/api/v1/cases");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var item = Assert.Single(json.RootElement.EnumerateArray());
        Assert.Equal(_publishedId.ToString(), item.GetProperty("id").GetString());
        Assert.Equal("Caso público", item.GetProperty("title").GetString());
        Assert.Equal("Sinopse pública", item.GetProperty("synopsis").GetString());
        Assert.Equal(3, item.EnumerateObject().Count());
    }

    [Fact]
    public async Task DetailsReturnOnlyPublicFields()
    {
        using var client = _factory.CreateClient();
        using var response = await client.GetAsync($"/api/v1/cases/{_publishedId}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal("Introdução pública", json.RootElement.GetProperty("introduction").GetString());
        Assert.Equal(4, json.RootElement.EnumerateObject().Count());
        Assert.DoesNotContain("suspect", await response.Content.ReadAsStringAsync(), StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("evidence", await response.Content.ReadAsStringAsync(), StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("solution", await response.Content.ReadAsStringAsync(), StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task DraftAndUnknownCasesReturnSameProblemResponse(bool draft)
    {
        using var client = _factory.CreateClient();
        using var response = await client.GetAsync($"/api/v1/cases/{(draft ? _draftId : Guid.NewGuid())}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal(404, json.RootElement.GetProperty("status").GetInt32());
        Assert.DoesNotContain("Rascunho secreto", json.RootElement.ToString());
    }

    [Fact]
    public async Task InvalidIdReturnsStandardProblem()
    {
        using var client = _factory.CreateClient();
        using var response = await client.GetAsync("/api/v1/cases/not-a-guid");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task OpenApiDocumentsBothRoutesAndPublicResponseContracts()
    {
        using var client = _factory.CreateClient();
        using var response = await client.GetAsync("/openapi/v1.json");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var paths = json.RootElement.GetProperty("paths");
        Assert.True(paths.TryGetProperty("/api/v1/cases", out _));
        Assert.True(paths.TryGetProperty("/api/v1/cases/{id}", out _));
        var schemas = json.RootElement.GetProperty("components").GetProperty("schemas");
        Assert.True(schemas.TryGetProperty("CaseSummaryResponse", out _));
        Assert.True(schemas.TryGetProperty("CaseDetailsResponse", out _));
    }
}
