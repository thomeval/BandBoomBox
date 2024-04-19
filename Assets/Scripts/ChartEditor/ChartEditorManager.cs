using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ChartEditorManager : ScreenManager
{

    [Header("Text Controls")]
    public Text TxtCursorPosition;
    public Text TxtChartDifficulty;
    public Text TxtStepSize;
    public Text TxtScrollSpeed;
    public Text TxtSelectedRegion;
    public Text TxtMessage;

    public double? SelectedRegionStart;
    public double? SelectedRegionEnd;

    public Color MessageColorNormal = Color.white;
    public Color MessageColorError = new Color(1.0f, 0.5f, 0.5f);
    public Color MessageColorWarning = new Color(1.0f, 1.0f, 0.5f);

    [Header("Components")]
    public SongChart CurrentChart;
    public SongData CurrentSongData;
    public NoteManager NoteManager;
    public ChartEditorMenuManager MenuManager;
    public EditorNotePaletteSet EditorNotePaletteSet;
    public ChartEditorNotePlacer ChartEditorNotePlacer;
    public ChartEditorNoteTransformer NoteTransformer;
    public ChartEditorClipboard Clipboard;
    public ChartEditorOptions Options;
    public NoteGenerator NoteGenerator;
    public ChartEditorPlaybackManager PlaybackManager;
    public SongManager SongManager;
    public SongProgressMeter SongProgressMeter;
    public SectionProgressMeter SectionProgressMeter;
    public SongChartNoteCountDisplay NoteCountDisplay;

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

    public const Difficulty DEFAULT_PALETTE = Difficulty.Expert;

    private readonly float[] _stepSizes = new[] { 0.25f, 0.5f, 1, 2, 3, 4, 6, 8 };

    #region Properties

    [SerializeField] private float _cursorStepSize = 1;

    /// <summary>
    /// Gets or sets the amount that the cursor position should change when moving left or right, represented as 1 / beats. Higher numbers result in smaller steps.
    /// </summary>
    public float CursorStepSize
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

    public bool RegionSelected
    {
        get { return SelectedRegionStart != null && SelectedRegionEnd != null; }
    }

    #endregion

    private void UpdateHud()
    {
        TxtCursorPosition.text = string.Format(CultureInfo.InvariantCulture, "{0:F2}", CursorPosition);

        var stepText = CursorStepSize < 1
            ? $"Step: {1 / CursorStepSize}/1 beats"
            : $"Step: 1/{CursorStepSize} beats";

        TxtStepSize.text = stepText;
        TxtScrollSpeed.text = $"Speed: {NoteManager.ScrollSpeed}";
        UpdateHudSelectedRegion();
        SongProgressMeter.CurrentPosition = CursorPosition;
        SectionProgressMeter.CurrentPosition = CursorPosition;
    }

    private void UpdateHudSelectedRegion()
    {
        var selectedRegionText = "Selected: ";

        if (SelectedRegionStart == null && SelectedRegionEnd == null)
        {
            selectedRegionText += "None";
        }
        else
        {
            var startText = SelectedRegionStart == null
                ? "..."
                : SelectedRegionStart.Value.ToString("F2", CultureInfo.InvariantCulture);
            var endText = SelectedRegionEnd == null
                ? "..."
                : SelectedRegionEnd.Value.ToString("F2", CultureInfo.InvariantCulture);
            selectedRegionText += $"{startText} to {endText}";
        }

        TxtSelectedRegion.text = selectedRegionText;
    }

    /// <summary>
    /// Gets the position of the cursor, in seconds relative to the song's offset point.
    /// </summary>
    public float CursorPositionInSeconds
    {
        get
        {
            return (float)(CursorPosition / CurrentSongData.Bpm * 60);
        }
    }

    void Awake()
    {
        Helpers.AutoAssign(ref NoteGenerator);
        SongManager = FindObjectOfType<SongManager>();
    }
    // Start is called before the first frame update
    void Start()
    {
        if (!FindCoreManager())
        {
            return;
        }

        this.Options.Load(CoreManager.Settings);
        LoadChart();
        SetupSongManager();
        UpdateHud();
        DisplayMessage("To see the full list of controls, press Esc and select 'Controls'.");
    }

    private void LoadChart()
    {
        var args = CoreManager.SceneLoadArgs;

        CurrentSongData = Helpers.TryGetArg<SongData>(args, "SelectedSongData") ?? CoreManager.SongLibrary.Songs[0];
        CurrentChart = Helpers.TryGetArg<SongChart>(args, "SelectedSongChart") ?? CurrentSongData.GetChart("Main", Difficulty.Hard);
        TxtChartDifficulty.text = CurrentChart.Difficulty.GetDisplayName();

        NoteGenerator.GenerateBeatLines(CurrentSongData, this.NoteManager);
        var temp = NoteGenerator.LoadSongNotes(CurrentChart);
        var notes = NoteGenerator.InstantiateNotes(temp);
        NoteManager.AttachNotes(notes);
        NoteManager.CalculateAbsoluteTimes(CurrentSongData.Bpm);
        RefreshNoteCounts();
        ShowNotePalette();

        ApplyNoteSkin();
    }

    public void RefreshNoteCounts()
    {
        CurrentChart.NoteCounts = NoteCounter.CountNotes(NoteManager.Notes, CurrentSongData.Length - CurrentSongData.Offset);
        UpdateNoteCountDisplay();
    }

    public void UpdateNoteCountDisplay()
    {
        NoteCountDisplay.UpdateNoteCountDisplay(CurrentChart.NoteCounts);
    }

    public void ApplyNoteSkin()
    {
        NoteManager.ApplyNoteSkin(Options.NoteSkin, Options.LabelSkin);
        EditorNotePaletteSet.SetNoteSkin(Options.NoteSkin, Options.LabelSkin);
    }

    public void ShowNotePalette()
    {
        EditorNotePaletteSet.DisplayedPalette = Options.AllowAllNotes ? DEFAULT_PALETTE : CurrentChart.Difficulty;
        EditorNotePaletteSet.SetNoteSkin(Options.NoteSkin, Options.LabelSkin);
    }

    private void SetupSongManager()
    {
        SongManager.LoadSong(CurrentSongData, null);
        SongProgressMeter.StartPosition = 0.0;
        SongProgressMeter.EndPosition = CoreManager.SongManager.GetSongEndInBeats();
        SectionProgressMeter.DisplayedSong = CurrentSongData;
    }

    void Update()
    {
        if (ChartEditorState == ChartEditorState.Playback)
        {
            CursorPosition = SongManager.GetSongPositionInBeats();
        }
        UpdateNoteHighway();
    }

    private void UpdateNoteHighway()
    {
        NoteManager.SongPosition = CursorPositionInSeconds;
        NoteManager.SongPositionInBeats = (float)CursorPosition;
        NoteManager.UpdateNotes();
    }

    public void ChangeStepSize(int delta)
    {
        CursorStepSize = Helpers.GetNextValue<float>(_stepSizes, CursorStepSize, delta, false);

        // Round to nearest step that falls within the new step size.
        SnapCursorToStep();
        PlaySfx(SoundEvent.SelectionShifted);
    }

    public void ClampCursorToLimits()
    {
        CursorPosition = Math.Clamp(CursorPosition, 0.0, CurrentSongData.LengthInBeats);
    }

    public void SnapCursorToStep()
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
                PlaybackManager.OnPlayerInputPlayback(inputEvent);
                break;
        }

        base.OnPlayerInput(inputEvent);
    }


    private void OnPlayerInputEdit(InputEvent inputEvent)
    {
        if (ChartEditorNotePlacer.ShouldHandleInput(inputEvent))
        {
            ChartEditorNotePlacer.OnPlayerInput(inputEvent);
            return;
        }

        if (Clipboard.ShouldHandleInput(inputEvent))
        {
            Clipboard.OnPlayerInput(inputEvent);
        }

        switch (inputEvent.Action)
        {
            case InputAction.Back:
            case InputAction.Editor_Confirm:
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
                PlaybackManager.BeginPlayback();
                break;
            case InputAction.Editor_PlayFromBeginning:
                PlaybackManager.PlayFromBeginning();
                break;
            case InputAction.Editor_SelectRegion:
                SetSelectedRegion();
                break;
            case InputAction.Editor_SwapNoteHands:
                NoteTransformer.SwapHandsAtCurrentPosition();
                break;
        }

    }

    private void SetSelectedRegion()
    {
        if (SelectedRegionStart == null ^ SelectedRegionEnd == null)
        {
            SelectedRegionEnd = CursorPosition;
            if (SelectedRegionStart != SelectedRegionEnd)
            {
                PlaySfx(SoundEvent.Editor_SelectRegionEnd);
            }
            else
            {
                SelectedRegionStart = null;
                SelectedRegionEnd = null;
                PlaySfx(SoundEvent.SelectionCancelled);
            }
        }
        else
        {
            SelectedRegionStart = CursorPosition;
            SelectedRegionEnd = null;
            PlaySfx(SoundEvent.Editor_SelectRegionStart);
        }

        if (SelectedRegionEnd != null && SelectedRegionStart != null &&
            SelectedRegionStart.Value > SelectedRegionEnd.Value)
        {
            (SelectedRegionStart, SelectedRegionEnd) = (SelectedRegionEnd, SelectedRegionStart);
        }

        UpdateHud();
        NoteManager.HighlightRegion((float?)SelectedRegionStart, (float?)SelectedRegionEnd, CurrentSongData.Bpm);
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
            targetSection = orderedSections.LastOrDefault(e => e.Key < CursorPosition);
        }

        // ReSharper disable once CompareOfFloatsByEqualityOperator
        // KeyValuePair does not have an == operator, and cannot be null as it is a struct.
        // Compare its keys and values manually instead to determine if the above returned the default result (meaning no match found).
        if (targetSection.Key == default && targetSection.Value == default)
        {
            PlaySfx(SoundEvent.Mistake);
            return;
        }

        CursorPosition = targetSection.Key;
    }

    private void ChangeZoom(int delta)
    {
        var newValue = Helpers.GetNextValue<int>(Player.ScrollSpeeds, NoteManager.ScrollSpeed, delta, false);
        NoteManager.ScrollSpeed = newValue;
        UpdateHud();
    }

    private void MoveCursorMeasures(int measures)
    {
        var measureAmount = CurrentSongData.BeatsPerMeasure;
        MoveCursor(measures * measureAmount);
    }
    public void MoveCursorSteps(int steps)
    {
        var stepAmount = 1.0 / CursorStepSize * steps;
        MoveCursor(stepAmount);
    }

    private void MoveCursor(double delta)
    {
        if (CursorPosition + delta > CurrentSongData.LengthInBeats)
        {
            PlaySfx(SoundEvent.Mistake);
            return;
        }

        if (CursorPosition <= 0.00 && delta < 0)
        {
            PlaySfx(SoundEvent.Mistake);
            return;
        }

        this.CursorPosition += delta;
        this.CursorPosition = Math.Max(0.0, this.CursorPosition);
        this.CursorPosition = Math.Min(CurrentSongData.LengthInBeats, this.CursorPosition);
        SnapCursorToStep();
    }

    public void CloseEditor()
    {
        SceneTransition(GameScene.Editor);
        this.Options.Save(CoreManager.Settings);
    }

    public void SaveChart()
    {
        try
        {
            var validationResult = ChartValidator.ValidateNotes(NoteManager.Notes);

            if (!string.IsNullOrEmpty(validationResult))
            {
                throw new Exception(validationResult);
            }

            var sjson = SjsonUtils.ToSjson(NoteManager.Notes);
            CurrentChart.Notes = sjson;
            CurrentChart.NoteCounts = NoteCounter.CountNotes(CurrentChart, CurrentSongData.Length - CurrentSongData.Offset);
            NoteCountDisplay.UpdateNoteCountDisplay(CurrentChart.NoteCounts);

            CoreManager.SongLibrary.SaveSongToDisk(CurrentSongData);
            PlaySfx(SoundEvent.Editor_SaveComplete);
            DisplayMessage($"Successfully saved SJSON file to {CurrentSongData.SjsonFilePath} ");
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            DisplayMistake(e.Message);
        }

    }

    public void DisplayMessage(string message, bool isError = false)
    {
        TxtMessage.color = isError ? MessageColorError : MessageColorNormal;
        TxtMessage.text = message;
    }

    public void DisplayMistake(string message)
    {
        DisplayMessage(message, true);
        PlaySfx(SoundEvent.Mistake);
    }
    public bool IsInPlayableArea(double position)
    {
        return position > 0 && position < CurrentSongData.LengthInBeats;
    }

    public void ForceCursorPosition(double position)
    {
        _cursorPosition = position;
    }

    public void ResolveHoldsWithReleases()
    {
        this.NoteGenerator.ResolveHoldsWithReleases(NoteManager.Notes);
    }
}
