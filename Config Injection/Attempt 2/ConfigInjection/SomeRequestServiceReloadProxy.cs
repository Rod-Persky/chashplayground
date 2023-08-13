namespace ConfigInjection;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Auto reloading proxy for SomeRequestService
/// This is a proxy class that builds SomeRequestService and then rebuilds
/// it every time the config changes.
/// 
/// The reason to be a proxy is because other services actually depend on
/// ISomeRequestService being around forever, but if we're going to reload
/// it then we need to control access to the actual implimentation.
///
/// Notes:
///
/// - I assume that this is thread safe, as should some call in ISomeRequestService be
///   running across a reload a refcounter will know how to deal with it.
/// - I'm not explicitely disposing the SomeRequestService, which would be a problem if
///   it actually required dispose. But I'm leaning on the first note, that the garbage
///   collector should know.
/// </summary>
public class SomeRequestServiceReloadProxy : ISomeRequestService
{
    private SomeRequestService _service;
    private ILogger<SomeRequestService> _logger;
    private IOptionsMonitor<SomeRequestServiceConfig> _config;

    public SomeRequestServiceReloadProxy(ILogger<SomeRequestService> logger, IOptionsMonitor<SomeRequestServiceConfig> config)
    {
        _logger = logger;
        _config = config;
        _service = new SomeRequestService(logger, config);
        config.OnChange(ReloadWithNewConfig);
    }

    private void ReloadWithNewConfig(SomeRequestServiceConfig config, string? s)
    {
        _service = new SomeRequestService(_logger, _config);
    }

    public string SomeValue(string query) => _service.SomeValue(query);
}
