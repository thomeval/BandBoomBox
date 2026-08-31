using System.Globalization;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;

public class SongResultFrame : MonoBehaviour
{
    public Text TxtScore;
    public Text LblMaxTeamCombo;
    public Text TxtMaxTeamCombo;
    public Text TxtMaxMultiplier;
    public Text TxtSongName;
    public Text TxtIsNewTb;
    public SpriteResolver ScoreCategorySprite;
    public StarMeter StarMeter;

    public Color ValidScoreColor = Color.white;
    public Color InvalidScoreColor = new Color(1.0f, 0.5f, 0.5f);

    public void DisplayResult(TeamScore teamScore, bool isNewTeamBest, bool invalid)
    {
        TxtSongName.text = teamScore.SongTitle;
        TxtScore.text = string.Format(CultureInfo.InvariantCulture, "{0:00000000}", teamScore.Score);
        TxtMaxTeamCombo.text = string.Format(CultureInfo.InvariantCulture, "{0:000}", teamScore.MaxTeamCombo);
        TxtMaxTeamCombo.color = ComboUtils.GetFcColor(teamScore.FullComboType);
        LblMaxTeamCombo.text = ComboUtils.GetFcCode(teamScore.FullComboType);
        LblMaxTeamCombo.color = ComboUtils.GetFcColor(teamScore.FullComboType);
        TxtMaxMultiplier.text = string.Format(CultureInfo.InvariantCulture, "{0:0.00}X", teamScore.MaxMultiplier);
        TxtIsNewTb.text = "";

        if (invalid)
        {
            TxtIsNewTb.text = "(Invalid)";
            TxtIsNewTb.color = InvalidScoreColor;
            TxtScore.color = InvalidScoreColor;
        }
        else if (isNewTeamBest)
        {
            TxtIsNewTb.text = "New Best!";
            TxtIsNewTb.color = ValidScoreColor;
        }
        ScoreCategorySprite.SetCategoryAndLabel("ScoreCategories", teamScore.Category.ToString());
        StarMeter.Value = teamScore.Stars;
    }
}
