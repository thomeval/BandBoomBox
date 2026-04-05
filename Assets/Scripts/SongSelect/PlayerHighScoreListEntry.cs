using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;

public class PlayerHighScoreListEntry : MonoBehaviour
{
    public Text TxtEntryNumber;
    public Text TxtPlayerName;
    public Text TxtDifficulty;
    public Text TxtPerfPoints;
    public Text TxtPercentage;

    public SpriteResolver PlayerNumberSprite;
    public SpriteResolver GradeSprite;
    public SpriteRenderer HighlightSprite;

    private DisplayedPlayerHighScore _displayedScore;

    public DisplayedPlayerHighScore DisplayedScore
    {
        get { return _displayedScore; }
        set
        {
            _displayedScore = value;
            Refresh();
        }
    }

    public int EntryNumber {get; set; }

    private void Refresh()
    {

        if (DisplayedScore == null)
        {
            return;
        }

        TxtEntryNumber.text = EntryNumber.ToString();
        TxtPlayerName.text = DisplayedScore.PlayerName;
        TxtPerfPoints.text = DisplayedScore.Score.PerfPoints.ToString();
        TxtPercentage.text = DisplayedScore.Score.PerfPercent.AsPercent(1);
        TxtDifficulty.text = DisplayedScore.Score.Difficulty.GetShortDisplayName();

        var grade = Helpers.PercentToGrade(DisplayedScore.Score.PerfPercent).ToString();
        GradeSprite.SetCategoryAndLabel("Grades", grade);
        var spriteName = DisplayedScore.PlayerSlot > 0 ? "P" + DisplayedScore.PlayerSlot : "None";
        PlayerNumberSprite.SetCategoryAndLabel("PlayerIdentifiers", spriteName);

        TxtPerfPoints.color = ComboUtils.GetFcColor(DisplayedScore.Score.FullComboType);

        var color = DisplayedScore.PlayerSlot <= 0 ? Color.clear : ColorLookups.PlayerColors[DisplayedScore.PlayerSlot - 1];
        HighlightSprite.color = color;
    }

}
