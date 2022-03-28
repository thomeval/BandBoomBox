using System.Linq;
using UnityEngine;

public class EvaluationManager : ScreenManager
{

    public PlayerResultFrame[] PlayerResultFrames = new PlayerResultFrame[4];
    public SongResultFrame SongResultFrame;

    public AudioSource SfxGradeNormal;
    public AudioSource SfxGradeHigh;

    void Awake()
    {
        FindCoreManager();
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (var frame in PlayerResultFrames)
        {
            frame.Hide();
        }

        foreach (var player in CoreManager.PlayerManager.GetLocalPlayers())
        {
            var isLevelUp = ExpLevelUtils.IsLevelUp(player.Exp, player.GetExpGain());
            var isPersonalBest = CoreManager.ProfileManager.SavePlayerScore(player, CoreManager.LastTeamScore.SongId, CoreManager.LastTeamScore.SongVersion);

            PlayerResultFrames[player.Slot - 1].DisplayResult(player, isPersonalBest, isLevelUp);
            PlayerResultFrames[player.Slot - 1].DisplayedPage = 0;
            player.SongsPlayed++;

            player.ApplyExpGain();

        }

        var isTeamBest = CoreManager.HighScoreManager.AddTeamScore(CoreManager.LastTeamScore);

        SongResultFrame.DisplayResult(CoreManager.LastTeamScore, isTeamBest);
        CoreManager.SaveAllActiveProfiles();
        PlayGradeSfx();
    }

    private void PlayGradeSfx()
    {
        var maxPerfPercent = CoreManager.PlayerManager.GetLocalPlayers().Max(e => e.PerfPercent);

        const float sfxDelay = 0.5f;
        const float percentForHighSfx = 0.9f; // S Grade or better

        if (maxPerfPercent >= percentForHighSfx)
        {
            SfxGradeHigh.PlayDelayed(sfxDelay);
        }
        else
        {
            SfxGradeNormal.PlayDelayed(sfxDelay);
        }
    }

    public override void OnPlayerInput(InputEvent inputEvent)
    {
        switch (inputEvent.Action)
        {
            case "Left":
                ChangeResultPage(inputEvent.Player, -1);
                break;
            case "Right":
                ChangeResultPage(inputEvent.Player, 1);
                break;
            case "A":
            case "B":
            case "Pause":
            case "Back":
                SceneTransition(GameScene.SongSelect);
                break;
        }
    }

    private void ChangeResultPage(int player, int amount)
    {
        PlayerResultFrames[player - 1].DisplayedPage += amount;
    }

}
