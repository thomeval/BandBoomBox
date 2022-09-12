using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{

    public List<Player> Players;

    public Player PlayerPrefab;

    public bool AllowPlayerJoining
    {
        get { return _playerInputManager.joiningEnabled; }
        set
        {
            if (value)
            {
                _playerInputManager.EnableJoining();
            }
            else
            {
                _playerInputManager.DisableJoining();
            }
        }
    }

    private PlayerInputManager _playerInputManager;
    private CoreManager _coreManager;
    private ControlsManager _controlsManager;

    void Awake()
    {
        _playerInputManager = GetComponent<PlayerInputManager>();
        _coreManager = FindObjectOfType<CoreManager>();
        _controlsManager = FindObjectOfType<ControlsManager>();
    }

    void Start()
    {
        foreach (var player in Players)
        {
            if (!player.ControllerConnected)
            {
                Debug.LogWarning($"Controller Disconnected for Player {player.Slot}");
            }
        }
    }

    public void Reset()
    {
        foreach (var player in Players)
        {
            player.Reset();
        }
    }

    public void ApplyInputActions(string json)
    {
        foreach (var player in GetLocalPlayers())
        {
            Debug.Log(_controlsManager.InputActionAsset.GetInstanceID());
            player.ApplyInputActions(json);          
        }
    }
    
    public Player GetPlayer(int slot)
    {
        return Players.FirstOrDefault(e => e.Slot == slot);
    }
    public void ApplyHitResult(HitResult hitResult, int playerNum)
    {
        Player player = Players.FirstOrDefault(e => e.Slot == playerNum);
        if (player == null)
        {
            return;
        }

        player.ApplyHitResult(hitResult);
    }

    public void SetMaxPerfPoints(int maxPerfPoints, int playerNum)
    {
        Player player = Players.FirstOrDefault(e => e.Slot == playerNum);
        if (player == null)
        {
            return;
        }

        player.MaxPerfPoints = maxPerfPoints;
    }

    public void SortPlayers()
    {
        Players = Players.OrderBy(e => e.Slot).ToList();
    }

    public List<Player> GetLocalPlayers()
    {
        return Players.Where(e => e.Slot != 0).ToList();
    }

    public void SetPlayerCount(int players)
    {
        players = Helpers.Clamp(players, 1, 4);
        while (players < Players.Count)
        {
            var playerToRemove = Players[Players.Count - 1];
            Players.Remove(playerToRemove);
            GameObject.Destroy(playerToRemove);
        }

        var playersToAdd = players - Players.Count;
        while (playersToAdd > 0)
        {
            // This will call OnPlayerJoined
            GameObject.Instantiate(PlayerPrefab);
            playersToAdd--;
        }

    }

    void OnPlayerJoined(PlayerInput playerInput)
    {
        var gameObj = playerInput.gameObject;
        var player = gameObj.GetComponent<Player>();

        gameObj.transform.parent = this.transform;
        gameObj.name = $"Player{playerInput.playerIndex + 1}";
        player.Slot = playerInput.playerIndex + 1;
        Players.Add(player);
        Debug.Log(_coreManager.ControlsManager.InputActionAsset.GetInstanceID());
        player.ApplyInputActions(_controlsManager.InputActionAsset.ToJson());

        _coreManager.OnPlayerJoined(player);
    }

    public void AutoSetNoteSkin()
    {
        foreach (var player in GetLocalPlayers())
        {
            player.AutoSetLabelSkin();
        }
    }

    public void RemovePlayer(int slot)
    {
        if (slot < 1)
        {
            return;
        }
        var player = Players.SingleOrDefault(e => e.Slot == slot);
        if (player == null)
        {
            return;
        }

        Players.Remove(player);
        GameObject.Destroy(player.gameObject);
    }

    public bool AnyTurboActive()
    {
        return Players.Any(e => e.TurboActive);
    }

    public void DisableAllTurbos()
    {
        foreach (var player in Players)
        {
            player.TurboActive = false;
        }
    }

    /// <summary>
    /// Returns whether a given profile ID is available for use.
    /// Guest Profiles will always return true.
    /// If the specified profileId is the same as the profile the specified slot is already using, returns true.
    /// Otherwise, returns false.
    /// </summary>
    /// <param name="profileId">The ID of the profile to check for availability.</param>
    /// <param name="playerSlot">The player slot that the profile should be loaded into.</param>
    /// <returns>Whether the given profile ID is available to the specified player slot.</returns>
    public bool ProfileAvailable(string profileId, int playerSlot)
    {
        if (profileId == null)
        {
            return true;
        }

        var playerWithProfile = Players.SingleOrDefault(e => e.ProfileId == profileId);
        if (playerWithProfile == null || playerWithProfile.Slot == playerSlot)
        {
            return true;
        }

        return false;
    }

    public TeamScoreCategory GetScoreCategory()
    {
        return HighScoreManager.GetCategory(this.Players.Count);
    }

    public void SetControlsActionMap(ActionMapType actionMap)
    {
        var x = GameObject.FindObjectOfType<PlayerInput>();
        x.SwitchCurrentActionMap("aaa");
    }
}
