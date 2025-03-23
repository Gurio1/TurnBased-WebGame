namespace Game.Core.Models;

public interface IMonster 
{
    public Dictionary<string,float> DropsTable { get; init; }
}