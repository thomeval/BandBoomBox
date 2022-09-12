using System.Collections.Generic;
using System.Linq;
using Assets;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SongSelectManager : ScreenManager
{
    
    public SelectedSongFrame SelectedSongFrame;

    public int SelectedSongIndex;

    public PlayerSongSelectFrame[] PlayerOptionsFrames = new PlayerSongSelectFrame[2];
    public DifficultyDisplay DifficultyDisplay;
    public HighScoreDisplay HighScoreDisplay;
    public Text TxtSongCount;
    public Text TxtSortMode;

    public SongList SongList;

    public List<SongData> OrderedSongs { get; private set; } = new List<SongData>();

    [SerializeField]
    private string _songSortMode = "TITLE";

    public string SongSortMode
    {
        get { return _songSortMode; }
        set
        {
            _songSortMode = value;
            SortSongs();
        }
    }

    private readonly string[] _availableSortModes = {"TITLE", "ARTIST", "BPM", "LENGTH"};

    public SongData SelectedSong
    {
        get
        {
            if (!OrderedSongs.Any())
            {
                return null;
            }
            return OrderedSongs[SelectedSongIndex];
        }
    }
    void Awake()
    {
        if (!FindCoreManager())
        {
            return;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        CoreManager.MenuMusicManager.StopAll();
        CoreManager.SongManager.StopSong();
        SetupPlayerOptionsFrames();
        SongSortMode = CoreManager.Settings.LastSortSongMode;
        SortSongs();
        SelectLastPlayedSong();
        ShowSelectedSong();
    }

    private void SortSongs()
    {

        var songs = CoreManager.SongLibrary.Songs;
        var selectedId = SelectedSong?.ID;

        switch (SongSortMode)
        {
            case "TITLE":
                OrderedSongs = songs.OrderBy(e => e.Title).ToList();
                break;
            case "ARTIST":
                OrderedSongs = songs.OrderBy(e => e.Artist).ToList();
                break;
            case "BPM":
                OrderedSongs = songs.OrderBy(e => e.Bpm).ToList();
                break;
            case "LENGTH":
                OrderedSongs = songs.OrderBy(e => e.Length).ToList();
                break;
        }

        SetSelectedIndex(selectedId);

        TxtSongCount.text = "" + OrderedSongs.Count;
        TxtSortMode.text = SongSortMode;
        CoreManager.Settings.LastSortSongMode = SongSortMode;
    }

    private void SelectLastPlayedSong()
    {
        var lastPlayedSongId = CoreManager.Settings.LastPlayedSong;
        SetSelectedIndex(lastPlayedSongId);
    }

    private void SetupPlayerOptionsFrames()
    {
        for (int x = 0; x < PlayerOptionsFrames.Length; x++)
        {
            PlayerOptionsFrames[x].Player = CoreManager.PlayerManager.GetPlayer(x + 1);
            PlayerOptionsFrames[x].Refresh();
        }
    }
    private void SetSelectedIndex(string songId)
    {
        if (songId == null)
        {
            return;
        }

        var songData = OrderedSongs.FirstOrDefault(e => e.ID == songId);
        if (songData != null)
        {
            SelectedSongIndex = OrderedSongs.IndexOf(songData);
        }
        SongList.PopulateSongListItems();
    }

    private void ShowSelectedSong()
    {

        var selectedSong = OrderedSongs[SelectedSongIndex];
        SelectedSongFrame.SelectedSong = selectedSong;
        CoreManager.SongPreviewManager.PlayPreview(selectedSong);
        DifficultyDisplay.SongData = selectedSong;

        var playerCount = CoreManager.PlayerManager.Players.Count;
        var highScore =
            CoreManager.HighScoreManager.GetTeamScore(selectedSong.ID, selectedSong.Version, playerCount);

        HighScoreDisplay.Display(highScore, playerCount);
    }

    public override void OnPlayerInput(InputEvent inputEvent)
    {

        if (!inputEvent.IsPressed)
        {
            return;
        }

        switch (inputEvent.Action)
        {
            case InputAction.Up:
                MoveSelection(-1);
                break;
            case InputAction.Down:
                MoveSelection(1);
                break;
            case InputAction.Pause:
                OnSongDecided();
                break;
            case InputAction.A:
                OnSongDecided();
                break;
            case InputAction.B:
            case InputAction.Back:
                CoreManager.SongPreviewManager.StopPreviews();
                SceneTransition(GameScene.PlayerJoin);
                break;
            case InputAction.Y:
                SongSortMode = Helpers.GetNextValue(_availableSortModes, SongSortMode, 1, true);
                break;
        }
    }
    private void RefreshPlayerText(int playerSlot)
    {
        PlayerOptionsFrames[playerSlot-1].Refresh();
    }


    private void MoveSelection(int delta)
    {
        var songCount = OrderedSongs.Count;

        if (songCount == 0)
        {
            return;
        }

        SelectedSongIndex = Helpers.Wrap(SelectedSongIndex + delta, songCount - 1);
        SongList.MoveSelection(delta);
        ShowSelectedSong();
    }
    private void OnSongDecided()
    {
        CoreManager.SelectedSong = SelectedSong.ID;
        CoreManager.Settings.LastPlayedSong = SelectedSong.ID;
        CoreManager.Settings.Save();
        SceneTransition(GameScene.DifficultySelect);
    }


    public TeamScore GetTeamScore(SongData songData)
    {
        var playerCount = CoreManager.PlayerManager.Players.Count;
        var teamScore = CoreManager.HighScoreManager.GetTeamScore(songData.ID, songData.Version, playerCount);
        return teamScore;
    }
}
