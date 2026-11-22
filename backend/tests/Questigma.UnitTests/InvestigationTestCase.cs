using Questigma.Domain.Investigation;

namespace Questigma.UnitTests;

internal static class InvestigationTestCase
{
    internal static readonly Guid CaseId = Guid.NewGuid();
    internal static readonly Guid SceneId = Guid.NewGuid();
    internal static readonly Guid CorrectSuspectId = Guid.NewGuid();
    internal static readonly Guid WrongSuspectId = Guid.NewGuid();
    internal static readonly Guid FirstEvidenceId = Guid.NewGuid();
    internal static readonly Guid SecondEvidenceId = Guid.NewGuid();
    internal static readonly Guid DeductionId = Guid.NewGuid();
    internal static readonly Guid IrrelevantDeductionId = Guid.NewGuid();

    internal static GameCase Case() => new(
        CaseId,
        [new Scene(SceneId, CaseId)],
        [new Suspect(CorrectSuspectId, CaseId), new Suspect(WrongSuspectId, CaseId)],
        [new Evidence(FirstEvidenceId, CaseId, SceneId),
         new Evidence(SecondEvidenceId, CaseId, SceneId, [FirstEvidenceId])],
        [new Deduction(DeductionId, CaseId, [FirstEvidenceId, SecondEvidenceId]),
         new Deduction(IrrelevantDeductionId, CaseId, [FirstEvidenceId])],
        CorrectSuspectId,
        [new AccusationRule(CaseId, CorrectSuspectId, [DeductionId]),
         new AccusationRule(CaseId, WrongSuspectId, [IrrelevantDeductionId])]);

    internal static Investigation NewInvestigation() => new(Guid.NewGuid(), Guid.NewGuid(), CaseId);
}
