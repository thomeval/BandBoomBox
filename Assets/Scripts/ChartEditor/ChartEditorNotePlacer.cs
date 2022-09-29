using System;
using System.Collections.Generic;
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
            _parent.PlaySfx(SoundEvent.Mistake);
            _parent.DisplayMessage($"The selected note type ({noteType}), does not belong in a {_parent.CurrentDifficulty} chart.",true);
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
                   || (noteType == NoteType.AnyT && (diff == Difficulty.Expert || diff == Difficulty.Master));
        }

        return NoteUtils.GetValidNoteTypesForDifficulty(diff).Contains(noteType);
    }

    private void PlaceNewNote(NoteType noteType, NoteClass noteClass, float position)
    {
        if (noteClass == NoteClass.Release)
        {
            PlaceNewReleaseNote(noteType, position);
            return;
        }
        if (noteClass == NoteClass.Tap)
        {
            PlaceNewNoteCommon(noteType, NoteClass.Tap, position);
            AutoStep();
        }
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
            _parent.PlaySfx(SoundEvent.Mistake);
            _parent.DisplayMessage("Cannot place a Hold Release here. First mark the beginning of the Hold note by placing a Tap note on the same lane.", true);
            return;
        }

        // Replace the previous Tap note with a Hold one, and add a Release Note at the desired position.
        var holdPosition = prevNote.Position;
        var holdType = prevNote.NoteType;

        RemoveExistingNote(prevNote);
        PlaceNewNoteCommon(holdType, NoteClass.Hold, holdPosition);
        PlaceNewNoteCommon(noteType, NoteClass.Release, position );
        AutoStep();
    }

    private void PlaceNewNoteCommon(NoteType noteType, NoteClass noteClass, float position)
    {
        var note = _parent.NoteGenerator.InstantiateNote(position, noteType, noteClass, ref _noteManager.Notes);
        _noteManager.AttachNote(note);
        _noteManager.CalculateAbsoluteTimes(_parent.CurrentSongData.Bpm);
        note.RefreshSprites();
        _parent.PlaySfx(SoundEvent.Editor_NotePlaced);
    }

    private void RemoveExistingNote(Note existing)
    {
        if (existing.NoteClass == NoteClass.Release)
        {
            var noteStart = _noteManager.FindReleaseNoteStart(existing);
            if (noteStart != null)
            {
                RemoveExistingNote(noteStart);
                return;
            }
        }
        if (existing.EndNote != null)
        {
            _noteManager.RemoveNote(existing.EndNote);
        }
        _noteManager.RemoveNote(existing);
        _parent.PlaySfx(SoundEvent.Editor_NoteRemoved);
    }

    private void ChangeExistingNote(Note existing, NoteType noteType, NoteClass noteClass)
    {
        // Don't try to modify Release notes.
        if (existing.NoteClass == NoteClass.Release)
        {
            _parent.DisplayMessage("Cannot modify a release note. To change a hold note, modify its beginning note instead.", true);
            return;
        }

        if (noteClass == NoteClass.Release)
        {
            _parent.DisplayMessage("Cannot place a release note here. Another note is in the way.", true);
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
        var position = (float) _parent.CursorPosition;

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
}

