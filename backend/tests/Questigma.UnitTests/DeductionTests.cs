using Questigma.Domain.Investigation;
using static Questigma.UnitTests.InvestigationTestCase;

namespace Questigma.UnitTests;

public class DeductionTests
{
    [Fact]
    public void Deduction_RequiresDiscoveredExactCombination()
    {
        var investigation = NewInvestigation();
        var gameCase = Case();
        Assert.Throws<InvalidOperationException>(() => investigation.MakeDeduction(gameCase, DeductionId, [FirstEvidenceId, SecondEvidenceId]));
        investigation.DiscoverEvidence(gameCase, FirstEvidenceId);
        Assert.Throws<ArgumentException>(() => investigation.MakeDeduction(gameCase, DeductionId, [FirstEvidenceId]));
        investigation.DiscoverEvidence(gameCase, SecondEvidenceId);
        Assert.Throws<InvalidOperationException>(() => investigation.MakeDeduction(gameCase, DeductionId, [SecondEvidenceId, Guid.NewGuid()]));
        Assert.Throws<ArgumentException>(() => investigation.MakeDeduction(gameCase, DeductionId, [FirstEvidenceId, FirstEvidenceId]));
        Assert.True(investigation.MakeDeduction(gameCase, DeductionId, [SecondEvidenceId, FirstEvidenceId]));
        Assert.False(investigation.MakeDeduction(gameCase, DeductionId, [FirstEvidenceId, SecondEvidenceId]));
    }
}
