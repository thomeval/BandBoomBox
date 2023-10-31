using System.Linq;

public class DifficultySelectManager : ScreenManager
{
    public DifficultySelectFrame[] DifficultySelectFrames;

    public int ReadyPlayers
    {
        get { return DifficultySelectFrames.Count(e => e.State == DifficultySelectState.Ready); }
    }

    public int JoinedPlayers
    {
        get { return DifficultySelectFrames.Count(e => e.State != DifficultySelectState.NotJoined); }
    }

    private void Awake()
    {
        FindCoreManager();
    }
    // Start is called before the first frame update
    void Start()
    {
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
                frame.State = DifficultySelectState.Selecting;
            }
            else
            {
                TryReturnToSongSelect();
            }
        }
        else
        {
            ApplySelectedChart(args.Player, frame.SelectedSongChart);
            frame.State = DifficultySelectState.Ready;
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
        var player = CoreManager.PlayerManager.Players.Single(e => e.Slot == playerSlot);
        player.Difficulty = selectedChart.Difficulty;
        player.ChartGroup = selectedChart.Group;
    }

    private void TryReturnToSongSelect()
    {
        SceneTransition(GameScene.SongSelect);
    }
}
