using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ChartEditorClipboard : MonoBehaviour
{

    public GameObject ItemsContainer;
    public double ClipboardRegionSize;

    private ChartEditorManager _parent;

    private readonly InputAction[] _inputsToHandle =
    {
        InputAction.Editor_Cut, InputAction.Editor_Copy, InputAction.Editor_Paste, InputAction.Editor_PasteInverted
    };

    public Note[] Items
    {
        get
        {
            return ItemsContainer.GetComponentsInChildren<Note>();
        }
    }

    private bool EnsureValidRegion()
    {
        if (!_parent.NoteTransformer.HasValidRegionSet())
        {
            _parent.PlaySfx(SoundEvent.Mistake);
            _parent.DisplayMessage("Select a region to cut or copy using the 'Set Region Start/End' key first.", true);
            return false;
        }

        return true;
    }

    public void AddToClipboard(IEnumerable<Note> notes, float positionOffset)
    {
        foreach (var note in notes)
        {
            note.EndNote = null;
            var newObj = Instantiate(note);
            newObj.transform.parent = ItemsContainer.transform;
            newObj.Position -= positionOffset;
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
        AddToClipboard(notes, (float) regionStart);
        _parent.DisplayMessage($"Copied {notes.Count} notes.");
        _parent.PlaySfx(SoundEvent.Editor_Copy);
    }

    public void Paste(double position, bool invert)
    {
        if (!Items.Any())
        {
            _parent.PlaySfx(SoundEvent.Mistake);
            _parent.DisplayMessage("Nothing to paste.", true);
            return;
        }

        _parent.NoteTransformer.ClearRegion(position, position + ClipboardRegionSize);

        var notesPasted = 0;
        foreach (var note in Items)
        {
            var destPosition = note.Position + position;

            if (!_parent.IsInPlayableArea(destPosition))
            {
                continue;
            }

            var noteType = invert ? _parent.NoteTransformer.Invert(note.NoteType) : note.NoteType;
            var newNote = _parent.NoteGenerator.InstantiateNote((float) destPosition, noteType, note.NoteClass, ref  _parent.NoteManager.Notes);

            newNote.name = newNote.Description + " (Pasted)";

            _parent.NoteManager.AttachNote(newNote);
            notesPasted++;

        }
        _parent.NoteManager.CalculateAbsoluteTimes(_parent.CurrentSongData.Bpm);

        var invertStr = invert ? "(inverted)" : "";
        _parent.DisplayMessage($"Pasted {notesPasted} notes {invertStr}.");
        _parent.PlaySfx(SoundEvent.Editor_Paste);
    }

    public void Clear()
    {
        foreach (var note in ItemsContainer.GetChildren())
        {
            Destroy(note);
        }
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
                Paste(_parent.CursorPosition,true);
                break;
        }
    }
}
