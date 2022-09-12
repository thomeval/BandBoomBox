using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Assets;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ChartEditorManager : ScreenManager
{
    public string NoteLabels = "WASD";

    public Text TxtCursorPosition;
    public Text TxtChartDifficulty;
    public Text TxtStepSize;
    public Text TxtScrollSpeed;

    public SongChart CurrentChart;
    public SongData CurrentSongData;
    public NoteManager NoteManager;
    public ChartEditorMenuManager MenuManager;
    public EditorNotePaletteSet EditorNotePaletteSet;
    public ChartEditorNotePlacer ChartEditorNotePlacer;

    private SongManager _songManager;

    private double _playbackStartPosition;

    private static readonly int[] _scrollSpeeds = { 200, 250, 300, 400, 500, 600, 700, 800, 900, 1000, 1200, 1400, 1600, 1800, 2000 };

    [SerializeField]
    private ChartEditorState _chartEditorState;
    public ChartEditorState ChartEditorState
    {
        get
        {
            return _chartEditorState;
        }
        set
        {
            _chartEditorState = value;
            MenuManager.SetMenuVisibility(value);
        }
    }

    public NoteGenerator NoteGenerator;
    private readonly int[] _stepSizes = new[] { 1, 2, 3, 4, 6, 8};

    #region Properties

    [SerializeField] private int _cursorStepSize = 1;

    /// <summary>
    /// Gets or sets the amount that the cursor position should change when moving left or right, represented as 1 / beats. Higher numbers result in smaller steps.
    /// </summary>
    public int CursorStepSize
    {
        get
        {
            return _cursorStepSize;
        }
        set
        {
            _cursorStepSize = value;
            UpdateHud();
        }

    }

    [SerializeField]
    private double _cursorPosition;

    /// <summary>
    /// Gets or sets the position of the cursor, in beats, relative to the song's offset point.
    /// </summary>
    public double CursorPosition
    {
        get
        {
            return _cursorPosition;
        }
        set
        {
            _cursorPosition = value;
            UpdateHud();

        }
    }

    public Difficulty CurrentDifficulty
    {
        get
        {
            return CurrentChart.Difficulty;
        }
    }

    #endregion

    [Header("Sounds")]
    public AudioSource SfxMistake;
    public AudioSource SfxSelectionConfirmed;
    public AudioSource SfxSelectionCancelled;
    public AudioSource SfxSaveComplete;


    private void UpdateHud()
    {
        TxtCursorPosition.text = string.Format(CultureInfo.InvariantCulture, "{0:F2}", CursorPosition);
        TxtStepSize.text = $"Step: 1/{CursorStepSize} beats";
        TxtScrollSpeed.text = $"Speed: {NoteManager.ScrollSpeed}";
    }

    /// <summary>
    /// Gets the position of the cursor, in seconds relative to the song's offset point.
    /// </summary>
    public float CursorPositionInSeconds
    {
        get
        {
            return (float) (CursorPosition / CurrentSongData.Bpm * 60);
        }
    }

    void Awake()
    {
        Helpers.AutoAssign(ref NoteGenerator);
        _songManager = FindObjectOfType<SongManager>();
    }
    // Start is called before the first frame update
    void Start()
    {
        if (!FindCoreManager())
        {
            return;
        }

        LoadChart();
        SetupSongManager();
    }

    private void LoadChart()
    {
        var args = CoreManager.SceneLoadArgs;

        // TODO: Temp:
        CurrentSongData = Helpers.TryGetArg<SongData>(args, "SelectedSongData") ?? CoreManager.SongLibrary.Songs[0];
        CurrentChart =  Helpers.TryGetArg<SongChart>(args, "SelectedSongChart") ?? CurrentSongData.GetChart("Main", Difficulty.Hard);
        TxtChartDifficulty.text = CurrentChart.Difficulty.ToString();
        NoteGenerator.GenerateBeatLines(BeatLineType.Phrase, CurrentSongData.LengthInBeats, CurrentSongData.BeatsPerMeasure, this.NoteManager);
        NoteGenerator.LoadSongNotes(CurrentChart, this.NoteManager);
        NoteManager.CalculateAbsoluteTimes(CurrentSongData.Bpm);
        EditorNotePaletteSet.DisplayedPalette = CurrentChart.Difficulty;

        NoteManager.ApplyNoteSkin("Default", NoteLabels);
        EditorNotePaletteSet.SetNoteSkin("Default", NoteLabels);
    }

    private void SetupSongManager()
    {
        _songManager.LoadSong(CurrentSongData);
    }

    void Update()
    {
        if (ChartEditorState == ChartEditorState.Playback)
        {
            CursorPosition = _songManager.GetSongPositionInBeats();
        }
        UpdateNoteHighway();
    }

    private void UpdateNoteHighway()
    {
        NoteManager.SongPosition = CursorPositionInSeconds;
        NoteManager.UpdateNotes();
    }

    public void ChangeStepSize(int delta)
    {
        CursorStepSize = Helpers.GetNextValue<int>(_stepSizes, CursorStepSize, delta, false);

        // Round to nearest step that falls within the new step size.
        SnapCursorToStep();
    }

    private void SnapCursorToStep()
    {
        CursorPosition = Math.Round(CursorPosition * CursorStepSize) / CursorStepSize;

        if (CursorPosition > CurrentSongData.LengthInBeats)
        {
            MoveCursorSteps(-1);
        }
    }

    public override void OnPlayerInput(InputEvent inputEvent)
    {
        if (MenuManager.ShouldHandleState(ChartEditorState))
        {
            MenuManager.OnPlayerInputMenus(inputEvent, this.ChartEditorState);
            return;
        }
        switch (ChartEditorState)
        {
            case ChartEditorState.Edit:
                OnPlayerInputEdit(inputEvent);
                break;
            case ChartEditorState.Playback:
                OnPlayerInputPlayback(inputEvent);
                break;
        }
       
        base.OnPlayerInput(inputEvent);
    }

    private void OnPlayerInputPlayback(InputEvent inputEvent)
    {
        switch (inputEvent.Action)
        {
            case InputAction.Back:
                StopPlayback();
                CursorPosition = _playbackStartPosition;
                break;
            case InputAction.Editor_PlayPause:
                StopPlayback();
                SnapCursorToStep();
                break;
        }
    }

    private void OnPlayerInputEdit(InputEvent inputEvent)
    {
        if (ChartEditorNotePlacer.ShouldHandleInput(inputEvent))
        {
            ChartEditorNotePlacer.OnPlayerInput(inputEvent);
            return;
        }
        switch (inputEvent.Action)
        {
            case InputAction.Back:
                this.ChartEditorState = ChartEditorState.MainMenu;
                break;
            case InputAction.Editor_StepLeft:
                MoveCursorSteps(-1);
                break;
            case InputAction.Editor_StepRight:
                MoveCursorSteps(1);
                break;
            case InputAction.Editor_MeasureLeft:
                MoveCursorMeasures(-1);
                break;
            case InputAction.Editor_MeasureRight:
                MoveCursorMeasures(1);
                break;
            case InputAction.Editor_SectionLeft:
                MoveCursorSections(-1);
                break;
            case InputAction.Editor_SectionRight:
                MoveCursorSections(1);
                break;
            case InputAction.Editor_StepSizeUp:
                ChangeStepSize(1);
                break;
            case InputAction.Editor_StepSizeDown:
                ChangeStepSize(-1);
                break;
            case InputAction.Editor_ZoomIn:
                ChangeZoom(1);
                break;
            case InputAction.Editor_ZoomOut:
                ChangeZoom(-1);
                break;
            case InputAction.Editor_JumpToStart:
                CursorPosition = 0.0;
                break;
            case InputAction.Editor_JumpToEnd:
                CursorPosition = CurrentSongData.LengthInBeats;
                SnapCursorToStep();
                break;
            case InputAction.Editor_PlayPause:
                BeginPlayback();
                break;
        }

    }

    private void MoveCursorSections(int delta)
    {
        var orderedSections = CurrentSongData.Sections.OrderBy(e => e.Key);
        KeyValuePair<double, string> targetSection;
        if (delta >= 1)
        {
            targetSection = orderedSections.FirstOrDefault(e => e.Key > CursorPosition);
        }
        else
        {
            targetSection  = orderedSections.LastOrDefault(e => e.Key < CursorPosition);
        }

        // ReSharper disable once CompareOfFloatsByEqualityOperator
        // KeyValuePair does not have an == operator, and cannot be null as it is a struct.
        // Compare its keys and values manually instead to determine if the above returned the default result (meaning no match found).
        if (targetSection.Key == default && targetSection.Value == default)
        {
            PlaySfx("Mistake");
            return;
        }
        
        CursorPosition = targetSection.Key;
    }

    private void ChangeZoom(int delta)
    {
        var newValue = Helpers.GetNextValue<int>(_scrollSpeeds, (int) NoteManager.ScrollSpeed, delta, false);
        NoteManager.ScrollSpeed = newValue;
        UpdateHud();
    }

    private void MoveCursorMeasures(int measures)
    {
        var measureAmount = CurrentSongData.BeatsPerMeasure;
        MoveCursor(measures * measureAmount);
    }
    private void MoveCursorSteps(int steps)
    {
        var stepAmount = 1.0 / CursorStepSize * steps;
        MoveCursor(stepAmount);
    }

    private void MoveCursor(double delta)
    {
        if (CursorPosition + delta > CurrentSongData.LengthInBeats)
        {
            PlaySfx("Mistake");
            return;
        }

        if (CursorPosition <= 0.00 && delta < 0)
        {
            PlaySfx("Mistake");
            return;
        }

        this.CursorPosition += delta;
        this.CursorPosition = Math.Max(0.0, this.CursorPosition);
        this.CursorPosition = Math.Min(CurrentSongData.LengthInBeats, this.CursorPosition);
        SnapCursorToStep();
    }

    public void CloseEditor()
    {
        SceneTransition(GameScene.MainMenu);
    }

    public void PlaySfx(string sfxType)
    {
        switch (sfxType)
        {
            case "Mistake":
                SfxMistake.PlayUnlessNull();
                break;
            case "SelectionConfirmed":
                SfxSelectionConfirmed.PlayUnlessNull();
                break;
            case "SelectionCancelled":
                SfxSelectionCancelled.PlayUnlessNull();
                break;
            case "SaveComplete":
                SfxSaveComplete.PlayUnlessNull();
                break;
        }
    }

    public void PlayFromBeginning()
    {
        _cursorPosition = -8;
        BeginPlayback();
    }

    private void BeginPlayback()
    {
        _playbackStartPosition = Math.Max(0, _cursorPosition);
        _songManager.PlayFromPosition(CursorPositionInSeconds + CurrentSongData.Offset);
        ChartEditorState = ChartEditorState.Playback;
    }

    private void StopPlayback()
    {
        this.ChartEditorState = ChartEditorState.Edit;
        _songManager.StopSong();
    }

    public void SaveChart()
    {
        try
        {
            var sjson = SjsonUtils.ToSjson(NoteManager.Notes);
            CurrentChart.Notes = sjson;
            CoreManager.SongLibrary.SaveSongToDisk(CurrentSongData);
            PlaySfx("SaveComplete");
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            PlaySfx("Mistake");
        }

    }
}
