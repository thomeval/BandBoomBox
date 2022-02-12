using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HitJudge
{
    public static readonly Dictionary<JudgeResult, int> JudgePerfPointValues = new Dictionary<JudgeResult, int>()
    {
        {JudgeResult.Perfect, 3},
        {JudgeResult.Cool, 2},
        {JudgeResult.Ok, 1},
        {JudgeResult.Bad, 0},
        {JudgeResult.Wrong, 0},
        {JudgeResult.Miss, 0}
    };


    public static readonly Dictionary<JudgeResult, float> JudgeTimings = new Dictionary<JudgeResult, float>()
    {
        {JudgeResult.Perfect, 0.030f},
        {JudgeResult.Cool, 0.060f},
        {JudgeResult.Ok, 0.12f},
        {JudgeResult.Bad, 0.4f},
        {JudgeResult.Wrong, 1.0f }
    };

    public static readonly Dictionary<JudgeResult, float> JudgeReleaseTimings = new Dictionary<JudgeResult, float>()
    {
        {JudgeResult.Perfect, 0.045f},
        {JudgeResult.Cool, 0.090f},
        {JudgeResult.Ok, 0.18f},
        {JudgeResult.Bad, 0.4f},
        {JudgeResult.Miss, 10000.0f},
    };


    public static Dictionary<JudgeResult, int> JudgeScoreValues = new Dictionary<JudgeResult, int>()
    {
        { JudgeResult.Perfect, 50},
        { JudgeResult.Cool, 30 },
        { JudgeResult.Ok, 15 },
        { JudgeResult.Bad, 0 },
        { JudgeResult.Wrong, 0 },
        { JudgeResult.Miss, 0 }
    };


    public static Dictionary<JudgeResult, float> JudgeMxValues = new Dictionary<JudgeResult, float>()
    {
        { JudgeResult.Perfect, 0.05f},
        { JudgeResult.Cool, 0.03f },
        { JudgeResult.Ok, 0.01f },
        { JudgeResult.Bad, -0.01f },
        { JudgeResult.Wrong, -0.05f },
        { JudgeResult.Miss, -0.25f }
    };

    public static Dictionary<Difficulty, float> DifficultyMxValues = new Dictionary<Difficulty, float>()
    {
        {Difficulty.Beginner, 0.6f},
        {Difficulty.Medium, 0.8f},
        {Difficulty.Hard, 1.0f},
        {Difficulty.Expert, 1.1f},
        {Difficulty.Master, 1.2f}
    };

    public static Dictionary<Difficulty, float> DifficultyMissMxValues = new Dictionary<Difficulty, float>()
    {
        {Difficulty.Beginner, 0.5f},
        {Difficulty.Medium, 0.75f},
        {Difficulty.Hard, 1.0f},
        {Difficulty.Expert, 1.0f},
        {Difficulty.Master, 1.0f}
    };

    public static Dictionary<Difficulty, float> DifficultyExpValues = new Dictionary<Difficulty, float>()
    {
        {Difficulty.Beginner, 0.5f},
        {Difficulty.Medium, 0.75f},
        {Difficulty.Hard, 1.0f},
        {Difficulty.Expert, 1.1f},
        {Difficulty.Master, 1.2f}
    };

    public static Dictionary<Grade, float> GoalExpValues = new Dictionary<Grade, float>()
    {
        {Grade.D, 1.0f}, // D
        {Grade.C, 1.05f}, // C
        {Grade.B, 1.1f}, // B
        {Grade.BPlus, 1.15f}, // B+
        {Grade.A, 1.2f}, // A
        {Grade.APlus, 1.3f}, // A+
        {Grade.S, 1.4f}, // S
        {Grade.SPlus, 1.45f}, // S+
    };


    public HitResult GetHitResult(float deviation, int player, Difficulty difficulty, int lane, NoteType noteType, NoteClass noteClass)
    {
        var result = new HitResult();
        var value = NoteUtils.GetNoteValue(noteType, noteClass);
        result.DeviationResult = deviation > 0.0f ? DeviationResult.Late : DeviationResult.Early;

        var absDeviation = Mathf.Abs(deviation);
        var judgeResult = GetBestTiming(absDeviation, noteClass);

        result.JudgeResult = judgeResult;
        if (judgeResult == JudgeResult.Wrong || judgeResult == JudgeResult.Miss)
        {
            result.DeviationResult = DeviationResult.NotHit;
        }

        result.Lane = lane;
        result.PerfPoints = JudgePerfPointValues[judgeResult];
        result.ScorePoints = (int) (JudgeScoreValues[judgeResult] *  value);
        result.MxPoints = JudgeMxValues[judgeResult] * DifficultyMxValues[difficulty];

        result.Player = player;
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
            Player = player,
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
            Player = player,
            Lane = lane
        };
        return result;
    }

    private static readonly JudgeResult[] _comboBreakResults = new[] {JudgeResult.Miss, JudgeResult.Wrong, JudgeResult.Bad};
    private static readonly JudgeResult[] _comboAddResults = new[] {JudgeResult.Perfect, JudgeResult.Cool, JudgeResult.Ok};
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
