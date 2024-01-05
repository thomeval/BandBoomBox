using UnityEngine;
using UnityEngine.UI;

public class GameplayOnlinePlayerListItem : OnlinePlayerListItem
{
    [Header("Gameplay")]
    public Text TxtChartGroupDifficulty;
    public Text TxtPerfPercent;
    public Text TxtCombo;
    public Text TxtRanking;
    public GameObject TurboBackground;

    public override void Refresh()
    {
        base.Refresh();

        if (TxtChartGroupDifficulty != null)
        {
            if (Player.PlayerState != PlayerState.Gameplay)
            {
                TxtChartGroupDifficulty.text = GetStatusText(Player.PlayerState);
            }
            else if (Player.ChartGroup == "Main")
            {
                TxtChartGroupDifficulty.text = $"{Player.Difficulty}";
            }
            else
            {
                TxtChartGroupDifficulty.text = $"{Player.ChartGroup} - {Player.Difficulty}";
            }
        }

        SetTextSafe(TxtPerfPercent, Helpers.FormatPercent(Player.PerfPercent));
        SetTextSafe(TxtCombo, $"{Player.Combo:000}");
        SetTextSafe(TxtRanking, Helpers.FormatRanking(Player.Ranking));

        if (TurboBackground != null)
        {
            TurboBackground.SetActive(Player.TurboActive);
        }

    }

}
