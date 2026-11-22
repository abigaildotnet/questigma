namespace Questigma.Domain.Investigation;

internal static class DomainId
{
    internal static Guid Required(Guid id, string name) => id != Guid.Empty ? id : throw new ArgumentException("ID cannot be empty.", name);
}
