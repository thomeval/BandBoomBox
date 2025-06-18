using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;

public class PlayerHudManager : MonoBehaviour
{
    public int Slot;

    public Text TxtCombo;
    public Text TxtRanking;
    public Text TxtPerfPercent;
    public Text TxtDifficulty;
    public Text TxtAllyBoostCount;
    public Image ImgAllyBoostTickMeter;
    public SpriteResolver ImgAllyBoostIcon;
    public TimingDisplay TimingDisplay;
    public LaneFlasher LaneFlasher;
    public NoteHitFlasher NoteHitFlasher;
    public GameObject PlayerIdentifier;
    public CountdownDisplay CountdownDisplay;
    public GoalMeter GoalMeter;
    public AudioSource AudMistake;
    public HeldNoteDisplay HeldNoteDisplay;
    public LrrDisplay LrrDisplay;

    private SpriteResolver _playerIdentifierResolver;

    private bool _mistakeSfxEnabled;

    public void UpdateHud(Player player)
    {
        _playerIdentifierResolver.SetCategoryAndLabel("PlayerIdentifiers", player.GetPlayerIdSprite());
        TxtDifficulty.text = player.Difficulty.GetDisplayName();
        TxtPerfPercent.text = string.Format(CultureInfo.InvariantCulture, "{0:N1}%", player.PerfPercent * 100);
        TxtCombo.text = string.Format(CultureInfo.InvariantCulture, "{0:000}", player.Combo);
        TxtRanking.enabled = ShowRankings;
        CountdownDisplay.PlayerName = player.NameOrPlayerNumber;

        var ranking = Helpers.FormatRanking(player.Ranking);

        TxtRanking.text = ranking;

        TimingDisplay.gameObject.SetActive(player.TimingDisplayType != TimingDisplayType.Off);
        TimingDisplay.SpriteCategory = "Timing" + player.TimingDisplayType;

        UpdateGoalMeter(player);

        TxtAllyBoostCount.text = player.CanProvideAllyBoosts ? "" + player.AllyBoosts : "--";
        ImgAllyBoostIcon.SetCategoryAndLabel("AllyBoostIcons", "" + player.ProfileData.AllyBoostMode );
        ImgAllyBoostTickMeter.fillAmount = 1.0f * player.AllyBoostTicks / player.TicksForNextBoost;

        _mistakeSfxEnabled = player.MistakeSfxEnabled;
    }

    [SerializeField]
    private bool _showRankings;
    public bool ShowRankings
    {
        get { return _showRankings; }
        set
        {
            _showRankings = value;
        }
    }

    public void DisplayHitResult(HitResult result)
    {
        this.TimingDisplay.ShowHit(result);

        if (result.JudgeResult == JudgeResult.Wrong && _mistakeSfxEnabled)
        {
            this.AudMistake.Play();
        }

    }

    private void UpdateGoalMeter(Player player)
    {
        if (GoalMeter == null)
        {
            return;
        }

        GoalMeter.GoalGrade = player.GetGoalGrade();
        GoalMeter.Min = player.GoalPerfPoints.GetValueOrDefault();
        GoalMeter.Max = player.MaxPerfPoints;
        GoalMeter.PlayerCurrent = player.PerfPoints;
        GoalMeter.Value = player.MaxPerfPointsWithMistakes;
    }

    public void FlashLane(int lane)
    {
        this.LaneFlasher.LaneButtonPressed(lane);
    }

    public void ReleaseLane(int lane)
    {
        this.LaneFlasher.LaneButtonReleased(lane);
    }

    public void ReleaseAllLanes()
    {
        this.LaneFlasher.ReleaseAll();
    }

    public void FlashNoteHit(int lane)
    {
        this.NoteHitFlasher.FlashNoteHit(lane);
    }
    public void DisplayHeldNote(Note releaseNote)
    {
        HeldNoteDisplay.DisplayHeldNote(releaseNote);
    }

    public void DisplayBeat(float songTimeInBeats)
    {
        CountdownDisplay.DisplayBeat(songTimeInBeats);
    }

    void Awake()
    {
        _playerIdentifierResolver = PlayerIdentifier.GetComponent<SpriteResolver>();
    }
}
