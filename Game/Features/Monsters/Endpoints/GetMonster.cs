using System.Globalization;
using FastEndpoints;
using Game.Core;

namespace Game.Features.Monsters.Endpoints;

public class GetMonster : Endpoint<GetMonsterByNameRequest>
{
    private readonly IMonstersMongoRepository monstersMongoRepository;

    public GetMonster(IMonstersMongoRepository monstersMongoRepository) => 
        this.monstersMongoRepository = monstersMongoRepository;
    
    public override void Configure()
    {
        Get("/monsters/{MonsterName}");
        AllowAnonymous();
        Description(x => x.Accepts<GetMonsterByNameRequest>());
    }

    public override async Task HandleAsync(GetMonsterByNameRequest req, CancellationToken ct)
    {
        var monsterResult = await monstersMongoRepository.GetByNameWithAbilities(req.MonsterName);

        if (monsterResult.IsFailure)
        {
            await SendAsync(monsterResult.Error.Description,Convert.ToInt32(monsterResult.Error.Code,CultureInfo.InvariantCulture),ct);
            return;
        }

        await SendOkAsync(monsterResult.Value, ct);
    }
}
