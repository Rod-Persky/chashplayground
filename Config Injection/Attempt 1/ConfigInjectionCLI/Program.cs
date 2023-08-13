namespace ConfigInjectionCLI;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using ConfigInjection;

internal class Program
{
    public static void Main(string[] args)
    {
        // Configuration handler is setup and loads a simple ini file
        var configBuilder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddIniFile("config.ini", reloadOnChange: true, optional: false);
        var config = configBuilder.Build();

        // Create a empty service collection and add the Options resolver service
        var services = new ServiceCollection();
        services.AddOptions();

        // Add our service
        services.AddConfigInjectedService(config.GetSection("ConfigInjectedService"));

        // Build our service and get it
        var Container = services.BuildServiceProvider();
        var service = Container.GetService<IConfigInjectedService>() ?? throw new Exception();

        // Run forever!
        while (true)
        {
            var value = service.GetTheValue();
            Console.WriteLine($"Value is: {value}");
            Thread.Sleep(TimeSpan.FromSeconds(1));
        }

    }
}
