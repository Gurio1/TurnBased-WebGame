namespace Game.Utilities;

public static class RandomHelper
{
    private static readonly ThreadLocal<Random> localRandom = new (() => new Random(Guid.NewGuid().GetHashCode()));

    public static Random Instance => localRandom.Value!;

    public static float NextFloat(float min, float max) => (float)(min + Instance.NextDouble() * (max - min));
}
