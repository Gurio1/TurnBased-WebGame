using Game.Core.Abilities;
using Game.Core.Equipment;
using Game.Core.Models;
using Game.Features.Attributes;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using NetTopologySuite.Index.HPRtree;

namespace Game.Data.Mongo;

public class CustomDiscriminatorConvention : IDiscriminatorConvention
{
    public CustomDiscriminatorConvention(string elementName = "_t") => ElementName = elementName;
    
    public string ElementName { get; }
    
    public Type GetActualType(IBsonReader bsonReader, Type nominalType)
    {
        var bookmark = bsonReader.GetBookmark();
        bsonReader.ReadStartDocument(); // Now, start reading the document.

        var actualType = nominalType;

        // Read through the document and check if the type field is present
        while (bsonReader.ReadBsonType() != BsonType.EndOfDocument)
        {
            if (bsonReader.ReadName() == ElementName) // The field name that represents the type
            {
                string? typeName = bsonReader.ReadString(); // This is where we get the type from '_t'
                actualType = Type.GetType(typeName) ?? nominalType; // We find the type dynamically
                break; // We can break out once we find the type
            }

            bsonReader.SkipValue(); // Skip the other fields in the document
        }

        bsonReader.ReturnToBookmark(bookmark); // Return to the original bookmark to continue reading.

        return actualType;
    }

    public BsonValue GetDiscriminator(Type nominalType, Type actualType) => actualType.AssemblyQualifiedName;
}


public static class MongoDbConfig
{
    public static void RegisterDiscriminator()
    {
        var discriminatorConvention = new CustomDiscriminatorConvention();
        
        BsonSerializer.RegisterDiscriminatorConvention(typeof(Ability), discriminatorConvention);
        BsonSerializer.RegisterDiscriminatorConvention(typeof(EquipmentBase), discriminatorConvention);
        BsonSerializer.RegisterDiscriminatorConvention(typeof(Item), discriminatorConvention);
        BsonSerializer.RegisterDiscriminatorConvention(typeof(EquipmentStat), discriminatorConvention);
    }
}
