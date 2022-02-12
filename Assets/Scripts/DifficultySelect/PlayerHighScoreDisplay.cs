using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;

public class PlayerHighScoreDisplay : MonoBehaviour
{
    public Text TxtPercentage;

    public Text TxtMaxCombo;

    public SpriteResolver GradeSprite;

    public GameObject InnerObject;

    private PlayerScore _displayedScore;
    public PlayerScore DisplayedScore
    {
        get { return _displayedScore; }
        set
        {
            _displayedScore = value;
            Refresh();
        }
    }

    private void Refresh()
    {
        if (DisplayedScore == null)
        {
            InnerObject.SetActive(false);
            TxtPercentage.text = "NONE";
            return;
        }

        InnerObject.SetActive(true);
        TxtMaxCombo.text = "" + DisplayedScore.MaxCombo;
        TxtPercentage.text = DisplayedScore.PerfPercent.AsPercent(1);
        var grade = Helpers.PercentToGrade(DisplayedScore.PerfPercent).ToString();
        GradeSprite.SetCategoryAndLabel("Grades", grade);
    }

}
