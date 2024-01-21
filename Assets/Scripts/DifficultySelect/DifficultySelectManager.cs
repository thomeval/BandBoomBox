using System;
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
            frame.Hide();
        }

        var selectedSong = CoreManager.SongLibrary[CoreManager.SelectedSong];
        foreach (var player in CoreManager.PlayerManager.GetLocalPlayers())
        {
            var frame = DifficultySelectFrames[player.Slot - 1];
            frame.Show();
            frame.Player = player;

            frame.DisplayedSongData = selectedSong;
            DifficultySelectFrames[player.Slot - 1].State = PlayerState.DifficultySelect_Selecting;
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

        switch (frame.State)
        {
            case PlayerState.DifficultySelect_Selecting:
                DifficultyMenuItemSelected(frame, args);
                break;
            case PlayerState.DifficultySelect_Ready:
                ReadyMenuItemSelected(frame, args);
                break;
            case PlayerState.DifficultySelect_ConfirmDisconnect:
                ConfirmDisconnectMenuItemSelected(frame, args);
                break;
            case PlayerState.DifficultySelect_ConfirmNerf:
                ConfirmNerfMenuItemSelected(frame, args);
                break;
        }
    }

    private void ConfirmNerfMenuItemSelected(DifficultySelectFrame frame, MenuEventArgs args)
    {
        if (args.SelectedItem == "Yes")
        {
            frame.Player.ProfileData.SeenNerfWarning = true;
            DifficultyMenuItemSelected(frame, args);
            return;
        }

        UpdateFrameState(frame, PlayerState.DifficultySelect_Selecting);
    }

    private void ConfirmDisconnectMenuItemSelected(DifficultySelectFrame frame, MenuEventArgs args)
    {
        if (args.SelectedItem == "Yes")
        {
            CoreManager.SongPreviewManager.StopPreviews();
            if (CoreManager.IsNetGame && CoreManager.IsHost)
            {
                // Send shutdown RPC to all clients.
                CoreManager.ServerNetApi.ShutdownNetGameServerRpc();
            }
            else
            {
                // MainMenuScene will shut down NetworkManager.
                SceneTransition(GameScene.MainMenu);
            }
            return;
        }

        UpdateFrameState(frame, PlayerState.DifficultySelect_Selecting);

    }

    private void ReadyMenuItemSelected(DifficultySelectFrame frame, MenuEventArgs args)
    {
        if (args.SelectedItem == "Back")
        {
            UpdateFrameState(frame, PlayerState.DifficultySelect_Selecting);
        }
    }

    private void DifficultyMenuItemSelected(DifficultySelectFrame frame, MenuEventArgs args)
    {
        if (args.SelectedItem == "Back")
        {
            TryReturnToSongSelect(frame);
            return;
        }

        if (ShowNerfWarning(frame))
        {
            UpdateFrameState(frame, PlayerState.DifficultySelect_ConfirmNerf);
            return;
        }

        ApplySelectedChart(args.Player, frame.SelectedSongChart);
        UpdateFrameState(frame, PlayerState.DifficultySelect_Ready);
        TryStartSong();
    }

    private bool ShowNerfWarning(DifficultySelectFrame frame)
    {
        return frame.SelectedDifficulty == Difficulty.Nerf && !frame.Player.ProfileData.SeenNerfWarning;
    }

    private void TryStartSong()
    {
        if (this.JoinedPlayers == this.ReadyPlayers)
        {
            UpdatePlayersState(PlayerState.Gameplay_Loading);
            SceneTransition(GameScene.Gameplay);
        }
    }

    private void ApplySelectedChart(int playerSlot, SongChart selectedChart)
    {
        var player = CoreManager.PlayerManager.GetLocalPlayers().Single(e => e.Slot == playerSlot);
        player.Difficulty = selectedChart.Difficulty;
        player.ChartGroup = selectedChart.Group;
    }

    private void TryReturnToSongSelect(DifficultySelectFrame frame)
    {
        if (!CoreManager.IsNetGame)
        {
            SceneTransition(GameScene.SongSelect);
            return;
        }

        UpdateFrameState(frame, PlayerState.DifficultySelect_ConfirmDisconnect);
    }

    private void UpdateFrameState(DifficultySelectFrame frame, PlayerState state)
    {
        UpdatePlayerState(frame.Player, state);
        frame.State = state;
        RefreshPlayerList();
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
