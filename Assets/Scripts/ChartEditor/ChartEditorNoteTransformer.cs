using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChartEditorNoteTransformer : MonoBehaviour
{
    private ChartEditorManager _parent;
    private NoteManager _noteManager;

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

    private NoteType[] _mediumNoteTypes = { NoteType.A, NoteType.B, NoteType.Left, NoteType.Down };

    private Dictionary<NoteType, NoteType> GetRotate90Lookup()
    {
        return _rotate90Lookup;
    }

    private Dictionary<NoteType, NoteType> GetRotate180Lookup()
    {
        return _parent.CurrentChart.Difficulty == Difficulty.Master || _parent.CurrentChart.Difficulty == Difficulty.Extra ? _rotate180MasterLookup : _rotate180Lookup;
    }

    #endregion
    void Awake()
    {
        Helpers.AutoAssign(ref _parent);
        _noteManager = _parent.NoteManager;
    }

    public NoteType Invert(NoteType noteType)
    {
        var lookup = _parent.CurrentChart.Difficulty == Difficulty.Medium ? _invertMediumLookup : _invertLookup;
        return lookup[noteType];
    }

    public void Invert()
    {
        if (!HasValidRegionSet())
        {
            _parent.PlaySfx(SoundEvent.Mistake);
            return;
        }

        var notesAffected = GetNotesInCurrentRegion();
        var lookup = _parent.CurrentChart.Difficulty == Difficulty.Medium ? _invertMediumLookup : _invertLookup;

        TransformNotes(notesAffected, lookup);
    }

    public void SwapHands()
    {
        if (!HasValidRegionSet())
        {
            _parent.PlaySfx(SoundEvent.Mistake);
            return;
        }

        var notesAffected = GetNotesInCurrentRegion();
        var lookup = _parent.CurrentChart.Difficulty == Difficulty.Medium ? _swapHandsMediumLookup : _swapHandsLookup;

        TransformNotes(notesAffected, lookup);
    }

    public void Rotate90()
    {
        if (!HasValidRegionSet())
        {
            _parent.PlaySfx(SoundEvent.Mistake);
            return;
        }

        var notesAffected = GetNotesInCurrentRegion();
        var lookup = GetRotate90Lookup();

        TransformNotes(notesAffected, lookup);
    }

    public void Rotate180()
    {
        if (!HasValidRegionSet())
        {
            _parent.PlaySfx(SoundEvent.Mistake);
            return;
        }

        var notesAffected = GetNotesInCurrentRegion();
        var lookup = GetRotate180Lookup();

        TransformNotes(notesAffected, lookup);
    }

    public void ExpandMediumToHard()
    {
        if (!HasValidRegionSet())
        {
            _parent.PlaySfx(SoundEvent.Mistake);
            return;
        }

        var notesAffected = GetNotesInCurrentRegion();


        if (notesAffected.Any(e => !IsMediumNote(e)))
        {
            _parent.DisplayMistake("Cannot expand the selected region to Hard difficulty. There are already notes that are not Medium difficulty.");
            return;
        }

        var lookup = GetRotate180Lookup();

        var transformNextNote = true;
        foreach (var note in notesAffected)
        {
            if (transformNextNote)
            {
                TransformNote(note, lookup);
            }
            transformNextNote = !transformNextNote;
        }
    }

    private bool IsMediumNote(Note note)
    {
        return _mediumNoteTypes.Contains(note.NoteType);      
    }

    public void SwapHandsAtCurrentPosition()
    {
        var notesAffected = GetNotesAtCurrentPosition();

        if (!notesAffected.Any())
        {
            _parent.DisplayMistake("There aren't any notes at the current position to swap.");
            return;
        }

        if (notesAffected.Any(e => e.NoteClass == NoteClass.Release))
        {
            _parent.DisplayMistake("Cannot swap hands for release notes. Try swapping the beginning of the hold note instead.");
            return;
        }
        var lookup = _parent.CurrentChart.Difficulty == Difficulty.Medium ? _swapHandsMediumLookup : _swapHandsLookup;
        var releases = notesAffected.Select(e => e.EndNote).Where(e => e != null);
        notesAffected.AddRange(releases);

        TransformNotes(notesAffected, lookup);
        _parent.PlaySfx(SoundEvent.Editor_NotePlaced);
    }

    public void ClearRegion(double regionStart, double regionEnd)
    {
        if (regionEnd <= regionStart)
        {
            _parent.PlaySfx(SoundEvent.Mistake);
            return;
        }

        var notesToRemove = _noteManager.GetNotesInRegion(regionStart, regionEnd);

        foreach (var note in notesToRemove)
        {
            _noteManager.RemoveNote(note);
        }
        _parent.PlaySfx(SoundEvent.Editor_NoteRemoved);
    }

    public void ClearRegion()
    {
        if (!HasValidRegionSet())
        {
            _parent.PlaySfx(SoundEvent.Mistake);
            return;
        }

        ClearRegion(_parent.SelectedRegionStart!.Value, _parent.SelectedRegionEnd!.Value);

    }

    public void ClampToDifficulty(Difficulty difficulty)
    {
        var lookup = GetClampLookup(difficulty);
        var notesAffected = GetNotesInCurrentRegion();
        TransformNotes(notesAffected, lookup);
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
            case Difficulty.Hard:
                lookup = _clampDifficultyHardLookup;
                break;
            case Difficulty.Expert:
                lookup = _clampDifficultyExpertLookup;
                break;

        }

        return lookup;
    }

    public bool HasValidRegionSet()
    {
        return _parent.SelectedRegionStart != null && _parent.SelectedRegionEnd != null && (_parent.SelectedRegionEnd - _parent.SelectedRegionStart > 0);
    }

    private List<Note> GetNotesInCurrentRegion()
    {
        if (!HasValidRegionSet())
        {
            return new List<Note>();
        }
        var result = _noteManager.GetNotesInRegion(_parent.SelectedRegionStart!.Value, _parent.SelectedRegionEnd!.Value);

        return result;
    }

    private List<Note> GetNotesAtCurrentPosition()
    {
        var result = _noteManager.GetNotesAtPosition(_parent.CursorPosition);
        return result;
    }

    private void TransformNotes(List<Note> notesAffected, Dictionary<NoteType, NoteType> lookup)
    {
        foreach (var note in notesAffected)
        {
            TransformNote(note, lookup);
        }
    }

    private void TransformNote(Note note, Dictionary<NoteType, NoteType> lookup)
    {
        if (!lookup.ContainsKey(note.NoteType))
        {
            return;
        }

        note.NoteType = lookup[note.NoteType];
        note.Refresh();

        var yPos = _noteManager.TopLanePos - (note.Lane * _noteManager.LaneHeight);
        note.SetYPosition(yPos);
    }
}
