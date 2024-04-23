using System;
using Unity.Netcode;
using UnityEngine;

public class ClientNetApi : NetworkBehaviour
{
    private PlayerManager _playerManager;
    private CoreManager _coreManager;
    private NetGameSettings _netGameSettings;

    public event EventHandler NetSettingsUpdated;

    void Awake()
    {
        Helpers.AutoAssign(ref _playerManager);
        Helpers.AutoAssign(ref _coreManager);
        Helpers.AutoAssign(ref _netGameSettings);
        DontDestroyOnLoad(this);
    }

    /// <summary>
    /// Called by the server whenever a player is updated (name, current state, etc).
    /// </summary>
    /// <param name="updatedPlayer">The player that has been updated.</param>
    /// <param name="param"></param>
    [ClientRpc]
    public void UpdatePlayerClientRpc(PlayerDto updatedPlayer, ClientRpcParams param = default)
    {
        if (!_playerManager.HasNetPlayer(updatedPlayer))
        {
            _playerManager.RegisterNetPlayer(updatedPlayer);
            return;
        }


        _playerManager.UpdateNetPlayer(updatedPlayer);
        _playerManager.UpdateRankings();
    }

    /// <summary>
    /// Called by the server whenever a player's individual score is updated.
    /// </summary>
    /// <param name="dto">The player's score that has been updated.</param>
    /// <param name="param"></param>
    [ClientRpc]
    public void UpdatePlayerScoreClientRpc(PlayerScoreDto dto, ClientRpcParams param = default)
    {
        if (!_playerManager.HasNetPlayer(dto))
        {
            return;
        }

        _playerManager.UpdateNetPlayer(dto);
        _playerManager.UpdateRankings();
    }


    /// <summary>
    /// Called by the server once all players have finished loading the selected song. This starts playback on the GameplayScene.
    /// </summary>
    /// <param name="param"></param>
    [ClientRpc]
    public void StartSongPlaybackClientRpc(ClientRpcParams param = default)
    {
        Debug.Log("(Client) Received Start Song Signal from server");
        _coreManager.ActiveMainManager.OnNetStartSongSignal();
    }

    /// <summary>
    /// Called by the server to provide the client with a list of all players currently in the game. 
    /// This is a response to a RequestPlayerListServerRpc() request.
    /// </summary>
    /// <param name="players">An array contains the players currently in the game (including those belonging to the client). </param>
    /// <param name="param"></param>
    [ClientRpc]
    public void ReceivePlayerListClientRpc(PlayerDto[] players, ClientRpcParams param = default)
    {
        Debug.Log($"(Client) Received Player List with {players.Length} players");

        _playerManager.UpdateNetPlayerList(players);

    }

    /// <summary>
    /// Called by the server to respond to a RequestSongServerRpc() request. Provides a response that indicates whether the request
    /// was approved, and if not, the reason why it was denied.
    /// </summary>
    /// <param name="response"></param>
    /// <param name="param"></param>
    [ClientRpc]
    public void RespondRequestSongClientRpc(NetSongChoiceResponse response, ClientRpcParams param)
    {
        _coreManager.OnNetRequestSongResponse(response);
    }

    /// <summary>
    /// Called by the server to indicate that a song has been selected for the current game. Provides the ID, version, title and artist 
    /// of the song that was selected.
    /// </summary>
    /// <param name="request"></param>
    [ClientRpc]
    public void SongSelectedClientRpc(NetSongChoiceRequest request)
    {
        _coreManager.OnNetSongSelected(request);
    }

    [ClientRpc]
    public void ReceiveNetGameSettingsClientRpc(NetGameSettings serverSettings, ClientRpcParams param)
    {
        _netGameSettings.CopyFrom(serverSettings);
        NetSettingsUpdated?.Invoke(this, null);
    }

    [ClientRpc]
    public void ReceiveNetGameplayStateValuesClientRpc(GameplayStateValuesDto dto)
    {
        _coreManager.OnNetGameplayStateValuesUpdated(dto);
    }

    [ClientRpc]
    public void ShutdownNetGameClientRpc()
    {
        _coreManager.ActiveMainManager.OnNetShutdown();
    }

    [ClientRpc]
    public void SetCurrentSongSelectTurnClientRpc(NetSongSelectTurnResponse currentTurn, ClientRpcParams param)
    {
        _coreManager.NetSongSelectTurnManager.CurrentTurnId = currentTurn.NetId;
        _coreManager.ActiveMainManager.OnNetCurrentTurnUpdated(currentTurn);
    }
}