using Game.Core.PlayerProfile.Aggregates;
using Game.Persistence.Mongo;
using Game.SharedKernel;
using MongoDB.Driver;

namespace Game.Features.Players.Delete;

public sealed class DeleteCommandHandler : IRequestHandler<DeleteCommand, ResultWithoutValue>
{
    private readonly IMongoCollection<GamePlayer> collection;

    public DeleteCommandHandler(IMongoCollectionProvider provider) =>
        collection = provider.GetCollection<GamePlayer>();

    public async Task<ResultWithoutValue> Handle(DeleteCommand request, CancellationToken cancellationToken)
    {
        var deleteResult = await collection.DeleteOneAsync(p => p.Id == request.PlayerId, cancellationToken);

        return deleteResult.DeletedCount == 0
            ? ResultWithoutValue.NotFound($"Can't delete player with id '{request.PlayerId}'. Not found.")
            : ResultWithoutValue.Success();
    }
}
