using System;
using System.Collections.Generic;
using UnityEngine;

public class ChartEditorNoteClapper : MonoBehaviour
{
    public ChartEditorManager Parent;

    private int _currentNoteIndex = 0;

    private Note _currentNote
    {
        get
        {
            if (_currentNoteIndex < 0 || _currentNoteIndex >= Parent.NoteManager.Notes.Count)
            {
                return null;
            }
            return Parent.NoteManager.Notes[_currentNoteIndex];
        }
    }

    public void Tick(double currentBeat)
    {
        bool noteHit = false;
        bool releaseNoteHit = false;
        if (_currentNote == null)
        {
            return;
        }

        while (_currentNote != null && _currentNote.Position <= currentBeat)
        {
            if (_currentNote.NoteClass == NoteClass.Release)
            {
                releaseNoteHit = true;
            }
            else
            {
                noteHit = true;
            }
            _currentNoteIndex++;
        }

        if (releaseNoteHit)
        {
            Parent.PlaySfx(SoundEvent.Editor_NoteReleaseClap);
        }
        if (noteHit)
        {
            Parent.PlaySfx(SoundEvent.Editor_NoteClap);
        }
        
    }


    public void ResetIndex()
    {
        var note = Parent.NoteManager.FindNextNote(false);
        _currentNoteIndex = Parent.NoteManager.Notes.IndexOf(note);
    }
}