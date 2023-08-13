using Microsoft.Extensions.Options;

namespace ConfigInjection;

/// <summary>
/// Our very complex service
///
/// Takes an IOptionsMonitor which has a OnChange watcher
/// the OnChange could do whatever you need, including simply
/// updating the config as in this case.
/// </summary>
public class ConfigInjectedService : IConfigInjectedService
{
    private ConfigInjectionConfig _config;

    public ConfigInjectedService(IOptionsMonitor<ConfigInjectionConfig> config)
    {
        _config = config.CurrentValue;
        config.OnChange((ConfigInjectionConfig c, string? a) =>
        {
            _config = c;
            Console.WriteLine("config chaged");
        });
    }

    public bool GetTheValue()
    {
        return _config.SomeParameter;
    }
}
