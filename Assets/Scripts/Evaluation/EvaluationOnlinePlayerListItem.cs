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

        var fullComboType = Player.GetFullComboType();

        // TODO: Consider refactoring into separate class
        if (TxtMaxCombo != null)
        {
            SetTextSafe(TxtMaxCombo, $"{Player.MaxCombo:000}");
            TxtMaxCombo.color = PlayerHighScoreDisplay.GetFcColor(fullComboType);
        }

        SetTextSafe(TxtPlayerLevel, $"{ExpLevelUtils.GetLevel(Player.Exp)}");

        if (TxtDifficulty != null)
        {
            TxtDifficulty.text = Player.GroupAndDifficulty;
        }

        var ranking = Player.IsParticipating ? Helpers.FormatRanking(Player.Ranking) : "-";
        SetTextSafe(TxtRanking, ranking);
    }
}
