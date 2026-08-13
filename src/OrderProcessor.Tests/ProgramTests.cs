using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace OrderProcessor.Tests;

public sealed class ProgramTests
{
    [Fact]
    public void CreateHostBuilder_RegistersWorker()
    {
        using var host = Program.CreateHostBuilder([]).Build();

        var services = host.Services.GetServices<IHostedService>();

        Assert.Contains(services, s => s is Worker);
    }
}
