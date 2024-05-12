using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HitJudge
{
    /// <summary>
    /// These values control the amount of performance points gained from each note successfully hit. Performance points are used to calculate a player's individual score percentage.
    /// </summary>
    public static readonly Dictionary<JudgeResult, int> JudgePerfPointValues = new()
    {
        {JudgeResult.Crit, 3},
        {JudgeResult.Perfect, 3},
        {JudgeResult.Cool, 2},
        {JudgeResult.Ok, 1},
        {JudgeResult.Bad, 0},
        {JudgeResult.Wrong, 0},
        {JudgeResult.Miss, 0}
    };

    /// <summary>
    /// These values control the amount of time a player has to hit a Tap note, and what result they will receive. 0.03f indicates a 30ms window, either early or late. 
    /// Note that Crit hits are only possible if the player has their Turbo active.
    /// </summary>
    public static readonly Dictionary<JudgeResult, float> JudgeTimings = new()
    {
        {JudgeResult.Crit, 0.020f},
        {JudgeResult.Perfect, 0.030f},
        {JudgeResult.Cool, 0.060f},
        {JudgeResult.Ok, 0.12f},
        {JudgeResult.Bad, 0.4f},
        {JudgeResult.Wrong, 1.0f }
    };

    /// <summary>
    /// These values control the amount of time a player has to hit a Tap note when playing on Beginner difficulty, and what result they will receive. 0.03f indicates a 30ms window, either early or late.
    /// These values are used instead of the default values, to make the game more forgiving for new players.
    /// Note that Crit hits are only possible if the player has their Turbo active.
    /// </summary>
    public static readonly Dictionary<JudgeResult, float> BeginnerJudgeTimings = new()
    {
        {JudgeResult.Crit, 0.020f},
        {JudgeResult.Perfect, 0.0375f},
        {JudgeResult.Cool, 0.075f},
        {JudgeResult.Ok, 0.15f},
        {JudgeResult.Bad, 0.4f},
        {JudgeResult.Wrong, 1.0f }
    };

    /// <summary>
    /// These values control the amount of time a player has to let go at the end of a Hold note (known as a Release note), and what result they will receive. 0.03f indicates a 30ms window, either early or late.
    /// These values are used instead of the default values, to make the game more forgiving for Release notes, as these are generally harder to time correctly.
    /// Note that Crit hits are only possible if the player has their Turbo active.
    /// </summary>
    public static readonly Dictionary<JudgeResult, float> JudgeReleaseTimings = new()
    {
        {JudgeResult.Crit, 0.030f},
        {JudgeResult.Perfect, 0.045f},
        {JudgeResult.Cool, 0.090f},
        {JudgeResult.Ok, 0.18f},
        {JudgeResult.Bad, 0.4f},
        {JudgeResult.Miss, 10000.0f},
    };

    /// <summary>
    /// These values control the amount of time a player has to let go at the end of a Hold note (known as a Release note), when playing on Beginner difficulty, and what result they will receive. 
    /// 0.03f indicates a 30ms window, either early or late.
    /// These values are used instead of the default values, to make the game more forgiving for new players.
    /// Note that Crit hits are only possible if the player has their Turbo active.
    /// </summary>
    public static readonly Dictionary<JudgeResult, float> BeginnerJudgeReleaseTimings = new()
    {
        {JudgeResult.Crit, 0.030f},
        {JudgeResult.Perfect, 0.0525f},
        {JudgeResult.Cool, 0.105f},
        {JudgeResult.Ok, 0.21f},
        {JudgeResult.Bad, 0.4f},
        {JudgeResult.Miss, 10000.0f},
    };

    /// <summary>
    /// These values control the amount of Team score gained from each note successfully hit. This value is multiplied by the current Team Multiplier (AKA Momentum) to determine the final score value.
    /// </summary>
    public static Dictionary<JudgeResult, int> JudgeScoreValues = new()
    {
        { JudgeResult.Crit, 50 },
        { JudgeResult.Perfect, 50},
        { JudgeResult.Cool, 30 },
        { JudgeResult.Ok, 15 },
        { JudgeResult.Bad, 0 },
        { JudgeResult.Wrong, 0 },
        { JudgeResult.Miss, 0 }
    };

    /// <summary>
    /// These values control the amount of score multiplier (AKA Momentum) gained from each note successfully hit, or lost when a note is missed.
    /// </summary>
    public static Dictionary<JudgeResult, float> JudgeMxValues = new()
    {
        { JudgeResult.Crit, 0.065f},
        { JudgeResult.Perfect, 0.05f},
        { JudgeResult.Cool, 0.03f },
        { JudgeResult.Ok, 0.01f },
        { JudgeResult.Bad, -0.01f },
        { JudgeResult.Wrong, -0.05f },
        { JudgeResult.Miss, -0.25f }
    };

    /// <summary>
    /// These values apply a bonus or reduction to the amount of score multiplier (AKA Momentum) gained from each note successfully hit, based on the player's selected difficulty. 1.0 indicates no change.
    /// </summary>
    public static Dictionary<Difficulty, float> DifficultyMxValues = new()
    {
        {Difficulty.Beginner, 0.8f},
        {Difficulty.Medium, 0.9f},
        {Difficulty.Hard, 1.0f},
        {Difficulty.Expert, 1.05f},
        {Difficulty.Nerf, 1.05f},
        {Difficulty.Extra, 1.0f}
    };

    /// <summary>
    /// These values adjust the amount of score multiplier (AKA Momentum) lost when a note is missed, based on the player's selected difficulty. 1.0 indicates no change. 
    /// This is used to make the game more forgiving on lower difficulties.
    /// </summary>
    public static Dictionary<Difficulty, float> DifficultyMissMxValues = new()
    {
        {Difficulty.Beginner, 0.5f},
        {Difficulty.Medium, 0.75f},
        {Difficulty.Hard, 1.0f},
        {Difficulty.Expert, 1.0f},
        {Difficulty.Nerf, 1.0f},
        {Difficulty.Extra, 1.0f}
    };


    /// <summary>
    /// These values apply a bonus or reduction to the amount of experience a player receives at the end of each song, based on the player's selected difficulty. 1.0 indicates no change.
    /// </summary>
    public static Dictionary<Difficulty, float> DifficultyExpValues = new()
    {
        {Difficulty.Beginner, 0.5f},
        {Difficulty.Medium, 0.75f},
        {Difficulty.Hard, 1.0f},
        {Difficulty.Extra, 1.0f},
        {Difficulty.Expert, 1.1f},
        {Difficulty.Nerf, 1.2f}
    };

    /// <summary>
    /// These values apply a bonus or reduction to the amount of experience a player receives at the end of each song, based on the player's selected goal. 1.0 indicates no change. Note that
    /// these bonuses are only applied if the player actually meets the goal. If the player fails to meet the goal, they will instead receive a -50% penalty to their experience.
    /// </summary>
    public static Dictionary<Grade, float> GoalExpValues = new()
    {
        {Grade.D, 1.0f},        // D
        {Grade.DPlus, 1.02f},   // D+
        {Grade.C, 1.05f},       // C
        {Grade.CPlus, 1.07f},   // C+
        {Grade.B, 1.1f},        // B
        {Grade.BPlus, 1.15f},   // B+
        {Grade.A, 1.2f},        // A
        {Grade.APlus, 1.3f},    // A+
        {Grade.S, 1.4f},        // S
        {Grade.SPlus, 1.45f},   // S+
    };

    /// <summary>
    /// These values apply a bonus to the amount of experience a player receives at the end of each song if they manage to complete a Full Combo. 1.0 indicates no change.
    /// </summary>
    public static Dictionary<FullComboType, float> FullComboExpValues = new()
    {
        { FullComboType.None , 1.0f},
        { FullComboType.SemiFullCombo , 1.10f},
        { FullComboType.FullCombo , 1.25f},
        { FullComboType.PerfectFullCombo , 1.5f},
    };

    public HitResult GetHitResult(float deviation, int player, Difficulty difficulty, int lane, NoteType noteType, NoteClass noteClass, bool allowCrit)
    {
        var result = new HitResult();
        var value = NoteUtils.GetNoteValue(noteType, noteClass);
        result.DeviationResult = deviation > 0.0f ? DeviationResult.Late : DeviationResult.Early;

        var absDeviation = Mathf.Abs(deviation);
        var judgeResult = GetBestTiming(absDeviation, noteClass, difficulty);

        if (!allowCrit && judgeResult == JudgeResult.Crit)
        {
            judgeResult = JudgeResult.Perfect;
        }
        result.JudgeResult = judgeResult;
        if (judgeResult == JudgeResult.Wrong || judgeResult == JudgeResult.Miss)
        {
            result.DeviationResult = DeviationResult.NotHit;
        }

        result.Lane = lane;
        result.PerfPoints = JudgePerfPointValues[judgeResult];
        result.ScorePoints = (int)(JudgeScoreValues[judgeResult] * value);
        result.MxPoints = JudgeMxValues[judgeResult] * DifficultyMxValues[difficulty];
        result.Deviation = deviation;

        result.PlayerSlot = player;
        return result;

    }

    public HitResult GetMissResult(int lane, int player, Difficulty difficulty)
    {
        var result = new HitResult
        {
            DeviationResult = DeviationResult.NotHit,
            JudgeResult = JudgeResult.Miss,
            PerfPoints = JudgePerfPointValues[JudgeResult.Miss],
            ScorePoints = JudgeScoreValues[JudgeResult.Miss],
            MxPoints = JudgeMxValues[JudgeResult.Miss] * DifficultyMissMxValues[difficulty],
            PlayerSlot = player,
            Lane = lane
        };
        return result;
    }

    public HitResult GetWrongResult(int lane, int player, Difficulty difficulty)
    {
        var result = new HitResult
        {
            DeviationResult = DeviationResult.NotHit,
            JudgeResult = JudgeResult.Wrong,
            PerfPoints = JudgePerfPointValues[JudgeResult.Wrong],
            ScorePoints = JudgeScoreValues[JudgeResult.Wrong],
            MxPoints = JudgeMxValues[JudgeResult.Wrong] * DifficultyMxValues[difficulty],
            PlayerSlot = player,
            Lane = lane
        };
        return result;
    }

    private static readonly JudgeResult[] _comboBreakResults = { JudgeResult.Miss, JudgeResult.Wrong, JudgeResult.Bad };
    private static readonly JudgeResult[] _comboAddResults = { JudgeResult.Crit, JudgeResult.Perfect, JudgeResult.Cool, JudgeResult.Ok };

    public static bool? IsComboBreak(JudgeResult result)
    {

        if (_comboAddResults.Contains(result))
        {
            return false;
        }

        if (_comboBreakResults.Contains(result))
        {
            return true;
        }

        // This will neither break nor add to the current combo. Supported, but not currently used.
        return null;
    }

    private JudgeResult GetBestTiming(float absDeviation, NoteClass noteClass, Difficulty difficulty)
    {
        var timings = GetTimings(noteClass, difficulty);
        foreach (var timing in timings)
        {
            if (timing.Value >= absDeviation)
            {
                return timing.Key;
            }
        }

        return JudgeResult.Miss;
    }

    public Dictionary<JudgeResult, float> GetTimings(NoteClass noteClass, Difficulty difficulty)
    {
        return noteClass == NoteClass.Release ? JudgeReleaseTimings : JudgeTimings;
        if (noteClass == NoteClass.Release)
        {
            return difficulty == Difficulty.Beginner ? BeginnerJudgeReleaseTimings : JudgeReleaseTimings;
        }

        return difficulty == Difficulty.Beginner ? BeginnerJudgeTimings : JudgeTimings;

    }
}
