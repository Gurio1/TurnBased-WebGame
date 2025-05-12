using Game.Core.Models;
using Game.Core.SharedKernel;
using Game.Persistence.Mongo;
using MongoDB.Driver;

namespace Game.Features.Monsters.DeleteMonster;

public sealed class DeleteCommandHandler : IRequestHandler<DeleteCommand,ResultWithoutValue>
{
    private readonly IMongoCollection<Monster> collection;
    
    public DeleteCommandHandler(IMongoCollectionProvider provider) => collection = provider.GetCollection<Monster>();
    public async Task<ResultWithoutValue> Handle(DeleteCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await collection.DeleteOneAsync(m => m.Name == request.MonsterName, cancellationToken: cancellationToken);
            return ResultWithoutValue.Success();
        }
        catch (Exception e)
        {
            return ResultWithoutValue.Failure(e.Message);
        }
    }
}
