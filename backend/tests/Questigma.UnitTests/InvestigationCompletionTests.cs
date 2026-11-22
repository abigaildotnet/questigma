using Questigma.Domain.Investigation;
using static Questigma.UnitTests.InvestigationTestCase;

namespace Questigma.UnitTests;

public class InvestigationCompletionTests
{
    [Fact]
    public void Completion_StopsNarrativeMutations()
    {
        var investigation = NewInvestigation();
        var gameCase = Case();
        investigation.DiscoverEvidence(gameCase, FirstEvidenceId);
        investigation.DiscoverEvidence(gameCase, SecondEvidenceId);
        investigation.MakeDeduction(gameCase, DeductionId, [FirstEvidenceId, SecondEvidenceId]);
        investigation.Accuse(gameCase, CorrectSuspectId, DeductionId);
        Assert.Throws<InvalidOperationException>(() => investigation.DiscoverEvidence(gameCase, FirstEvidenceId));
        Assert.Throws<InvalidOperationException>(() => investigation.MakeDeduction(gameCase, DeductionId, [FirstEvidenceId, SecondEvidenceId]));
        Assert.Throws<InvalidOperationException>(() => investigation.Accuse(gameCase, WrongSuspectId, DeductionId));
    }
}
