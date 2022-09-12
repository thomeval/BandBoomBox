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
        InputAction.Editor_NoteReleaseAnyT
    };

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

    public void PlaceOrRemoveNote(NoteType noteType, NoteClass noteClass, double position)
    {
        if (_parent.CurrentDifficulty == Difficulty.Beginner)
        {
            noteType = NoteUtils.GetLaneAnyNote(noteType);
        }
        if (!NoteIsValidForDifficulty(noteType, noteClass))
        {
            _parent.PlaySfx("Mistake");
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

    private bool NoteIsValidForDifficulty(NoteType noteType, NoteClass noteClass)
    {

        var diff = _parent.CurrentChart.Difficulty;
        if (noteClass == NoteClass.Release)
        {
            return noteType == NoteType.AnyB
                   || noteType == NoteType.AnyD
                   || (noteType == NoteType.AnyT && (diff == Difficulty.Expert || diff == Difficulty.Master));
        }

        return NoteUtils.GetValidNoteTypesForDifficulty(diff).Contains(noteType);
    }

    private void PlaceNewNote(NoteType noteType, NoteClass noteClass, double position)
    {
        var note = _parent.NoteGenerator.InstantiateNote((float) position, noteType, noteClass, ref _noteManager.Notes);
        _noteManager.AttachNote(note);
        _noteManager.CalculateAbsoluteTimes(_parent.CurrentSongData.Bpm);
        note.RefreshSprites();
    }

    private void RemoveExistingNote(Note existing)
    {
        if (existing.NoteClass == NoteClass.Release)
        {
            var noteStart = _noteManager.FindReleaseNoteStart(existing);
            RemoveExistingNote(noteStart);
            return;
        }
        if (existing.EndNote != null)
        {
            _noteManager.RemoveNote(existing.EndNote);
        }
        _noteManager.RemoveNote(existing);
    }

    private void ChangeExistingNote(Note existing, NoteType noteType, NoteClass noteClass)
    {
        // Don't try to modify Release notes.
        if (existing.NoteClass == NoteClass.Release)
        {
            _parent.PlaySfx("Mistake");
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
    }

    public void OnPlayerInput(InputEvent inputEvent)
    {
        var position = _parent.CursorPosition;
        var actionStr = inputEvent.Action.ToString();
        var noteClass = actionStr.Contains("Release") ? NoteClass.Release : NoteClass.Tap;
        var noteTypeStr = actionStr.Replace("Editor_NoteRelease", "");
        noteTypeStr = noteTypeStr.Replace("Editor_Note", "");
        var noteType = Enum.Parse<NoteType>(noteTypeStr, true);

        PlaceOrRemoveNote(noteType, noteClass, position);

    }
}

