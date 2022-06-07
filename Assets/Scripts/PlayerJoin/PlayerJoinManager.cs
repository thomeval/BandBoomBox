using System;
using System.Linq;
using Assets;

public class PlayerJoinManager : ScreenManager
{
    public PlayerJoinFrame[] PlayerJoinFrames = new PlayerJoinFrame[4];

    public const int MAX_ALLOWED_PLAYERS = 4;
    public int ReadyPlayerCount
    {
        get { return PlayerJoinFrames.Count(e => e.State == PlayerJoinState.Ready); }
    }

    public int JoinedPlayerCount
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

        foreach (var frame in PlayerJoinFrames)
        {
            frame.PlayerLeft += HandlePlayerLeft;
        }
    }

    private void HandlePlayerLeft(object sender, EventArgs e)
    {
        var player = ((PlayerJoinFrame)sender).Player;

        if (CoreManager.PlayerManager.GetLocalPlayers().Count > 1)
        {
            CoreManager.PlayerManager.RemovePlayer(player.Slot);
        }

        CoreManager.PlayerManager.AllowPlayerJoining = CoreManager.PlayerManager.GetLocalPlayers().Count < MAX_ALLOWED_PLAYERS;
    }

    // Start is called before the first frame update
    void Start()
    {
        CoreManager.PlayerManager.AllowPlayerJoining = true;
        SetupInitialFrameStates();
    }

    private void SetupInitialFrameStates()
    {
        var players = CoreManager.PlayerManager.Players;
        for (int x = 1; x < 5; x++)
        {
            var player = players.SingleOrDefault(e => e.Slot == x);
            var frame = PlayerJoinFrames[x - 1];
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

        if (ReadyPlayerCount > 0 && JoinedPlayerCount == ReadyPlayerCount)
        {
            CoreManager.PlayerManager.AllowPlayerJoining = false;
            CoreManager.SaveAllActiveProfiles();
            CoreManager.PlayerManager.SortPlayers();
            SceneTransition(GameScene.SongSelect);
        }
        else if (ReadyPlayerCount == 0 && JoinedPlayerCount == 0)
        {
            CoreManager.PlayerManager.AllowPlayerJoining = false;
            SceneTransition(GameScene.MainMenu);
        }
    }

    public override void OnPlayerJoined(Player player)
    {
        player.AutoSetLabelSkin();
        var frame = PlayerJoinFrames[player.Slot - 1];
        AssignFrameToPlayer(frame, player, true);
        CoreManager.PlayerManager.AllowPlayerJoining = CoreManager.PlayerManager.GetLocalPlayers().Count < MAX_ALLOWED_PLAYERS;
    }

}
