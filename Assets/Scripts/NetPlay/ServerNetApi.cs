using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class ServerNetApi : NetworkBehaviour
{
    private PlayerManager _playerManager;
    private ClientNetApi _clientNetApi;
    private NetworkManager _networkManager;
    private NetGameSettings _netGameSettings;
    private CoreManager _coreManager;

    [field: SerializeField]
    public string ServerPasswordHash { get; set; }

    public int MaxNetPlayers
    {
        get => _netGameSettings.MaxNetPlayers;
        set => _netGameSettings.MaxNetPlayers = value;
    }

    public NetSongSelectRules SongSelectRules
    {
        get => _netGameSettings.SongSelectRules;
        set => _netGameSettings.SongSelectRules = value;
    }

    void Awake()
    {
        Helpers.AutoAssign(ref _playerManager);
        Helpers.AutoAssign(ref _clientNetApi);
        Helpers.AutoAssign(ref _networkManager);
        Helpers.AutoAssign(ref _netGameSettings);
        Helpers.AutoAssign(ref _coreManager);

        DontDestroyOnLoad(this);
    }

    #region RPC methods

    [ServerRpc(RequireOwnership = false)]
    public void RegisterNetPlayerServerRpc(PlayerDto player, ServerRpcParams serverParams = default)
    {
        var realNetId = serverParams.Receive.SenderClientId;
        Debug.Assert(player.NetId == realNetId, $"Player NetId {player.NetId} does not match real NetId {realNetId}");
        Debug.Log($"(Server) Registering Player from client ID {player.NetId}, slot {player.Slot}");
        _playerManager.RegisterNetPlayer(player);

        _clientNetApi.ReceivePlayerListClientRpc(GetPlayerList());
    }

    /// <summary>
    /// Removes a single player from the current game that belongs to the client the sent the request. This is used when a client
    /// has two local players, and one of them leaves the game.
    /// </summary>
    /// <param name="slot">The local slot of the player to remove.</param>
    /// <param name="serverParams"></param>
    [ServerRpc(RequireOwnership = false)]
    public void RemoveNetPlayerServerRpc(int slot, ServerRpcParams serverParams = default)
    {
        var netId = serverParams.Receive.SenderClientId;
        _playerManager.RemoveNetPlayer(netId, slot);
        Debug.Log($"(Server) Removing Player from client ID {netId}, slot {slot}");
        _clientNetApi.ReceivePlayerListClientRpc(GetPlayerList());
    }


    /// <summary>
    /// Removes all players from the current game that belong to the client that sent the request. 
    /// </summary>
    /// <param name="serverParams"></param>
    [ServerRpc(RequireOwnership = false)]
    public void RemoveNetPlayerServerRpc(ServerRpcParams serverParams = default)
    {
        var netId = serverParams.Receive.SenderClientId;
        Debug.Log($"(Server) Removing all players from client ID {netId}");
        _clientNetApi.ReceivePlayerListClientRpc(GetPlayerList());
    }

    /// <summary>
    /// Removes all players from the current game that belong to the client with the given netId.
    /// </summary>
    /// <param name="netId"></param>
    [ServerRpc(RequireOwnership = true)]
    public void RemoveNetPlayersServerRpc(ulong netId)
    {
        Debug.Log($"(Server) Removing all Players from client ID {netId}");
        _playerManager.RemoveNetPlayer(netId);
        _clientNetApi.ReceivePlayerListClientRpc(GetPlayerList());
    }

    /// <summary>
    /// Updates the provided player (player name, score, current state, etc) on the server, and then sends the updated player to all clients.
    /// </summary>
    /// <param name="player"></param>
    /// <param name="serverParams"></param>
    [ServerRpc(RequireOwnership = false)]
    public void UpdatePlayerServerRpc(PlayerDto player, ServerRpcParams serverParams = default)
    {
        player.NetId = serverParams.Receive.SenderClientId;
        _clientNetApi.UpdatePlayerClientRpc(player);

        TryToStartPlayback(false);
    }

    /// <summary>
    /// Updates the provided player's score on the server, and then sends the updated player to all clients.
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="serverParams"></param>
    [ServerRpc(RequireOwnership = false)]
    public void UpdatePlayerScoreServerRpc(PlayerScoreDto dto, ServerRpcParams serverParams = default)
    {
        dto.NetId = serverParams.Receive.SenderClientId;
        _clientNetApi.UpdatePlayerScoreClientRpc(dto);
    }

    /// <summary>
    /// Requests the list of players currently in the game from the server. The server will respond by calling ReceivePlayerListClientRpc().
    /// </summary>
    /// <param name="serverParams"></param>
    [ServerRpc(RequireOwnership = false)]
    public void RequestPlayerListServerRpc(ServerRpcParams serverParams = default)
    {
        var netId = serverParams.Receive.SenderClientId;
        Debug.Log($"(Server) Sending Player List to client ID {netId}");
        var players = _playerManager.Players.Select(e => PlayerDto.FromPlayer(e)).ToArray();
        var param = GetSingleClientParams(netId);
        _clientNetApi.ReceivePlayerListClientRpc(players, param);
    }

    /// <summary>
    /// Requests a specific song as the next song to be played. The server will respond by calling RespondRequestSongClientRpc().
    /// If the request is approved, the server will also call SongSelectedClientRpc() to notify all clients that the song has been selected.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="serverParams"></param>
    [ServerRpc(RequireOwnership = false)]
    public void RequestSongServerRpc(NetSongChoiceRequest request, ServerRpcParams serverParams = default)
    {
        var netId = serverParams.Receive.SenderClientId;
        Debug.Log($"(Server) Song request received from client ID {netId}: {request.SongId} - {request.Title} - {request.Artist}");
        var response = GetChooseSongResponse(request, netId);

        var param = GetSingleClientParams(netId);
        _clientNetApi.RespondRequestSongClientRpc(response, param);

        if (response.ResponseType != NetSongChoiceResponseType.Ok)
        {
            return;
        }

        _clientNetApi.SongSelectedClientRpc(request);
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestNetGameSettingsServerRpc(ServerRpcParams serverParams = default)
    {
        var netId = serverParams.Receive.SenderClientId;
        Debug.Log($"(Server) Sending Game Settings to client ID {netId}");
        var param = GetSingleClientParams(netId);
        _clientNetApi.ReceiveNetGameSettingsClientRpc(_netGameSettings, param);
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestCurrentSongSelectTurnServerRpc(ServerRpcParams serverParams = default)
    {
        var netId = serverParams.Receive.SenderClientId;
        Debug.Log($"(Server) Sending current song select turn to client ID {netId}");
        var param = GetSingleClientParams(netId);
        var response = _coreManager.NetSongSelectTurnManager.CurrentTurn;
        _clientNetApi.SetCurrentSongSelectTurnClientRpc(_coreManager.NetSongSelectTurnManager.CurrentTurn, param);
    }

    [ServerRpc(RequireOwnership = true)]
    public void ForceNextSongSelectTurnServerRpc(ServerRpcParams serverParams = default)
    {
        Debug.Log("(Server) Forcing next song select turn.");
        _coreManager.NetSongSelectTurnManager.NextTurn();
        _clientNetApi.SetCurrentSongSelectTurnClientRpc(_coreManager.NetSongSelectTurnManager.CurrentTurn);
    }

    [ServerRpc(RequireOwnership = false)]
    public void ApplyHitResultServerRpc(HitResult hitResult, ServerRpcParams serverParams = default)
    {
        _coreManager.ActiveMainManager.OnNetHitResult(hitResult);
    }

    [ServerRpc(RequireOwnership = true)]
    public void ShutdownNetGameServerRpc()
    {
        Debug.Log("(Server) Shutting down game.");
        _clientNetApi.ShutdownNetGameClientRpc();
    }

    [ServerRpc(RequireOwnership = true)]
    public void CancelSelectedSongServerRpc()
    {
        Debug.Log("(Server) Cancelling selected song.");
        _clientNetApi.CancelSelectedSongClientRpc();
    }

    #endregion

    public override void OnNetworkSpawn()
    {
        // Prevent the server from syncronizing the current scene when a client joins. This is not needed since clients will load the scene themselves,
        // and must transition to PlayerJoinScene first to iniailize the player Prefab properly.
        if (IsServer)
        {
            _networkManager.SceneManager.VerifySceneBeforeLoading += (sceneIndex, scenePath, loadSceneMode) => { return false; };
        }

        base.OnNetworkSpawn();
    }

    #region Helpers

    /// <summary>
    /// Determines the response to send for the provided NetSongChoiceRequest. This will check whether all players have the provided song in their library,
    /// and that all players are ready to play (currently in the Song Select Scene).
    /// </summary>
    /// <param name="request">The NetSongChoiceRequest to be processed.</param>
    /// <param name="netId"></param>
    /// <returns></returns>
    private NetSongChoiceResponse GetChooseSongResponse(NetSongChoiceRequest request, ulong netId)
    {
        var response = new NetSongChoiceResponse
        {
            SongId = request.SongId,
            SongVersion = request.SongVersion,
            ResponseMessage = "",
            ResponseType = NetSongChoiceResponseType.Ok
        };

        var playersWithoutSong = FindPlayersWithoutSong(request);
        if (playersWithoutSong.Any())
        {
            response.ResponseType = NetSongChoiceResponseType.SongNotInLibrary;
            response.ResponseMessage = $"{playersWithoutSong.Count} player(s) don't have the selected song.";
            return response;
        }

        var turnResponse = GetChooseSongTurnResponse(netId);

        if (turnResponse != "")
        {
            response.ResponseType = NetSongChoiceResponseType.NotAllowed;
            response.ResponseMessage = turnResponse;
            return response;
        }

        var playersNotReady = _playerManager.Players.Count(e => e.PlayerState != PlayerState.SelectSong);
        if (playersNotReady > 0)
        {

            response.ResponseType = NetSongChoiceResponseType.PlayersNotReady;
            response.ResponseMessage = $"{playersNotReady} player(s) are not ready.";
            return response;
        }

        return response;
    }

    private string GetChooseSongTurnResponse(ulong netId)
    {
        switch (_netGameSettings.SongSelectRules)
        {
            case NetSongSelectRules.AnyonePicks:
                return "";
            case NetSongSelectRules.HostPicks:
                if (netId != 0)
                {
                    return "Only the host can choose the song.";
                }
                return "";
            case NetSongSelectRules.Turns:
                var currentTurn = _coreManager.NetSongSelectTurnManager.CurrentTurnId;
                if (currentTurn != netId)
                {
                    return "It's not your turn to choose a song.";
                }
                return "";
            default:
                throw new NotImplementedException();

        }
    }

    private List<string> FindPlayersWithoutSong(NetSongChoiceRequest request)
    {
        var result = new List<string>();

        if (!_coreManager.SongLibrary.Contains(request))
        {
            result.Add(Helpers.NumberToNetIdLetter(_coreManager.NetId));
        }

        // TODO: Check clients as well.
        return result;
    }

    private ClientRpcParams GetSingleClientParams(ulong netId)
    {
        return new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = new[] { netId } } };
    }

    private PlayerDto[] GetPlayerList()
    {
        return _playerManager.Players.Select(e => PlayerDto.FromPlayer(e)).ToArray();
    }

    public void ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        Debug.Log("(Server) Verifying connection for client ID " + request.ClientNetworkId);


        var json = System.Text.Encoding.UTF8.GetString(request.Payload);
        var joinParams = JsonConvert.DeserializeObject<NetGameJoinParams>(json);

        if (joinParams.ClientGameVersion != Application.version)
        {
            DeclineClient(response, $"Server is running a different version of the game. Yours: {joinParams.ClientGameVersion}, Server: {Application.version}.");
            return;
        }

        if (ServerPasswordHash != null && joinParams.PasswordHash != ServerPasswordHash)
        {
            DeclineClient(response, "Invalid password.");
            return;
        }

        if (_playerManager.Players.Count == _netGameSettings.MaxNetPlayers)
        {
            DeclineClient(response, "Server is full.");
            return;
        }

        Debug.Log("(Server) Connection approved for client ID " + request.ClientNetworkId);
        response.Approved = true;
        response.Pending = false;
        response.Reason = "";
    }

    private void DeclineClient(NetworkManager.ConnectionApprovalResponse response, string reason)
    {
        Debug.Log("Connection declined for client. " + reason);
        response.Approved = false;
        response.Pending = false;
        response.Reason = reason;
    }

    public void TryToStartPlayback(bool force)
    {
        var allReady = _playerManager.Players.All(e => e.PlayerState == PlayerState.Gameplay_ReadyToStart);

        if (allReady || force)
        {
            Debug.Log("(Server) Sending start signal to all players.");
            _clientNetApi.StartSongPlaybackClientRpc();
        }
    }

    #endregion
}