using System;

public static class GameplayMultiplierUtils
{
    /// <summary>
    /// The basic rate that momentum (score multiplier) will decay, per second, provided that the current multiplier is above 1.0x.
    /// </summary>
    public const double MX_BASE_DECAY_RATE = 0.0125;

    /// <summary>
    /// The basic rate that momentum (score multiplier) will recover, per second, provided that the current multiplier is below 1.0x.
    /// </summary>
    public const double MX_BASE_RECOVER_RATE = 0.05;

    /// <summary>
    /// Controls the minimum possible score multiplier.
    /// </summary>
    public const double MX_MINIMUM = 0.1;

    /// <summary>
    /// Calculates an updated score multiplier by taking the current score multiplier and time elapsed as inputs,
    /// and applying a decay function. No effect if the current multiplier is below or at 1.0x
    /// </summary>
    /// <param name="multiplier">The current score multiplier.</param>
    /// <param name="timeDiff">The amount of time that has passed, in seconds.</param>
    /// <returns>The score multiplier after being decayed.</returns>
    /// <exception cref="ArgumentException">Thrown if timediff is a negative value.</exception>
    public static double DecayMultiplier(double multiplier, double timeDiff)
    {
        if (timeDiff < 0.0)
        {
            throw new ArgumentException("timeDiff cannot be a negative value.");
        }
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

    /// <summary>
    /// Calculates an updated score multiplier by taking the current score multiplier and time elapsed as inputs,
    /// and applying a decay function. No effect if the current multiplier is above 1.0x.
    /// </summary>
    /// <param name="multiplier">The current score multiplier.</param>
    /// <param name="timeDiff">The amount of time that has passed, in seconds.</param>
    /// <returns>The score multiplier after being decayed.</returns>
    /// <exception cref="ArgumentException">Thrown if timediff is a negative value.</exception>
    public static double RecoverMultiplier(double multiplier, double timeDiff)
    {
        if (timeDiff < 0.0)
        {
            throw new ArgumentException("timeDiff cannot be a negative value.");
        }

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

