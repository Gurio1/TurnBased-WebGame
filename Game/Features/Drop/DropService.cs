using Game.Core.Common;
using Game.Core.Models;
using Game.Features.Equipment;
using Game.Utilities;

namespace Game.Features.Drop;

public class DropService : IDropService
{
    private readonly EquipmentGenerator equipmentGenerator;
    
    public DropService(EquipmentGenerator equipmentGenerator)
        => this.equipmentGenerator = equipmentGenerator;
    
    public async Task<Result<Item?>> GenerateDrop(Monster monster)
    {
        float random = (float)RandomHelper.Instance.NextDouble();
        float cumulativeRate = 0f;
        foreach (var pair in monster.DropsTable)
        {
            cumulativeRate += pair.Value;
            if (!(random <= cumulativeRate)) continue;
            
            var result = await equipmentGenerator.GenerateEquipment(pair.Key);
            
            return (result.IsSuccess
                ? Result<Item>.Success(result.Value)
                : result.AsError<Item>())!;
        }
        
        return Result<Item?>.Success(null);
    }
}
