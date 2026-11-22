using Microsoft.EntityFrameworkCore;
using Questigma.Domain.Investigation;

namespace Questigma.Infrastructure.Persistence;

public sealed class QuestigmaDbContext(DbContextOptions<QuestigmaDbContext> options) : DbContext(options)
{
    public DbSet<CatalogCase> Cases => Set<CatalogCase>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(QuestigmaDbContext).Assembly);
    }
}
