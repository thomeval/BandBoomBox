using System.Globalization;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;

public class EvaluationOnlinePlayerListItem : OnlinePlayerListItem
{
    [Header("Evaluation")]
    public Text TxtPerfPercent;
    public SpriteResolver GradeSprite;
    public Text TxtMaxCombo;
    public Text TxtDifficulty;
    public Text TxtRanking;

    public override void Refresh()
    {
        base.Refresh();

        if (Player == null)
        {
            return;
        }

        SetTextSafe(TxtPerfPercent, Helpers.FormatPercent(Player.PerfPercent));

        if (GradeSprite != null)
        {
            GradeSprite.SetCategoryAndLabel("Grades", Player.GetCurrentGrade().ToString());
        }

        SetTextSafe(TxtMaxCombo, $"{Player.MaxCombo:000}");
        SetTextSafe(TxtPlayerLevel, $"{ExpLevelUtils.GetLevel(Player.Exp)}");

        if (TxtDifficulty != null)
        {
            var diff = Helpers.GetDisplayName(Player.Difficulty);
            var result = Player.ChartGroup == "Main" ? diff : Player.ChartGroup + " - " + diff;
            TxtDifficulty.text = result;
        }

        SetTextSafe(TxtRanking, Helpers.FormatRanking(Player.Ranking));
    }
}
