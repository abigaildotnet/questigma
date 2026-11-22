namespace Questigma.Application.Investigation;

public sealed class PublishedCaseQueries(IPublishedCaseReader reader)
{
    public Task<IReadOnlyList<PublishedCaseSummary>> ListAsync(CancellationToken cancellationToken) =>
        reader.ListAsync(cancellationToken);

    public Task<PublishedCaseDetails?> FindAsync(Guid id, CancellationToken cancellationToken) =>
        reader.FindAsync(id, cancellationToken);
}
