using FastEndpoints;
using Game.Core.Models;

namespace Game.Features.Monsters.Endpoints;

//TODO : Write validation for all endpoints
public class CreateMonster : Endpoint<CreateMonsterRequest>
{
    private readonly IMonstersMongoRepository _monstersMongoRepository;

    public CreateMonster(IMonstersMongoRepository monstersMongoRepository)
    {
        _monstersMongoRepository = monstersMongoRepository;
    }
    public override void Configure()
    {
        Post("/monsters");
    }
    
    public override async Task HandleAsync(CreateMonsterRequest req, CancellationToken ct)
    {
        var monster = new Monster()
        {
            Name = req.Name,
            AbilityIds = req.AbilityIds,
            Stats = req.Stats,
            DropsTable = req.DropsTable
        };
        
       var result = await _monstersMongoRepository.CreateAsync(monster);

       if (result.IsFailure)
       {
           await SendAsync(result.Error.Description, int.Parse(result.Error.Code), ct);
           return;
       }
       
       await SendCreatedAtAsync<GetMonster>(new {Name = monster.Name}, result.Value, cancellation: ct);
    }
}