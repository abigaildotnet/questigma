namespace Questigma.Domain.Investigation;

public sealed class GameCase
{
    private readonly Dictionary<Guid, Evidence> _evidence;
    private readonly Dictionary<Guid, Deduction> _deductions;
    private readonly HashSet<Guid> _suspectIds;
    private readonly Dictionary<Guid, AccusationRule> _accusationRules;
    private readonly Guid _correctSuspectId;

    public Guid Id { get; }

    public GameCase(Guid id, IEnumerable<Scene> scenes, IEnumerable<Suspect> suspects,
        IEnumerable<Evidence> evidence, IEnumerable<Deduction> deductions, Guid correctSuspectId,
        IEnumerable<AccusationRule> accusationRules)
    {
        Id = DomainId.Required(id, nameof(id));
        var sceneArray = (scenes ?? throw new ArgumentNullException(nameof(scenes))).ToArray();
        var suspectArray = (suspects ?? throw new ArgumentNullException(nameof(suspects))).ToArray();
        var evidenceArray = (evidence ?? throw new ArgumentNullException(nameof(evidence))).ToArray();
        var deductionArray = (deductions ?? throw new ArgumentNullException(nameof(deductions))).ToArray();
        var ruleArray = (accusationRules ?? throw new ArgumentNullException(nameof(accusationRules))).ToArray();
        if (sceneArray.Any(item => item is null || item.GameCaseId != id) || sceneArray.Select(item => item.Id).Distinct().Count() != sceneArray.Length ||
            suspectArray.Any(item => item is null || item.GameCaseId != id) || suspectArray.Select(item => item.Id).Distinct().Count() != suspectArray.Length ||
            evidenceArray.Any(item => item is null || item.GameCaseId != id) || evidenceArray.Select(item => item.Id).Distinct().Count() != evidenceArray.Length ||
            deductionArray.Any(item => item is null || item.GameCaseId != id) || deductionArray.Select(item => item.Id).Distinct().Count() != deductionArray.Length)
            throw new ArgumentException("Case definitions must belong to this case and have unique IDs.");

        var sceneIds = sceneArray.Select(item => item.Id).ToHashSet();
        _suspectIds = suspectArray.Select(item => item.Id).ToHashSet();
        _evidence = evidenceArray.ToDictionary(item => item.Id);
        _deductions = deductionArray.ToDictionary(item => item.Id);
        if (!_suspectIds.Contains(correctSuspectId))
            throw new ArgumentException("The correct suspect must belong to this case.", nameof(correctSuspectId));
        if (evidenceArray.Any(item => !sceneIds.Contains(item.SceneId) || item.PrerequisiteEvidenceIds.Any(required => !_evidence.ContainsKey(required))) ||
            deductionArray.Any(item => item.EvidenceIds.Any(required => !_evidence.ContainsKey(required))))
            throw new ArgumentException("Evidence, prerequisites and deductions must reference members of this case.");
        if (ruleArray.Any(rule => rule is null || rule.GameCaseId != id || !_suspectIds.Contains(rule.SuspectId) ||
                rule.SupportingDeductionIds.Any(deductionId => !_deductions.ContainsKey(deductionId))) ||
            ruleArray.Select(rule => rule.SuspectId).Distinct().Count() != ruleArray.Length ||
            ruleArray.Length != _suspectIds.Count)
            throw new ArgumentException("Each suspect needs one rule supported by deductions from this case.", nameof(accusationRules));
        _accusationRules = ruleArray.ToDictionary(rule => rule.SuspectId);
        _correctSuspectId = correctSuspectId;
    }

    internal Evidence GetEvidence(Guid id) => _evidence.TryGetValue(id, out var value) ? value : throw new ArgumentException("Unknown evidence for this case.", nameof(id));
    internal Deduction GetDeduction(Guid id) => _deductions.TryGetValue(id, out var value) ? value : throw new ArgumentException("Unknown deduction for this case.", nameof(id));
    internal bool ContainsSuspect(Guid id) => _suspectIds.Contains(id);
    internal bool SupportsAccusation(Guid suspectId, Guid deductionId) =>
        _accusationRules[suspectId].SupportingDeductionIds.Contains(deductionId);
    internal bool IsCorrectSuspect(Guid id) => id == _correctSuspectId;
}
