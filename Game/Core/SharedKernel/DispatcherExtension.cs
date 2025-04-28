namespace Game.Core.SharedKernel;

public static class DispatcherExtension
{
    public static IServiceCollection RegisterDispatcher(this IServiceCollection serviceCollection)
    {
        var handlerType = typeof(IRequestHandler<,>);
        var assembly = typeof(Program).Assembly;
        
        foreach (var type in assembly.GetTypes())
        {
            var iface = type.GetInterfaces()
                .FirstOrDefault(i =>
                    i.IsGenericType
                    && i.GetGenericTypeDefinition() == handlerType);
            
            if (iface != null)
                serviceCollection.AddTransient(iface, type);
        }
        
        serviceCollection.AddTransient<IDispatcher, Dispatcher>();
        
        return serviceCollection;
    }
}
