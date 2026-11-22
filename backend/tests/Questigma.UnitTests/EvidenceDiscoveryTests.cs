using Questigma.Domain.Investigation;
using static Questigma.UnitTests.InvestigationTestCase;

namespace Questigma.UnitTests;

public class EvidenceDiscoveryTests
{
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
        var otherEvidenceId = Guid.NewGuid();
        var otherDeductionId = Guid.NewGuid();
        var otherSceneId = Guid.NewGuid();
        var otherCase = new GameCase(otherCaseId, [new Scene(otherSceneId, otherCaseId)],
            [new Suspect(otherSuspectId, otherCaseId)], [new Evidence(otherEvidenceId, otherCaseId, otherSceneId)],
            [new Deduction(otherDeductionId, otherCaseId, [otherEvidenceId])], otherSuspectId,
            [new AccusationRule(otherCaseId, otherSuspectId, [otherDeductionId])]);
        Assert.Throws<ArgumentException>(() => investigation.DiscoverEvidence(otherCase, FirstEvidenceId));
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
    public void InvestigationState_DoesNotExposeMutableCollections()
    {
        var investigation = NewInvestigation();
        investigation.DiscoverEvidence(Case(), FirstEvidenceId);
        Assert.Throws<NotSupportedException>(() => ((ICollection<Guid>)investigation.DiscoveredEvidenceIds).Clear());
        Assert.Throws<NotSupportedException>(() => ((IList<EvidenceDiscovered>)investigation.Events).Clear());
        Assert.Single(investigation.DiscoveredEvidenceIds);
    }
}
