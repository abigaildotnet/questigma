namespace Questigma.Application.Investigation;

public sealed record PublishedCaseSummary(Guid Id, string Title, string Synopsis);
public sealed record PublishedCaseDetails(Guid Id, string Title, string Synopsis, string Introduction);

public interface IPublishedCaseReader
{
    Task<IReadOnlyList<PublishedCaseSummary>> ListAsync(CancellationToken cancellationToken);
    Task<PublishedCaseDetails?> FindAsync(Guid id, CancellationToken cancellationToken);
}

public sealed class PublishedCaseQueries(IPublishedCaseReader reader)
{
    public Task<IReadOnlyList<PublishedCaseSummary>> ListAsync(CancellationToken cancellationToken) =>
        reader.ListAsync(cancellationToken);

    public Task<PublishedCaseDetails?> FindAsync(Guid id, CancellationToken cancellationToken) =>
        reader.FindAsync(id, cancellationToken);
}
