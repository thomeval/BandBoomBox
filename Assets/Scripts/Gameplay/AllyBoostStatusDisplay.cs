using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;

public class AllyBoostStatusDisplay : MonoBehaviour
{
    public Text TxtAllyBoostCount;
    public Image ImgAllyBoostTickMeter;
    public SpriteResolver ImgAllyBoostIcon;

    public void UpdateDisplay(AllyBoostPlayerEntry player)
    {
        if (player == null)
        {
            return;
        }

        TxtAllyBoostCount.text = player.CanProvideAllyBoosts ? "" + player.AllyBoostTokens : "--";
        ImgAllyBoostIcon.SetCategoryAndLabel("AllyBoostIcons", "" + player.AllyBoostMode);
        if (ImgAllyBoostTickMeter != null)
        {
            ImgAllyBoostTickMeter.fillAmount = 1.0f * player.AllyBoostTicks / player.TicksForNextBoost;
        }
    }
}