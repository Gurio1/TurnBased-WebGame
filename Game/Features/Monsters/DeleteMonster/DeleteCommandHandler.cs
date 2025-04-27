using Game.Core.Common;
using Game.Core.Models;
using Game.Data.Mongo;
using MongoDB.Driver;

namespace Game.Features.Monsters.DeleteMonster;

public sealed class DeleteCommandHandler : IRequestHandler<DeleteCommand,ResultWithoutValue>
{
    private readonly IMongoCollection<Monster> collection;
    
    public DeleteCommandHandler(IMongoCollectionProvider<Monster> provider) => collection = provider.Collection;
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
