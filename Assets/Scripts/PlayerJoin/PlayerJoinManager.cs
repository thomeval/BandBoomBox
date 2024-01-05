using System;
using System.Linq;

public class PlayerJoinManager : ScreenManager
{
    public PlayerJoinFrame[] PlayerJoinFrames = new PlayerJoinFrame[4];
    public OnlinePlayerList OnlinePlayerList;

    public int MaxAllowedPlayers = 4;
    public virtual int ReadyPlayerCount
    {
        get { return PlayerJoinFrames.Count(e => e.State == PlayerJoinState.Ready); }
    }

    public virtual int JoinedPlayerCount
    {
        get
        {
            return PlayerJoinFrames.Count(e => e.State != PlayerJoinState.NotJoined);
        }
    }

    void Awake()
    {
        if (!FindCoreManager())
        {
            return;
        }

        MaxAllowedPlayers = CoreManager.IsNetGame ? 2 : 4;
        OnlinePlayerList.gameObject.SetActive(CoreManager.IsNetGame);

        foreach (var frame in PlayerJoinFrames)
        {
            frame.PlayerLeft += HandlePlayerLeft;
        }
    }

    private void HandlePlayerLeft(object sender, EventArgs e)
    {
        var player = ((PlayerJoinFrame)sender).Player;
        var manager = CoreManager.PlayerManager;
        if (player.Slot > 1)
        {
            manager.RemovePlayer(player.Slot);
            manager.AllowPlayerJoining = CoreManager.PlayerManager.GetLocalPlayers().Count < MaxAllowedPlayers;
            RefreshPlayerList();

            if (CoreManager.IsNetGame)
            {
                CoreManager.ServerNetApi.RemoveNetPlayerServerRpc(player.Slot);
            }
        }
        else
        {
            manager.SetPlayerCount(1);
            manager.AllowPlayerJoining = false;
            if (CoreManager.IsNetGame)
            {
                CoreManager.ServerNetApi.RemoveNetPlayerServerRpc();
            }
            SceneTransition(GameScene.MainMenu);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        CoreManager.PlayerManager.AllowPlayerJoining = true;
        SetupInitialFrameStates();
    }

    private void SetupInitialFrameStates()
    {
        foreach (var frame in PlayerJoinFrames)
        {
            frame.gameObject.SetActive(false);
        }

        var players = CoreManager.PlayerManager.Players;
        for (int x = 1; x <= MaxAllowedPlayers; x++)
        {

            var player = players.SingleOrDefault(e => e.Slot == x && e.IsLocalPlayer);
            var frame = PlayerJoinFrames[x - 1];
            frame.gameObject.SetActive(true);

            if (CoreManager.Settings.EnableMomentumOption)
            {
                frame.ShowMomentumOption();
            }

            if (player != null)
            {
                AssignFrameToPlayer(frame, player, false);
            }
            else
            {
                frame.State = PlayerJoinState.NotJoined;
            }
            frame.Refresh();
        }
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
            CoreManager.PlayerManager.AllowPlayerJoining = false;
            CoreManager.SaveAllActiveProfiles();
            CoreManager.PlayerManager.SortPlayers();
            SceneTransition(GameScene.SongSelect);
        }
    }

    public override void OnPlayerJoined(Player player)
    {
        player.NetId = CoreManager.NetId;
        player.AutoSetLabelSkin();
        var frame = PlayerJoinFrames[player.Slot - 1];
        AssignFrameToPlayer(frame, player, true);
        CoreManager.PlayerManager.AllowPlayerJoining = CoreManager.PlayerManager.GetLocalPlayers().Count < MaxAllowedPlayers;

        SendNetPlayerUpdate(player);
        RefreshPlayerList();
    }

    public override void OnNetPlayerListUpdated()
    {
        OnlinePlayerList.RefreshAll();
    }

    public override void OnNetPlayerUpdated(Player player)
    {
        OnlinePlayerList.Refresh(player);

    }

    public void RefreshPlayerList()
    {
        OnlinePlayerList.RefreshAll();
    }
}
