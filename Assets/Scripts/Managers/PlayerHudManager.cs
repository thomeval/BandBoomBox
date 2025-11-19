using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;

public class PlayerHudManager : MonoBehaviour
{
    public int Slot => Player?.Slot ?? 0;
    public Player Player;

    public HighwayNameDisplay HighwayNameDisplay = HighwayNameDisplay.SongStart;

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
    public SectionResultDisplay SectionResultDisplay;

    private SpriteResolver _playerIdentifierResolver;

    private bool _mistakeSfxEnabled;

    public void UpdateHud()
    {
        _playerIdentifierResolver.SetCategoryAndLabel("PlayerIdentifiers", Player.GetPlayerIdSprite());
        TxtDifficulty.text = Player.Difficulty.GetDisplayName();
        TxtPerfPercent.text = string.Format(CultureInfo.InvariantCulture, "{0:N1}%", Player.PerfPercent * 100);
        TxtCombo.text = string.Format(CultureInfo.InvariantCulture, "{0:000}", Player.Combo);
        TxtRanking.enabled = ShowRankings;
        CountdownDisplay.PlayerName = Player.NameOrPlayerNumber;

        var ranking = Helpers.FormatRanking(Player.Ranking);

        TxtRanking.text = ranking;

        TimingDisplay.gameObject.SetActive(Player.TimingDisplayType != TimingDisplayType.Off);
        TimingDisplay.SpriteCategory = "Timing" + Player.TimingDisplayType;

        UpdateGoalMeter();

        TxtAllyBoostCount.text = Player.CanProvideAllyBoosts ? "" + Player.AllyBoosts : "--";
        ImgAllyBoostIcon.SetCategoryAndLabel("AllyBoostIcons", "" + Player.ProfileData.AllyBoostMode );
        ImgAllyBoostTickMeter.fillAmount = 1.0f * Player.AllyBoostTicks / Player.TicksForNextBoost;

        _mistakeSfxEnabled = Player.MistakeSfxEnabled;
        CountdownDisplay.HighwayNameDisplay = HighwayNameDisplay;
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

    private void UpdateGoalMeter()
    {
        if (GoalMeter == null)
        {
            return;
        }

        GoalMeter.GoalGrade = Player.GetGoalGrade();
        GoalMeter.Min = Player.GoalPerfPoints.GetValueOrDefault();
        GoalMeter.Max = Player.MaxPerfPoints;
        GoalMeter.PlayerCurrent = Player.PerfPoints;
        GoalMeter.Value = Player.MaxPerfPointsWithMistakes;
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

    public void ShowSectionResult(SectionJudgeResult sectionResult)
    { 
        SectionResultDisplay.ShowSectionResult(sectionResult);
    }

    void Awake()
    {
        _playerIdentifierResolver = PlayerIdentifier.GetComponent<SpriteResolver>();
    }
}
