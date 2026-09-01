using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoPlayManager : MonoBehaviour
{
    private GameplayManager _gameplayManager;
    private List<InputEvent> _pendingReleases = new();

    void Awake()
    {
        Helpers.AutoAssign(ref _gameplayManager);
    }

    void Update()
    {

        if (_gameplayManager.GameplayState != GameplayScreenState.Playing)
        {
            return;
        }

        ProcessPendingReleases();

        foreach (var noteManager in _gameplayManager.NoteManagers)
        {
            if (noteManager == null || !noteManager.AutoPlayEnabled)
            {
                continue;
            }

            var nextNote = noteManager.FindNextNote(false, true);

            while (nextNote != null && nextNote.Position <= noteManager.SongPositionInBeats)
            {
                // Should this input be pressed or released?
                bool isPressed = false;

                // Should this input be released on the next frame?
                bool isOnceOff = false;

                switch (nextNote.NoteClass)
                {
                    case NoteClass.Tap:
                        isPressed = true;
                        isOnceOff = true;
                        break;
                    case NoteClass.Hold:
                        isPressed = true;
                        break;
                    case NoteClass.Release:
                        isPressed = false;
                        break;
                }
                var inputEvent = AsInputEvent(noteManager.Slot, nextNote, isPressed);
                if (isOnceOff)
                {
                    _pendingReleases.Add(inputEvent);
                }
                _gameplayManager.OnGameplayPlayerInput(inputEvent);

                nextNote = noteManager.FindNextNote(false, true);
            }
        }

    }

    private void ProcessPendingReleases()
    {
        foreach (var inputEvent in _pendingReleases)
        {
            inputEvent.IsPressed = false;
            _gameplayManager.OnGameplayPlayerInput(inputEvent);
        }

        _pendingReleases.Clear();
    }

    private InputEvent AsInputEvent(int slot, Note note, bool isPressed)
    {
        return new InputEvent
        {
            Action = NoteUtils.GetInputActionForNoteType(note.NoteType),
            Player = slot,
            IsPressed = isPressed
        };
    }
}