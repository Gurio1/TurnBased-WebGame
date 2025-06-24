using Game.Application.SharedKernel;
using Game.Core.PlayerProfile;
using Game.Core.PlayerProfile.Aggregates;
using Game.Core.PlayerProfile.ValueObjects;
using Game.Persistence.Mongo;
using MongoDB.Driver;

namespace Game.Features.Identity.CreateUser;

public sealed class CreatePlayerCommandHandler : IRequestHandler<CreatePlayerCommand, Result<string>>
{
    private readonly IMongoCollection<GamePlayer> collection;
    
    public CreatePlayerCommandHandler(IMongoCollectionProvider provider) =>
        collection = provider.GetCollection<GamePlayer>();
    
    public async Task<Result<string>> Handle(CreatePlayerCommand request, CancellationToken cancellationToken)
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
