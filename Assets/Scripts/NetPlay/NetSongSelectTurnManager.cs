using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NetSongSelectTurnManager : MonoBehaviour
{
    private PlayerManager _playerManager;
    private ServerNetApi _serverNetApi;

    private readonly List<ulong> _spentTurns = new();

    [SerializeField]
    private ulong _currentTurn;
    public ulong CurrentTurn
    {
        get
        {
            return _currentTurn;
        }
        set
        {
            _currentTurn = value;
        }
    }

    public ulong NextTurn()
    {
        if (_playerManager.Players.Count == 0)
        {
            return 0;
        }

        if (_serverNetApi.SongSelectRules != NetSongSelectRules.Turns)
        {
            return 0;
        }

        foreach (var player in _playerManager.Players)
        {
            if (_spentTurns.Contains(player.NetId))
            {
                continue;
            }
            _currentTurn = player.NetId;
            _spentTurns.Add(player.NetId);
            return CurrentTurn;
        }

        // If we reach here, we've looped through all players. Reset the list and try again.
        _spentTurns.Clear();
        return NextTurn();
    }

    public void Reset()
    {
        _spentTurns.Clear();
        _currentTurn = 0;
    }

    private void Start()
    {
        Helpers.AutoAssign(ref _playerManager);
        Helpers.AutoAssign(ref _serverNetApi);
    }

    public string GetTurnMessage()
    {
        switch (_serverNetApi.SongSelectRules)
        {
            case NetSongSelectRules.AnyonePicks:
                return "Anyone can choose the next song.";
            case NetSongSelectRules.HostPicks:
                return "Only the host can choose the next song.";
            case NetSongSelectRules.Turns:
                return GetTurnMessageForCurrentPlayer();
            default:
                return "Unknown song select rules.";
        }
    }

    private string GetTurnMessageForCurrentPlayer()
    {
        var currentPlayers = _playerManager.Players.Where(p => p.NetId == CurrentTurn);

        Debug.Assert(currentPlayers.Any(), "Current song select turn is invalid: " + CurrentTurn);

        var currentPlayer = currentPlayers.First();
        if (currentPlayer.IsLocalPlayer)
        {
            return "It's your turn to choose the next song.";
        }

        var displayId = Helpers.NumberToNetIdLetter(CurrentTurn);

        return $"Waiting for machine {displayId} to choose the next song.";
    }
}