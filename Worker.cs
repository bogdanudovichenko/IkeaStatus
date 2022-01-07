namespace IkeaBot;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IkeaStatusService _ikeaStatusService;
    private DateTimeOffset _lastRun;

    public Worker(
        ILogger<Worker> logger,
        IkeaStatusService ikeaStatusService)
    {
        _logger = logger;
        _ikeaStatusService = ikeaStatusService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            if (DateTimeOffset.Now - _lastRun < TimeSpan.FromHours(3))
            {
                await Task.Delay(TimeSpan.FromMinutes(10));
                continue;
            }

            await _ikeaStatusService.CheckProductsStatusAsync();
            _lastRun = DateTimeOffset.Now;
        }
    }
}
