using System.Globalization;
using FastEndpoints;
using Game.Core.Models;

namespace Game.Features.Monsters.CreateMonster;

//TODO : Write validation for all endpoints
public sealed class CreateMonster : Endpoint<CreateMonsterRequest>
{
    private readonly IMonstersMongoRepository monstersMongoRepository;

    public CreateMonster(IMonstersMongoRepository monstersMongoRepository) => 
        this.monstersMongoRepository = monstersMongoRepository;
    
    public override void Configure() => 
        Post("/monsters");
    
    public override async Task HandleAsync(CreateMonsterRequest req, CancellationToken ct)
    {
        var monster = new Monster()
        {
            Name = req.Name,
            AbilityIds = req.AbilityIds,
            Stats = req.Stats,
            DropsTable = req.DropsTable
        };
        
       var result = await monstersMongoRepository.CreateAsync(monster);

       if (result.IsFailure)
       {
           await SendAsync(result.Error.Description, Convert.ToInt32(result.Error.Code,CultureInfo.InvariantCulture), ct);
           return;
       }
       
       await SendCreatedAtAsync<GetMonster.GetMonster>(new {Name = monster.Name}, result.Value, cancellation: ct);
    }
}
