using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EditorFineTunePage : EditorPageManager
{
    public override EditorPage EditorPage
    {
        get { return EditorPage.FineTune; }
    }

    public InputField TxtCurrentBpm;
    public InputField TxtCurrentOffset;
    public InputField TxtCurrentScrollSpeed;
    public InputField TxtLastHit;
    public InputField TxtLastHitTiming;
    public InputField TxtLast10;
    public InputField TxtLast10Timing;
    public InputField TxtLast32;
    public InputField TxtLast32Timing;
    public Toggle ChkAutoAdjustOffset;

    public Text LblCurrentTime;
    public Button DefaultButton;
    public NoteManager NoteManager;
    public Player Player;
    public PlayerHudManager PlayerHudManager;
    public NoteGenerator NoteGenerator;

    public int PerfectTimingCutoff = 15;
    public int GoodTimingCutoff = 30;

    private HitJudge _hitJudge;

    [SerializeField]
    private readonly List<float> _hits = new();

    public float SongPosition
    {
        get { return _songManager.GetSongPosition(); }
    }

    public bool AutoAdjustOffsetEnabled
    {
        get { return ChkAutoAdjustOffset.isOn; }
        set { ChkAutoAdjustOffset.isOn = value; }
    }

    private SongManager _songManager;


    public Action OnTuneComplete { get; set; }

    void Awake()
    {
        _songManager = FindObjectOfType<SongManager>();
        _hitJudge = new HitJudge();
    }

    void Update()
    {
        LblCurrentTime.text = string.Format(CultureInfo.InvariantCulture, "{0:F3}", _songManager.GetRawAudioPosition());
        NoteManager.SongPosition = this.SongPosition;
        NoteManager.SongPositionInBeats = _songManager.GetSongPositionInBeats();
        NoteManager.UpdateNotes();
        PlayerHudManager.UpdateHud(Player);
        PlayerHudManager.DisplayBeat(_songManager.GetSongPositionInBeats());
    }
    public void BeginFineTune()
    {
        EventSystem.current.SetSelectedGameObject(DefaultButton.gameObject);
        _songManager.LoadSong(Parent.CurrentSong, OnSongLoaded);

        SetupNoteManager();
        DisplaySong(Parent.CurrentSong);
        ClearHits();
    }

    private void OnSongLoaded()
    {
        _songManager.StartSong();
        _songManager.SetAudioPosition(Parent.CurrentSong.AudioStart);
    }

    private void SetupNoteManager()
    {
        var endBeat = (int)Parent.CurrentSong.LengthInBeats;
        NoteManager.ClearNotes();
        NoteGenerator.GenerateTestNotes(endBeat, ref NoteManager.Notes);
        NoteManager.AttachNotes();
        NoteManager.ApplyNoteSkin("Default", "None");

        NoteGenerator.GenerateBeatLines(Parent.CurrentSong, NoteManager);

        NoteManager.CalculateAbsoluteTimes(_songManager.CurrentSong.Bpm);
        NoteManager.ScrollSpeed = this.Player.ScrollSpeed;
    }

    private void DisplaySong(SongData song)
    {
        TxtCurrentBpm.text = string.Format(CultureInfo.InvariantCulture, "{0:F1}", song.Bpm);
        TxtCurrentOffset.text = string.Format(CultureInfo.InvariantCulture, "{0:F3}", song.Offset);
        TxtCurrentScrollSpeed.text = string.Format(CultureInfo.InvariantCulture, "{0:F0}", this.Player.ScrollSpeed);
    }

    private float? RollingAverage(int count)
    {

        if (_hits.Count < count)
        {
            return null;
        }

        return _hits.Take(count).Average();
    }

    public override void HandleInput(InputEvent inputEvent)
    {
        if (inputEvent.IsPressed)
        {
            HandleInputPress(inputEvent);
        }
        else
        {
            HandleInputRelease(inputEvent);
        }

    }

    private void HandleInputRelease(InputEvent inputEvent)
    {
        switch (inputEvent.Action)
        {
            case InputAction.B:
            case InputAction.X:
            case InputAction.Y:
                PlayerHudManager.ReleaseLane(2);
                break;
        }
    }

    private void HandleInputPress(InputEvent inputEvent)
    {
        switch (inputEvent.Action)
        {

            case InputAction.B:
            case InputAction.X:
            case InputAction.Y:
                HitNotes();
                PlayerHudManager.FlashLane(2);
                break;
            case InputAction.LB:
                AdjustOffset(-0.01f);
                break;
            case InputAction.RB:
                AdjustOffset(0.01f);
                break;
            case InputAction.LT:
                AdjustScrollSpeed(-100);
                break;
            case InputAction.RT:
                AdjustScrollSpeed(100);
                break;
            case InputAction.Turbo:
                RestartSong();
                break;
            case InputAction.Back:
                BtnDone_OnClick();
                break;
        }
    }

    private void HitNotes()
    {
        var noteType = NoteType.B;

        var note = NoteManager.FindNextNote(noteType, true);

        if (note == null)
        {
            return;
        }
        // Note was hit. Apply a hit result.
        var deviation = SongPosition - note.AbsoluteTime;
        NoteManager.RemoveNote(note);
        var hitResult = _hitJudge.GetHitResult(deviation, 1, Difficulty.Beginner, 2, note.NoteType, note.NoteClass, false);
        PlayerHudManager.DisplayHitResult(hitResult);
        DisplayHit(deviation * 1000f);
        if (AutoAdjustOffsetEnabled)
        {
            AutoAdjustOffset(deviation);
        }
    }

    private void AutoAdjustOffset(float deviation)
    {
        deviation *= 1000;
        var multiplier = Math.Sign(deviation);
        deviation = Math.Abs(deviation);

        var result = 0.0f;
        if (deviation > 100)
        {
            result = 0.025f;
        }
        else if (deviation > 50)
        {
            result = 0.01f;
        }
        else if (deviation > 15)
        {
            result = 0.005f;
        }

        AdjustOffset(result * multiplier);
    }

    public void ClearHits()
    {
        _hits.Clear();
        TxtLastHit.text = "";
        TxtLastHitTiming.text = "";
        TxtLast10.text = "";
        TxtLast10Timing.text = "";
        TxtLast32.text = "";
        TxtLast32Timing.text = "";
    }

    public void DisplayHit(float deviation)
    {
        TxtLastHit.text = string.Format(CultureInfo.InvariantCulture, "{0:F0} ms", deviation);
        TxtLastHitTiming.text = GetTiming(deviation);

        _hits.Insert(0, deviation);

        var avg = RollingAverage(10);
        TxtLast10.text = string.Format(CultureInfo.InvariantCulture, "{0:F0} ms", avg);
        TxtLast10Timing.text = GetTiming(avg);

        avg = RollingAverage(32);
        TxtLast32.text = string.Format(CultureInfo.InvariantCulture, "{0:F0} ms", avg);
        TxtLast32Timing.text = GetTiming(avg);
    }

    private string GetTiming(float? deviation)
    {
        var deviationText = deviation > 0 ? "Late" : "Early";
        if (!deviation.HasValue)
        {
            return "";
        }

        if (Mathf.Abs(deviation.Value) < PerfectTimingCutoff)
        {
            return "Perfect!";
        }

        if (Mathf.Abs(deviation.Value) < GoodTimingCutoff)
        {
            return $"Good ({deviationText[0]})";
        }

        return deviationText;

    }


    #region Button Event Handlers

    public void AdjustBpm(float amount)
    {
        Parent.CurrentSong.Bpm = Mathf.Clamp(Parent.CurrentSong.Bpm + amount, 0.0f, 999.0f);
        DisplaySong(Parent.CurrentSong);
    }

    public void AdjustOffset(float amount)
    {
        Parent.CurrentSong.Offset = Mathf.Clamp(Parent.CurrentSong.Offset + amount, 0.0f, 9999.0f);
        DisplaySong(Parent.CurrentSong);
    }
    public void AdjustScrollSpeed(int amount)
    {
        var newValue = this.Player.ScrollSpeed + amount;
        newValue = Math.Max(200, newValue);
        newValue = Math.Min(2000, newValue);
        this.Player.ScrollSpeed = newValue;
        NoteManager.ScrollSpeed = newValue;
        DisplaySong(Parent.CurrentSong);
    }

    public void AdjustSongPosition(float amount)
    {
        var newPosition = _songManager.GetRawAudioPosition() + amount;
        _songManager.SetAudioPosition(newPosition);
    }

    public void RestartSong()
    {
        _songManager.SetAudioPosition(Parent.CurrentSong.AudioStart);
        SetupNoteManager();
    }
    public void BtnDone_OnClick()
    {
        _songManager.StopSong();
        OnTuneComplete?.Invoke();
    }

    #endregion
}
