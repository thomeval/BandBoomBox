using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoostStatsDisplay : MonoBehaviour
{
    public Text TxtBoostsProvided;
    public Text TxtBoostsReceived;
    public Text TxtBoostsLeft;

    public void ShowBoostStats(Player player)
    {
        this.gameObject.SetActive(player.ProfileData.AllyBoostMode != AllyBoostMode.Off);
        TxtBoostsProvided.text = $"{player.AllyBoostsProvided}";
        TxtBoostsReceived.text = $"{player.AllyBoostsReceived}";
        TxtBoostsLeft.text = $"{player.AllyBoosts}";
    }
}
