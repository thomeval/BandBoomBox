using System;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;

public class PlayerHighScoreDisplay : MonoBehaviour
{
    public Text TxtPercentage;

    public Text TxtMaxCombo;
    public Text LblMaxCombo;
    public Text TxtDifficulty;

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
            TxtDifficulty.text = "None";
            return;
        }

        InnerObject.SetActive(true);
        TxtMaxCombo.text = "" + DisplayedScore.MaxCombo;
        TxtPercentage.text = DisplayedScore.PerfPercent.AsPercent(1);
        TxtDifficulty.text = DisplayedScore.Difficulty.ToString();

        if (LblMaxCombo != null)
        {
            LblMaxCombo.text = GetFcCode(DisplayedScore.FullComboType);
            LblMaxCombo.color = GetFcColor(DisplayedScore.FullComboType);
        }
        TxtMaxCombo.color = GetFcColor(DisplayedScore.FullComboType);
        var grade = Helpers.PercentToGrade(DisplayedScore.PerfPercent).ToString();
        GradeSprite.SetCategoryAndLabel("Grades", grade);
    }


    private Color GetFcColor(FullComboType fullComboType)
    {
        switch (fullComboType)
        {
            case FullComboType.None:
                return Color.white;
            case FullComboType.FullCombo:
                return Color.green;
            case FullComboType.PerfectFullCombo:
                return Color.cyan;
            case FullComboType.SemiFullCombo:
                return new Color(1.0f, 0.5f, 0.5f);
            default:
                throw new ArgumentOutOfRangeException("fullComboType", fullComboType, null);
        }
    }

    private string GetFcCode(FullComboType fullComboType)
    {
        switch (fullComboType)
        {
            case FullComboType.None:
                return "MAX";
            case FullComboType.FullCombo:
                return "FC";
            case FullComboType.PerfectFullCombo:
                return "PFC";
            case FullComboType.SemiFullCombo:
                return "SFC";
            default:
                throw new ArgumentOutOfRangeException("fullComboType", fullComboType, null);
        }
    }

    public void Clear()
    {
        this.DisplayedScore = null;
    }
}
