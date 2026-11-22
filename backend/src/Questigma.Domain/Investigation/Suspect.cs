namespace Questigma.Domain.Investigation;

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
