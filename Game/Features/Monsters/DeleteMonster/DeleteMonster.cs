using System.Globalization;
using FastEndpoints;

namespace Game.Features.Monsters.DeleteMonster;

public sealed class DeleteMonster : Endpoint<DeleteMonsterByNameRequest>
{
    private readonly IMonstersMongoRepository monstersMongoRepository;

    public DeleteMonster(IMonstersMongoRepository monstersMongoRepository) => 
        this.monstersMongoRepository = monstersMongoRepository;
    
    public override void Configure() => 
        Delete("/monsters/{MonsterName}");
    
    public override async Task HandleAsync(DeleteMonsterByNameRequest req, CancellationToken ct)
    {
        var result = await monstersMongoRepository.RemoveAsync(req.MonsterName);

        if (result.IsFailure)
        {
            await SendAsync(result.Error.Description, Convert.ToInt32(result.Error.Code,CultureInfo.InvariantCulture), ct);
            return;
        }

        await SendOkAsync(ct);
    }
}
