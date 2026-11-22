namespace Questigma.Domain.Investigation;

public sealed class CatalogCase
{
    public Guid Id { get; private set; }
    public string Title { get; private set; } = null!;
    public string Synopsis { get; private set; } = null!;
    public string Introduction { get; private set; } = null!;
    public bool IsPublished { get; private set; }

    private CatalogCase() { } // EF Core

    public CatalogCase(Guid id, string title, string synopsis, string introduction, bool isPublished = false)
    {
        Id = DomainId.Required(id, nameof(id));
        Title = RequiredText(title, nameof(title));
        Synopsis = RequiredText(synopsis, nameof(synopsis));
        Introduction = RequiredText(introduction, nameof(introduction));
        IsPublished = isPublished;
    }

    public void Publish() => IsPublished = true;
    public void Unpublish() => IsPublished = false;

    private static string RequiredText(string value, string name) =>
        !string.IsNullOrWhiteSpace(value) ? value.Trim() : throw new ArgumentException("Text is required.", name);
}
