using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Questigma.Infrastructure.Persistence;
using Questigma.Application.Investigation;

namespace Questigma.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Questigma");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "A configuração ConnectionStrings:Questigma é obrigatória. Defina a variável de ambiente ConnectionStrings__Questigma.");
        }

        services.AddDbContext<QuestigmaDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IPublishedCaseReader, PublishedCaseReader>();
        services.AddScoped<PublishedCaseQueries>();
        return services;
    }
}
