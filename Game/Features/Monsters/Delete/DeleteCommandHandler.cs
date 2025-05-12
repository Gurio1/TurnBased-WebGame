using Game.Core.Models;
using Game.Core.SharedKernel;
using Game.Persistence.Mongo;
using MongoDB.Driver;

namespace Game.Features.Monsters.Delete;

public sealed class DeleteCommandHandler : IRequestHandler<DeleteCommand, ResultWithoutValue>
{
    private readonly IMongoCollection<Monster> collection;
    
    public DeleteCommandHandler(IMongoCollectionProvider provider) => collection = provider.GetCollection<Monster>();
    
    public async Task<ResultWithoutValue> Handle(DeleteCommand request, CancellationToken cancellationToken)
    {
        var deleteResult = await collection.DeleteOneAsync(m => m.Name == request.MonsterName, cancellationToken);
        
        return deleteResult.DeletedCount == 0
            ? ResultWithoutValue.NotFound($"Can't delete monster with name '{request.MonsterName}'. Not found.")
            : ResultWithoutValue.Success();
    }
}
