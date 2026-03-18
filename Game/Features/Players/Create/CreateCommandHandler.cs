using Game.Core.PlayerProfile.Aggregates;
using Game.Core.PlayerProfile.ValueObjects;
using Game.Persistence.Mongo;
using Game.SharedKernel;
using MongoDB.Driver;

namespace Game.Features.Players.Create;

public sealed class CreateCommandHandler(IMongoCollectionProvider provider)
    : IRequestHandler<CreateCommand, Result<string>>
{
    readonly IMongoCollection<GamePlayer> collection = provider.GetCollection<GamePlayer>();
    
    public async Task<Result<string>> Handle(CreateCommand request, CancellationToken cancellationToken)
    {
        var player = new GamePlayer
        {
            AbilityIds =
                ["0", "1"],
            Stats = new Stats
            {
                MaxHealth = 250,
                CriticalDamage = 1.3f,
                CriticalChance = 0.1f,
                Damage = 20f,
                CurrentHealth = 250f
            }
        };
        
        try
        {
            await collection.InsertOneAsync(player, cancellationToken: cancellationToken);
            return Result<string>.Success(player.Id);
        }
        catch (Exception e)
        {
            return Result<string>.Failure(e.Message);
        }
    }
}

