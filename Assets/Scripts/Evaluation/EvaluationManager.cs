using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class EvaluationManager : ScreenManager
{
    public float DelayBeforeContinueAllowed = 2f;
    public PlayerResultFrame[] PlayerResultFrames = new PlayerResultFrame[4];
    public PlayerResultFrame[] WidePlayerResultFrames = new PlayerResultFrame[4];
    public SongResultFrame SongResultFrame;
    public GameObject PbContinue;
    public bool UseWidePlayerResultFrames;

    private DateTime _screenStartTime;
    private readonly float[] _percentSfxCutoffs = { 0.8f, 0.9f, 0.96f };
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
        UseWidePlayerResultFrames = CoreManager.PlayerManager.GetLocalPlayers().Count <= 2;
        foreach (var frame in PlayerResultFrames)
        {
            frame.Hide();
        }

        foreach (var frame in WidePlayerResultFrames)
        {
            frame.Hide();
        }

        foreach (var player in CoreManager.PlayerManager.GetLocalPlayers())
        {

            var isPersonalBest = CoreManager.ProfileManager.SavePlayerScore(player, CoreManager.LastTeamScore.SongId, CoreManager.LastTeamScore.SongVersion);

            DisplayPlayerResultFrame(player,  isPersonalBest);
            player.SongsPlayed++;

        }

        var isTeamBest = CoreManager.HighScoreManager.AddTeamScore(CoreManager.LastTeamScore);

        SongResultFrame.DisplayResult(CoreManager.LastTeamScore, isTeamBest);
        CoreManager.SaveAllActiveProfiles();
        StartCoroutine(DisplayContinueAfterDelay());
        StartCoroutine(PlayGradeSfx());
    }

    private void DisplayPlayerResultFrame(Player player, bool isPersonalBest)
    {
        var frame = UseWidePlayerResultFrames
            ? WidePlayerResultFrames[player.Slot - 1]
            : PlayerResultFrames[player.Slot - 1];

        var stars = CoreManager.LastTeamScore.Stars;
        var numPlayers = CoreManager.PlayerManager.GetLocalPlayers().Count;
        frame.DisplayResult(player, isPersonalBest, stars, numPlayers);
        frame.DisplayedPage = 0;

        var totalModifier = frame.ExpModifierList.TotalExpModifier;
        player.ApplyExpGain(totalModifier);
    }

    private IEnumerator DisplayContinueAfterDelay()
    {
        PbContinue.SetActive(false);
        yield return new WaitForSeconds(DelayBeforeContinueAllowed);
        PbContinue.SetActive(true);
    }

    private IEnumerator PlayGradeSfx()
    {
        var maxPerfPercent = CoreManager.PlayerManager.GetLocalPlayers().Max(e => e.PerfPercent);
        var sfxId = _percentSfxCutoffs.Count(e => e <= maxPerfPercent);

        yield return new WaitForSeconds(0.5f);
        CoreManager.SoundEventHandler.PlayEvaluationGradeSfx(sfxId);
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
