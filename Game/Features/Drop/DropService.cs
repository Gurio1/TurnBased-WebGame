using Game.Core.Equipment;
using Game.Core.Models;
using Game.Services;

namespace Game.Features.Drop;

public class DropService
{
    private readonly EquipmentGenerator _equipmentGenerator;

    public DropService(EquipmentGenerator equipmentGenerator)
    {
        _equipmentGenerator = equipmentGenerator;
    }


    public async Task<Item?> HandleDrop(Monster monster)
    {
        var random = (float)RandomHelper.Instance.NextDouble();
        var comulativeRate = 0f;
        foreach (var pair in monster.DropsTable)
        {
            comulativeRate += pair.Value; 
            if (random <= comulativeRate)
            {
                return await _equipmentGenerator.GenerateEquipment(pair.Key);
            }
        }

        return null;
    }
}