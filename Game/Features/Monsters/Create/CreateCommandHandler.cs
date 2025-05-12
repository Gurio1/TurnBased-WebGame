using Game.Core.Models;
using Game.Core.SharedKernel;
using Game.Persistence.Mongo;
using MongoDB.Driver;

namespace Game.Features.Monsters.Create;

public sealed class CreateCommandHandler : IRequestHandler<CreateCommand, Result<Monster>>
{
    private readonly IMongoCollection<Monster> collection;
    
    public CreateCommandHandler(IMongoCollectionProvider provider) => collection = provider.GetCollection<Monster>();
    
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
