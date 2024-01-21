using System;
using System.Collections.Generic;
using System.Linq;
using Assets;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Audio;
using Unity.Netcode;

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

    public string SelectedSong;

    public SongData CurrentSongData
    {
        get { return SelectedSong == null ? null : SongLibrary[SelectedSong]; }
    }

    public TeamScore LastTeamScore;

    public AudioMixer AudioMixer;

    [HideInInspector]
    public SettingsManager Settings;
    public ControlsManager ControlsManager;
    public InputManager InputManager;

    public bool TitleScreenShown;
    public bool IsNetGame;
    public bool IsHost = true;
    public int MaxNetPlayers = 8;

    public int MaxLocalPlayers
    {
        get
        {
            return IsNetGame ? 2 : 4;
        }
    }

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



    public Dictionary<string, object> SceneLoadArgs = new();

    private const double FLOAT_TOLERANCE = 0.000001;

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
        ApplySettings();
    }

    public void ApplySettings()
    {
        SongLibrary.SongFolders = Settings.SongFolders;

        ApplyAudioSettings();
        ApplyGraphicsSettings();
        //  PlayerManager.ApplyInputActions();
    }

    public void ApplyAudioSettings()
    {
        ApplyAudioVolume("MasterVolume", Settings.MasterVolume);
        ApplyAudioVolume("GameplaySfxVolume", Settings.GameplaySfxVolume);
        ApplyAudioVolume("GameplayMusicVolume", Settings.GameplayMusicVolume);
        ApplyAudioVolume("MistakeSfxVolume", Settings.MistakeVolume);
        ApplyAudioVolume("MenuSfxVolume", Settings.MenuSfxVolume);
        ApplyAudioVolume("MenuMusicVolume", Settings.MenuMusicVolume);
    }

    private void ApplyAudioVolume(string mixerParam, float volume)
    {
        volume = Mathf.Max(volume, 0.00001f);
        var temp = Mathf.Log10(volume) * 20;
        AudioMixer.SetFloat(mixerParam, temp);
    }

    private void ApplyGraphicsSettings()
    {
        var resolution = Settings.ScreenResolution;
        const int DEFAULT_HEIGHT = 720;
        const int DEFAULT_WIDTH = 1280;

        if (string.IsNullOrWhiteSpace(resolution))
        {
            resolution = Screen.currentResolution.width + "x" + Screen.currentResolution.height;
        }

        var resParts = resolution.Trim().Split('x');
        var width = Convert.ToInt32(resParts[0]);
        var height = Convert.ToInt32(resParts[1]);

        var aspectRatio = 1.0 * width / height;
        var expectedAspectRatio = 1.0 * 16 / 9;

        if (Math.Abs(aspectRatio - expectedAspectRatio) > FLOAT_TOLERANCE)
        {
            Debug.LogWarning($"Unsupported aspect ratio detected: {width}x{height}. Using default resolution of {DEFAULT_WIDTH}x{DEFAULT_HEIGHT} instead. ");
            width = DEFAULT_WIDTH;
            height = DEFAULT_HEIGHT;
        }

        Screen.SetResolution(width, height, Settings.FullScreenMode);
        Application.targetFrameRate = Settings.TargetFrameRate;
        QualitySettings.vSyncCount = Settings.VSyncEnabled ? 1 : 0;
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

    public void OnNetPlayerListUpdated()
    {
        if (!IsNetGame)
        {
            return;
        }
        ActiveMainManager.OnNetPlayerListUpdated();
    }

    public void SaveAllActiveProfiles()
    {
        foreach (var player in PlayerManager.GetLocalPlayers().Where(e => !string.IsNullOrEmpty(e.ProfileId)))
        {

            var data = player.ProfileData;
            var scores = ProfileManager[player.ProfileId]?.PlayerScores;
            data.PlayerScores = scores ?? new List<PlayerScore>();
            ProfileManager.Save(data);
        }
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
