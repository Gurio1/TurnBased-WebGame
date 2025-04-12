using FastEndpoints;

namespace Game.Features.Monsters.Endpoints;

public class DeleteMonster : Endpoint<DeleteMonsterByNameRequest>
{
    private readonly IMonstersMongoRepository _monstersMongoRepository;

    public DeleteMonster(IMonstersMongoRepository monstersMongoRepository)
    {
        _monstersMongoRepository = monstersMongoRepository;
    }
    public override void Configure()
    {
        Delete("/monsters/{MonsterName}");
    }

    public override async Task HandleAsync(DeleteMonsterByNameRequest req, CancellationToken ct)
    {
        var result = await _monstersMongoRepository.RemoveAsync(req.MonsterName);

        if (result.IsFailure)
        {
            await SendAsync(result.Error.Description, int.Parse(result.Error.Code), ct);
            return;
        }

        await SendOkAsync(ct);
    }
}