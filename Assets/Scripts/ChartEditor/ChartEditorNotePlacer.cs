using System;
using System.Linq;
using UnityEngine;

public class ChartEditorNotePlacer : MonoBehaviour
{
    private ChartEditorManager _parent;
    private NoteManager _noteManager;

    private readonly InputAction[] _inputsToHandle =
    {
        InputAction.Editor_NoteA,
        InputAction.Editor_NoteB,
        InputAction.Editor_NoteX,
        InputAction.Editor_NoteY,
        InputAction.Editor_NoteDown,
        InputAction.Editor_NoteRight,
        InputAction.Editor_NoteLeft,
        InputAction.Editor_NoteUp,
        InputAction.Editor_NoteLB,
        InputAction.Editor_NoteRB,
        InputAction.Editor_NoteLT,
        InputAction.Editor_NoteRT,
        InputAction.Editor_NoteAnyB,
        InputAction.Editor_NoteAnyD,
        InputAction.Editor_NoteAnyT,
        InputAction.Editor_NoteReleaseAnyB,
        InputAction.Editor_NoteReleaseAnyD,
        InputAction.Editor_NoteReleaseAnyT,
        InputAction.Editor_DeleteNote,
        InputAction.Editor_ClearRegion,
    };

    private const float FLOAT_TOLERANCE = 0.01f;

    void Awake()
    {
        Helpers.AutoAssign(ref _parent);
        _noteManager = _parent.NoteManager;
    }

    public bool ShouldHandleState(ChartEditorState state)
    {
        return state == ChartEditorState.Edit;
    }

    public bool ShouldHandleInput(InputEvent inputEvent)
    {
        return _inputsToHandle.Contains(inputEvent.Action);
    }

    public void PlaceOrRemoveNote(NoteType noteType, NoteClass noteClass, float position)
    {
        noteType = ConvertNoteToBeginner(noteType, !_parent.Options.AllowAllNotes);

        if (!NoteIsValidForDifficulty(noteType, noteClass))
        {
            _parent.DisplayMistake($"The selected note type ({noteType}), does not belong in a {_parent.CurrentDifficulty} chart. To disable this check, set 'Allow All Note Types' to On in the Options menu.");
            return;
        }

        var existing = _noteManager.GetNoteAtPosition(position, NoteUtils.GetNoteLane(noteType));

        if (existing != null)
        {
            if (existing.NoteType == noteType)
            {
                RemoveExistingNote(existing);
            }
            else
            {
                ChangeExistingNote(existing, noteType, noteClass);
            }
            return;
        }

        PlaceNewNote(noteType, noteClass, position);
    }

    private NoteType ConvertNoteToBeginner(NoteType noteType, bool shouldConvert)
    {
        if (!shouldConvert || _parent.CurrentDifficulty != Difficulty.Beginner)
        {
            return noteType;
        }

        return NoteUtils.GetLaneAnyNote(noteType);

    }

    private bool NoteIsValidForDifficulty(NoteType noteType, NoteClass noteClass)
    {
        if (_parent.Options.AllowAllNotes)
        {
            return true;
        }

        var diff = _parent.CurrentChart.Difficulty;
        if (noteClass == NoteClass.Release)
        {
            return noteType == NoteType.AnyB
                   || noteType == NoteType.AnyD
                   || (noteType == NoteType.AnyT && (diff == Difficulty.Expert || diff == Difficulty.Nerf || diff == Difficulty.Extra));
        }

        return NoteUtils.GetValidNoteTypesForDifficulty(diff).Contains(noteType);
    }

    private void PlaceNewNote(NoteType noteType, NoteClass noteClass, float position)
    {
        if (noteClass == NoteClass.Release)
        {
            var lane = NoteUtils.GetNoteLane(noteType);
            var existingRelease = FindReleaseNearPosition(position, lane);

            if (existingRelease != null)
            {
                MoveExistingReleaseNote(existingRelease, position);
                return;
            }

            PlaceNewReleaseNote(noteType, position);
            return;
        }
        if (noteClass == NoteClass.Tap)
        {
            PlaceNewNoteCommon(noteType, NoteClass.Tap, position);
            AutoStep();
        }
    }

    private Note FindReleaseNearPosition(float position, int lane)
    {
        var result = _noteManager.FindNoteBefore(position, lane);

        if (result != null && result.NoteClass == NoteClass.Release)
        {
            return result;
        }

        result = _noteManager.FindNoteAfter(position, lane);
        if (result != null && result.NoteClass == NoteClass.Release)
        {
            return result;
        }

        return null;
    }

    private void MoveExistingReleaseNote(Note releaseNote, float position)
    {
        releaseNote.Position = position;
        _noteManager.CalculateAbsoluteTimes(_parent.CurrentSongData.Bpm);
        _parent.PlaySfx(SoundEvent.Editor_NotePlaced);
    }

    private void AutoStep()
    {
        if (_parent.Options.AutoStepForward)
        {
            _parent.MoveCursorSteps(1);
        }
    }

