using UnityEngine;
using UnityEngine.UI;

public class GameplayNetworkPlayerListItem : NetworkPlayerListItem
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
            if (Player.PlayerState != PlayerState.Gameplay_Playing)
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


        SetTextSafe(TxtCombo, $"{Player.Combo:000}");

        if (!Player.IsParticipating)
        {
            SetTextSafe(TxtRanking, "-");
            SetTextSafe(TxtPerfPercent, "");
        }
        else
        {
            SetTextSafe(TxtRanking, Helpers.FormatRanking(Player.Ranking));
            SetTextSafe(TxtPerfPercent, Helpers.FormatPercent(Player.PerfPercent));
        }

        if (TurboBackground != null)
        {
            TurboBackground.SetActive(Player.TurboActive);
        }

    }

}
