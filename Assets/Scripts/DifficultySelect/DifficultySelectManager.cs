using System.Linq;

public class DifficultySelectManager : ScreenManager
{
    public DifficultySelectFrame[] DifficultySelectFrames;
    public OnlinePlayerList OnlinePlayerList;

    private PlayerManager _playerManager;

    public int ReadyPlayers
    {
        get { return _playerManager.Players.Count(e => e.PlayerState == PlayerState.DifficultySelect_Ready); }
    }

    public int JoinedPlayers
    {
        get { return _playerManager.Players.Count(); }
    }

    private void Awake()
    {
        FindCoreManager();
        Helpers.AutoAssign(ref _playerManager);
    }
    // Start is called before the first frame update
    void Start()
    {
        OnlinePlayerList.gameObject.SetActive(CoreManager.IsNetGame);

        foreach (var frame in DifficultySelectFrames)
        {
            frame.State = DifficultySelectState.NotJoined;
            frame.Hide();
        }

        var selectedSong = CoreManager.SongLibrary[CoreManager.SelectedSong];
        foreach (var player in CoreManager.PlayerManager.GetLocalPlayers())
        {
            var frame = DifficultySelectFrames[player.Slot - 1];
            frame.Show();
            frame.Player = player;

            frame.DisplayedSongData = selectedSong;
            DifficultySelectFrames[player.Slot - 1].State = DifficultySelectState.Selecting;
        }

        UpdatePlayersState(PlayerState.DifficultySelect_Selecting);
        RefreshPlayerList();
    }

    public override void OnPlayerInput(InputEvent inputEvent)
    {
        DifficultySelectFrames[inputEvent.Player - 1].HandleInput(inputEvent);
    }

    public void MenuItemSelected(MenuEventArgs args)
    {
        var frame = DifficultySelectFrames[args.Player - 1];
        if (args.SelectedItem == "Back")
        {
            if (frame.State == DifficultySelectState.Ready)
            {
                UpdatePlayerState(frame.Player, PlayerState.DifficultySelect_Selecting);
                frame.State = DifficultySelectState.Selecting;
                RefreshPlayerList();
            }
            else
            {
                TryReturnToSongSelect();
            }
        }
        else
        {
            ApplySelectedChart(args.Player, frame.SelectedSongChart);
            UpdatePlayerState(frame.Player, PlayerState.DifficultySelect_Ready);
            frame.State = DifficultySelectState.Ready;
            RefreshPlayerList();
            TryStartSong();
        }
    }

    private void TryStartSong()
    {
        if (this.JoinedPlayers == this.ReadyPlayers)
        {
            SceneTransition(GameScene.Gameplay);
        }
    }

    private void ApplySelectedChart(int playerSlot, SongChart selectedChart)
    {
        var player = CoreManager.PlayerManager.GetLocalPlayers().Single(e => e.Slot == playerSlot);
        player.Difficulty = selectedChart.Difficulty;
        player.ChartGroup = selectedChart.Group;
    }

    private void TryReturnToSongSelect()
    {
        if (!CoreManager.IsNetGame)
        {
            SceneTransition(GameScene.SongSelect);
        }
    }

    public override void OnNetPlayerListUpdated()
    {
        base.OnNetPlayerListUpdated();
        OnlinePlayerList.RefreshAll();
    }

    public override void OnNetPlayerUpdated(Player player)
    {
        base.OnNetPlayerUpdated(player);
        OnlinePlayerList.RefreshAll();
        TryStartSong();
    }

    public void RefreshPlayerList()
    {
        OnlinePlayerList.RefreshAll();
        TryStartSong();
    }
}
