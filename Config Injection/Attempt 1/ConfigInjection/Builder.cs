namespace ConfigInjection;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

/// <summary>
/// Add our service to the service collection.
/// This only really applies to a IApplicationBuilder
/// </summary>
public static class ConfigInjectedServiceCollectionExtensions
{
    public static IServiceCollection AddConfigInjectedService(this IServiceCollection collection, IConfiguration config)
    {
        if (collection == null) throw new ArgumentNullException(nameof(collection));
        if (config == null) throw new ArgumentNullException(nameof(config));

        collection.Configure<ConfigInjectionConfig>(config);
        return collection.AddTransient<IConfigInjectedService, ConfigInjectedService>();
    }
}
