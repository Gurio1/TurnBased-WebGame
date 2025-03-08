namespace Game.Core.Models;

public interface IItem
{
    string Name { get; }
    int Value { get; }

    Dictionary<string,string>  Stats { get; }
}