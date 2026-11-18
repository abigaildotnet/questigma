using System.Reflection;

namespace Questigma.UnitTests;

public class DomainDependenciesTests
{
    [Fact]
    public void DomainDoesNotReferenceOtherQuestigmaProjects()
    {
        var references = Assembly.Load("Questigma.Domain").GetReferencedAssemblies();

        Assert.DoesNotContain(references, reference =>
            reference.Name?.StartsWith("Questigma.", StringComparison.Ordinal) == true);
    }
}
