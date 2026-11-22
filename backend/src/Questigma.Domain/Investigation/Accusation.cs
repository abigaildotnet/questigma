namespace Questigma.Domain.Investigation;

public sealed record Accusation(Guid SuspectId, Guid DeductionId, bool IsCorrect);
