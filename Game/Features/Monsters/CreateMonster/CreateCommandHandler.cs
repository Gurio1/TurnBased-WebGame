using Game.Core.Models;
using Game.Core.SharedKernel;
using Game.Data.Mongo;
using MongoDB.Driver;

namespace Game.Features.Monsters.CreateMonster;

public sealed class CreateCommandHandler : IRequestHandler<CreateCommand,Result<Monster>>
{
    private readonly IMongoCollection<Monster> collection;
    
    public CreateCommandHandler(IMongoCollectionProvider<Monster> provider) => collection = provider.Collection;
    
    public async Task<Result<Monster>> Handle(CreateCommand request, CancellationToken cancellationToken)
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
