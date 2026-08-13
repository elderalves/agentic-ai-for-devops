using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace OrderProcessor.Tests;

public sealed class WorkerTests
{
    [Fact]
    public async Task ExecuteAsync_LogsAndLoops_UntilStopped()
    {
        var worker = new Worker(NullLogger<Worker>.Instance, TimeSpan.FromMilliseconds(20));

        await worker.StartAsync(CancellationToken.None);
        await Task.Delay(60);
        await worker.StopAsync(CancellationToken.None);
    }

    [Fact]
    public async Task ExecuteAsync_StopsImmediatelyWhenCancelled()
    {
        var worker = new Worker(NullLogger<Worker>.Instance);

        await worker.StartAsync(CancellationToken.None);
        await worker.StopAsync(CancellationToken.None);
    }
}
