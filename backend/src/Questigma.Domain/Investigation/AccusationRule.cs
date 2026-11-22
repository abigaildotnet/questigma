namespace Questigma.Domain.Investigation;

public sealed class AccusationRule
{
    public Guid GameCaseId { get; }
    public Guid SuspectId { get; }
    public IReadOnlyList<Guid> SupportingDeductionIds { get; }

    public AccusationRule(Guid gameCaseId, Guid suspectId, IEnumerable<Guid> supportingDeductionIds)
    {
        GameCaseId = DomainId.Required(gameCaseId, nameof(gameCaseId));
        SuspectId = DomainId.Required(suspectId, nameof(suspectId));
        var ids = (supportingDeductionIds ?? throw new ArgumentNullException(nameof(supportingDeductionIds))).ToArray();
        if (ids.Length == 0 || ids.Any(id => id == Guid.Empty) || ids.Distinct().Count() != ids.Length)
            throw new ArgumentException("A rule needs distinct supporting deductions.", nameof(supportingDeductionIds));
        SupportingDeductionIds = Array.AsReadOnly(ids);
    }
}
