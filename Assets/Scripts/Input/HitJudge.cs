using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HitJudge
{
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

    public static readonly Dictionary<JudgeResult, float> JudgeTimings = new()
    {
        {JudgeResult.Crit, 0.020f},
        {JudgeResult.Perfect, 0.030f},
        {JudgeResult.Cool, 0.060f},
        {JudgeResult.Ok, 0.12f},
        {JudgeResult.Bad, 0.4f},
        {JudgeResult.Wrong, 1.0f }
    };

    public static readonly Dictionary<JudgeResult, float> JudgeReleaseTimings = new()
    {
        {JudgeResult.Crit, 0.030f},
        {JudgeResult.Perfect, 0.045f},
        {JudgeResult.Cool, 0.090f},
        {JudgeResult.Ok, 0.18f},
        {JudgeResult.Bad, 0.4f},
        {JudgeResult.Miss, 10000.0f},
    };

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

    public static Dictionary<Difficulty, float> DifficultyMxValues = new()
    {
        {Difficulty.Beginner, 0.8f},
        {Difficulty.Medium, 0.9f},
        {Difficulty.Hard, 1.0f},
        {Difficulty.Expert, 1.05f},
        {Difficulty.Nerf, 1.05f},
        {Difficulty.Extra, 1.0f}
    };

    public static Dictionary<Difficulty, float> DifficultyMissMxValues = new()
    {
        {Difficulty.Beginner, 0.5f},
        {Difficulty.Medium, 0.75f},
        {Difficulty.Hard, 1.0f},
        {Difficulty.Expert, 1.0f},
        {Difficulty.Nerf, 1.0f},
        {Difficulty.Extra, 1.0f}
    };

    public static Dictionary<Difficulty, float> DifficultyExpValues = new()
    {
        {Difficulty.Beginner, 0.5f},
        {Difficulty.Medium, 0.75f},
        {Difficulty.Hard, 1.0f},
        {Difficulty.Extra, 1.0f},
        {Difficulty.Expert, 1.1f},
        {Difficulty.Nerf, 1.2f}
    };

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
        var judgeResult = GetBestTiming(absDeviation, noteClass);

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

        return null;
    }

    private JudgeResult GetBestTiming(float absDeviation, NoteClass noteClass)
    {
        Dictionary<JudgeResult, float> timings = noteClass == NoteClass.Release ? JudgeReleaseTimings : JudgeTimings;
        foreach (var timing in timings)
        {
            if (timing.Value >= absDeviation)
            {
                return timing.Key;
            }
        }

        return JudgeResult.Miss;
    }
}
