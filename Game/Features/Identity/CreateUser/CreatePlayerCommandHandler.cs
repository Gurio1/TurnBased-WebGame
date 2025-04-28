using Game.Core.Models;
using Game.Core.SharedKernel;
using Game.Data.Mongo;
using MongoDB.Driver;

namespace Game.Features.Identity.CreateUser;

public sealed class CreatePlayerCommandHandler : IRequestHandler<CreatePlayerCommand,Result<string>>
{
    private readonly IMongoCollection<Player> collection;
    
    public CreatePlayerCommandHandler(IMongoCollectionProvider<Player> provider) => collection = provider.Collection;
    
    public async Task<Result<string>> Handle(CreatePlayerCommand request, CancellationToken cancellationToken)
    {
        var player = new Player
        {
            AbilityIds =
                ["0", "1", "2"],
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
