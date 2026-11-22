using Questigma.Domain.Investigation;

namespace Questigma.UnitTests;

public class InvestigationTests
{
    private static readonly Guid CaseId = Guid.NewGuid();
    private static readonly Guid SceneId = Guid.NewGuid();
    private static readonly Guid CorrectSuspectId = Guid.NewGuid();
    private static readonly Guid WrongSuspectId = Guid.NewGuid();
    private static readonly Guid FirstEvidenceId = Guid.NewGuid();
    private static readonly Guid SecondEvidenceId = Guid.NewGuid();
    private static readonly Guid DeductionId = Guid.NewGuid();

    private static GameCase Case() => new(
        CaseId,
        [new Scene(SceneId, CaseId)],
        [new Suspect(CorrectSuspectId, CaseId), new Suspect(WrongSuspectId, CaseId)],
        [new Evidence(FirstEvidenceId, CaseId, SceneId),
         new Evidence(SecondEvidenceId, CaseId, SceneId, [FirstEvidenceId])],
        [new Deduction(DeductionId, CaseId, [FirstEvidenceId, SecondEvidenceId])],
        CorrectSuspectId);

    private static Investigation NewInvestigation() => new(Guid.NewGuid(), Guid.NewGuid(), CaseId);

    [Fact]
    public void DiscoverEvidence_ValidAndDuplicate_EmitsOnlyOneEvent()
    {
        var investigation = NewInvestigation();
        var gameCase = Case();

        Assert.True(investigation.DiscoverEvidence(gameCase, FirstEvidenceId));
        Assert.False(investigation.DiscoverEvidence(gameCase, FirstEvidenceId));
        Assert.Single(investigation.DiscoveredEvidenceIds);
        Assert.Single(investigation.Events);
        Assert.IsType<EvidenceDiscovered>(investigation.Events[0]);
    }

    [Fact]
    public void DiscoverEvidence_RejectsUnknownOtherCaseAndMissingPrerequisite()
    {
        var investigation = NewInvestigation();
        var gameCase = Case();

        Assert.Throws<ArgumentException>(() => investigation.DiscoverEvidence(gameCase, Guid.NewGuid()));
        Assert.Throws<InvalidOperationException>(() => investigation.DiscoverEvidence(gameCase, SecondEvidenceId));
        var otherCaseId = Guid.NewGuid();
        var otherSuspectId = Guid.NewGuid();
        var otherCase = new GameCase(otherCaseId, [], [new Suspect(otherSuspectId, otherCaseId)], [], [], otherSuspectId);
        Assert.Throws<ArgumentException>(() => investigation.DiscoverEvidence(otherCase, FirstEvidenceId));
    }

    [Fact]
    public void Case_RejectsForeignOrInvalidDefinitions()
    {
        Assert.Throws<ArgumentException>(() => new GameCase(CaseId, [new Scene(SceneId, Guid.NewGuid())], [new Suspect(CorrectSuspectId, CaseId)], [], [], CorrectSuspectId));
        Assert.Throws<ArgumentException>(() => new GameCase(CaseId, [new Scene(SceneId, CaseId)], [new Suspect(CorrectSuspectId, CaseId)],
            [new Evidence(FirstEvidenceId, Guid.NewGuid(), SceneId)], [], CorrectSuspectId));
        Assert.Throws<ArgumentException>(() => new GameCase(CaseId, [new Scene(SceneId, CaseId)], [new Suspect(CorrectSuspectId, CaseId)],
            [new Evidence(FirstEvidenceId, CaseId, SceneId, [Guid.NewGuid()])], [], CorrectSuspectId));
    }

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

    [Fact]
    public void Accusation_NeedsGrounds_AndCanBeWrong()
    {
        var investigation = NewInvestigation();
        var gameCase = Case();
        Assert.Throws<InvalidOperationException>(() => investigation.Accuse(gameCase, WrongSuspectId, DeductionId));
        investigation.DiscoverEvidence(gameCase, FirstEvidenceId);
        investigation.DiscoverEvidence(gameCase, SecondEvidenceId);
        investigation.MakeDeduction(gameCase, DeductionId, [FirstEvidenceId, SecondEvidenceId]);
        var accusation = investigation.Accuse(gameCase, WrongSuspectId, DeductionId);
        Assert.False(accusation.IsCorrect);
        Assert.False(investigation.IsCompleted);
        Assert.Single(investigation.Accusations);
        Assert.True(investigation.Accuse(gameCase, CorrectSuspectId, DeductionId).IsCorrect);
        Assert.True(investigation.IsCompleted);
    }

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

    [Fact]
    public void Investigations_AreIsolatedByPlayerAndInstance()
    {
        var gameCase = Case();
        var first = NewInvestigation();
        var second = NewInvestigation();
        first.DiscoverEvidence(gameCase, FirstEvidenceId);
        Assert.NotEqual(first.PlayerId, second.PlayerId);
        Assert.Empty(second.DiscoveredEvidenceIds);
        Assert.Empty(second.Events);
    }

    [Fact]
    public void DefinitionsAndState_DoNotExposeMutableCollections()
    {
        var prerequisites = new List<Guid> { FirstEvidenceId };
        var evidence = new Evidence(SecondEvidenceId, CaseId, SceneId, prerequisites);
        prerequisites.Clear();
        Assert.Single(evidence.PrerequisiteEvidenceIds);
        Assert.Throws<NotSupportedException>(() => ((IList<Guid>)evidence.PrerequisiteEvidenceIds).Clear());

        var investigation = NewInvestigation();
        investigation.DiscoverEvidence(Case(), FirstEvidenceId);
        Assert.Throws<NotSupportedException>(() => ((ICollection<Guid>)investigation.DiscoveredEvidenceIds).Clear());
        Assert.Throws<NotSupportedException>(() => ((IList<EvidenceDiscovered>)investigation.Events).Clear());
        Assert.Single(investigation.DiscoveredEvidenceIds);
    }
}
