using Game.Core.Equipment;
using Game.Features.Equipment.Blueprints.Create;
using Game.Features.Equipment.Blueprints.Delete;
using Game.Features.Equipment.Blueprints.GetByEquipmentId;
using Game.Features.Equipment.Blueprints.Update;
using Game.SharedKernel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Game.Features.Equipment.Blueprints;

[Route("equipmentBlueprints")]
public sealed class EquipmentBlueprintsController(IDispatcher dispatcher) : Controllers.ApiControllerBase
{
    [HttpGet("{equipmentId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetBlueprintByEquipmentId([FromRoute] string equipmentId, CancellationToken cancellationToken)
    {
        var result = await dispatcher.DispatchAsync(new GetQuery(equipmentId), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> CreateBlueprint([FromBody] CreateEquipmentBlueprintRequest request, CancellationToken cancellationToken)
    {
        var result = await dispatcher.DispatchAsync(new CreateCommand(request.EquipmentId), cancellationToken);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetBlueprintByEquipmentId), new { equipmentId = result.Value.EquipmentId }, result.Value)
            : StatusCode(result.Error.Code, result.Error.Description);
    }

    [HttpPut]
    [AllowAnonymous]
    public async Task<IActionResult> UpdateBlueprint([FromBody] EquipmentBlueprint blueprint, CancellationToken cancellationToken)
    {
        var result = await dispatcher.DispatchAsync(new UpdateCommand(blueprint), cancellationToken);
        return HandleResult(result, StatusCodes.Status200OK);
    }

    [HttpDelete("{blueprintId}")]
    [AllowAnonymous]
    public async Task<IActionResult> DeleteBlueprint([FromRoute] string blueprintId, CancellationToken cancellationToken)
    {
        var result = await dispatcher.DispatchAsync(new DeleteCommand(blueprintId), cancellationToken);
        return HandleResult(result, StatusCodes.Status200OK);
    }
}
