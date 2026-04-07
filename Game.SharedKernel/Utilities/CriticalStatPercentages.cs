namespace Game.SharedKernel.Utilities;

public static class CriticalStatPercentages
{
    public static int NormalizeCriticalChance(double value)
    {
        if (double.IsNaN(value) || double.IsInfinity(value))
            return 0;

        if (!IsWholeNumber(value) && value is >= 0 and <= 1)
            return ClampChance((int)Math.Round(value * 100, MidpointRounding.AwayFromZero));

        return ClampChance((int)Math.Round(value, MidpointRounding.AwayFromZero));
    }

    public static int NormalizeCriticalDamage(double value)
    {
        if (double.IsNaN(value) || double.IsInfinity(value))
            return 0;

        if (!IsWholeNumber(value))
        {
            if (value is >= 0 and <= 1)
                return ClampDamage((int)Math.Round(value * 100, MidpointRounding.AwayFromZero));

            if (value is > 1 and <= 2)
                return ClampDamage((int)Math.Round((value - 1d) * 100, MidpointRounding.AwayFromZero));
        }

        return ClampDamage((int)Math.Round(value, MidpointRounding.AwayFromZero));
    }

    public static float ApplyCriticalDamageBonus(float damage, int criticalDamagePercent) =>
        damage * (1f + (criticalDamagePercent / 100f));

    private static int ClampChance(int value) => Math.Clamp(value, 0, 100);

    private static int ClampDamage(int value) => Math.Max(0, value);

    private static bool IsWholeNumber(double value) =>
        Math.Abs(value - Math.Round(value, MidpointRounding.AwayFromZero)) < 0.0000001d;
}
