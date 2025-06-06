using UnityEngine;
using UnityEngine.UI;

public class PlayerJoinNetworkPlayerListItem : NetworkPlayerListItem
{
    [Header("Player Join")]
    public Text TxtNoteLabels;

    public override void Refresh()
    {
        base.Refresh();

        if (Player == null)
        {
            return;
        }

        SetTextSafe(TxtNoteLabels, Player.LabelSkin);
    }
}
