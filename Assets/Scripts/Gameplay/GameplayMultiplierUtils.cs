using System;
using UnityEngine.UIElements.Experimental;

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
    /// Controls the maximum possible score multiplier.
    /// </summary>
    public const double MX_MAXIMUM = 99.0;

    private static readonly float[] _turboMxGainRates = { 0.0f,  1.0f,  2.5f,  4.25f, 6.0f,  8.0f,  10.0f, 12.0f, 14.0f,
                                                   16.0f, 18.0f, 20.0f, 22.0f, 24.0f, 26.0f, 28.0f, 30.0f, 32.0f  };

        /// <summary>
    /// Controls the amount of combo required to gain a bonus to the momentum gain rate.
    /// </summary>
    public const float GR_COMBO_FOR_BONUS = 50;

    /// <summary>
    /// Controls the amount of bonus awarded to momentum gain rate if the current team combo is at least GR_COMBO_FOR_BONUS (applied multiple times if appropriate).
    /// </summary>
    public const float GR_COMBO_BONUS_AMOUNT = 0.05f;

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
        multiplier = Math.Clamp(multiplier, 1.0, MX_MAXIMUM);
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

    public static float GetMultiplierGainRate(int teamCombo, int playersInTurbo)
    {
        var newGainRate = 1.0f;

        var comboGainBonus = ((int)(teamCombo / GR_COMBO_FOR_BONUS)) * GR_COMBO_BONUS_AMOUNT;
        comboGainBonus = Math.Min(comboGainBonus, 1.0f);
        newGainRate += comboGainBonus;

        var turboBonus = _turboMxGainRates[playersInTurbo];
        newGainRate += turboBonus;
        return newGainRate;
    }
}

