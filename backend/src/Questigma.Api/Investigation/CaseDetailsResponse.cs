namespace Questigma.Api.Investigation;

public sealed record CaseDetailsResponse(Guid Id, string Title, string Synopsis, string Introduction);
