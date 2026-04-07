using System.Globalization;
using Game.SharedKernel.Utilities;
using Newtonsoft.Json;

namespace Game.SharedKernel.Serialization.NewtonsoftJson;

public sealed class CriticalDamagePercentageNewtonsoftConverter : JsonConverter<int>
{
    public override int ReadJson(JsonReader reader, Type objectType, int existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
            return 0;

        if (reader.Value is null)
            throw new JsonSerializationException("Critical damage must be a number.");

        return CriticalStatPercentages.NormalizeCriticalDamage(Convert.ToDouble(reader.Value, CultureInfo.InvariantCulture));
    }

    public override void WriteJson(JsonWriter writer, int value, JsonSerializer serializer) =>
        writer.WriteValue(CriticalStatPercentages.NormalizeCriticalDamage(value));
}
