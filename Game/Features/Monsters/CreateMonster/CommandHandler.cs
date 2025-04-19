using Game.Core.Common;
using Game.Core.Models;
using Game.Data.Mongo;
using MongoDB.Driver;

namespace Game.Features.Monsters.CreateMonster;

public sealed class CommandHandler : IRequestHandler<Command,Result<Monster>>
{
    private readonly IMongoCollection<Monster> collection;
    
    public CommandHandler(IMongoCollectionProvider<Monster> provider) => collection = provider.Collection;
    
    public async Task<Result<Monster>> Handle(Command request, CancellationToken cancellationToken)
    {
        try
        {
            await collection.InsertOneAsync(request.Monster, cancellationToken: cancellationToken);
            return Result<Monster>.Success(request.Monster);
        }
        catch (Exception e)
        {
            return Result<Monster>.Failure(e.Message);
        }
    }
}
