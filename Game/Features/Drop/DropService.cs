using Game.Core;
using Game.Core.Equipment;
using Game.Core.Models;
using Game.Features.Equipment;
using Game.Shared;
using NetTopologySuite.Index.HPRtree;

namespace Game.Features.Drop;

public class DropService : IDropService
{
    private readonly EquipmentGenerator _equipmentGenerator;

    public DropService(EquipmentGenerator equipmentGenerator)
    {
        _equipmentGenerator = equipmentGenerator;
    }


    public async Task<Result<Item?>> GenerateDrop(Monster monster)
    {
        var random = (float)RandomHelper.Instance.NextDouble();
        var comulativeRate = 0f;
        foreach (var pair in monster.DropsTable)
        {
            comulativeRate += pair.Value;
            if (!(random <= comulativeRate)) continue;
            
            var result =  await _equipmentGenerator.GenerateEquipment(pair.Key);

            return (result.IsSuccess
                ? Result<Item>.Success(result.Value)
                : result.AsError<Item>())!;
        }

        return Result<Item?>.Success(null);
    }
}