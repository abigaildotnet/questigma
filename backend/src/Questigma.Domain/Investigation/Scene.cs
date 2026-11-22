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
