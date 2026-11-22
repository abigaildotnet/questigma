using Microsoft.EntityFrameworkCore;

namespace Questigma.Infrastructure.Persistence;

public sealed class QuestigmaDbContext(DbContextOptions<QuestigmaDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(QuestigmaDbContext).Assembly);
    }
}
