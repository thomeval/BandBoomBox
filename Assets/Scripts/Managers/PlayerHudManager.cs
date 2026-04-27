using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;

public class PlayerHudManager : MonoBehaviour
{
    public int Slot => Player?.Slot ?? 0;
    public Player Player = new();

    public HighwayNameDisplay HighwayNameDisplay = HighwayNameDisplay.SongStart;

    public Text TxtCombo;
    public Text TxtRanking;
    public Text TxtPerfPercent;
    public Text TxtDifficulty;
    public TimingDisplay TimingDisplay;
    public LaneFlasher LaneFlasher;
    public NoteHitFlasher NoteHitFlasher;
    public GameObject PlayerIdentifier;
    public CountdownDisplay CountdownDisplay;
    public PaceDisplay PaceDisplay;
    public AllyBoostStatusDisplay AllyBoostStatusDisplay;
    public SoundEventProvider SoundEventProvider;
    public HeldNoteDisplay HeldNoteDisplay;
    public LrrDisplay LrrDisplay;
    public SectionResultDisplay SectionResultDisplay;

    private SpriteResolver _playerIdentifierResolver;
    private AllyBoostManager _allyBoostManager;
    private bool _mistakeSfxEnabled;

    public void UpdateHud()
    {
        if (_allyBoostManager == null)
        {
            Helpers.AutoAssign(ref _allyBoostManager);
        }

        var playerSpriteId = Player == null ? "None" : Player.GetPlayerIdSprite();
        _playerIdentifierResolver.SetCategoryAndLabel("PlayerIdentifiers", playerSpriteId);
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

        AllyBoostStatusDisplay.UpdateDisplay(_allyBoostManager.GetPlayer(Player.NetId, Player.Slot));

        _mistakeSfxEnabled = Player.MistakeSfxEnabled;
        CountdownDisplay.HighwayNameDisplay = HighwayNameDisplay;

        LaneFlasher.SetLaneOrder(Player.LaneOrderType);
        NoteHitFlasher.SetLaneOrder(Player.LaneOrderType);
        HeldNoteDisplay.SetLaneOrder(Player.LaneOrderType);
        TimingDisplay.SetLaneOrder(Player.LaneOrderType);
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
            this.SoundEventProvider.PlaySfx(SoundEvent.Mistake, this.Slot);
        }
    }

    public void ShowAllyBoost(int lane)
    {
        this.TimingDisplay.ShowAllyBoost(lane);
    }

    private void UpdateGoalMeter()
    {
        if (PaceDisplay == null)
        {
            return;
        }

        var progress = 1.0f * Player.MaxPerfPointsFromHits / Player.MaxPerfPoints;
        int? pbValue = Player.PbPerfPoints.HasValue ? (int)(Player.PbPerfPoints.Value * progress) : null;
        int? rbValue = Player.RbPerfPoints.HasValue ? (int)(Player.RbPerfPoints.Value * progress) : null;
        int? globalValue = Player.GoalPerfPoints.HasValue ? (int)(Player.GoalPerfPoints.Value * progress) : null;

        PaceDisplay.Display(Player.PerfPoints, pbValue, rbValue, globalValue, Player.GetGoalGrade());
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
        Helpers.AutoAssign(ref SoundEventProvider);
    }

}
