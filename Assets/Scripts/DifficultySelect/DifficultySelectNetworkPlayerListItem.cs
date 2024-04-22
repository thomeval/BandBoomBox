using UnityEngine;
using UnityEngine.UI;

public class DifficultySelectNetworkPlayerListItem : NetworkPlayerListItem
{
    [Header("Difficulty Select")]
    public Text TxtChartGroup;
    public Text TxtDifficulty;

    public override void Refresh()
    {
        base.Refresh();

        if (Player == null)
        {
            return;
        }

        if (Player.PlayerState != PlayerState.DifficultySelect_Ready)
        {
            SetTextSafe(TxtChartGroup, "");
            SetTextSafe(TxtDifficulty, "");
            return;
        }

        SetTextSafe(TxtChartGroup, Player.ChartGroup);
        SetTextSafe(TxtDifficulty, Helpers.GetDisplayName(Player.Difficulty));
    }
}
