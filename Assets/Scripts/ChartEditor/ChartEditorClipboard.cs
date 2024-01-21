using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChartEditorClipboard : MonoBehaviour
{

    public double ClipboardRegionSize;
    public List<NoteBase> ClipboardNotes = new();
    private ChartEditorManager _parent;

    private readonly InputAction[] _inputsToHandle =
    {
        InputAction.Editor_Cut, InputAction.Editor_Copy, InputAction.Editor_Paste, InputAction.Editor_PasteInverted
    };

    private bool EnsureValidRegion()
    {
        if (!_parent.NoteTransformer.HasValidRegionSet())
        {
            _parent.DisplayMessage("Select a region to cut or copy using the 'Set Region Start/End' key first.", true);
            return false;
        }

        return true;
    }

    public void AddToClipboard(IEnumerable<Note> notes, float positionOffset)
    {
        foreach (var note in notes)
        {
            var baseNote = note.NoteBase.Clone();
            baseNote.Position -= positionOffset;
            ClipboardNotes.Add(baseNote);
        }
    }

    public void Cut(double regionStart, double regionEnd)
    {
        this.Clear();
        ClipboardRegionSize = regionEnd - regionStart;

        var notes = _parent.NoteManager.GetNotesInRegion(regionStart, regionEnd);
        AddToClipboard(notes, (float)regionStart);
        _parent.NoteTransformer.ClearRegion(regionStart, regionEnd);
        _parent.DisplayMessage($"Cut {notes.Count} notes.");
        _parent.PlaySfx(SoundEvent.Editor_Cut);
    }

    public void Copy(double regionStart, double regionEnd)
    {
        this.Clear();
        ClipboardRegionSize = regionEnd - regionStart;

        var notes = _parent.NoteManager.GetNotesInRegion(regionStart, regionEnd).OrderBy(e => e.Position).ToList();
        AddToClipboard(notes, (float)regionStart);
        _parent.DisplayMessage($"Copied {notes.Count} notes.");
        _parent.PlaySfx(SoundEvent.Editor_Copy);
    }

    public void Paste(double position, bool invert)
    {
        if (!ClipboardNotes.Any())
        {
            _parent.DisplayMistake("Nothing to paste.");
            return;
        }

        _parent.NoteTransformer.ClearRegion(position, position + ClipboardRegionSize);

        var notesPasted = 0;
        foreach (var note in ClipboardNotes)
        {
            var destPosition = note.Position + position;

            if (!_parent.IsInPlayableArea(destPosition))
            {
                continue;
            }

            var noteType = invert ? _parent.NoteTransformer.Invert(note.NoteType) : note.NoteType;
            var newNote = _parent.NoteGenerator.InstantiateNote((float)destPosition, noteType, note.NoteClass);

            newNote.name = newNote.Description + " (Pasted)";

            _parent.NoteManager.AttachNote(newNote);
            notesPasted++;
        }

        _parent.ResolveHoldsWithReleases();
        _parent.NoteManager.CalculateAbsoluteTimes(_parent.CurrentSongData.Bpm);

        var invertStr = invert ? "(inverted)" : "";
        _parent.DisplayMessage($"Pasted {notesPasted} notes {invertStr}.");
        _parent.PlaySfx(SoundEvent.Editor_Paste);
    }

    public void Clear()
    {
        ClipboardNotes.Clear();
    }

    void Awake()
    {
        Helpers.AutoAssign(ref _parent);
    }

    public bool ShouldHandleInput(InputEvent inputEvent)
    {
        return _inputsToHandle.Contains(inputEvent.Action);
    }

    public void OnPlayerInput(InputEvent inputEvent)
    {
        OnPlayerInput(inputEvent.Action);
    }

    public void OnPlayerInput(InputAction action)
    {
        switch (action)
        {
            case InputAction.Editor_Cut:
                if (!EnsureValidRegion())
                {
                    return;
                }
                Cut(_parent.SelectedRegionStart!.Value, _parent.SelectedRegionEnd!.Value);
                break;
            case InputAction.Editor_Copy:
                if (!EnsureValidRegion())
                {
                    return;
                }
                Copy(_parent.SelectedRegionStart!.Value, _parent.SelectedRegionEnd!.Value);
                break;
            case InputAction.Editor_Paste:
                Paste(_parent.CursorPosition, false);
                break;
            case InputAction.Editor_PasteInverted:
                Paste(_parent.CursorPosition, true);
                break;
        }
    }
}
