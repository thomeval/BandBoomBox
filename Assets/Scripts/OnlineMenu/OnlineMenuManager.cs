using System.Linq;
using System.Net;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class OnlineMenuManager : ScreenManager
{
    public OnlineMainSubmenu MainMenu;
    public OnlineHostSubmenu HostMenu;
    public OnlineJoinSubmenu JoinMenu;

    public OnlineSubmenuBase CurrentSubmenu
    {
        get
        {
            switch (OnlineMenuState)
            {
                case OnlineMenuState.MainMenu:
                    return MainMenu;
                case OnlineMenuState.HostMenu:
                    return HostMenu;
                case OnlineMenuState.JoinMenu:
                    return JoinMenu;
                default:
                    return MainMenu;
            }
        }
    }

    public const ushort DEFAULT_PORT = 3334;

    [SerializeField]
    private OnlineMenuState _onlineMenuState = OnlineMenuState.MainMenu;

    public OnlineMenuState OnlineMenuState
    {
        get
        {
            return _onlineMenuState;
        }
        set
        {
            _onlineMenuState = value;
            MainMenu.gameObject.SetActive(_onlineMenuState == OnlineMenuState.MainMenu);
            HostMenu.gameObject.SetActive(_onlineMenuState == OnlineMenuState.HostMenu);
            JoinMenu.gameObject.SetActive(_onlineMenuState == OnlineMenuState.JoinMenu);
            CurrentSubmenu.UpdateDisplayedValues();
        }
    }

    private UnityTransport UnityTransport => CoreManager.NetworkManager.GetComponent<UnityTransport>();
    private SettingsManager _settingsManager;

    private void Awake()
    {
        FindCoreManager();
        Helpers.AutoAssign(ref _settingsManager);
    }

    private void Start()
    {
        LoadFromSettings();
        OnlineMenuState = OnlineMenuState.MainMenu;
        MainMenu.GetLocalIps();
    }

    public void Connect(string passwordHash)
    {
        if (string.IsNullOrEmpty(JoinMenu.JoinIpAddress))
        {
            JoinMenu.Message = "Please enter a valid IP address.";
        }

        JoinMenu.JoinIpAddress = JoinMenu.JoinIpAddress.Trim();

        CoreManager.IsNetGame = true;
        CoreManager.IsHost = false;

        UnityTransport.SetConnectionData(JoinMenu.JoinIpAddress, JoinMenu.JoinPort);
        var bytes = System.Text.Encoding.UTF8.GetBytes(passwordHash);
        CoreManager.NetworkManager.NetworkConfig.ConnectionData = bytes;

        Debug.Log($"Starting client on port {JoinMenu.JoinPort}.");
        var result = CoreManager.NetworkManager.StartClient();

        if (!result)
        {
            Debug.LogError("Failed to start client.");
            return;
        }

        JoinMenu.Message = "Connecting...";
        SaveToSettings();

    }

    public void StartHosting(string passwordHash)
    {

        UnityTransport.SetConnectionData("127.0.0.1", HostMenu.HostPort, "0.0.0.0");
        CoreManager.IsNetGame = true;
        CoreManager.IsHost = true;

        CoreManager.ServerNetApi.MaxNetPlayers = HostMenu.MaxPlayers;
        CoreManager.ServerNetApi.SongSelectRules = HostMenu.SongSelectRules;
        CoreManager.ServerNetApi.ServerPasswordHash = passwordHash;

        CoreManager.NetworkManager.ConnectionApprovalCallback = CoreManager.ServerNetApi.ConnectionApprovalCallback;
        Debug.Log($"Starting server on port {HostMenu.HostPort}.");
        var result = CoreManager.NetworkManager.StartHost();

        if (!result)
        {
            Debug.LogError("Failed to start host.");
            return;
        }

        SaveToSettings();
    }

    public void ReturnToMainMenu()
    {
        CoreManager.IsNetGame = false;
        CoreManager.IsHost = true;
        CoreManager.NetworkManager.Shutdown();
        this.SceneTransition(GameScene.MainMenu);
    }

    public override void OnNetClientConnected(ulong id)
    {
        base.OnNetClientConnected(id);
        this.SceneTransition(GameScene.PlayerJoin);
    }

    public override void OnNetClientDisconnected(ulong id)
    {

        base.OnNetClientDisconnected(id);

        if (!CoreManager.NetworkManager.IsHost)
        {
            JoinMenu.Message = "Disconnected from server: " + CoreManager.NetworkManager.DisconnectReason;
        }
    }

    public override void OnPlayerInput(InputEvent inputEvent)
    {
        CurrentSubmenu.HandleInput(inputEvent);
    }

    public void LoadFromSettings()
    {
        _settingsManager.EnsureDefaultsForNetGame();
        HostMenu.LoadFromSettings(_settingsManager);
        JoinMenu.LoadFromSettings(_settingsManager);
    }

    public void SaveToSettings()
    {
        HostMenu.SaveToSettings(_settingsManager);
        JoinMenu.SaveToSettings(_settingsManager);
        _settingsManager.Save();
    }

}
