using Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Audio;

public class CoreManager : MonoBehaviour
{

    public ScreenManager ActiveMainManager;

    public PlayerManager PlayerManager;
    public ProfileManager ProfileManager;
    public HighScoreManager HighScoreManager;
    public MenuMusicManager MenuMusicManager;
    public SongPreviewManager SongPreviewManager;
    public SongManager SongManager;
    public SceneTransitionManager SceneTransitionManager;
    public SoundEventHandler SoundEventHandler;
    public SongLibrary SongLibrary;
    public NetworkManager NetworkManager;
    public ClientNetApi ClientNetApi;
    public ServerNetApi ServerNetApi;
    public NetSongSelectTurnManager NetSongSelectTurnManager;
    public GameplayStateRecorder GameplayStateRecorder;

    public string SelectedSong;

    public SongData CurrentSongData
    {
        get { return SelectedSong == null ? null : SongLibrary[SelectedSong]; }
    }

    public TeamScore LastTeamScore;

    public AudioMixer AudioMixer;

    [HideInInspector]
    public SettingsManager Settings;
    public SettingsHelper SettingsHelper;

    public ControlsManager ControlsManager;
    public InputManager InputManager;

    public bool TitleScreenShown;
    public bool IsNetGame;
    public bool IsHost = true;

    public ulong NetId
    {
        get
        {
            if (!NetworkManager.IsConnectedClient && !(NetworkManager.IsListening))
            {
                return 255;
            }
            return NetworkManager.LocalClientId;

        }
    }

    public bool TransitionInProgress
    {
        get
        {
            return SceneTransitionManager.TransitionInProgress;
        }
    }

    public Dictionary<string, object> SceneLoadArgs = new();

    void Awake()
    {
        // One instance of CoreManager is created every time the first scene loads. If a CoreManager already exists, discard this one.
        var objs = FindObjectsOfType<CoreManager>();

        if (objs.Length > 1)
        {
            GameObject.Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(this);
        PlayerManager = FindObjectOfType<PlayerManager>();
        SongLibrary = FindObjectOfType<SongLibrary>();
        Settings = GetComponent<SettingsManager>();

        NetworkManager.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        NetworkManager.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong id)
    {
        ActiveMainManager.OnNetClientDisconnected(id);
    }

    private void NetworkManager_OnClientConnectedCallback(ulong id)
    {
        ActiveMainManager.OnNetClientConnected(id);
    }

    void Start()
    {
        PlayerManager.SetPlayerCount(1);
    }

    public void LoadSettings()
    {
        Settings.Load();
        ControlsManager.LoadInputActions();
        SettingsHelper.ApplySettings();
    }


    void OnPlayerInput(InputEvent inputEvent)
    {
        if (ActiveMainManager.IgnoreReleaseInputs && !inputEvent.IsPressed)
        {
            return;
        }
        ActiveMainManager.OnPlayerInput(inputEvent);
    }

    void OnDeviceLost(DeviceLostArgs args)
    {
        ActiveMainManager.OnDeviceLost(args);
    }

    void OnPlayerControlsChanged(ControlsChangedArgs args)
    {
        Debug.Log($"Applying controls for player {args.Player}. Preferred Labels: {args.ControllerType}, Device: {args.Device ?? "[None]"}");
        ActiveMainManager.OnPlayerControlsChanged(args);
    }

    public void OnPlayerJoined(Player player)
    {
        Debug.Log($"Player {player.Slot} joined.");
        ActiveMainManager.OnPlayerJoined(player);
    }

    #region Netplay Events

    public void OnNetPlayerUpdated(Player player)
    {
        if (!IsNetGame)
        {
            return;
        }
        ActiveMainManager.OnNetPlayerUpdated(player);
    }

    public void OnNetPlayerScoreUpdated(Player player)
    {
        if (!IsNetGame)
        {
            return;
        }
        ActiveMainManager.OnNetPlayerScoreUpdated(player);
    }

    public void OnNetPlayerListUpdated(bool playerJoined, bool playerLeft)
    {
        if (!IsNetGame)
        {
            return;
        }
        ActiveMainManager.OnNetPlayerListUpdated(playerJoined, playerLeft);
    }


    public void ShutdownNetPlay()
    {
        PlayerManager.ClearNetPlayers();

        if (NetworkManager.IsListening || NetworkManager.IsClient)
        {
            NetworkManager.Shutdown();
        }

        PlayerManager.SetPlayerCount(1);
        PlayerManager.AllowPlayerJoining = false;
    }

    public void OnNetRequestSongResponse(NetSongChoiceResponse response)
    {
        if (!IsNetGame)
        {
            return;
        }

        ActiveMainManager.OnNetRequestSongResponse(response);
    }

    public void OnNetSongSelected(NetSongChoiceRequest request)
    {
        if (!IsNetGame)
        {
            return;
        }

        ActiveMainManager.OnNetSongSelected(request);
    }

    public void OnNetGameplayStateValuesUpdated(GameplayStateValuesDto dto)
    {
        if (!IsNetGame)
        {
            return;
        }

        ActiveMainManager.OnNetGameplayStateValuesUpdated(dto);
    }

    #endregion

    public void SaveAllActiveProfiles()
    {
        foreach (var player in PlayerManager.GetLocalPlayers().Where(e => !string.IsNullOrEmpty(e.ProfileId)))
        {

            var data = player.ProfileData;
            var scores = ProfileManager[player.ProfileId]?.PlayerScores;
            data.PlayerScores = scores ?? new List<PlayerScore>();
            data.LastPlayed = DateTime.Now;
            ProfileManager.Save(data);
        }
    }

    private void OnApplicationQuit()
    {
        if (IsNetGame)
        {
            if (IsHost)
            {
                ServerNetApi.ShutdownNetGameServerRpc();
            }
        }

    }
}
