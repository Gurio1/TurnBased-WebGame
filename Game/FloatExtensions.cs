namespace Game;

public static class FloatExtensions
{
    public static float RoundTo1(this float value) => MathF.Round(value, 1);
    public static float RoundTo2(this float value) => MathF.Round(value, 2);
}
