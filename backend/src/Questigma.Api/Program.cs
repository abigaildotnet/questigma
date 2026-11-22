using Questigma.Infrastructure;
using Questigma.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddInfrastructure(builder.Configuration);
var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));
if (app.Environment.IsDevelopment())
{
    app.MapGet("/health/db", async (QuestigmaDbContext db, CancellationToken cancellationToken) =>
    {
        try
        {
            return await db.Database.CanConnectAsync(cancellationToken)
                ? Results.Ok(new { status = "healthy" })
                : Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
        }
    });
}

app.Run();

public partial class Program;
