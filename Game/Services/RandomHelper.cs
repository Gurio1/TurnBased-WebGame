namespace Game.Services;

public class RandomHelper
{
    private static readonly ThreadLocal<Random> LocalRandom = new (() => new Random(Guid.NewGuid().GetHashCode()));

    public static Random Instance => LocalRandom.Value;

    public static float NextFloat(float min, float max)
    {
        return (float)(min + Instance.NextDouble() * (max - min));
    }
}