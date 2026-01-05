using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DifficultySelectFrame : MonoBehaviour
{
    [Header("Basics")]

    public Text TxtScrollSpeed;
    public Text TxtPlayerName;
    public ExpMeter ExpMeter;
    public Menu DifficultyMenu;
    public Menu ConfirmNerfMenu;
    public Menu ConfirmDisconnectMenu;
    public GameObject DifficultyMenuItemPrefab;
    public PlayerHighScoreDisplay PlayerHighScoreDisplay;
    public SongChartNoteCountDisplay NoteCountDisplay;
    public DifficultySelectManager Parent;

    [Header("Pages")]
    public FramePageForPlayerState[] Pages = new FramePageForPlayerState[0];

    [Header("Chart Groups")]
    public GameObject ChartGroupSelector;
    public Text TxtChartGroup;
    public Text TxtChartSelectedIndex;
    private string[] _chartGroups;
    public string SelectedChartGroup;

    [Header("Other")]
    public Text TxtDisconnectMessage;

    public SoundEventProvider SoundEventProvider;


    public Difficulty SelectedDifficulty
    {
        get
        {
            var selectedItem = DifficultyMenu.SelectedGameObject.GetComponent<DifficultyDisplayItem>();
            return selectedItem.Difficulty;
        }
    }

    public PlayerState State
    {
        get { return Player.PlayerState; }
        set
        {
            Player.PlayerState = value;
            UpdateShownPage();
        }
    }

    private void UpdateShownPage()
    {
        foreach (var page in Pages)
        {
            page.gameObject.SetActive(page.PlayerState == State);
        }

    }

    public SongChart SelectedSongChart
    {
        get { return DisplayedSongData.GetChart(SelectedChartGroup, SelectedDifficulty); }
    }

    [SerializeField]
    private SongData _displayedSongData;

    public SongData DisplayedSongData
    {
        get { return _displayedSongData; }
        set
        {
            _displayedSongData = value;
            Init();
        }
    }

    [SerializeField]
    private Player _player;
    public Player Player
    {
        get { return _player; }
        set
        {
            _player = value;
            TxtPlayerName.text = _player.Name;
            ExpMeter.Exp = _player.Exp;
        }
    }

    private SettingsManager _settingsManager
    {
        get
        {
            return Parent.CoreManager.Settings;
        }
    }

    private void Init()
    {
        Helpers.AutoAssign(ref SoundEventProvider);
        _chartGroups = DisplayedSongData.SongCharts.Select(e => e.Group).Distinct().ToArray();
        ChartGroupSelector.SetActive(_chartGroups.Length > 1);
        SelectedChartGroup = _chartGroups.Contains("Main") ? "Main" : _chartGroups[0];
        RefreshText();
        RefreshMenu();
        FetchHighScore();
        UpdateNoteCountDisplay();
        UpdateShownPage();
    }

    private void RefreshText()
    {
        TxtChartGroup.text = SelectedChartGroup;

        TxtScrollSpeed.text = "" + Player.ScrollSpeed;
        var idx = Array.IndexOf(_chartGroups, SelectedChartGroup) + 1;
        TxtChartSelectedIndex.text = $"{idx} / {_chartGroups.Length}";
    }

    private void RefreshMenu()
    {
        DifficultyMenu.ClearItems();
        var charts = DisplayedSongData.SongCharts.Where(e => e.Group == SelectedChartGroup && _settingsManager.IsDifficultyVisible(e.Difficulty)).ToArray();
        foreach (var chart in charts.OrderBy(e => e.DifficultyLevel).ThenBy(e => e.Difficulty))
        {
            var obj = Instantiate(DifficultyMenuItemPrefab);
            var ddi = obj.GetComponent<DifficultyDisplayItem>();
            ddi.DisplayDifficulty(chart.Difficulty, chart.DifficultyLevel);
            var score = GetHighScore(chart.Difficulty);
            var grade = score == null ? (Grade?)null : Helpers.PercentToGrade(score.PerfPercent);
            ddi.DisplayGrade(grade);
            DifficultyMenu.AddItem(obj);
        }
    }

    public void HandleInput(InputEvent inputEvent)
    {
        switch (this.Player.PlayerState)
        {
            case PlayerState.DifficultySelect_Ready:
                HandleInputReadyState(inputEvent);
                return;
            case PlayerState.DifficultySelect_Selecting:
                HandleInputSelectingState(inputEvent);
                return;
            case PlayerState.DifficultySelect_ConfirmDisconnect:
                HandleInputConfirmDisconnectState(inputEvent);
                return;
            case PlayerState.DifficultySelect_ConfirmNerf:
                HandleInputConfirmNerfState(inputEvent);
                return;
        }

    }

    private void HandleInputReadyState(InputEvent inputEvent)
    {
        switch (inputEvent.Action)
        {
            case InputAction.B:
            case InputAction.Back:
                DifficultyMenu.HandleInput(inputEvent);
                break;
        }
    }

    private void HandleInputSelectingState(InputEvent inputEvent)
    {
        switch (inputEvent.Action)
        {
            case InputAction.Left:
                ChangeChartGroup(-1);
                break;
            case InputAction.Right:
                ChangeChartGroup(1);
                break;
            case InputAction.LB:
                ChangeScrollSpeed(-1);
                break;
            case InputAction.RB:
                ChangeScrollSpeed(1);
                break;
            case InputAction.X:
                NoteCountDisplay.ToggleVisibility();
                PlayerHighScoreDisplay.ToggleVisibility();
                SoundEventProvider.PlaySfx(SoundEvent.SelectionShifted, Player.LocalSlot);
                break;
            default:
                DifficultyMenu.HandleInput(inputEvent);
                FetchHighScore();
                UpdateNoteCountDisplay();
                break;
        }
    }

    private void HandleInputConfirmDisconnectState(InputEvent inputEvent)
    {
        ConfirmDisconnectMenu.HandleInput(inputEvent);
    }

    private void HandleInputConfirmNerfState(InputEvent inputEvent)
    {
        ConfirmNerfMenu.HandleInput(inputEvent);
    }

    public PlayerScore GetHighScore(Difficulty diff)
    {
        var playerId = Player.ProfileId;
        var selectedSong = Parent.CoreManager.SongLibrary[Parent.CoreManager.SelectedSong];
        var group = SelectedChartGroup;

        var result = Parent.CoreManager.ProfileManager.GetPlayerHighScore(playerId, selectedSong.ID, selectedSong.Version, diff, group);
        return result;
    }

    private void FetchHighScore()
    {
        var score = GetHighScore(SelectedDifficulty);
        PlayerHighScoreDisplay.DisplayedScore = score;
    }

    private void UpdateNoteCountDisplay()
    {
        NoteCountDisplay.UpdateNoteCountDisplay(SelectedSongChart.NoteCounts);
    }

    private void ChangeScrollSpeed(int delta)
    {
        Player.ChangeScrollSpeed(delta);
        SoundEventProvider.PlaySfx(SoundEvent.SelectionShifted, Player.LocalSlot);
        RefreshText();
    }

    public void MenuItemSelected(MenuEventArgs args)
    {
        Parent.MenuItemSelected(args);
    }

    private void ChangeChartGroup(int delta)
    {
        if (_chartGroups.Length == 1)
        {
            return;
        }

        SoundEventProvider.PlaySfx(SoundEvent.SelectionShifted, Player.LocalSlot);
        SelectedChartGroup = Helpers.GetNextValue(_chartGroups, SelectedChartGroup, delta, true);
        RefreshMenu();
        RefreshText();
        FetchHighScore();
        UpdateNoteCountDisplay();
    }
}


