using Game.SharedKernel.Utilities;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Game.SharedKernel.Serialization.Bson;

public sealed class CriticalChancePercentageBsonSerializer : SerializerBase<int>
{
    public override int Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var reader = context.Reader;

        return reader.GetCurrentBsonType() switch
        {
            BsonType.Int32 => CriticalStatPercentages.NormalizeCriticalChance(reader.ReadInt32()),
            BsonType.Int64 => CriticalStatPercentages.NormalizeCriticalChance(reader.ReadInt64()),
            BsonType.Double => CriticalStatPercentages.NormalizeCriticalChance(reader.ReadDouble()),
            BsonType.Decimal128 => CriticalStatPercentages.NormalizeCriticalChance((double)reader.ReadDecimal128()),
            BsonType.Null => ReadNull(reader),
            _ => throw new FormatException($"Cannot deserialize critical chance from BSON type '{reader.GetCurrentBsonType()}'.")
        };
    }

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, int value) =>
        context.Writer.WriteInt32(CriticalStatPercentages.NormalizeCriticalChance(value));

    private static int ReadNull(IBsonReader reader)
    {
        reader.ReadNull();
        return 0;
    }
}
