using System;
using UnityEngine;


public class ChartEditorPlaybackManager : MonoBehaviour
{
    private double _playbackStartPosition;

    private ChartEditorManager _parent;
    private HitJudge _hitJudge;

    public LaneFlasher LaneFlasher;
    public TimingDisplay TimingDisplay;

    void Awake()
    {
        Helpers.AutoAssign(ref _parent);
        _hitJudge = new HitJudge();
    }

    void Start()
    {
        TimingDisplay.SpriteCategory = "TimingEarlyLate";
    }

    public void OnPlayerInputPlayback(InputEvent inputEvent)
    {
        var noteType = NoteUtils.GetNoteTypeForInput(inputEvent.Action);

        if (noteType != null)
        {
            if (inputEvent.IsPressed)
            {
                HandleNoteInputPressed(noteType.Value);
            }
            else
            {
                HandleNoteInputReleased(noteType.Value);
            }

            return;
        }
        
        switch (inputEvent.Action)
        {
            case InputAction.Pause:
            case InputAction.Back:
                StopPlayback();
                _parent.CursorPosition = _playbackStartPosition;
                break;
            case InputAction.Turbo:
                PausePlayback();
                break;
        }
    }

    private void HandleNoteInputPressed(NoteType noteType)
    {
        var lane = NoteUtils.GetNoteLane(noteType);
        LaneFlasher.LaneButtonPressed(lane);

        var note = _parent.NoteManager.FindNearestNote(noteType);
        ProcessNote(note);
    }

    private void ProcessNote(Note note)
    {
        if (note == null)
        {
            return;
        }

        var deviation = _parent.SongManager.GetSongPosition() - note.AbsoluteTime;

        var result =
            _hitJudge.GetHitResult(deviation, 1, _parent.CurrentDifficulty, note.Lane, note.NoteType, note.NoteClass, false);
        TimingDisplay.ShowHit(result);
    }

    private void HandleNoteInputReleased(NoteType noteType)
    {
        var lane = NoteUtils.GetNoteLane(noteType);
        LaneFlasher.LaneButtonReleased(lane);

        var releaseNote = _parent.NoteManager.FindNearestRelease(lane);
        var pressNote = _parent.NoteManager.FindNearestNote(noteType);

        var currentPosition = _parent.SongManager.GetSongPosition();

        if (releaseNote == null)
        {
            return;
        }

        if (pressNote != null && pressNote.DistanceTo(currentPosition) < releaseNote.DistanceTo(currentPosition))
        {
            return;
        }

        ProcessNote(releaseNote);
    }

    public void PausePlayback()
    {
        StopPlayback();
        var newPosition =  Math.Min(_parent.CursorPosition, _parent.CurrentSongData.LengthInBeats);
        _parent.CursorPosition = newPosition;

        _parent.ClampCursorToLimits();
        _parent.SnapCursorToStep();
    }

    public void PlayFromBeginning()
    {
        _parent.ForceCursorPosition(-8.0);
        BeginPlayback();
    }

    public void BeginPlayback()
    {
        _playbackStartPosition = Math.Max(0, _parent.CursorPosition);
        var songPosition = Math.Max(0, _parent.CursorPositionInSeconds + _parent.CurrentSongData.Offset);
        _parent.SongManager.PlayFromPosition(songPosition);
        _parent.ChartEditorState = ChartEditorState.Playback;
        _parent.SetActionMap(ActionMapType.Gameplay);
        _parent.DisplayMessage("Press Esc to stop playback, or Space to pause.");
    }
    private void StopPlayback()
    {
        _parent.ChartEditorState = ChartEditorState.Edit;
        _parent.SongManager.StopSong();
        LaneFlasher.ReleaseAll();
        _parent.SetActionMap(ActionMapType.Editor);
        _parent.DisplayMessage("");
    }

}

