namespace Questigma.Domain.Investigation;

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
