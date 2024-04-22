using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SongSelectNetworkPlayerList : MonoBehaviour
{
    public Text TxtPlayerCount;
    public Text TxtNotReadyCount;
    private PlayerManager _playerManager;
    private NetGameSettings _netGameSettings;

    public Color NotReadyColor = Color.red;
    public Color AllReadyColor = Color.white;

    private void Awake()
    {
        Helpers.AutoAssign(ref _playerManager);
        Helpers.AutoAssign(ref _netGameSettings);
    }

    public void Refresh()
    {
        var playerCount = _playerManager.Players.Count();
        var notReadyCount = _playerManager.Players.Count(e => e.PlayerState != PlayerState.SelectSong);
        TxtPlayerCount.text = $"{playerCount}/{_netGameSettings.MaxNetPlayers} Players";

        TxtNotReadyCount.text = (notReadyCount == 0) ? "All Players Ready" : $"{notReadyCount} Not Ready";
        TxtNotReadyCount.color = notReadyCount > 0 ? NotReadyColor : AllReadyColor;
    }
}
