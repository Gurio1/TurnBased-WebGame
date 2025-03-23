using System.Text.Json;
using System.Text.Json.Serialization;

namespace Game.Services;

public static class JsonHelper
{
    public static string SerializeWithStringEnum(object obj)
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new JsonStringEnumConverter());
        return JsonSerializer.Serialize(obj, options);
    }
}