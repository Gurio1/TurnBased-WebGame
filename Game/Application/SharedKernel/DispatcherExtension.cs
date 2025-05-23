namespace Game.Application.SharedKernel;

public static class DispatcherExtension
{
    public static IServiceCollection RegisterDispatcher(this IServiceCollection serviceCollection)
    {
        Type[] handlerTypes = [typeof(IRequestHandler<,>), typeof(INotificationHandler<>)];
        var assembly = typeof(Program).Assembly;
        
        foreach (var type in assembly.GetTypes())
        {
            var iface = type.GetInterfaces()
                .FirstOrDefault(i =>
                    i.IsGenericType
                    && handlerTypes.Contains(i.GetGenericTypeDefinition()));
            
            if (iface != null)
                serviceCollection.AddTransient(iface, type);
        }
        
        serviceCollection.AddTransient<IDispatcher, Dispatcher>();
        
        return serviceCollection;
    }
}
