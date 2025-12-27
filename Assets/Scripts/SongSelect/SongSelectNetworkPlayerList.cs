using System;
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

        TxtNotReadyCount.text = GetPlayerReadyText();
        TxtNotReadyCount.color = notReadyCount > 0 ? NotReadyColor : AllReadyColor;
    }

    private string GetPlayerReadyText()
    {    
        var notReadyCount = _playerManager.Players.Count(e => e.PlayerState != PlayerState.SelectSong);

        if (notReadyCount == 0)
        {
            return "All Players Ready";
        }
        if (notReadyCount == 1)
        {
            var notReadyPlayer = _playerManager.Players.Single(e => e.PlayerState != PlayerState.SelectSong);
            return $"{notReadyPlayer.Name} ({notReadyPlayer.DisplayNetId}) Not Ready";
        }

        return $"{notReadyCount} Not Ready";
    }
}
