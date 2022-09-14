using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChartEditorNoteTransformer : MonoBehaviour
{
    private ChartEditorManager _parent;
    private NoteManager _noteManager;

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
        {NoteType.A, NoteType.Up},
        {NoteType.B, NoteType.Left},
        {NoteType.X, NoteType.Right},
        {NoteType.Y, NoteType.Down},
        {NoteType.Left, NoteType.B},
        {NoteType.Down, NoteType.Y},
        {NoteType.Up, NoteType.A},
        {NoteType.Right, NoteType.X},
        {NoteType.LB, NoteType.RB},
        {NoteType.LT, NoteType.RT},
        {NoteType.RB, NoteType.LB},
        {NoteType.RT, NoteType.LT},
        {NoteType.AnyB, NoteType.AnyD},
        {NoteType.AnyD, NoteType.AnyB},
        {NoteType.AnyT, NoteType.AnyT},
    };

    private readonly Dictionary<NoteType, NoteType> _invertMediumLookup = new()
    {
        {NoteType.A, NoteType.Down},
        {NoteType.B, NoteType.Left},
        {NoteType.Left, NoteType.B},
        { NoteType.Down, NoteType.A}
    };

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
        var result =  _noteManager.GetNotesInRegion(_parent.SelectedRegionStart!.Value, _parent.SelectedRegionEnd!.Value);

        //TODO: Handle case where Hold note is in this region, but its release is not.
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
