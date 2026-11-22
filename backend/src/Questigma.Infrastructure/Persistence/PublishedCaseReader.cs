using Microsoft.EntityFrameworkCore;
using Questigma.Application.Investigation;

namespace Questigma.Infrastructure.Persistence;

public sealed class PublishedCaseReader(QuestigmaDbContext db) : IPublishedCaseReader
{
    public async Task<IReadOnlyList<PublishedCaseSummary>> ListAsync(CancellationToken cancellationToken) =>
        await db.Cases.AsNoTracking().Where(item => item.IsPublished).OrderBy(item => item.Title)
            .ThenBy(item => item.Id)
            .Select(item => new PublishedCaseSummary(item.Id, item.Title, item.Synopsis))
            .ToListAsync(cancellationToken);

    public Task<PublishedCaseDetails?> FindAsync(Guid id, CancellationToken cancellationToken) =>
        db.Cases.AsNoTracking().Where(item => item.IsPublished && item.Id == id)
            .Select(item => new PublishedCaseDetails(item.Id, item.Title, item.Synopsis, item.Introduction))
            .SingleOrDefaultAsync(cancellationToken);
}
