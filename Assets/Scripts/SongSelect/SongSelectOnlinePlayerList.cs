using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SongSelectOnlinePlayerList : MonoBehaviour
{
    public Text TxtPlayerCount;
    public Text TxtNotReadyCount;
    private PlayerManager _playerManager;

    public Color NotReadyColor = Color.red;
    public Color AllReadyColor = Color.white;
    private void Awake()
    {
        Helpers.AutoAssign(ref _playerManager);
    }

    public void Refresh()
    {
        var playerCount = _playerManager.Players.Count();
        var notReadyCount = _playerManager.Players.Count(e => e.PlayerState != PlayerState.SelectSong);
        var suffix = playerCount == 1 ? "" : "s";
        TxtPlayerCount.text = $"{playerCount}/{_playerManager.MaxNetPlayers} Player{suffix}";

        TxtNotReadyCount.text = (notReadyCount == 0) ? "All Players Ready" : $"{notReadyCount} Not Ready";
        TxtNotReadyCount.color = notReadyCount > 0 ? NotReadyColor : AllReadyColor;
    }
}
