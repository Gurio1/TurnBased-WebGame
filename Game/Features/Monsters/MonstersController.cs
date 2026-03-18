using Game.Core.Models;
using Game.Features.Monsters.Create;
using Game.Features.Monsters.Delete;
using Game.Features.Monsters.Get;
using Game.SharedKernel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Game.Features.Monsters;

[Route("monsters")]
public sealed class MonstersController(IDispatcher dispatcher) : Game.Controllers.ApiControllerBase
{
    [HttpGet("{monsterName}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetMonster([FromRoute] string monsterName, CancellationToken cancellationToken)
    {
        var result = await dispatcher.DispatchAsync(new GetQuery(monsterName), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> CreateMonster([FromBody] CreateMonsterRequest request, CancellationToken cancellationToken)
    {
        var monster = new Monster
        {
            Name = request.Name,
            AbilityIds = request.AbilityIds,
            Stats = request.Stats,
            DropsTable = request.DropsTable
        };

        var result = await dispatcher.DispatchAsync(new CreateCommand(monster), cancellationToken);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetMonster), new { monsterName = result.Value.Name }, result.Value)
            : StatusCode(result.Error.Code, result.Error.Description);
    }

    [HttpDelete("{monsterName}")]
    [AllowAnonymous]
    public async Task<IActionResult> DeleteMonster([FromRoute] string monsterName, CancellationToken cancellationToken)
    {
        var result = await dispatcher.DispatchAsync(new DeleteCommand(monsterName), cancellationToken);
        return HandleResult(result, StatusCodes.Status200OK);
    }
}
