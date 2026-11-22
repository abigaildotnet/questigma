namespace Questigma.Domain.Investigation;

public sealed class Deduction
{
    public Guid Id { get; }
    public Guid GameCaseId { get; }
    public IReadOnlyList<Guid> EvidenceIds { get; }

    public Deduction(Guid id, Guid gameCaseId, IEnumerable<Guid> evidenceIds)
    {
        Id = DomainId.Required(id, nameof(id));
        GameCaseId = DomainId.Required(gameCaseId, nameof(gameCaseId));
        var ids = (evidenceIds ?? throw new ArgumentNullException(nameof(evidenceIds))).ToArray();
        if (ids.Length == 0 || ids.Any(value => value == Guid.Empty) || ids.Distinct().Count() != ids.Length)
            throw new ArgumentException("A deduction needs a nonempty combination of distinct evidence.", nameof(evidenceIds));
        EvidenceIds = Array.AsReadOnly(ids);
    }
}
