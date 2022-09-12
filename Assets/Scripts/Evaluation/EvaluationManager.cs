using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class EvaluationManager : ScreenManager
{
    public float DelayBeforeContinueAllowed = 2f;
    public PlayerResultFrame[] PlayerResultFrames = new PlayerResultFrame[4];
    public SongResultFrame SongResultFrame;
    public GameObject PbContinue;
    
    public AudioSource SfxGradeNormal;
    public AudioSource SfxGradeHigh;

    private DateTime _screenStartTime;

    public bool AllowContinue
    {
        get
        {
            return DateTime.Now.Subtract(_screenStartTime).TotalSeconds > DelayBeforeContinueAllowed;
        }
    }
    
    void Awake()
    {
        FindCoreManager();
    }

    // Start is called before the first frame update
    void Start()
    {
        _screenStartTime = DateTime.Now;
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
        StartCoroutine(DisplayContinueAfterDelay());
        PlayGradeSfx();
    }

    private IEnumerator DisplayContinueAfterDelay()
    {
        PbContinue.SetActive(false);
        yield return new WaitForSeconds(DelayBeforeContinueAllowed);
        PbContinue.SetActive(true);
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
            case InputAction.Left:
                ChangeResultPage(inputEvent.Player, -1);
                break;
            case InputAction.Right:
                ChangeResultPage(inputEvent.Player, 1);
                break;
            case InputAction.A:
            case InputAction.B:
            case InputAction.Pause:
            case InputAction.Back:
                if (AllowContinue)
                {
                    SceneTransition(GameScene.SongSelect);
                }
                break;
        }
    }

    private void ChangeResultPage(int player, int amount)
    {
        PlayerResultFrames[player - 1].DisplayedPage += amount;
    }

}
