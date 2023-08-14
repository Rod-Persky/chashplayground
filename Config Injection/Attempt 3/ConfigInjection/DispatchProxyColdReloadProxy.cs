using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// Coming from https://medium.com/@nik96a/using-di-with-dispatchproxy-based-decorators-in-c-net-core-ac02f02c5fe5
// also see https://greatrexpectations.com/2018/10/25/decorators-in-net-core-with-dependency-injection

namespace ConfigInjection;

/// <summary>
/// This is the spiritual equivilent to Attempt 1/SomeRequestServiceReloadProxy
/// but with DispatchProxy which does automatic building of the class.
///
/// In this example I'm just logging if the invocation failed, and the main example
/// from @nik96a was to log every time Invoke was called.
///
/// I haven't yet figured out how to find all instances of IOptions going to the
/// proxied class but when I do, it'll be simple enough to recreate what Attempt 1 did.
/// </summary>
public class DispatchProxyColdReloadProxy<TDecorated> : DispatchProxy
{
    private TDecorated? _decorated;

    private ILogger<DispatchProxyColdReloadProxy<TDecorated>>? _logger;

    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        try
        {
            return targetMethod!.Invoke(_decorated, args);
        }
        catch (TargetInvocationException ex)
        {
            _logger!.LogError(ex.InnerException ?? ex,
                "Error during invocation of {decoratedClass}.{methodName}",
                typeof(TDecorated), targetMethod!.Name);
            throw ex.InnerException ?? ex;
        }
    }

    public static TDecorated Create(TDecorated decorated, ILogger<DispatchProxyColdReloadProxy<TDecorated>> logger)
    {
        object proxy = Create<TDecorated, DispatchProxyColdReloadProxy<TDecorated>>()!;

        ((DispatchProxyColdReloadProxy<TDecorated>)proxy!).SetParameters(decorated, logger);

        return (TDecorated)proxy;
    }

    private void SetParameters(TDecorated decorated, ILogger<DispatchProxyColdReloadProxy<TDecorated>> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _decorated = decorated ?? throw new ArgumentNullException(nameof(decorated));
    }
}


/// <summary>
/// Extension method to IServiceCollection that finds all service implimenting TInterface
/// and adds the proxy as 'middleware'.
/// </summary>
public static class ColdReloadProxyExtension
{
    public static IServiceCollection DecorateWithColdReloadProxy<TInterface, TProxy>(this IServiceCollection services)
            where TInterface : class
            where TProxy : DispatchProxy
    {
        MethodInfo? createMethod;
        try
        {
            createMethod = typeof(TProxy)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .First(info => !info.IsGenericMethod && info.ReturnType == typeof(TInterface));
        }
        catch (InvalidOperationException e)
        {
            throw new InvalidOperationException($"Looks like there is no static method in {typeof(TProxy)} " +
                                                $"which creates instance of {typeof(TInterface)} (note that this method should not be generic)", e);
        }

        var argInfos = createMethod.GetParameters();

        // Save all descriptors that needs to be decorated into a list.
        var descriptorsToDecorate = services
            .Where(s => s.ServiceType == typeof(TInterface))
            .ToList();

        if (descriptorsToDecorate.Count == 0)
        {
            throw new InvalidOperationException($"Attempted to Decorate services of type {typeof(TInterface)}, " +
                                                "but no such services are present in ServiceCollection");
        }

        foreach (var descriptor in descriptorsToDecorate)
        {
            var decorated = ServiceDescriptor.Describe(
                typeof(TInterface),
                sp =>
                {
                    var decoratorInstance = createMethod.Invoke(null,
                        argInfos.Select(
                                info => info.ParameterType == (descriptor.ServiceType ?? descriptor.ImplementationType)
                                    ? sp.CreateInstance(descriptor)
                                    : sp.GetRequiredService(info.ParameterType))
                            .ToArray());
                    return (TInterface)decoratorInstance!;
                },
                descriptor.Lifetime);

            services.Remove(descriptor);
            services.Add(decorated);
        }

        return services;
    }

    private static object CreateInstance(this IServiceProvider services, ServiceDescriptor descriptor)
    {
        if (descriptor.ImplementationInstance != null)
        {
            return descriptor.ImplementationInstance;
        }

        if (descriptor.ImplementationFactory != null)
        {
            return descriptor.ImplementationFactory(services);
        }

        return ActivatorUtilities.GetServiceOrCreateInstance(services, descriptor.ImplementationType!);
    }
}

