using Game.Core.Models;
using Game.Drop;

namespace Game.Core.Equipment;

public interface IEquipment : IDropable
{
    public string EquipmentType { get; set; }
    public string Name { get; set; }

    public void ApplyStats(CharacterBase characterBase);
    public void RemoveStats(CharacterBase characterBase);

}