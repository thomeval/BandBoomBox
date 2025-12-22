using System;
using System.Linq;

public class PlayerJoinManager : ScreenManager
{
    public PlayerJoinFrame[] PlayerJoinFrames = new PlayerJoinFrame[4];
    public NetworkPlayerList NetworkPlayerList;

    private PlayerManager _playerManager;
    public virtual int ReadyPlayerCount
    {
        get { return PlayerJoinFrames.Count(e => e.State == PlayerState.PlayerJoin_Ready); }
    }

    public virtual int JoinedPlayerCount
    {
        get
        {
            return PlayerJoinFrames.Count(e => e.State != PlayerState.NotPlaying);
        }
    }

    void Awake()
    {
        if (!FindCoreManager())
        {
            return;
        }

        Helpers.AutoAssign(ref _playerManager);
        NetworkPlayerList.gameObject.SetActive(CoreManager.IsNetGame);

        foreach (var frame in PlayerJoinFrames)
        {
            frame.PlayerLeft += HandlePlayerLeft;
        }
    }

    private void HandlePlayerLeft(object sender, EventArgs e)
    {
        var player = ((PlayerJoinFrame)sender).Player;
        if (player.Slot > 1)
        {
            _playerManager.RemovePlayer(player.Slot);
            _playerManager.UpdateAllowPlayerJoining();
            RefreshPlayerList();
            DisplayFrames();

            if (CoreManager.IsNetGame)
            {
                CoreManager.ServerNetApi.RemoveNetPlayerServerRpc(player.Slot);
            }
        }
        else
        {
            ReturnToMainMenu();
        }
    }

    private void ReturnToMainMenu()
    {
        CoreManager.PlayerManager.SetPlayerCount(1);
        CoreManager.PlayerManager.AllowPlayerJoining = false;

        if (CoreManager.IsNetGame && CoreManager.IsHost)
        {
            CoreManager.ServerNetApi.ShutdownNetGameServerRpc();
            return;
        }

        if (CoreManager.IsNetGame)
        {
            CoreManager.ServerNetApi.RemoveNetPlayerServerRpc();
        }

        SceneTransition(GameScene.MainMenu);
    }

    // Start is called before the first frame update
    void Start()
    {
        CoreManager.PlayerManager.AllowPlayerJoining = true;
        SetupInitialFrameStates();
        ForceDisableAllyBoost();
    }

    private void ForceDisableAllyBoost()
    {
        if (!CoreManager.IsNetGame)
        {
            return;
        }

        foreach (var player in CoreManager.PlayerManager.GetLocalPlayers())
        {
            player.ProfileData.AllyBoostMode = AllyBoostMode.Off;
        }
    }

    private void SetupInitialFrameStates()
    {
        foreach (var frame in PlayerJoinFrames)
        {
            frame.gameObject.SetActive(false);
        }

        for (var x = 1; x <= _playerManager.MaxLocalPlayers; x++)
        {

            var player = _playerManager.GetLocalPlayer(x);
            var frame = PlayerJoinFrames[x - 1];
            frame.gameObject.SetActive(true);

            frame.ToggleMenuOptions(CoreManager.Settings.EnableMomentumOption, !CoreManager.IsNetGame, CoreManager.Settings.EnableSectionDifficulty, CoreManager.Settings.EnableLaneOrderOption);

            if (player != null)
            {
                player.Reset();
                player.IsParticipating = false;
                AssignFrameToPlayer(frame, player, false);
            }
            else
            {
                frame.State = PlayerState.NotPlaying;
            }
            frame.Refresh();
        }

        DisplayFrames();
    }

    private void AssignFrameToPlayer(PlayerJoinFrame frame, Player player, bool withSfx)
    {
        frame.AssignPlayer(player, withSfx);
    }
    public override void OnPlayerInput(InputEvent inputEvent)
    {
        PlayerJoinFrames[inputEvent.Player - 1].HandleInput(inputEvent);
        TryToStart();
    }

    private void TryToStart()
    {
        if (ReadyPlayerCount > 0 && JoinedPlayerCount == ReadyPlayerCount)
        {
            ForceDisableAllyBoost();
            CoreManager.PlayerManager.AllowPlayerJoining = false;
            CoreManager.SaveAllActiveProfiles();
            CoreManager.PlayerManager.SortPlayers();
            SceneTransition(GameScene.SongSelect);
        }
    }

    public override void OnPlayerJoined(Player player)
    {
        player.NetId = CoreManager.NetId;
        var useControllerNoteLabels = CoreManager.Settings.AutoSetNoteLabelsFromController;
        player.AutoSetLabelSkin(useControllerNoteLabels);
        player.Reset();
        player.IsParticipating = false;
        var frame = PlayerJoinFrames[player.Slot - 1];
        AssignFrameToPlayer(frame, player, true);
        CoreManager.PlayerManager.UpdateAllowPlayerJoining();

        SendNetPlayerJoined(player);
        RefreshPlayerList();
        DisplayFrames();
    }

    private void DisplayFrames()
    {
        var lastPlayer = CoreManager.PlayerManager.Players.Max(e => e.LocalSlot);

        PlayerJoinFrames[5].gameObject.SetActive(lastPlayer >= 5);
        PlayerJoinFrames[4].gameObject.SetActive(lastPlayer >= 4);
        PlayerJoinFrames[2].gameObject.SetActive(lastPlayer >= 2 || ! CoreManager.IsNetGame);
    }

    private void SendNetPlayerJoined(Player player)
    {
        if (!CoreManager.IsNetGame)
        {
            return;
        }

        CoreManager.ServerNetApi.RegisterNetPlayerServerRpc(PlayerDto.FromPlayer(player));
    }

    public override void OnNetPlayerListUpdated(bool playerJoined, bool playerLeft)
    {
        base.OnNetPlayerListUpdated(playerJoined, playerLeft);
        _playerManager.UpdateAllowPlayerJoining();
        NetworkPlayerList.RefreshAll();
    }

    public override void OnNetPlayerUpdated(Player player)
    {
        NetworkPlayerList.Refresh(player);
    }

    public void RefreshPlayerList()
    {
        NetworkPlayerList.RefreshAll();
    }
}
