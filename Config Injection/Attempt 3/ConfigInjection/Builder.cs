namespace ConfigInjection;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

/// <summary>
/// Build our main service, similar to Attempt 1, but now
/// uses the IHostBuilder and self registers it's config
/// class.
/// </summary>
public static class ConfigInjectedServiceCollectionExtensions
{
    public static IHostBuilder AddConfigInjectedService(this IHostBuilder builder)
    {
        builder.ConfigureServices((ctx, services) =>
        {
            services.Configure<ConfigInjectionConfig>(ctx.Configuration.GetSection("ConfigInjectedService"));
            services.AddHostedService<ConfigInjectedService>();
        });

        return builder;
    }
}
