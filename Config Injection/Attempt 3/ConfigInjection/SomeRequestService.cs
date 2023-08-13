namespace ConfigInjection;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

/// <summary>
/// SomeRequestService is another simple service, it has a config and
/// uses it. The service has no idea it will be torn down if the config
/// changes.
///
/// It's also a but awkward that I'm giving it a IOptionsMonitor, but
/// that's... because I'm lazy
/// </summary>
public class SomeRequestService : ISomeRequestService
{
    private readonly ILogger _logger;
    private readonly SomeRequestServiceConfig _config;

    public SomeRequestService(ILogger<SomeRequestService> logger, IOptionsMonitor<SomeRequestServiceConfig> config)
    {
        _logger = logger;
        _logger.LogInformation("constructed");

        _config = config.CurrentValue;
    }

    public string SomeValue(string query)
    {
        _logger.LogInformation("Got query: {}", query);
        return _config.Message ?? "no message";
    }
}
