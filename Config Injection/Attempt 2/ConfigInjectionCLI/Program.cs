namespace ConfigInjectionCLI;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ConfigInjection;



internal class Program
{
    public static void Main(string[] args)
    {
        // Build that DI!
        var builder = Host.CreateDefaultBuilder(args);

        // pin lifetime to the console and use the current working directory
        builder.UseConsoleLifetime();
        builder.UseContentRoot(Directory.GetCurrentDirectory());

        // Add the configuration
        builder.ConfigureAppConfiguration(configuration =>
        {
            configuration.SetBasePath(Directory.GetCurrentDirectory());
            configuration.AddIniFile("config.ini", optional: true, reloadOnChange: true);
            configuration.AddJsonFile("config.json", optional: true, reloadOnChange: true);
        });

        // Add options
        builder.ConfigureServices((ctx, services) =>
        {
            services.AddOptions();

            // Our request service
            services.Configure<SomeRequestServiceConfig>(ctx.Configuration.GetSection("SomeRequestService"));
            services.AddTransient<ISomeRequestService, SomeRequestServiceReloadProxy>();
        });

        // add the main service
        builder.AddConfigInjectedService();

        // run the app
        builder.Build().Run();
    }
}