    private void PlaceNewReleaseNote(NoteType noteType, float position)
    {
        var lane = NoteUtils.GetNoteLane(noteType);
        var prevNote = _noteManager.FindNoteBefore(position, lane);

        if (prevNote == null || prevNote.NoteClass != NoteClass.Tap)
        {
            _parent.DisplayMistake("Cannot place a Hold Release here. First mark the beginning of the Hold note by placing a Tap note on the same lane.");
            return;
        }

        // Replace the previous Tap note with a Hold one, and add a Release Note at the desired position.
        var holdPosition = prevNote.Position;
        var holdType = prevNote.NoteType;

        RemoveExistingNote(prevNote);
        var holdNote = PlaceNewNoteCommon(holdType, NoteClass.Hold, holdPosition);
        var releaseNote = PlaceNewNoteCommon(holdType, NoteClass.Release, position);
        holdNote.EndNote = releaseNote;
        AutoStep();
    }

    private Note PlaceNewNoteCommon(NoteType noteType, NoteClass noteClass, float position)
    {
        var note = _parent.NoteGenerator.InstantiateNote(position, noteType, noteClass);
        _noteManager.AttachNote(note);
        _noteManager.CalculateAbsoluteTimes(_parent.CurrentSongData.Bpm);
        note.RefreshSprites();
        _parent.PlaySfx(SoundEvent.Editor_NotePlaced);
        IncrementNoteCount(note);

        return note;
    }

    private void RemoveExistingNote(Note existing)
    {
        _parent.PlaySfx(SoundEvent.Editor_NoteRemoved);
        if (existing.NoteClass == NoteClass.Release)
        {
            // Remove a Hold note from its end (release). Convert the note to a tap.
            RemoveReleaseNote(existing);
            return;
        }

        // Remove a Hold note from its beginning.
        if (existing.EndNote != null)
        {
            DecrementNoteCount(existing.EndNote);
            _noteManager.RemoveNote(existing.EndNote);
        }

        DecrementNoteCount(existing);
        _noteManager.RemoveNote(existing);

    }

    private void RemoveReleaseNote(Note existing)
    {
        var noteStart = _noteManager.FindReleaseNoteStart(existing);

        // Should not happen. This is a failsafe in case a Hold Release note ends up separated from its Start note.
        if (noteStart == null)
        {
            DecrementNoteCount(existing);
            _noteManager.RemoveNote(existing);
            return;
        }

        // Convert the Hold note associated with this Release note to a Tap Note.
        var noteType = noteStart.NoteType;
        var position = noteStart.Position;
        RemoveExistingNote(noteStart);
        PlaceNewNoteCommon(noteType, NoteClass.Tap, position);
    }

    private void ChangeExistingNote(Note existing, NoteType noteType, NoteClass noteClass)
    {
        // Don't try to modify Release notes.
        if (existing.NoteClass == NoteClass.Release)
        {
            _parent.DisplayMistake("Cannot modify a release note. To change a hold note, modify its beginning note instead.");
            return;
        }

        if (noteClass == NoteClass.Release)
        {
            _parent.DisplayMistake("Cannot place a release note here. Another note is in the way.");
            return;
        }

        existing.NoteType = noteType;
        //existing.NoteClass = noteClass;
        existing.RefreshSprites();

        if (existing.EndNote != null)
        {
            existing.EndNote.NoteType = noteType;
            existing.EndNote.RefreshSprites();
        }

        _parent.PlaySfx(SoundEvent.Editor_NotePlaced);
    }

    public void OnPlayerInput(InputEvent inputEvent)
    {
        var position = (float)_parent.CursorPosition;

        if (inputEvent.Action == InputAction.Editor_DeleteNote)
        {
            RemoveNotesAt(position);
            return;
        }

        if (inputEvent.Action == InputAction.Editor_ClearRegion)
        {
            _parent.NoteTransformer.ClearRegion();
            return;
        }

        var actionStr = inputEvent.Action.ToString();
        var noteClass = actionStr.Contains("Release") ? NoteClass.Release : NoteClass.Tap;
        var noteTypeStr = actionStr.Replace("Editor_NoteRelease", "");
        noteTypeStr = noteTypeStr.Replace("Editor_Note", "");
        var noteType = Enum.Parse<NoteType>(noteTypeStr, true);

        PlaceOrRemoveNote(noteType, noteClass, position);

    }

    private void RemoveNotesAt(float position)
    {
        var notesToRemove = _noteManager.Notes.Where(e => Math.Abs(e.Position - position) < FLOAT_TOLERANCE).ToList();
        foreach (var note in notesToRemove)
        {
            RemoveExistingNote(note);
        }

        _parent.PlaySfx(SoundEvent.Editor_NoteRemoved);
    }

    private void IncrementNoteCount(Note note)
    {
        var noteCounts = _parent.CurrentChart.NoteCounts;

        noteCounts.LaneNotes[note.Lane]++;
        switch (note.NoteClass)
        {
            case NoteClass.Tap:
                noteCounts.TapNotes++;
                break;
            case NoteClass.Hold:
                noteCounts.HoldNotes++;
                break;
        }
        _parent.UpdateNoteCountDisplay();
    }

    private void DecrementNoteCount(Note note)
    {
        var noteCounts = _parent.CurrentChart.NoteCounts;

        noteCounts.LaneNotes[note.Lane]--;
        switch (note.NoteClass)
        {
            case NoteClass.Tap:
                noteCounts.TapNotes--;
                break;
            case NoteClass.Hold:
                noteCounts.HoldNotes--;
                break;
        }
        _parent.UpdateNoteCountDisplay();
    }
}

