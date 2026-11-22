using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Questigma.Infrastructure;
using Questigma.Infrastructure.Persistence;

namespace Questigma.IntegrationTests;

public class InfrastructureConfigurationTests
{
    [Fact]
    public void MissingConnectionStringHasClearError()
    {
        var configuration = new ConfigurationBuilder().Build();
        var exception = Assert.Throws<InvalidOperationException>(() =>
            new ServiceCollection().AddInfrastructure(configuration));

        Assert.Contains("ConnectionStrings__Questigma", exception.Message);
    }

    [Fact]
    public void ConnectionStringRegistersDbContext()
    {
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(
            new Dictionary<string, string?>
            {
                ["ConnectionStrings:Questigma"] = "Host=localhost;Database=questigma;Username=unused;Password=unused"
            }).Build();
        using var provider = new ServiceCollection()
            .AddInfrastructure(configuration)
            .BuildServiceProvider();

        Assert.NotNull(provider.GetRequiredService<QuestigmaDbContext>());
    }
}
