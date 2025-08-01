﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{

    public List<Player> Players;

    public Player PlayerPrefab;
    public Player NetPlayerPrefab;

    public const int DEFAULT_NET_ID = 255;

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

    public int MaxLocalPlayers
    {
        get
        {
            return _coreManager.IsNetGame ? 2 : 6;
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
        foreach (var player in GetLocalPlayers())
        {
            player.Reset();
        }

        UpdateRankings();
    }

    public PlayerDto[] AsDto()
    {
        return this.Players.Select(e => PlayerDto.FromPlayer(e)).ToArray();
    }

    public Player GetLocalPlayer(int slot)
    {
        return Players.FirstOrDefault(e => e.Slot == slot && e.IsLocalPlayer);
    }

    public List<Player> GetLocalPlayers()
    {
        return Players.Where(e => e.IsLocalPlayer).ToList();
    }

    public void ApplyHitResult(HitResult hitResult, int playerNum)
    {
        var player = GetLocalPlayer(playerNum);
        if (player == null)
        {
            return;
        }

        player.ApplyHitResult(hitResult);
    }

    public void SetMaxPerfPoints(int maxPerfPoints, int playerNum)
    {
        var player = GetLocalPlayer(playerNum);
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

    public void SetPlayerCount(int players)
    {
        players = Helpers.Clamp(players, 1, 4);
        while (players < GetLocalPlayers().Count)
        {
            var playerToRemove = Players[Players.Count - 1];
            Players.Remove(playerToRemove);
            GameObject.Destroy(playerToRemove.gameObject);
        }

        var playersToAdd = players - GetLocalPlayers().Count;
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
        _controlsManager.ApplyCustomBindings();

        Debug.Log($"Player {player.Slot} joined with device {playerInput.devices[0]}");
        _coreManager.OnPlayerJoined(player);
    }

    public void AutoSetNoteSkin()
    {
        var useControllerNoteLabels = _coreManager.Settings.AutoSetNoteLabelsFromController;
        foreach (var player in GetLocalPlayers())
        {
            player.AutoSetLabelSkin(useControllerNoteLabels);
        }
    }

    public void RemovePlayer(int slot)
    {
        if (slot < 1)
        {
            return;
        }
        var player = Players.SingleOrDefault(e => e.Slot == slot && e.IsLocalPlayer);
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
        return HighScoreManager.GetScoreCategory(this.Players.Count);
    }

    public void UpdateRankings()
    {
        var orderedPlayers = Players.Where(e => e.IsParticipating).OrderByDescending(e => e.PerfPercent).ToList();
        var count = 0;
        var last = 1000.0f;
        for (var x = 0; x < orderedPlayers.Count; x++)
        {
            var perc = orderedPlayers[x].PerfPercent;
            if (perc < last)
            {
                last = perc;
                count++;
            }
            orderedPlayers[x].Ranking = count;
        }

        foreach (var player in Players.Where(e => !e.IsParticipating))
        {
            player.Ranking = 99;
        }
    }

    public void SetNetId()
    {
        foreach (var player in Players)
        {
            player.NetId = _coreManager.NetId;
        }
    }

    public void ResetNetId()
    {
        foreach (var player in Players)
        {
            player.NetId = DEFAULT_NET_ID;
        }
    }

    public void RegisterNetPlayer(PlayerDto serverPlayer)
    {

        var myPlayer = Players.FirstOrDefault(e => e.NetId == serverPlayer.NetId && e.Slot == serverPlayer.Slot);
        if (myPlayer == null)
        {
            myPlayer = GameObject.Instantiate(NetPlayerPrefab);
            CopyValues(serverPlayer, myPlayer);
            var gameObj = myPlayer.gameObject;
            gameObj.transform.parent = this.transform;
            gameObj.name = $"Network Player {serverPlayer.NetId}-{serverPlayer.Slot}";
            Players.Add(myPlayer);
            _coreManager.OnNetPlayerListUpdated(true, false);

        }

        UpdateNetPlayer(serverPlayer);
    }

    public void RemoveNetPlayer(ulong netId)
    {
        if (netId == _coreManager.NetId)
        {
            return;
        }

        var players = Players.Where(e => e.NetId == netId).ToList();

        foreach (var player in players)
        {
            Players.Remove(player);
            GameObject.Destroy(player.gameObject);
        }

        _coreManager.OnNetPlayerListUpdated(false, true);
    }

    public void RemoveNetPlayer(ulong netId, int slot)
    {
        if (netId == _coreManager.NetId)
        {
            // Don't remove local players through ClientRpc. This is handled locally instead.
            return;
        }

        var player = Players.FirstOrDefault(e => e.NetId == netId && e.Slot == slot);
        if (player == null)
        {
            return;
        }

        Players.Remove(player);
        GameObject.Destroy(player.gameObject);

        _coreManager.OnNetPlayerListUpdated(false, true);
    }

    public void UpdateNetPlayer(PlayerDto player)
    {
        var myPlayer = Players.FirstOrDefault(e => e.NetId == player.NetId && e.Slot == player.Slot);
        if (myPlayer == null)
        {
            return;
        }

        var previousTurboState = myPlayer.TurboActive;
        CopyValues(player, myPlayer);

        _coreManager.OnNetPlayerUpdated(myPlayer);
        CheckTurboStarted(myPlayer, previousTurboState);
    }

    private void CheckTurboStarted(Player myPlayer, bool previousTurboState)
    {
        if (myPlayer.TurboActive && !previousTurboState)
        {
            _coreManager.OnNetPlayerTurboStarted(myPlayer);
        }
    }

    public void UpdateNetPlayer(PlayerScoreDto player)
    {
        var myPlayer = Players.FirstOrDefault(e => e.NetId == player.NetId && e.Slot == player.Slot);
        if (myPlayer == null)
        {
            return;
        }

        var previousTurboState = myPlayer.TurboActive;
        CopyValues(player, myPlayer);
        _coreManager.OnNetPlayerUpdated(myPlayer);
        CheckTurboStarted(myPlayer, previousTurboState);
    }

    public void ClearNetPlayers()
    {
        foreach (var player in Players.Where(e => !e.IsLocalPlayer).ToList())
        {

            Players.Remove(player);
            GameObject.Destroy(player.gameObject);
        }

        _coreManager.OnNetPlayerListUpdated(false, false);
    }

    public bool HasNetPlayer(PlayerDto updatedPlayer)
    {
        return Players.Any(e => e.NetId == updatedPlayer.NetId && e.Slot == updatedPlayer.Slot);
    }

    public bool HasNetPlayer(PlayerScoreDto updatedPlayer)
    {
        return Players.Any(e => e.NetId == updatedPlayer.NetId && e.Slot == updatedPlayer.Slot);
    }

    public void CopyValues(PlayerDto fromPlayer, Player toPlayer)
    {
        toPlayer.NetId = fromPlayer.NetId;
        toPlayer.Slot = fromPlayer.Slot;
        toPlayer.NoteSkin = fromPlayer.NoteSkin;
        toPlayer.LabelSkin = fromPlayer.LabelSkin;
        toPlayer.PerfPoints = fromPlayer.PerfPoints;
        toPlayer.MaxPerfPoints = fromPlayer.MaxPerfPoints;
        toPlayer.Combo = fromPlayer.Combo;
        toPlayer.MaxCombo = fromPlayer.MaxCombo;
        toPlayer.Goal = fromPlayer.Goal == 0 ? null : fromPlayer.Goal;

        if (fromPlayer.PlayerState != toPlayer.PlayerState)
        {
            Debug.Log($"Player {toPlayer.DisplayNetId}-{toPlayer.Slot} changed state from {toPlayer.PlayerState} to {fromPlayer.PlayerState}");
        }
        toPlayer.PlayerState = fromPlayer.PlayerState;

        toPlayer.Difficulty = fromPlayer.Difficulty;
        toPlayer.Exp = fromPlayer.Exp;
        toPlayer.Momentum = fromPlayer.Momentum;
        toPlayer.Name = fromPlayer.Name;
        toPlayer.ChartGroup = fromPlayer.ChartGroup;
        toPlayer.ScrollSpeed = fromPlayer.ScrollSpeed;
        toPlayer.TurboActive = fromPlayer.TurboActive;
        toPlayer.IsParticipating = fromPlayer.IsParticipating;
        toPlayer.NetFullComboType = fromPlayer.NetFullComboType;
    }

    public void CopyValues(PlayerScoreDto fromPlayer, Player toPlayer)
    {
        toPlayer.PerfPoints = fromPlayer.PerfPoints;
        toPlayer.MaxPerfPoints = fromPlayer.MaxPerfPoints;
        toPlayer.Combo = fromPlayer.Combo;
        toPlayer.MaxCombo = fromPlayer.MaxCombo;
        toPlayer.PlayerState = fromPlayer.PlayerState;
        toPlayer.TurboActive = fromPlayer.TurboActive;
        toPlayer.NetFullComboType = fromPlayer.FullComboType;
    }

    public void SetPlayerState(PlayerState state)
    {
        foreach (var player in GetLocalPlayers())
        {
            player.PlayerState = state;
        }
    }

    public void UpdateNetPlayerList(PlayerDto[] newPlayerList)
    {
        foreach (var player in newPlayerList)
        {
            if (!HasNetPlayer(player))
            {
                RegisterNetPlayer(player);
            }
        }

        foreach (var player in this.Players)
        {
            if (!newPlayerList.Any(e => e.NetId == player.NetId && e.Slot == player.Slot))
            {
                RemoveNetPlayer(player.NetId, player.Slot);
            }
        }
    }

    public void ClearParticipation()
    {
        foreach (var player in Players)
        {
            player.IsParticipating = false;
        }
    }

    public void AddParticipation(PlayerState playerState)
    {
        foreach (var player in Players.Where(e => e.PlayerState == playerState))
        {
            player.IsParticipating = true;
        }
    }

    public void UpdateAllowPlayerJoining()
    {
        AllowPlayerJoining = GetLocalPlayers().Count < MaxLocalPlayers;

        if (!_coreManager.IsNetGame)
        {
            return;
        }

        AllowPlayerJoining &= Players.Count < _coreManager.ServerNetApi.MaxNetPlayers;
    }

    public Player FindAllyBoostForPlayer(Player player)
    {
        if (!player.CanReceiveAllyBoosts)
        {
            return null;
        }
        var ally = Players.Where(e => e != player && e.CanProvideAllyBoosts && e.AllyBoosts > 0).OrderBy(e => e.AllyBoosts).FirstOrDefault();

        return ally;
    }

    public void ApplyAllyBoost(Player providingPlayer, Player receivingPlayer)
    {
        if (providingPlayer == null || receivingPlayer == null || providingPlayer == receivingPlayer)
        {
            return;
        }

        providingPlayer.AllyBoosts--;
        providingPlayer.AllyBoostsProvided++;
        receivingPlayer.AllyBoostsReceived++;
    }
}
