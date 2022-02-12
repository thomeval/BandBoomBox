using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;

public class PlayerSongSelectFrame : MonoBehaviour
{
    // Start is called before the first frame update

    public Text TxtPlayerName;
    public SpriteResolver PlayerIdentifier;
    public ExpMeter ExpMeter;

    public Player Player;

    public void Refresh()
    {
        this.gameObject.SetActive(Player != null);

        if (Player == null)
        {
            return;
        }

        PlayerIdentifier.SetCategoryAndLabel("PlayerIdentifiers", Player.GetPlayerIdSprite());
        TxtPlayerName.text = Player.Name;
        ExpMeter.Exp = Player.Exp;
    }

}
