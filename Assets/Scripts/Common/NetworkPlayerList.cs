using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class NetworkPlayerList : MonoBehaviour
{
    private PlayerManager _playerManager;
    private NetGameSettings _netGameSettings;

    public NetworkPlayerListItem ListItemPrefab;
    public List<NetworkPlayerListItem> Children = new();
    public GameObject ListContainer;
    public Text TxtPlayerCount;
    public bool ShowRemotePlayersOnly;
    public NetworkPlayerListSortMode SortMode = NetworkPlayerListSortMode.NetId;

    public int VisibleEntries = 8;

    private void Awake()
    {
        Helpers.AutoAssign(ref _playerManager);
        Helpers.AutoAssign(ref _netGameSettings);
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

    /// <summary>
    /// Refreshes the list of players.
    /// </summary>
    public void RefreshAll()
    {
        var players = ShowRemotePlayersOnly ? _playerManager.Players.Where(e => !e.IsLocalPlayer).ToList() : _playerManager.Players;
        players = SortPlayers(players);
        AddRemoveEntries(players);

        var index = 0;
        foreach (var child in Children)
        {
            child.transform.SetSiblingIndex(index);
            child.Refresh();
            index++;
        }

        RefreshPlayerCount();
    }

    private void RefreshPlayerCount()
    {
        var playerCount = _playerManager.Players.Count();
        if (TxtPlayerCount != null)
        {
            TxtPlayerCount.text = $"{playerCount}/{_netGameSettings.MaxNetPlayers}";
        }
    }

    private void AddRemoveEntries(List<Player> players)
    {
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
            if (Children.Count >= VisibleEntries)
            {
                break;
            }

            var child = Instantiate(ListItemPrefab, transform);
            child.Player = addedPlayer;
            Children.Add(child);
            child.transform.SetParent(ListContainer.transform);
        }
    }

    private List<Player> SortPlayers(List<Player> players)
    {
        switch (SortMode)
        {
            case NetworkPlayerListSortMode.NetId:
                return players.OrderBy(e => e.NetId).ToList();
            case NetworkPlayerListSortMode.Name:
                return players.OrderBy(e => e.Name).ToList();
            case NetworkPlayerListSortMode.PerfPoints:
                return players.OrderByDescending(e => e.PerfPoints).ToList();
            case NetworkPlayerListSortMode.Ranking:
                return players.OrderBy(e => e.Ranking).ToList();
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void DisplaySectionResults(SectionResultSetDto resultSet)
    {
        foreach (var result in resultSet.SectionResults)
        {
            var item = Children.FirstOrDefault(e => e.Player.NetId == result.NetId && e.Player.Slot == result.PlayerSlot);

            if (item != null)
            {
                item.DisplaySectionResult(result);
            }
        }
    }
}
