using System.Linq.Expressions;
using System.Reflection;
using Game.Core.Craft.Components;
using Game.Core.Equipment;
using Game.Core.Marketplace;
using Game.SharedKernel.Results;

namespace Game.Core.Models;

public static class ItemCatalog
{
    private static readonly Dictionary<string, Func<Item>> ItemFactories = new(StringComparer.Ordinal);
    private static readonly IReadOnlyList<ItemCatalogEntry> Definitions;

    static ItemCatalog()
    {
        var definitions = new List<ItemCatalogEntry>();

        var itemTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(type => type is { IsClass: true, IsAbstract: false } && typeof(Item).IsAssignableFrom(type));

        foreach (var type in itemTypes)
        {
            var factory = BuildFactory(type);
            ItemFactories[type.Name] = factory;

            var item = factory();
            definitions.Add(new ItemCatalogEntry(
                type.Name,
                GetCatalogItemId(item),
                item.Name,
                item.ImageUrl,
                ResolveCategory(item),
                item is EquipmentBase));
        }

        Definitions = definitions
            .OrderBy(item => item.Category)
            .ThenBy(item => item.Name)
            .ToList();
    }

    public static IReadOnlyList<ItemCatalogEntry> GetDefinitions() => Definitions;

    public static Result<Item> Create(string typeName) =>
        ItemFactories.TryGetValue(typeName, out var factory)
            ? Result<Item>.Success(factory())
            : Result<Item>.NotFound($"Item type '{typeName}' was not found.");

    public static bool TryGetDefinition(string typeName, string itemId, out ItemCatalogEntry? definition)
    {
        definition = Definitions.FirstOrDefault(entry =>
            entry.TypeName.Equals(typeName, StringComparison.Ordinal) &&
            entry.ItemId.Equals(itemId, StringComparison.OrdinalIgnoreCase));

        return definition is not null;
    }

    private static Func<Item> BuildFactory(Type type)
    {
        var constructor = Expression.New(type);
        var converted = Expression.Convert(constructor, typeof(Item));
        return Expression.Lambda<Func<Item>>(converted).Compile();
    }

    private static string GetCatalogItemId(Item item) =>
        item is EquipmentBase equipment
            ? equipment.EquipmentId
            : item.Id;

    private static string ResolveCategory(Item item) =>
        item switch
        {
            EquipmentBase equipment => equipment.Slot,
            CraftingComponent => "Crafting",
            Currency => "Currency",
            _ => item.GetType().Name
        };
}
