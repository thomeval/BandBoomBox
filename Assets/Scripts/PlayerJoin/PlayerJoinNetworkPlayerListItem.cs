using System;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;

public class PlayerJoinNetworkPlayerListItem : NetworkPlayerListItem
{
    [Header("Player Join")]
    public Text TxtNoteLabels;
    public SpriteResolver ImgAllyBoostIcon;

    public override void Refresh()
    {
        base.Refresh();

        if (Player == null)
        {
            return;
        }

        var goalText = Player.GetGoalGrade() == null ? "NG" : Player.GetGoalGrade().ToString().Replace("Plus","+");
        var autoTurboText = Player.AutoTurboEnabled ? "AT" : "MT";
        var text = $"{Player.LabelSkin} | {Player.ScrollSpeed}{Environment.NewLine}{goalText} | {autoTurboText}";
        SetTextSafe(TxtNoteLabels, text);

        if (ImgAllyBoostIcon != null)
        {
            ImgAllyBoostIcon.SetCategoryAndLabel("AllyBoostIcons", "" + Player.ProfileData.AllyBoostMode);
        }
    }
}
