using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NetSongSelectTurnManager : MonoBehaviour
{
    private PlayerManager _playerManager;
    private NetGameSettings _netGameSettings;

    private readonly List<ulong> _spentTurns = new();

    [SerializeField]
    private ulong _currentTurnId;
    public ulong CurrentTurnId
    {
        get
        {
            return _currentTurnId;
        }
        set
        {
            _currentTurnId = value;
        }
    }

    public NetSongSelectTurnResponse CurrentTurn
    {
        get
        {
            return new NetSongSelectTurnResponse
            {
                SongSelectRules = _netGameSettings.SongSelectRules,
                NetId = _currentTurnId
            };
        }
    }

    public ulong NextTurn()
    {
        if (_playerManager.Players.Count == 0)
        {
            return 0;
        }

        if (_netGameSettings.SongSelectRules != NetSongSelectRules.Turns)
        {
            return 0;
        }

        foreach (var player in _playerManager.Players.OrderByDescending(e => e.NetId))
        {
            if (_spentTurns.Contains(player.NetId))
            {
                continue;
            }
            _currentTurnId = player.NetId;
            _spentTurns.Add(player.NetId);
            return CurrentTurnId;
        }

        // If we reach here, we've looped through all players. Reset the list and try again.
        _spentTurns.Clear();
        return NextTurn();
    }

    public void Reset()
    {
        _spentTurns.Clear();
        _currentTurnId = 0;
    }

    private void Start()
    {
        Helpers.AutoAssign(ref _playerManager);
        Helpers.AutoAssign(ref _netGameSettings);
    }

    public string GetTurnMessage()
    {
        switch (_netGameSettings.SongSelectRules)
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
        var currentPlayers = _playerManager.Players.Where(p => p.NetId == CurrentTurnId);

        Debug.Assert(currentPlayers.Any(), "Current song select turn is invalid: " + CurrentTurn);

        var currentPlayer = currentPlayers.First();
        if (currentPlayer.IsLocalPlayer)
        {
            return "It's your turn to choose the next song.";
        }

        var displayId = Helpers.NumberToNetIdLetter(CurrentTurnId);
        var displayName = currentPlayers.Select(p => p.Name).Aggregate((a, b) => $"{a}, {b}");

        return $"Waiting for machine {displayId} ({displayName}) to choose the next song.";
    }
}