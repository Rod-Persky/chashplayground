namespace ConfigInjection;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

/// <summary>
/// Our very complex service
///
/// Takes an IOptionsMonitor which has a OnChange watcher
/// the OnChange could do whatever you need, including simply
/// updating the config as in this case.
/// </summary>
public class ConfigInjectedService : BackgroundService, IConfigInjectedService
{
    private ConfigInjectionConfig _config;
    private readonly ILogger _logger;
    private readonly ISomeRequestService _someRequestService;

    public ConfigInjectedService(IOptionsMonitor<ConfigInjectionConfig> config, ILogger<ConfigInjectedService> logger, ISomeRequestService requestService)
    {
        _logger = logger;
        _logger.LogInformation("constructing");

        _someRequestService = requestService;

        _config = config.CurrentValue;
        config.OnChange((ConfigInjectionConfig c, string? a) =>
        {
            _config = c;
            _logger.LogWarning("config chaged");
        });
    }

    public bool GetTheValue()
    {
        _logger.LogInformation("Got {}", _someRequestService.SomeValue("hello"));
        return _config.SomeParameter;
    }

    public void RunLoop(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var value = GetTheValue();
            _logger.LogInformation("Value is: {0}", value);
            Thread.Sleep(TimeSpan.FromSeconds(1));
        }
        _logger.LogInformation("Stopped");
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("starting");
        return Task.Run(() => RunLoop(stoppingToken), stoppingToken);
    }
}
