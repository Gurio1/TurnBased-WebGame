using FastEndpoints;

namespace Game.Features.Monsters.Endpoints;

public class GetMonster : Endpoint<GetMonsterByNameRequest>
{
    private readonly IMonstersMongoRepository _monstersMongoRepository;

    public GetMonster(IMonstersMongoRepository monstersMongoRepository)
    {
        _monstersMongoRepository = monstersMongoRepository;
    }
    public override void Configure()
    {
        Get("/monsters/{MonsterName}");
        AllowAnonymous();
        Description(x => x.Accepts<GetMonsterByNameRequest>());
    }

    public override async Task HandleAsync(GetMonsterByNameRequest req, CancellationToken ct)
    {
        var monsterResult = await _monstersMongoRepository.GetByNameWithAbilities(req.MonsterName);

        if (monsterResult.IsFailure)
        {
            await SendAsync(monsterResult.Error.Description,int.Parse(monsterResult.Error.Code),ct);
            return;
        }

        await SendOkAsync(monsterResult.Value, ct);
    }
}