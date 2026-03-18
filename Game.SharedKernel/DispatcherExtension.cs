using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Game.SharedKernel;

public static class DispatcherExtension
{
    public static IServiceCollection RegisterDispatcher(
        this IServiceCollection serviceCollection,
        params Assembly[] assembliesToScan)
    {
        ArgumentNullException.ThrowIfNull(assembliesToScan);
        if (assembliesToScan.Length == 0)
            throw new ArgumentException("At least one assembly must be provided.", nameof(assembliesToScan));

        var handlerTypes = new[] { typeof(IRequestHandler<,>), typeof(INotificationHandler<>) };
        foreach (var assembly in assembliesToScan.Distinct())
        {
            foreach (var type in assembly.GetTypes())
            {
                var iface = type.GetInterfaces()
                    .FirstOrDefault(i =>
                        i.IsGenericType
                        && handlerTypes.Contains(i.GetGenericTypeDefinition()));

                if (iface != null)
                    serviceCollection.AddTransient(iface, type);
            }
        }

        serviceCollection.AddTransient<IDispatcher, Dispatcher>();

        return serviceCollection;
    }

    public static IServiceCollection RegisterDispatcher(
        this IServiceCollection serviceCollection,
        params Type[] markerTypes)
    {
        ArgumentNullException.ThrowIfNull(markerTypes);

        var assemblies = markerTypes.Select(type => type.Assembly).Distinct().ToArray();
        return serviceCollection.RegisterDispatcher(assemblies);
    }
}
