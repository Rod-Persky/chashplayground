namespace ConfigInjectionCLI;

using ConfigInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


internal class Program
{
    public static void Main(string[] args)
    {
        ConfigInjection.HelloWorld.SayHello();

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
            services.Configure<SomeRequestServiceConfig>(ctx.Configuration.GetSection("SomeRequestService"));
            services.AddTransient<ISomeRequestService, SomeRequestService>().DecorateWithColdReloadProxy<ISomeRequestService, DispatchProxyColdReloadProxy<ISomeRequestService>>();
        });

        // add the service
        builder.AddConfigInjectedService();

        // run the app
        builder.Build().Run();
    }
}
