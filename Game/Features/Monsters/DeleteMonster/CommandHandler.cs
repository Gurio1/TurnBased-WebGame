using Game.Core.Common;
using Game.Core.Models;
using Game.Data.Mongo;
using MongoDB.Driver;

namespace Game.Features.Monsters.DeleteMonster;

public sealed class CommandHandler : IRequestHandler<Command,ResultWithoutValue>
{
    private readonly IMongoCollection<Monster> collection;
    
    public CommandHandler(IMongoCollectionProvider<Monster> provider) => collection = provider.Collection;
    public async Task<ResultWithoutValue> Handle(Command request, CancellationToken cancellationToken)
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
