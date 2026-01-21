using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public partial class ChartEditorNoteTransformer
{
    #region Lookups
    private readonly Dictionary<NoteType, NoteType> _swapHandsLookup = new()
    {
        {NoteType.A, NoteType.Down},
        {NoteType.B, NoteType.Right},
        {NoteType.X, NoteType.Left},
        {NoteType.Y, NoteType.Up},
        {NoteType.Left, NoteType.X},
        {NoteType.Down, NoteType.A},
        {NoteType.Up, NoteType.Y},
        {NoteType.Right, NoteType.B},
        {NoteType.LB, NoteType.RB},
        {NoteType.LT, NoteType.RT},
        {NoteType.RB, NoteType.LB},
        {NoteType.RT, NoteType.LT},
        {NoteType.AnyB, NoteType.AnyD},
        {NoteType.AnyD, NoteType.AnyB},
        {NoteType.AnyT, NoteType.AnyT},
    };

    private readonly Dictionary<NoteType, NoteType> _swapHandsMediumLookup = new()
    {
        {NoteType.A, NoteType.Down},
        {NoteType.B, NoteType.Left},
        {NoteType.Left, NoteType.B},
        { NoteType.Down, NoteType.A}
    };

    private readonly Dictionary<NoteType, NoteType> _invertLookup = new()
    {
        { NoteType.A, NoteType.Up},
        { NoteType.B, NoteType.Left},
        { NoteType.X, NoteType.Right},
        { NoteType.Y, NoteType.Down},
        { NoteType.Left, NoteType.B},
        { NoteType.Down, NoteType.Y},
        { NoteType.Up, NoteType.A},
        { NoteType.Right, NoteType.X},
        { NoteType.LB, NoteType.RB},
        { NoteType.LT, NoteType.RT},
        { NoteType.RB, NoteType.LB},
        { NoteType.RT, NoteType.LT},
        { NoteType.AnyB, NoteType.AnyD},
        { NoteType.AnyD, NoteType.AnyB},
        { NoteType.AnyT, NoteType.AnyT},
    };

    private readonly Dictionary<NoteType, NoteType> _invertMediumLookup = new()
    {
        {NoteType.A, NoteType.Down},
        {NoteType.B, NoteType.Left},
        {NoteType.Left, NoteType.B},
        { NoteType.Down, NoteType.A}
    };

    private readonly Dictionary<NoteType, NoteType> _invertMildLookup = new()
    {
        {NoteType.A, NoteType.Down},
        {NoteType.B, NoteType.Left},
        {NoteType.X, NoteType.Right},
        {NoteType.Left, NoteType.B},
        { NoteType.Down, NoteType.A},
        {NoteType.Right, NoteType.X},
    };

    private readonly Dictionary<NoteType, NoteType> _clampDifficultyBeginnerLookup = new()
    {
        { NoteType.A, NoteType.AnyB },
        { NoteType.B, NoteType.AnyB },
        { NoteType.X, NoteType.AnyB },
        { NoteType.Y, NoteType.AnyB },
        { NoteType.Left, NoteType.AnyD },
        { NoteType.Down, NoteType.AnyD },
        { NoteType.Up, NoteType.AnyD },
        { NoteType.Right, NoteType.AnyD },
        { NoteType.LB, NoteType.AnyT },
        { NoteType.LT, NoteType.AnyT },
        { NoteType.RB, NoteType.AnyT },
        { NoteType.RT, NoteType.AnyT },
    };

    private readonly Dictionary<NoteType, NoteType> _clampDifficultyMediumLookup = new()
    {
        { NoteType.X, NoteType.A },
        { NoteType.Y, NoteType.B },
        { NoteType.Up, NoteType.Down },
        { NoteType.Right, NoteType.Left },
        { NoteType.LB, NoteType.Down },
        { NoteType.LT, NoteType.Left },
        { NoteType.RB, NoteType.A },
        { NoteType.RT, NoteType.B },
    };
    private readonly Dictionary<NoteType, NoteType> _clampDifficultyMildLookup = new()
    {
        { NoteType.Y, NoteType.A },
        { NoteType.Up, NoteType.Down },
        { NoteType.LB, NoteType.Down },
        { NoteType.LT, NoteType.Left },
        { NoteType.RB, NoteType.A },
        { NoteType.RT, NoteType.B },
    };

    private readonly Dictionary<NoteType, NoteType> _clampDifficultyHardLookup = new()
    {
        { NoteType.LB, NoteType.Down },
        { NoteType.LT, NoteType.Left },
        { NoteType.RB, NoteType.A },
        { NoteType.RT, NoteType.B },
    };

    private readonly Dictionary<NoteType, NoteType> _clampDifficultyExpertLookup = new()
    {
        { NoteType.LT, NoteType.LB },
        { NoteType.RT, NoteType.RB },
    };

    private readonly Dictionary<NoteType, NoteType> _rotate90Lookup = new()
    {
        { NoteType.A, NoteType.B },
        { NoteType.B, NoteType.Y },
        { NoteType.X, NoteType.A },
        { NoteType.Y, NoteType.X },
        { NoteType.Left, NoteType.Down },
        { NoteType.Down, NoteType.Right },
        { NoteType.Up, NoteType.Left },
        { NoteType.Right, NoteType.Up },
    };

    private readonly Dictionary<NoteType, NoteType> _rotate90MediumLookup = new()
    {
        { NoteType.A, NoteType.B },
        { NoteType.B, NoteType.A },
        { NoteType.Left, NoteType.Down },
        { NoteType.Down, NoteType.Left },
    };

    private readonly Dictionary<NoteType, NoteType> _rotate90MildLookup = new()
    {
        { NoteType.A, NoteType.B },
        { NoteType.B, NoteType.X },
        { NoteType.X, NoteType.A },
        { NoteType.Left, NoteType.Down },
        { NoteType.Down, NoteType.Right },
        { NoteType.Right, NoteType.Left },
    };

    private readonly Dictionary<NoteType, NoteType> _rotate180Lookup = new()
    {
        { NoteType.A, NoteType.Y },
        { NoteType.B, NoteType.X },
        { NoteType.X, NoteType.B },
        { NoteType.Y, NoteType.A },
        { NoteType.Left, NoteType.Right },
        { NoteType.Down, NoteType.Up },
        { NoteType.Up, NoteType.Down },
        { NoteType.Right, NoteType.Left },
    };

    private readonly Dictionary<NoteType, NoteType> _rotate180MasterLookup = new()
    {
        { NoteType.A, NoteType.Y },
        { NoteType.B, NoteType.X },
        { NoteType.X, NoteType.B },
        { NoteType.Y, NoteType.A },
        { NoteType.Left, NoteType.Right },
        { NoteType.Down, NoteType.Up },
        { NoteType.Up, NoteType.Down },
        { NoteType.Right, NoteType.Left },
        { NoteType.LB, NoteType.LT },
        { NoteType.LT, NoteType.LB },
        { NoteType.RB, NoteType.RT },
        { NoteType.RT, NoteType.RB },
    };

    private readonly Dictionary<NoteType, NoteType> _expandToExpertLookup = new()
    {
        { NoteType.A, NoteType.RB },
        { NoteType.B, NoteType.RB },
        { NoteType.X, NoteType.RB },
        { NoteType.Y, NoteType.RB },
        { NoteType.Left, NoteType.LB },
        { NoteType.Down, NoteType.LB },
        { NoteType.Up, NoteType.LB },
        { NoteType.Right, NoteType.LB },
        { NoteType.AnyB, NoteType.RB },
        { NoteType.AnyD, NoteType.LB },
    };

    private NoteType[] _mediumNoteTypes = { NoteType.A, NoteType.B, NoteType.Left, NoteType.Down };
    private NoteType[] _topLaneNoteTypes = { NoteType.LB, NoteType.LT, NoteType.RB, NoteType.RT, NoteType.AnyT };

    #endregion

    #region Resolvers

    private Dictionary<NoteType, NoteType> GetRotate90Lookup(Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Difficulty.Medium:
                return _rotate90MediumLookup;
            case Difficulty.Mild:
                return _rotate90MildLookup;
            default:
                return _rotate90Lookup;
        }
    }

    private Dictionary<NoteType, NoteType> GetRotate180Lookup(Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Difficulty.Nerf:
            case Difficulty.Extra:
                return _rotate180MasterLookup;
            default:
                return _rotate180Lookup;
        }
    }

    private Dictionary<NoteType, NoteType> GetSwapHandsLookup(Difficulty difficulty)
    {
        return difficulty == Difficulty.Medium ? _swapHandsMediumLookup : _swapHandsLookup;
    }
    private Dictionary<NoteType, NoteType> GetInvertLookup(Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Difficulty.Mild:
                return _invertMildLookup;
            case Difficulty.Medium:
                return _invertMediumLookup;
            default:
                return _invertLookup;
        }
    }

    private Dictionary<NoteType, NoteType> GetClampLookup(Difficulty difficulty)
    {
        var lookup = new Dictionary<NoteType, NoteType>();
        switch (difficulty)
        {
            case Difficulty.Beginner:
                lookup = _clampDifficultyBeginnerLookup;
                break;
            case Difficulty.Medium:
                lookup = _clampDifficultyMediumLookup;
                break;
            case Difficulty.Mild:
                lookup = _clampDifficultyMildLookup;
                break;
            case Difficulty.Hard:
                lookup = _clampDifficultyHardLookup;
                break;
            case Difficulty.Expert:
                lookup = _clampDifficultyExpertLookup;
                break;

        }

        return lookup;
    }
    #endregion
}

