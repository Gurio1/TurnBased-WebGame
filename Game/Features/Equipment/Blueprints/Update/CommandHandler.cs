using Game.Core.Equipment;
using Game.Core.SharedKernel;
using Game.Persistence.Mongo;
using MongoDB.Driver;

namespace Game.Features.Equipment.Blueprints.Update;

public sealed class CommandHandler : IRequestHandler<Command, ResultWithoutValue>
{
    private readonly IMongoCollection<EquipmentBlueprint> collection;
    
    public CommandHandler(IMongoCollectionProvider provider) => collection = provider.GetCollection<EquipmentBlueprint>();
    
    public async Task<ResultWithoutValue> Handle(Command request, CancellationToken cancellationToken)
    {
        try
        {
            await collection.ReplaceOneAsync(x => x.Id == request.Blueprint.Id,
                request.Blueprint, cancellationToken: cancellationToken);
            
            return ResultWithoutValue.Success();
        }
        catch (Exception e)
        {
            return ResultWithoutValue.Failure(e.Message);
        }
    }
}
