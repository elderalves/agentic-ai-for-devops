using Xunit;

namespace OrderService.Tests;

public sealed class HealthEndpointsTests
{
    [Fact]
    public void GetHealthResponse_ReturnsOkStatus()
    {
        var response = HealthEndpoints.GetHealthResponse();

        Assert.Equal("ok", response.Status);
    }

    [Fact]
    public void GetVersionResponse_ReturnsVersion()
    {
        var response = VersionEndpoints.GetVersionResponse();

        Assert.Equal("1.0.0", response.Version);
    }
}
