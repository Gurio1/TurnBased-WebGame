using Game.Application.SharedKernel;
using Game.Core.Equipment;
using Game.Persistence.Mongo;
using MongoDB.Driver;

namespace Game.Features.Equipment.Blueprints.Create;

public sealed class CreateCommandHandler : IRequestHandler<CreateCommand, Result<EquipmentBlueprint>>
{
    private readonly IMongoCollection<EquipmentBlueprint> collection;
    
    public CreateCommandHandler(IMongoCollectionProvider provider) =>
        collection = provider.GetCollection<EquipmentBlueprint>();
    
    public async Task<Result<EquipmentBlueprint>> Handle(CreateCommand request, CancellationToken cancellationToken)
    {
        //TODO : Figure out how to deal with EquipmentStat in AttributeRange
        var template = new EquipmentBlueprint
        {
            EquipmentId = request.EquipmentId,
            AttributeRanges = new List<AttributeRange>
            {
                new() { Stat = new AttackStat(), MinValue = 5, MaxValue = 10 },
                new() { Stat = new ArmorStat(), MinValue = 10, MaxValue = 20 },
                new() { Stat = new CriticalChanceStat(), MinValue = 0.05f, MaxValue = 0.12f },
                new() { Stat = new CriticalDamageStat(), MinValue = 0.05f, MaxValue = 0.2f }
            },
            AttributeCountWeights =
                new Dictionary<string, double> { { "1", 0.4 }, { "2", 0.3 }, { "3", 0.2 }, { "4", 0.1 } }
        };
        /*var newTemplate = new EquipmentBlueprint
        {
            AttributeRanges = request.AttributeRanges,
            EquipmentId = request.EquipmentId,
            AttributeCountWeights = request.AttributeCountWeights
        };*/
        
        try
        {
            await collection.InsertOneAsync(template, cancellationToken: cancellationToken);
            return Result<EquipmentBlueprint>.Success(template);
        }
        //TODO : Should i specify exact exception? e.g MongoWriteException 
        catch (Exception e)
        {
            return Result<EquipmentBlueprint>.Failure(e.Message);
        }
    }
}
