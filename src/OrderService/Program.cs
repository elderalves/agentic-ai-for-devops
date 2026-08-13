using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(HealthEndpoints.GetHealthResponse()));

app.MapGet("/version", () => Results.Ok(VersionEndpoints.GetVersionResponse()));

app.MapGet("/api/orders", () =>
{
    var orders = new List<OrderDto>
    {
        new("ORD-1001", "Pending", 125.00m),
        new("ORD-1002", "Shipped", 89.50m),
        new("ORD-1003", "Delivered", 42.25m)
    };

    return Results.Ok(orders);
});

app.Run();

public sealed record HealthResponse(string Status);

public static class HealthEndpoints
{
    public static HealthResponse GetHealthResponse() => new("ok");
}

public sealed record VersionResponse(string Version);

public static class VersionEndpoints
{
    public static VersionResponse GetVersionResponse()
    {
        var informationalVersion = typeof(Program).Assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;

        var version = informationalVersion?.Split('+')[0] ?? "0.0.0";

        return new VersionResponse(version);
    }
}

public partial class Program { }
