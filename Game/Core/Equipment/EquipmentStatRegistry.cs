using System.Reflection;

namespace Game.Core.Equipment;

public static class EquipmentStatRegistry
{
    private static readonly Lazy<Dictionary<string, EquipmentStatRegistration>> Registrations = new(CreateRegistrations);

    public static IReadOnlyList<EquipmentStatDefinition> GetDefinitions() =>
        Registrations.Value.Values
            .Select(registration => registration.Definition)
            .OrderBy(definition => definition.Name)
            .ToList();

    public static bool IsSupported(string statKey) =>
        Registrations.Value.ContainsKey(statKey);

    public static EquipmentStat Create(string statKey)
    {
        if (!Registrations.Value.TryGetValue(statKey, out var registration))
            throw new InvalidOperationException($"Stat '{statKey}' is not supported.");

        return registration.Factory();
    }

    private static Dictionary<string, EquipmentStatRegistration> CreateRegistrations()
    {
        var registrations = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(type => type is { IsClass: true, IsAbstract: false }
                && typeof(EquipmentStat).IsAssignableFrom(type))
            .Select(type =>
            {
                var stat = (EquipmentStat)Activator.CreateInstance(type)!;
                var key = type.Name.EndsWith(nameof(EquipmentStat), StringComparison.Ordinal)
                    ? type.Name[..^nameof(EquipmentStat).Length]
                    : type.Name;

                return new EquipmentStatRegistration(
                    key,
                    new EquipmentStatDefinition(key, stat.Name),
                    () => (EquipmentStat)Activator.CreateInstance(type)!);
            })
            .ToDictionary(registration => registration.Key, StringComparer.OrdinalIgnoreCase);

        return registrations;
    }

    private sealed record EquipmentStatRegistration(
        string Key,
        EquipmentStatDefinition Definition,
        Func<EquipmentStat> Factory);
}
