using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CalibrationSceneManager : ScreenManager
{
    public InputField TxtPreviousLatency;
    public InputField TxtCurrentLatency;
    public InputField TxtCurrentScrollSpeed;
    public InputField TxtLastHit;
    public InputField TxtLastHitTiming;
    public InputField TxtLast10;
    public InputField TxtLast10Timing;
    public InputField TxtLast32;
    public InputField TxtLast32Timing;
    public Toggle ChkAutoAdjust;

    public Text LblCurrentTime;
    public NoteManager NoteManager;
    public Player Player;
    public PlayerHudManager PlayerHudManager;
    public NoteGenerator NoteGenerator;
    public Menu PauseMenu;
    public GameObject PauseMenuContainer;

    public int PerfectTimingCutoff = 15;
    public int GoodTimingCutoff = 30;
    public float PreviousLatency;

    public SongData CurrentSong;

    /// <summary>
    /// Gets the ID of the preferred song to use for calibration. This ID refers to "Lightseeker", which is included with the game.
    /// </summary>
    public const string PREFERRED_CALIBRATION_SONG = "ec4a377a-5fce-4896-a9ba-8470ec9fc31b";
    private HitJudge _hitJudge;


    [SerializeField]
    private readonly List<float> _hits = new();

    public float SongPosition
    {
        get { return _songManager.GetSongPosition(); }
    }

    public int ScrollSpeed
    {
        get { return CoreManager.Settings.EditorScrollSpeed; }
        set { CoreManager.Settings.EditorScrollSpeed = value; }
    }

    public float AudioLatency
    {
        get { return CoreManager.Settings.AudioLatency; }
        set { CoreManager.Settings.AudioLatency = value; }
    }
    public bool AutoAdjustEnabled
    {
        get { return ChkAutoAdjust.isOn; }
        set { ChkAutoAdjust.isOn = value; }
    }

    public bool PauseMenuShown
    {
        get { return PauseMenuContainer.activeSelf; }
        set { PauseMenuContainer.SetActive(value); }
    }

    private SongManager _songManager;

    void Awake()
    {
        FindCoreManager();
        _songManager = FindObjectOfType<SongManager>();
        _hitJudge = new HitJudge();
    }

    private void Start()
    {
        // Enable the Timing Display (defaults to Off)
        Player.TimingDisplayType = TimingDisplayType.EarlyLate;

        CoreManager.MenuMusicManager.StopAll();
        PreviousLatency = CoreManager.Settings.AudioLatency;
        FindCalibrationSong();
        BeginCalibration();
    }

    private void FindCalibrationSong()
    {
        CurrentSong = CoreManager.SongLibrary[PREFERRED_CALIBRATION_SONG];

        if (CurrentSong == null)
        {
            Debug.LogWarning($"Calibration song with ID {PREFERRED_CALIBRATION_SONG} not found. Using first available song instead.");
            CurrentSong = CoreManager.SongLibrary.Songs.FirstOrDefault();

            if (CurrentSong == null)
            {
                throw new NullReferenceException("No songs loaded. Cannot begin calibration without one!");
            }
        }
    }

    void Update()
    {
        NoteManager.SongPosition = this.SongPosition;
        NoteManager.SongPositionInBeats = _songManager.GetSongPositionInBeats();
        NoteManager.UpdateNotes();
        PlayerHudManager.UpdateHud(Player);
        PlayerHudManager.DisplayBeat(_songManager.GetSongPositionInBeats());
    }
    public void BeginCalibration()
    {
        _songManager.LoadSong(CurrentSong, OnSongLoaded);

        SetupNoteManager();
        DisplaySong(CurrentSong);
        ClearHits();
    }

    private void OnSongLoaded()
    {
        _songManager.StartSong();
        _songManager.SetAudioPosition(CurrentSong.AudioStart);

        // If the current song is not playing, SongManager.GetSongPositionInBeats() will return 0, which causes some notes to be visible prematurely.
        // Since the song has started, hide all notes immediately. They will then be positioned correctly as part of NoteManager.UpdateNotes().
        NoteManager.HideAllNotes();
    }

    private void SetupNoteManager()
    {
        var endBeat = (int)CurrentSong.LengthInBeats;
        NoteManager.ClearNotes();
        NoteManager.ScrollSpeed = this.ScrollSpeed;
        var notes = NoteGenerator.GenerateTestNotes(endBeat);
        NoteManager.AttachNotes(notes);
        NoteManager.ApplyNoteSkin("Default", "None");
        NoteManager.SetImpactZoneSprites(false);

        NoteGenerator.GenerateBeatLines(CurrentSong, NoteManager);
        NoteManager.CalculateAbsoluteTimes(CurrentSong.Bpm);

    }

    private void DisplaySong(SongData song)
    {
        TxtCurrentLatency.text = string.Format(CultureInfo.InvariantCulture, "{0:F0} ms", AudioLatency * 1000);
        TxtPreviousLatency.text = string.Format(CultureInfo.InvariantCulture, "{0:F0} ms", PreviousLatency * 1000);
        TxtCurrentScrollSpeed.text = string.Format(CultureInfo.InvariantCulture, "{0:F0}", this.ScrollSpeed);
    }

    private float? RollingAverage(int count)
    {

        if (_hits.Count < count)
        {
            return null;
        }

        return _hits.Take(count).Average();
    }

    public override void OnPlayerInput(InputEvent inputEvent)
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
        if (PauseMenuShown)
        {
            return;
        }

        switch (inputEvent.Action)
        {
            case InputAction.B:
            case InputAction.X:
                PlayerHudManager.ReleaseLane(2);
                break;
        }
    }

    private void HandleInputPress(InputEvent inputEvent)
    {
        if (PauseMenuShown)
        {
            PauseMenu.HandleInput(inputEvent);
            return;
        }

        switch (inputEvent.Action)
        {

            case InputAction.B:
            case InputAction.X:
                HitNotes();
                PlayerHudManager.FlashLane(2);
                break;
            case InputAction.Y:
                AutoAdjustEnabled = !AutoAdjustEnabled;
                break;
            case InputAction.LB:
                AdjustLatency(-0.005f);
                break;
            case InputAction.RB:
                AdjustLatency(0.005f);
                break;
            case InputAction.LT:
            case InputAction.Down:
                AdjustScrollSpeed(-100);
                break;
            case InputAction.RT:
            case InputAction.Up:
                AdjustScrollSpeed(100);
                break;
            case InputAction.Turbo:
                RestartSong();
                break;
            case InputAction.Back:
            case InputAction.Pause:
                _songManager.PauseSong(true);
                PauseMenuShown = true;
                break;
        }
    }

    private void MenuItemSelected(MenuEventArgs args)
    {
        switch (args.SelectedItem)
        {
            case "Continue":
                PauseMenuShown = false;
                RestartSong();
                break;
            case "Discard and exit":
                CoreManager.Settings.AudioLatency = PreviousLatency;
                CoreManager.Settings.Save();
                this.SceneTransition(GameScene.Options);
                break;
            case "Save and exit":
                CoreManager.Settings.Save();
                this.SceneTransition(GameScene.Options);
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
        var hitResult = _hitJudge.GetHitResult(deviation, 1, Difficulty.Beginner, 2, note.NoteType, note.NoteClass, false, false);
        PlayerHudManager.DisplayHitResult(hitResult);
        DisplayHit(deviation * 1000f);
        if (AutoAdjustEnabled)
        {
            AutoAdjustLatency(deviation);
        }
    }

    private void AutoAdjustLatency(float deviation)
    {
        deviation *= 1000;
        var multiplier = Math.Sign(deviation) * -1;
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

        AdjustLatency(result * multiplier);
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
            // Returns "Good (E)" or "Good (L)"
            return $"Good ({deviationText[0]})";
        }

        return deviationText;

    }

    #region Button Event Handlers

    public void AdjustLatency(float amount)
    {
        this.AudioLatency = Mathf.Clamp(this.AudioLatency + amount, -0.25f, 0.25f);
        DisplaySong(CurrentSong);
    }

    public void AdjustScrollSpeed(int amount)
    {
        var newValue = Math.Clamp(this.ScrollSpeed + amount, 200, 2000);
        this.ScrollSpeed = newValue;
        NoteManager.ScrollSpeed = newValue;
        DisplaySong(CurrentSong);
    }

    public void AdjustSongPosition(float amount)
    {
        var newPosition = _songManager.GetRawAudioPosition() + amount;
        _songManager.SetAudioPosition(newPosition);
    }

    public void RestartSong()
    {
        _songManager.PauseSong(false);
        _songManager.SetAudioPosition(CurrentSong.AudioStart);
        SetupNoteManager();
    }

    #endregion
}
