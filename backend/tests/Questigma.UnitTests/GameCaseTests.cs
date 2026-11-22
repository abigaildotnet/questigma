using Questigma.Domain.Investigation;
using static Questigma.UnitTests.InvestigationTestCase;

namespace Questigma.UnitTests;

public class GameCaseTests
{
    [Fact]
    public void Case_RejectsForeignOrInvalidDefinitions()
    {
        var suspect = new Suspect(CorrectSuspectId, CaseId);
        var deduction = new Deduction(DeductionId, CaseId, [FirstEvidenceId]);
        var rule = new AccusationRule(CaseId, CorrectSuspectId, [DeductionId]);
        Assert.Throws<ArgumentException>(() => new GameCase(CaseId, [new Scene(SceneId, Guid.NewGuid())], [suspect],
            [new Evidence(FirstEvidenceId, CaseId, SceneId)], [deduction], CorrectSuspectId, [rule]));
        Assert.Throws<ArgumentException>(() => new GameCase(CaseId, [new Scene(SceneId, CaseId)], [new Suspect(CorrectSuspectId, CaseId)],
            [new Evidence(FirstEvidenceId, Guid.NewGuid(), SceneId)], [deduction], CorrectSuspectId, [rule]));
        Assert.Throws<ArgumentException>(() => new GameCase(CaseId, [new Scene(SceneId, CaseId)], [new Suspect(CorrectSuspectId, CaseId)],
            [new Evidence(FirstEvidenceId, CaseId, SceneId, [Guid.NewGuid()])], [deduction], CorrectSuspectId, [rule]));
        Assert.Throws<ArgumentException>(() => new GameCase(CaseId, [new Scene(SceneId, CaseId)], [suspect],
            [new Evidence(FirstEvidenceId, CaseId, SceneId)], [deduction], CorrectSuspectId,
            [new AccusationRule(CaseId, CorrectSuspectId, [Guid.NewGuid()])]));
    }

    [Fact]
    public void Definitions_DoNotExposeMutableCollections()
    {
        var prerequisites = new List<Guid> { FirstEvidenceId };
        var evidence = new Evidence(SecondEvidenceId, CaseId, SceneId, prerequisites);
        prerequisites.Clear();
        Assert.Single(evidence.PrerequisiteEvidenceIds);
        Assert.Throws<NotSupportedException>(() => ((IList<Guid>)evidence.PrerequisiteEvidenceIds).Clear());

        var ruleIds = new List<Guid> { DeductionId };
        var rule = new AccusationRule(CaseId, CorrectSuspectId, ruleIds);
        ruleIds.Clear();
        Assert.Single(rule.SupportingDeductionIds);
        Assert.Throws<NotSupportedException>(() => ((IList<Guid>)rule.SupportingDeductionIds).Clear());
    }
}
