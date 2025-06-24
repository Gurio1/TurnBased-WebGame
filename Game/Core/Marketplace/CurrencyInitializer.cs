using System.Reflection;

namespace Game.Core.Marketplace;

public sealed class CurrencyInitializer
{
    private static readonly List<Currency> currencies = new();
    
    static CurrencyInitializer()
    {
        var currencyTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false }
                        && typeof(Currency).IsAssignableFrom(t));
        
        foreach (var type in currencyTypes)
        {
            currencies.Add((Currency)Activator.CreateInstance(type)!);
        }
    }
    
    public static List<Currency> InitializeAllCurrencies() => currencies;
}
