using Game.SharedKernel.Utilities;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Game.SharedKernel.Serialization.Bson;

public sealed class CriticalDamagePercentageBsonSerializer : SerializerBase<int>
{
    public override int Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var reader = context.Reader;

        return reader.GetCurrentBsonType() switch
        {
            BsonType.Int32 => CriticalStatPercentages.NormalizeCriticalDamage(reader.ReadInt32()),
            BsonType.Int64 => CriticalStatPercentages.NormalizeCriticalDamage(reader.ReadInt64()),
            BsonType.Double => CriticalStatPercentages.NormalizeCriticalDamage(reader.ReadDouble()),
            BsonType.Decimal128 => CriticalStatPercentages.NormalizeCriticalDamage((double)reader.ReadDecimal128()),
            BsonType.Null => ReadNull(reader),
            _ => throw new FormatException($"Cannot deserialize critical damage from BSON type '{reader.GetCurrentBsonType()}'.")
        };
    }

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, int value) =>
        context.Writer.WriteInt32(CriticalStatPercentages.NormalizeCriticalDamage(value));

    private static int ReadNull(IBsonReader reader)
    {
        reader.ReadNull();
        return 0;
    }
}
