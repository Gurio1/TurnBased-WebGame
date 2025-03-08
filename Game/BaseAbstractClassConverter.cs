using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Game.Core;
using Game.Core.Abilities;

namespace Game;

public class BaseAbstractClassConverter : JsonConverter<Ability>
{
    private static readonly Dictionary<string, Type> AbilityTypes = new();
    private readonly string _searchField = "TypeName";

    static BaseAbstractClassConverter()
    {
        var abilityTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false } 
                        && typeof(Ability).IsAssignableFrom(t));

        foreach (var type in abilityTypes)
        {
            AbilityTypes[type.Name] = type;
        }
    }
    
    public override Ability? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using JsonDocument doc = JsonDocument.ParseValue(ref reader);
        
        var root = doc.RootElement;

        if (!root.TryGetProperty(_searchField, out JsonElement typeNameElement))
        {
            throw new JsonException("Missing TypeName property");
        }

        var typeName = typeNameElement.GetString()!;

        if (AbilityTypes.TryGetValue(typeName, out Type? targetType))
        {
           
            return (Ability)JsonSerializer.Deserialize(root.GetRawText(), targetType, options)!;
        }
        else
        {
            throw new InvalidOperationException($"Unknown type: {typeName}");
        }
    }

    public override void Write(Utf8JsonWriter writer, Ability value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
    
}