using Questigma.Domain.Investigation;
using static Questigma.UnitTests.InvestigationTestCase;

namespace Questigma.UnitTests;

public class AccusationTests
{
    [Fact]
    public void Accusation_NeedsGrounds_AndCanBeWrong()
    {
        var investigation = NewInvestigation();
        var gameCase = Case();
        Assert.Throws<InvalidOperationException>(() => investigation.Accuse(gameCase, WrongSuspectId, DeductionId));
        investigation.DiscoverEvidence(gameCase, FirstEvidenceId);
        investigation.MakeDeduction(gameCase, IrrelevantDeductionId, [FirstEvidenceId]);
        investigation.DiscoverEvidence(gameCase, SecondEvidenceId);
        investigation.MakeDeduction(gameCase, DeductionId, [FirstEvidenceId, SecondEvidenceId]);
        var accusation = investigation.Accuse(gameCase, WrongSuspectId, IrrelevantDeductionId);
        Assert.False(accusation.IsCorrect);
        Assert.False(investigation.IsCompleted);
        Assert.Single(investigation.Accusations);
        Assert.True(investigation.Accuse(gameCase, CorrectSuspectId, DeductionId).IsCorrect);
        Assert.True(investigation.IsCompleted);
    }

    [Fact]
    public void Accusation_RejectsUnrelatedDeductionEvenWhenAccepted()
    {
        var investigation = NewInvestigation();
        var gameCase = Case();
        investigation.DiscoverEvidence(gameCase, FirstEvidenceId);
        investigation.MakeDeduction(gameCase, IrrelevantDeductionId, [FirstEvidenceId]);

        Assert.Throws<InvalidOperationException>(() => investigation.Accuse(gameCase, CorrectSuspectId, IrrelevantDeductionId));
        Assert.Empty(investigation.Accusations);
        Assert.False(investigation.IsCompleted);
    }
}
