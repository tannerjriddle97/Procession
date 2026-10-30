namespace Procession.Core;

public static class StabilityMath
{
    /// <summary>
    /// The game interprets values below one as drain. Scale the storm's additional drain,
    /// preserving geographic drain and avoiding multiplication of merely lost recovery.
    /// </summary>
    public static double ApplyStormDrain(double current, double withoutStorm, double multiplier)
    {
        if (!double.IsFinite(current) || !double.IsFinite(withoutStorm) || !double.IsFinite(multiplier) || multiplier < 1)
            throw new ArgumentOutOfRangeException(nameof(multiplier));
        double stormDrain = Math.Max(0, 1 - current) - Math.Max(0, 1 - withoutStorm);
        return current - Math.Max(0, stormDrain) * (multiplier - 1);
    }
}
