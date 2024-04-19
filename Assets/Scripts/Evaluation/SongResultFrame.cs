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

    public void DisplayResult(TeamScore teamScore, bool isNewTeamBest)
    {
        TxtSongName.text = teamScore.SongTitle;
        TxtScore.text = string.Format(CultureInfo.InvariantCulture, "{0:00000000}", teamScore.Score);
        TxtMaxTeamCombo.text = string.Format(CultureInfo.InvariantCulture, "{0:000}", teamScore.MaxTeamCombo);
        TxtMaxTeamCombo.color = ComboUtils.GetFcColor(teamScore.FullComboType);
        LblMaxTeamCombo.text = ComboUtils.GetFcCode(teamScore.FullComboType);
        LblMaxTeamCombo.color = ComboUtils.GetFcColor(teamScore.FullComboType);
        TxtMaxMultiplier.text = string.Format(CultureInfo.InvariantCulture, "{0:0.00}X", teamScore.MaxMultiplier);
        TxtIsNewTb.gameObject.SetActive(isNewTeamBest);
        ScoreCategorySprite.SetCategoryAndLabel("ScoreCategories", teamScore.Category.ToString());
        StarMeter.Value = teamScore.Stars;
    }
}
