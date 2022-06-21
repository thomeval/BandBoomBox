using System;

public static class GameplayUtils
{
    public const double MX_BASE_DECAY_RATE = 0.0125;
    public const double MX_BASE_RECOVER_RATE = 0.05;

    public static double DecayMultiplier(double multiplier, double timeDiff)
    {
        var effectiveMx = (int)multiplier;

        for (int x = 2; x < multiplier; x += 2)
        {
            effectiveMx += (int)Math.Max(0, multiplier - x);
        }
        
        var decayRate = MX_BASE_DECAY_RATE * effectiveMx;

        multiplier -= decayRate * timeDiff;
        multiplier = Math.Max(1.0, multiplier);
        return multiplier;
    }

    public static double RecoverMultiplier(double multiplier, double timeDiff)
    {
        var rate = MX_BASE_RECOVER_RATE;

        if (multiplier < 0.5f)
        {
            rate *= 2;
        }

        multiplier += rate * timeDiff;
        multiplier = Math.Min(1.0, multiplier);

        return multiplier;
    }
}

