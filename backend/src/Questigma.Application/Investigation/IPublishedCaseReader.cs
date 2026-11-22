namespace Questigma.Application.Investigation;

public interface IPublishedCaseReader
{
    Task<IReadOnlyList<PublishedCaseSummary>> ListAsync(CancellationToken cancellationToken);
    Task<PublishedCaseDetails?> FindAsync(Guid id, CancellationToken cancellationToken);
}
