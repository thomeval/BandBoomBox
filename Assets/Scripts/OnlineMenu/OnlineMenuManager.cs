using System;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;

public class OnlineMenuManager : ScreenManager
{

    [Header("Hosting")]
    public InputField TxtHostPort;
    public InputField TxtHostMaxPlayers;
    public InputField TxtHostPassword;
    public Dropdown CmbSongSelectRules;
    public Text LblHostMessage;

    [Header("Joining")]
    public InputField TxtJoinIpAddress;
    public InputField TxtJoinPort;
    public InputField TxtJoinPassword;
    public Text LblJoinMessage;

    public GameObject MainMenu;
    public GameObject HostMenu;
    public GameObject JoinMenu;

    public const int DEFAULT_PORT = 3334;

    public UInt16 HostPort
    {
        get
        {
            var result = GetValueOrDefault(TxtHostPort.text, DEFAULT_PORT);
            return Convert.ToUInt16(result);
        }
        set
        {
            TxtHostPort.text = value.ToString();
        }
    }

    public UInt16 JoinPort
    {
        get
        {
            var result = GetValueOrDefault(TxtJoinPort.text, DEFAULT_PORT);
            return Convert.ToUInt16(result);
        }
        set
        {
            TxtJoinPort.text = value.ToString();
        }
    }

    public int MaxPlayers
    {
        get
        {
            var result = GetValueOrDefault(TxtHostMaxPlayers.text, 8);
            result = Math.Clamp(result, 2, 32);
            return result;
        }
        set
        {
            TxtHostMaxPlayers.text = value.ToString();
        }
    }

    public string JoinIpAddress
    {
        get
        {
            return TxtJoinIpAddress.text;
        }
        set
        {
            TxtJoinIpAddress.text = value;
        }
    }

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
            MainMenu.SetActive(_onlineMenuState == OnlineMenuState.MainMenu);
            HostMenu.SetActive(_onlineMenuState == OnlineMenuState.HostMenu);
            JoinMenu.SetActive(_onlineMenuState == OnlineMenuState.JoinMenu);
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
    }

    public void BtnHostGame_OnClick()
    {
        this.OnlineMenuState = OnlineMenuState.HostMenu;
    }

    public void BtnJoinGame_OnClick()
    {
        this.OnlineMenuState = OnlineMenuState.JoinMenu;
    }

    public void BtnConnect_OnClick()
    {
        if (string.IsNullOrEmpty(TxtJoinIpAddress.text))
        {
            LblJoinMessage.text = "Please enter a valid IP address.";
        }

        var ip = TxtJoinIpAddress.text.Trim();

        CoreManager.IsNetGame = true;
        CoreManager.IsHost = false;

        var passwordHash = ComputeHash(TxtJoinPassword.text);

        UnityTransport.SetConnectionData(ip, JoinPort);
        var bytes = System.Text.Encoding.UTF8.GetBytes(passwordHash);
        CoreManager.NetworkManager.NetworkConfig.ConnectionData = bytes;

        Debug.Log($"Starting client on port {JoinPort}.");
        var result = CoreManager.NetworkManager.StartClient();

        if (!result)
        {
            Debug.LogError("Failed to start client.");
            return;
        }

        LblJoinMessage.text = "Connecting...";
        SaveToSettings();

    }

    private int GetValueOrDefault(string text, int defaultValue)
    {
        var result = defaultValue;
        int.TryParse(text, out result);
        return result;
    }

    public void BtnStartHosting_OnClick()
    {

        UnityTransport.SetConnectionData("127.0.0.1", HostPort, "0.0.0.0");
        CoreManager.IsNetGame = true;
        CoreManager.IsHost = true;

        CoreManager.ServerNetApi.MaxNetPlayers = MaxPlayers;
        CoreManager.ServerNetApi.SongSelectRules = GetSongSelectRules();

        var passwordHash = ComputeHash(TxtHostPassword.text);
        CoreManager.ServerNetApi.ServerPasswordHash = passwordHash;

        CoreManager.NetworkManager.ConnectionApprovalCallback = CoreManager.ServerNetApi.ConnectionApprovalCallback;
        Debug.Log($"Starting server on port {HostPort}.");
        var result = CoreManager.NetworkManager.StartHost();

        if (!result)
        {
            Debug.LogError("Failed to start host.");
            return;
        }

        SaveToSettings();
    }

    private NetSongSelectRules GetSongSelectRules()
    {
        switch (CmbSongSelectRules.GetSelectedText())
        {
            case "Anyone picks":
                return NetSongSelectRules.AnyonePicks;
            case "Host picks":
                return NetSongSelectRules.HostPicks;
            default:
                return NetSongSelectRules.AnyonePicks;
        }
    }

    public void BtnBackToOnlineMenu_OnClick()
    {
        this.OnlineMenuState = OnlineMenuState.MainMenu;
    }

    public void BtnBackToMainMenu_OnClick()
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
            LblJoinMessage.text = "Disconnected from server: " + CoreManager.NetworkManager.DisconnectReason;
        }
    }

    public string ComputeHash(string password)
    {
        if (string.IsNullOrEmpty(password))
        {
            return "";
        }

        var crypt = new System.Security.Cryptography.SHA256Managed();
        var result = new System.Text.StringBuilder();
        var hashBytes = crypt.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password), 0, System.Text.Encoding.UTF8.GetByteCount(password));
        foreach (byte theByte in hashBytes)
        {
            result.Append(theByte.ToString("x2"));
        }

        return result.ToString();
    }

    public void LoadFromSettings()
    {
        _settingsManager.EnsureDefaultsForNetGame();
        MaxPlayers = _settingsManager.NetGameHostMaxPlayers;
        HostPort = (ushort)_settingsManager.NetGameHostPort;
        JoinPort = (ushort)_settingsManager.NetGameJoinPort;
        JoinIpAddress = _settingsManager.NetGameJoinIpAddress;
        CmbSongSelectRules.value = (int)_settingsManager.NetGameHostSongSelectRules;
    }

    public void SaveToSettings()
    {
        var serverApi = CoreManager.ServerNetApi;
        _settingsManager.NetGameHostMaxPlayers = serverApi.MaxNetPlayers;
        _settingsManager.NetGameHostSongSelectRules = serverApi.SongSelectRules;
        _settingsManager.NetGameHostPort = HostPort;
        _settingsManager.NetGameJoinIpAddress = JoinIpAddress;
        _settingsManager.NetGameJoinPort = JoinPort;
        _settingsManager.Save();

    }
}
