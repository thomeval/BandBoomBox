using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

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
                // Only transform the note if there isn't already a note at that position, to avoid stacking two notes on top of each other.
                var existing = _noteManager.GetNoteAtPosition(note.Position, 0);
                if (existing != null)
                {
                    continue;
                }

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

    public void ClearNotesOutsidePlayableArea()
    {
        var notesToRemove = _noteManager.Notes.Where(e => e.Position < 0 || e.Position >= _parent.CurrentSongData.LengthInBeats)
            .ToList();

        foreach (var note in notesToRemove)
        {
            _noteManager.RemoveNote(note);
        }
        _parent.PlaySfx(SoundEvent.Editor_NoteRemoved);
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
        var notesAffected = GetNotesInCurrentRegion();
        foreach (var note in notesAffected)
        {
            var salt = GetNoteSaltIndex(note);
            var options = GetNoteOptions(note.NoteType, difficulty);

            if (options == null)
            {
                TransformNote(note, lookup, false);
            }
            else
            {
                TransformNoteSalted(note, options, salt, false);
            }
        }
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


    private NoteType[] GetNoteOptions(NoteType noteType, Difficulty targetDifficulty)
    {
        var lane = NoteUtils.GetNoteLane(noteType);
        var altLane = NoteUtils.GetNoteLaneInTwoLanes(noteType);
        switch (targetDifficulty)
        {
            case Difficulty.Beginner:
                return null;
            case Difficulty.Mild:
                switch (noteType)
                {
                    case NoteType.Y:
                        return new NoteType[] { NoteType.A, NoteType.B, NoteType.X };
                        case NoteType.Up:
                        return new NoteType[] { NoteType.Down, NoteType.Right, NoteType.Left };
                    case NoteType.LB:
                    case NoteType.LT:
                        return new NoteType[] { NoteType.A, NoteType.B, NoteType.X };
                    case NoteType.RB:
                    case NoteType.RT:
                        return new NoteType[] { NoteType.Down, NoteType.Right, NoteType.Left };
                    default: return null;

                }
            case Difficulty.Medium:
                switch (noteType)
                {
                    case NoteType.LB:
                    case NoteType.LT:
                        return new NoteType[] { NoteType.A, NoteType.B };
                    case NoteType.RB:
                    case NoteType.RT:
                        return new NoteType[] { NoteType.Down, NoteType.Right };
                    default: return null;
                }
            case Difficulty.Hard:
                switch (noteType)
                {
                    case NoteType.LB:
                    case NoteType.LT:
                        return new NoteType[] { NoteType.A, NoteType.B, NoteType.X, NoteType.Y };
                    case NoteType.RB:
                    case NoteType.RT:
                        return new NoteType[] { NoteType.Down, NoteType.Right, NoteType.Left, NoteType.Up };
                    default: return null;
                }
            default:
                return null;
        }
    }

    private int GetNoteSaltIndex(Note note)
    {
        var result = (int)(note.Position / 4);
        result += (int)(note.Position / 16);
        return result;
    }

    private void TransformNotes(List<Note> notesAffected, Dictionary<NoteType, NoteType> lookup, bool allowCollisions = true)
    {
        foreach (var note in notesAffected)
        {
            TransformNote(note, lookup, allowCollisions);
        }
    }

    private void TransformNote(Note note, Dictionary<NoteType, NoteType> lookup, bool allowCollisions = true)
    {
        if (!lookup.ContainsKey(note.NoteType))
        {
            return;
        }

        if (!allowCollisions)
        {
            var newType = lookup[note.NoteType];
            var existing = _noteManager.GetNoteAtPosition(note.Position, NoteUtils.GetNoteLane(newType));
            if (existing != null && existing != note)
            {
                return;
            }
        }
        SetNoteType(note, lookup[note.NoteType]);
    }

    private void TransformNoteSalted(Note note, NoteType[] options, int salt, bool allowCollisions = true)
    {
        var newType = options[salt % options.Length];

        if (!allowCollisions)
        {

            var existing = _noteManager.GetNoteAtPosition(note.Position, NoteUtils.GetNoteLane(newType));
            if (existing != null && existing != note)
            {
                return;
            }
        }
        SetNoteType(note, newType);
    }

    private void SetNoteType(Note note, NoteType newType)
    {
        note.NoteType = newType;
        note.Refresh();

        var yPos = _noteManager.TopLanePos - (note.Lane * _noteManager.LaneHeight);
        note.SetRenderYPosition(yPos);
    }
}
