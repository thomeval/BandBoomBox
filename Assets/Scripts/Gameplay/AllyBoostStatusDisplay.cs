using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;

public class AllyBoostStatusDisplay : MonoBehaviour
{
    public Text TxtAllyBoostCount;
    public Image ImgAllyBoostTickMeter;
    public SpriteResolver ImgAllyBoostIcon;

    public void UpdateDisplay(Player player)
    {
        TxtAllyBoostCount.text = player.CanProvideAllyBoosts ? "" + player.AllyBoosts : "--";
        ImgAllyBoostIcon.SetCategoryAndLabel("AllyBoostIcons", "" + player.ProfileData.AllyBoostMode);
        if (ImgAllyBoostTickMeter != null)
        {
            ImgAllyBoostTickMeter.fillAmount = 1.0f * player.AllyBoostTicks / player.TicksForNextBoost;
        }
    }
}