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

    public TimingDisplay TimingDisplay;

    public LaneFlasher LaneFlasher;

    public NoteHitFlasher NoteHitFlasher;

    public GameObject PlayerIdentifier;

    public CountdownDisplay CountdownDisplay;

    public GoalMeter GoalMeter;

    public AudioSource AudMistake;

    public HeldNoteDisplay HeldNoteDisplay;


    private SpriteResolver _playerIdentifierResolver;


    public void UpdateHud(Player player)
    {
        _playerIdentifierResolver.SetCategoryAndLabel("PlayerIdentifiers",player.GetPlayerIdSprite());
        TxtDifficulty.text = player.Difficulty.ToString();
        TxtPerfPercent.text = string.Format(CultureInfo.InvariantCulture, "{0:P1}", player.PerfPercent).Replace(" ","");
        TxtCombo.text = string.Format(CultureInfo.InvariantCulture, "{0:000}", player.Combo);
        TxtRanking.enabled = ShowRankings;

        string suffix;

        switch (player.Ranking)
        {
            case 1:
                suffix = "st";
                break;
            case 2:
                suffix = "nd";
                break;
            case 3:
                suffix = "rd";
                break;
            default:
                suffix = "th";
                break;
        }

        TxtRanking.text = string.Format("{0}{1}", player.Ranking, suffix);

        TimingDisplay.gameObject.SetActive(player.TimingDisplayType != TimingDisplayType.Off);
        TimingDisplay.SpriteCategory = "Timing" + player.TimingDisplayType;

        UpdateGoalMeter(player);
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

        if (result.JudgeResult == JudgeResult.Wrong)
        {
            this.AudMistake.Play();
        }

    }

    private void UpdateGoalMeter(Player player)
    {
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
