public sealed class Worker(ILogger<Worker> logger, TimeSpan? delay = null) : BackgroundService
{
    private readonly ILogger<Worker> _logger = logger;
    private readonly TimeSpan _delay = delay ?? TimeSpan.FromSeconds(30);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("OrderProcessor running at: {Time}", DateTimeOffset.UtcNow);

            try
            {
                await Task.Delay(_delay, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                // Ignore cancellation during delay.
            }
        }
    }
}
