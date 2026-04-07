namespace Game.Application.Monsters;

public sealed record MonsterAbilityCatalogEntry(
    string Id,
    string TypeName,
    string Name,
    string ImageUrl,
    int Cooldown);
