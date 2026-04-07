using System.Text.Json;
using System.Text.Json.Serialization;
using Game.SharedKernel.Utilities;

namespace Game.SharedKernel.Serialization.SystemTextJson;

public sealed class CriticalChancePercentageJsonConverter : JsonConverter<int>
{
    public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            if (reader.TryGetInt32(out var intValue))
                return CriticalStatPercentages.NormalizeCriticalChance(intValue);

            return CriticalStatPercentages.NormalizeCriticalChance(reader.GetDouble());
        }

        if (reader.TokenType == JsonTokenType.Null)
            return 0;

        throw new JsonException("Critical chance must be a number.");
    }

    public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options) =>
        writer.WriteNumberValue(CriticalStatPercentages.NormalizeCriticalChance(value));
}
