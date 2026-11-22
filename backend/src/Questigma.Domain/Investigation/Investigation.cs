namespace Questigma.Domain.Investigation;

public sealed record EvidenceDiscovered(Guid EvidenceId);
public sealed record Accusation(Guid SuspectId, Guid DeductionId, bool IsCorrect);

public sealed class Investigation
{
    private readonly HashSet<Guid> _discoveredEvidenceIds = [];
    private readonly HashSet<Guid> _deductionIds = [];
    private readonly List<Accusation> _accusations = [];
    private readonly List<EvidenceDiscovered> _events = [];

    public Guid Id { get; }
    public Guid PlayerId { get; }
    public Guid GameCaseId { get; }
    public bool IsCompleted { get; private set; }
    public IReadOnlyCollection<Guid> DiscoveredEvidenceIds => Array.AsReadOnly(_discoveredEvidenceIds.ToArray());
    public IReadOnlyCollection<Guid> DeductionIds => Array.AsReadOnly(_deductionIds.ToArray());
    public IReadOnlyList<Accusation> Accusations => _accusations.AsReadOnly();
    public IReadOnlyList<EvidenceDiscovered> Events => _events.AsReadOnly();

    public Investigation(Guid id, Guid playerId, Guid gameCaseId)
    {
        Id = DomainId.Required(id, nameof(id));
        PlayerId = DomainId.Required(playerId, nameof(playerId));
        GameCaseId = DomainId.Required(gameCaseId, nameof(gameCaseId));
    }

    public bool DiscoverEvidence(GameCase gameCase, Guid evidenceId)
    {
        EnsureActiveCase(gameCase);
        var evidence = gameCase.GetEvidence(evidenceId);
        if (_discoveredEvidenceIds.Contains(evidenceId)) return false;
        if (evidence.PrerequisiteEvidenceIds.Any(required => !_discoveredEvidenceIds.Contains(required)))
            throw new InvalidOperationException("Evidence prerequisites have not been discovered.");
        _discoveredEvidenceIds.Add(evidenceId);
        _events.Add(new EvidenceDiscovered(evidenceId));
        return true;
    }

    public bool MakeDeduction(GameCase gameCase, Guid deductionId, IEnumerable<Guid> evidenceIds)
    {
        EnsureActiveCase(gameCase);
        var deduction = gameCase.GetDeduction(deductionId);
        var submitted = (evidenceIds ?? throw new ArgumentNullException(nameof(evidenceIds))).ToArray();
        if (submitted.Any(id => !_discoveredEvidenceIds.Contains(id)))
            throw new InvalidOperationException("Deduction uses undiscovered evidence.");
        if (submitted.Length != deduction.EvidenceIds.Count || !submitted.ToHashSet().SetEquals(deduction.EvidenceIds))
            throw new ArgumentException("Evidence combination does not match this deduction.", nameof(evidenceIds));
        return _deductionIds.Add(deductionId);
    }

    public Accusation Accuse(GameCase gameCase, Guid suspectId, Guid deductionId)
    {
        EnsureActiveCase(gameCase);
        if (!gameCase.ContainsSuspect(suspectId)) throw new ArgumentException("Unknown suspect for this case.", nameof(suspectId));
        if (!_deductionIds.Contains(deductionId)) throw new InvalidOperationException("An accepted deduction is required to support an accusation.");
        var accusation = new Accusation(suspectId, deductionId, gameCase.IsCorrectSuspect(suspectId));
        _accusations.Add(accusation);
        IsCompleted = accusation.IsCorrect;
        return accusation;
    }

    private void EnsureActiveCase(GameCase gameCase)
    {
        ArgumentNullException.ThrowIfNull(gameCase);
        if (IsCompleted) throw new InvalidOperationException("The investigation is completed.");
        if (gameCase.Id != GameCaseId) throw new ArgumentException("The case does not match this investigation.", nameof(gameCase));
    }
}
