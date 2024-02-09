using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenManager : MonoBehaviour
{
    public CoreManager CoreManager;
    public bool IgnoreReleaseInputs = false;
    public const GameScene STARTING_SCENE = GameScene.InitialLoad;
    public ActionMapType DefaultActionMapType = ActionMapType.Gameplay;

    private readonly Dictionary<string, object> _defaultSceneLoadArgs = new();

    public bool FindCoreManager()
    {
        if (CoreManager == null)
        {
            CoreManager = FindObjectOfType<CoreManager>();
        }

        if (CoreManager != null)
        {
            CoreManager.ActiveMainManager = this;
            SetActionMap(DefaultActionMapType);
            return true;
        }

        Debug.LogWarning("No CoreManager found. Transitioning to Starting Scene.");
        SceneTransition(STARTING_SCENE);
        return false;

    }

    public virtual void OnPlayerInput(InputEvent inputEvent)
    {
    }


    public virtual void OnPlayerControlsChanged(ControlsChangedArgs args)
    {

    }

    public virtual void OnPlayerJoined(Player player)
    {
    }

    public virtual void OnDeviceLost(DeviceLostArgs args)
    {
    }

    public virtual void OnNetPlayerUpdated(Player player)
    {

    }
    public virtual void OnNetPlayerScoreUpdated(Player player)
    {
    }

    public virtual void OnNetPlayerListUpdated(bool playerJoined, bool playerLeft)
    {
        if (playerLeft)
        {
            PlaySfx(SoundEvent.Net_PlayerLeft);
        }

        if (playerJoined)
        {
            PlaySfx(SoundEvent.Net_PlayerJoined);
        }
    }

    public virtual void OnNetRequestSongResponse(NetSongChoiceResponse response)
    {
        if (response.ResponseType != NetSongChoiceResponseType.Ok)
        {
            Debug.LogWarning($"(Client) Received song request response: {response.ResponseType} - {response.ResponseMessage}.");
            return;
        }

        Debug.Log($"(Client) Received song request response: {response.ResponseType}.");
    }

    public virtual void OnNetSongSelected(NetSongChoiceRequest request)
    {
    }

    public virtual void OnNetStartSongSignal()
    {
    }

    protected Player GetPlayer(int slot)
    {
        return CoreManager.PlayerManager.GetPlayer(slot);
    }

    protected void SceneTransition(GameScene gameScene, Dictionary<string, object> sceneLoadArgs = null, bool withTransition = true)
    {
        if (CoreManager != null)
        {
            CoreManager.SceneLoadArgs = sceneLoadArgs ?? _defaultSceneLoadArgs;
        }

        StartCoroutine(DoSceneTransition(gameScene, withTransition));
    }

    protected IEnumerator DoSceneTransition(GameScene gameScene, bool withTransition = true)

    {
        Debug.Log($"Transitioning to {gameScene}");
        var sceneName = gameScene + "Scene";

        if (CoreManager == null || !withTransition)
        {
            SceneManager.LoadScene(sceneName);

            yield break;    // Return statement, but for a coroutine.
        }

        yield return CoreManager.SceneTransitionManager.RunTransitionStart();
        CoreManager?.MenuMusicManager?.PlaySceneMusic(gameScene);
        SceneManager.LoadScene(sceneName);
        yield return CoreManager.SceneTransitionManager.RunTransitionEnd();

    }

    public void PlaySfx(SoundEvent soundEvent)
    {
        CoreManager.SoundEventHandler.PlaySfx(soundEvent);
    }

    public virtual void SetActionMap(ActionMapType actionMapType)
    {
        CoreManager.ControlsManager.SetActionMap(actionMapType);
    }

    public virtual void OnNetClientDisconnected(ulong id)
    {
        if (CoreManager.NetworkManager.IsHost)
        {
            CoreManager.ServerNetApi.RemoveNetPlayersServerRpc(id);
            Debug.Log("Client disconnected.");
        }
    }

    public virtual void OnNetClientConnected(ulong id)
    {

        Debug.Log($"Client connected. ID: {id}, Host: {CoreManager.IsHost}");
        CoreManager.PlayerManager.SetNetId();
        foreach (var player in CoreManager.PlayerManager.GetLocalPlayers())
        {
            var dto = PlayerDto.FromPlayer(player);
            CoreManager.ServerNetApi.RegisterNetPlayerServerRpc(dto);
        }

        if (!CoreManager.IsHost)
        {
            CoreManager.ServerNetApi.RequestPlayerListServerRpc();
            CoreManager.ServerNetApi.RequestNetGameSettingsServerRpc();
        }
    }

    public virtual void OnNetGameplayStateValuesUpdated(GameplayStateValuesDto dto)
    {
    }
    public virtual void OnNetNextTurnUpdated(ulong nextTurn)
    {
        CoreManager.NetSongSelectTurnManager.CurrentTurn = nextTurn;
    }

    public virtual void OnNetHitResult(HitResult hitResult)
    {
    }

    public virtual void OnNetShutdown()
    {
        if (!CoreManager.IsNetGame)
        {
            return;
        }

        Debug.Log("(Client) Server shutting down. Returning to main menu.");
        CoreManager.ShutdownNetPlay();
        SceneTransition(GameScene.MainMenu);
    }

    public void SendNetPlayerUpdate(Player player)
    {
        if (!CoreManager.IsNetGame)
        {
            return;
        }

        var dto = PlayerDto.FromPlayer(player);
        CoreManager.ServerNetApi.UpdatePlayerServerRpc(dto);
    }

    public void SendNetPlayerScoreUpdate(Player player)
    {
        if (!CoreManager.IsNetGame)
        {
            return;
        }

        var dto = PlayerScoreDto.FromPlayer(player);

        CoreManager.ServerNetApi.UpdatePlayerScoreServerRpc(dto);
    }

    public void UpdatePlayersState(PlayerState playerState)
    {
        foreach (var player in CoreManager.PlayerManager.GetLocalPlayers())
        {
            UpdatePlayerState(player, playerState);
        }
    }

    public void UpdatePlayerState(Player player, PlayerState playerState)
    {
        player.PlayerState = playerState;
        SendNetPlayerUpdate(player);
    }

}

