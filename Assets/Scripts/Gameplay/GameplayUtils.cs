using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public static class GameplayUtils
{
    public const double MX_BASE_DECAY_RATE = 0.0125;
    public const double MX_BASE_RECOVER_RATE = 0.05;

    public static double DecayMultiplier(double multiplier, double timeDiff)
    {
        var effectiveMx = (int)multiplier;
        effectiveMx += (int)Math.Max(0, multiplier - 2);
        effectiveMx += (int)Math.Max(0, multiplier - 4);
        effectiveMx += (int)Math.Max(0, multiplier - 6);
        effectiveMx += (int)Math.Max(0, multiplier - 8);
        effectiveMx += (int)Math.Max(0, multiplier - 10);
        effectiveMx += (int)Math.Max(0, multiplier - 12);

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

