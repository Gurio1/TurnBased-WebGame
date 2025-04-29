using Game.Core.Equipment.Generation;
using Game.Core.Models;
using Game.Core.SharedKernel;
using Game.Features.Loot;
using Game.Utilities;

namespace Game.Core.Loot;

public class LootService : ILootService
{
    //Implement application logic
    //THis is definitely domain logic,so should be moved in core
    private readonly IEquipmentGenerator equipmentGenerator;
    
    public LootService(IEquipmentGenerator equipmentGenerator)
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
