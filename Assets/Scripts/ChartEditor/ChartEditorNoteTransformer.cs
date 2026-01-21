using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class ChartEditorNoteTransformer : MonoBehaviour
{
    private ChartEditorManager _parent;
    private NoteManager _noteManager;

    void Awake()
    {
        Helpers.AutoAssign(ref _parent);
        _noteManager = _parent.NoteManager;
    }

     private void TransformNotesInCurrentRegion(Dictionary<NoteType, NoteType> lookup)
    {
        if (!HasValidRegionSet())
        {
            _parent.PlaySfx(SoundEvent.Mistake);
            return;
        }
        var notesAffected = GetNotesInCurrentRegion();
        TransformNotes(notesAffected, lookup);
    }

    public NoteType Invert(NoteType noteType)
    {
        var lookup = GetInvertLookup(_parent.CurrentChart.Difficulty);
        return lookup[noteType];
    }

    public void Invert()
    {
        var lookup = GetInvertLookup(_parent.CurrentChart.Difficulty);
        TransformNotesInCurrentRegion(lookup);
    }

    public void SwapHands()
    {
        var lookup = GetSwapHandsLookup(_parent.CurrentChart.Difficulty);
        TransformNotesInCurrentRegion(lookup);
    }

    public void Rotate90()
    {
        var lookup = GetRotate90Lookup(_parent.CurrentChart.Difficulty);
        TransformNotesInCurrentRegion(lookup);
    }

    public void Rotate180()
    {
        var lookup = GetRotate180Lookup(_parent.CurrentChart.Difficulty);
        TransformNotesInCurrentRegion(lookup);
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

        var lookup = GetRotate180Lookup(_parent.CurrentChart.Difficulty);

        var transformNextNote = true;
        foreach (var note in notesAffected)
        {
            if (note.IsEndNote)
            {
                continue;
            }
            if (transformNextNote)
            {
                TransformNote(note, lookup);
                if (note.EndNote != null)
                {
                    TransformNote(note.EndNote, lookup);
                }
            }
            transformNextNote = !transformNextNote;
        }
    }

    private bool IsMediumNote(Note note)
    {
        return _mediumNoteTypes.Contains(note.NoteType);
    }

    public void ExpandToExpert()
    {
        if (!HasValidRegionSet())
        {
            _parent.PlaySfx(SoundEvent.Mistake);
            return;
        }
        var notesAffected = GetNotesInCurrentRegion();

        if (notesAffected.Any(e => IsTopLaneNote(e)))
        {
            _parent.DisplayMistake("Cannot expand the selected region to Expert difficulty. There are already notes that are not Expert or N.E.R.F. difficulty.");
            return;
        }

        int notesProcessed = 0;

        foreach (var note in notesAffected)
        {
            if (note.IsEndNote)
            {
                continue;
            }

            if (notesProcessed % 4 == 0)
            {
                TransformNote(note, _expandToExpertLookup);
                if (note.EndNote != null)
                {
                    TransformNote(note.EndNote, _expandToExpertLookup);
                }
            }
            notesProcessed++;
        }
    }

    public bool IsTopLaneNote(Note note)
    {
        return _topLaneNoteTypes.Contains(note.NoteType);
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

    public void EnsureSpacing(float spacing)
    {
        var notesAffected = GetNotesInCurrentRegion();
        notesAffected = notesAffected.OrderBy(e => e.Position).ToList();
        double lastPosition = double.NegativeInfinity;
        var notesToRemove = new List<Note>();
        foreach (var note in notesAffected)
        {
            var delta = note.Position - lastPosition;

            // Allow notes at the same position (chords)
            if (delta == 0.0f)
            {
                continue;
            }

            // Include end notes in spacing checks, but never remove them.
            if (note.IsEndNote)
            {
                lastPosition = note.Position;
                continue;
            }

            if (delta < spacing)
            {
                notesToRemove.Add(note);
            }
            else
            {
                lastPosition = note.Position;
            }
        }

        foreach (var note in notesToRemove)
        {
            _noteManager.RemoveNote(note, true);
        }
        _parent.PlaySfx(SoundEvent.Editor_NoteRemoved);
        _parent.DisplayMessage($"Removed {notesToRemove.Count} notes to ensure spacing of {spacing} beats.");
    }

    public void ClampToDifficulty(Difficulty difficulty)
    {
        var lookup = GetClampLookup(difficulty);
        TransformNotesInCurrentRegion(lookup);
    }

    public void RemoveNotesInsideHolds()
    {
        var notesToCheck = GetNotesInCurrentRegion();

        var notesInHolds = ChartValidator.GetNotesInsideHolds(notesToCheck);

        foreach (var note in notesInHolds)
        {
            _noteManager.RemoveNote(note);
        }
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
        note.SetRenderYPosition(yPos);
    }
}
