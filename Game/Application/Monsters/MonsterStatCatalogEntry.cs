namespace Game.Application.Monsters;

public sealed record MonsterStatCatalogEntry(
    string Key,
    string Name,
    string ValueType,
    double DefaultValue);
