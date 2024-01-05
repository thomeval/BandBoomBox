using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class OnlinePlayerList : MonoBehaviour
{
    private PlayerManager _playerManager;
    public OnlinePlayerListItem ListItemPrefab;
    public List<OnlinePlayerListItem> Children = new();
    public GameObject ListContainer;
    public Text TxtPlayerCount;
    public bool ShowRemotePlayersOnly;
    public OnlinePlayerListSortMode SortMode = OnlinePlayerListSortMode.NetId;

    private void Awake()
    {
        Helpers.AutoAssign(ref _playerManager);
        ListContainer.ClearChildren();
    }

    public void Refresh(Player player)
    {
        var child = Children.FirstOrDefault(e => e.Player == player);

        if (child == null)
        {
            return;
        }

        child.Refresh();
    }

    public void RefreshAll()
    {
        var playerCount = _playerManager.Players.Count();
        var players = ShowRemotePlayersOnly ? _playerManager.Players.Where(e => !e.IsLocalPlayer).ToList() : _playerManager.Players;
        players = SortPlayers(players);
        var lastSeenPlayers = Children.Select(e => e.Player).ToList();
        var playersAdded = players.Except(lastSeenPlayers).ToList();
        var playersRemoved = lastSeenPlayers.Except(players).ToList();

        foreach (var removedPlayer in playersRemoved)
        {
            var child = Children.FirstOrDefault(e => e.Player == removedPlayer);
            if (child != null)
            {
                Children.Remove(child);
                Destroy(child.gameObject);
            }
        }

        foreach (var addedPlayer in playersAdded)
        {
            var child = Instantiate(ListItemPrefab, transform);
            child.Player = addedPlayer;
            Children.Add(child);
            child.transform.SetParent(ListContainer.transform);
        }

        foreach (var child in Children)
        {
            child.Refresh();
        }


        if (TxtPlayerCount != null)
        {
            TxtPlayerCount.text = $"{playerCount}/{_playerManager.MaxNetPlayers}";
        }
    }

    private List<Player> SortPlayers(List<Player> players)
    {
        switch (SortMode)
        {
            case OnlinePlayerListSortMode.NetId:
                return players.OrderBy(e => e.NetId).ToList();
            case OnlinePlayerListSortMode.Name:
                return players.OrderBy(e => e.Name).ToList();
            case OnlinePlayerListSortMode.PerfPoints:
                return players.OrderByDescending(e => e.PerfPoints).ToList();
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
