using Questigma.Application.Investigation;

namespace Questigma.Api.Investigation;

public sealed record CaseSummaryResponse(Guid Id, string Title, string Synopsis);
public sealed record CaseDetailsResponse(Guid Id, string Title, string Synopsis, string Introduction);

public static class CaseEndpoints
{
    public static IEndpointRouteBuilder MapCaseEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/cases").WithTags("Cases");
        group.MapGet("", async (PublishedCaseQueries queries, CancellationToken cancellationToken) =>
            (await queries.ListAsync(cancellationToken))
                .Select(item => new CaseSummaryResponse(item.Id, item.Title, item.Synopsis)).ToArray())
            .WithName("ListPublishedCases")
            .WithSummary("List published cases without investigation spoilers")
            .Produces<CaseSummaryResponse[]>(StatusCodes.Status200OK);

        group.MapGet("/{id}", async (string id, PublishedCaseQueries queries, CancellationToken cancellationToken) =>
        {
            if (!Guid.TryParse(id, out var caseId))
                return Results.Problem(statusCode: StatusCodes.Status400BadRequest, title: "Invalid case ID");
            var item = await queries.FindAsync(caseId, cancellationToken);
            return item is null
                ? Results.Problem(statusCode: StatusCodes.Status404NotFound, title: "Case not found")
                : Results.Ok(new CaseDetailsResponse(item.Id, item.Title, item.Synopsis, item.Introduction));
        })
            .WithName("GetPublishedCase")
            .WithSummary("Get public details of a published case")
            .Produces<CaseDetailsResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);
        return endpoints;
    }
}
