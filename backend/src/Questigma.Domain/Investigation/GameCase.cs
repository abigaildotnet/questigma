namespace Questigma.Domain.Investigation;

public sealed class Scene
{
    public Guid Id { get; }
    public Guid GameCaseId { get; }

    public Scene(Guid id, Guid gameCaseId)
    {
        Id = DomainId.Required(id, nameof(id));
        GameCaseId = DomainId.Required(gameCaseId, nameof(gameCaseId));
    }
}

public sealed class Suspect
{
    public Guid Id { get; }
    public Guid GameCaseId { get; }

    public Suspect(Guid id, Guid gameCaseId)
    {
        Id = DomainId.Required(id, nameof(id));
        GameCaseId = DomainId.Required(gameCaseId, nameof(gameCaseId));
    }
}

public sealed class Evidence
{
    public Guid Id { get; }
    public Guid GameCaseId { get; }
    public Guid SceneId { get; }
    public IReadOnlyList<Guid> PrerequisiteEvidenceIds { get; }

    public Evidence(Guid id, Guid gameCaseId, Guid sceneId, IEnumerable<Guid>? prerequisiteEvidenceIds = null)
    {
        Id = DomainId.Required(id, nameof(id));
        GameCaseId = DomainId.Required(gameCaseId, nameof(gameCaseId));
        SceneId = DomainId.Required(sceneId, nameof(sceneId));
        var prerequisites = (prerequisiteEvidenceIds ?? []).ToArray();
        if (prerequisites.Any(value => value == Guid.Empty || value == id) || prerequisites.Distinct().Count() != prerequisites.Length)
            throw new ArgumentException("Prerequisites must be unique, valid evidence from this case.", nameof(prerequisiteEvidenceIds));
        PrerequisiteEvidenceIds = Array.AsReadOnly(prerequisites);
    }
}

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

public sealed class GameCase
{
    private readonly Dictionary<Guid, Evidence> _evidence;
    private readonly Dictionary<Guid, Deduction> _deductions;
    private readonly HashSet<Guid> _suspectIds;
    private readonly Guid _correctSuspectId;

    public Guid Id { get; }

    public GameCase(Guid id, IEnumerable<Scene> scenes, IEnumerable<Suspect> suspects,
        IEnumerable<Evidence> evidence, IEnumerable<Deduction> deductions, Guid correctSuspectId)
    {
        Id = DomainId.Required(id, nameof(id));
        var sceneArray = (scenes ?? throw new ArgumentNullException(nameof(scenes))).ToArray();
        var suspectArray = (suspects ?? throw new ArgumentNullException(nameof(suspects))).ToArray();
        var evidenceArray = (evidence ?? throw new ArgumentNullException(nameof(evidence))).ToArray();
        var deductionArray = (deductions ?? throw new ArgumentNullException(nameof(deductions))).ToArray();
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
        _correctSuspectId = correctSuspectId;
    }

    internal Evidence GetEvidence(Guid id) => _evidence.TryGetValue(id, out var value) ? value : throw new ArgumentException("Unknown evidence for this case.", nameof(id));
    internal Deduction GetDeduction(Guid id) => _deductions.TryGetValue(id, out var value) ? value : throw new ArgumentException("Unknown deduction for this case.", nameof(id));
    internal bool ContainsSuspect(Guid id) => _suspectIds.Contains(id);
    internal bool IsCorrectSuspect(Guid id) => id == _correctSuspectId;
}

internal static class DomainId
{
    internal static Guid Required(Guid id, string name) => id != Guid.Empty ? id : throw new ArgumentException("ID cannot be empty.", name);
}
