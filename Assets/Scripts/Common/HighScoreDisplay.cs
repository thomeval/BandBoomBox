using System.Globalization;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;

public class HighScoreDisplay : MonoBehaviour
{
    public Text LblMaxCombo;
    public Text TxtMaxCombo;
    public Text TxtMaxMultiplier;
    public Text TxtScore;
    public SpriteResolver ScoreCategorySprite;
    public StarMeter StarMeter;
    public void Display(TeamScore teamScore, int numPlayers)
    {
        var defaultCategory = HighScoreManager.GetScoreCategory(numPlayers);
        if (teamScore == null)
        {
            TxtScore.text = "NONE";
            TxtMaxMultiplier.text = "0.00X";
            TxtMaxCombo.text = "000";
            StarMeter.Value = 0.0;
            LblMaxCombo.color = Color.white;
            TxtMaxCombo.color = Color.white;
            SetCategorySprite(defaultCategory);
            return;
        }

        TxtScore.text = $"{teamScore.Score:00000000}";
        TxtMaxMultiplier.text = string.Format(CultureInfo.InvariantCulture, "{0:F2}X", teamScore.MaxMultiplier);
        LblMaxCombo.text = ComboUtils.GetFcCode(teamScore.FullComboType);
        LblMaxCombo.color = ComboUtils.GetFcColor(teamScore.FullComboType);
        TxtMaxCombo.text = $"{teamScore.MaxTeamCombo:000}";
        TxtMaxCombo.color = ComboUtils.GetFcColor(teamScore.FullComboType);
        StarMeter.Value = teamScore.Stars;
        SetCategorySprite(teamScore.Category);
    }

    private void SetCategorySprite(TeamScoreCategory category)
    {
        ScoreCategorySprite.SetCategoryAndLabel("ScoreCategories", category.ToString());
    }
}
