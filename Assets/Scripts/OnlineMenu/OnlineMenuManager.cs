using System;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;

public class OnlineMenuManager : ScreenManager
{

    public InputField TxtHostPort;
    public InputField TxtHostMaxPlayers;
    public InputField TxtHostPassword;

    public InputField TxtJoinIpAddress;
    public InputField TxtJoinPort;
    public InputField TxtJoinPassword;

    public Text LblHostMessage;
    public Text LblJoinMessage;

    public GameObject MainMenu;
    public GameObject HostMenu;
    public GameObject JoinMenu;

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

    private void Awake()
    {
        FindCoreManager();
    }

    private void Start()
    {
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

        var ip = TxtJoinIpAddress.text;

        var port = GetValueOrDefault(TxtJoinPort.text, 3334);
        var port2 = Convert.ToUInt16(port);

        CoreManager.IsNetGame = true;
        CoreManager.IsHost = false;

        var passwordHash = ComputeHash(TxtJoinPassword.text);

        UnityTransport.SetConnectionData(ip, port2);
        var bytes = System.Text.Encoding.UTF8.GetBytes(passwordHash);
        CoreManager.NetworkManager.NetworkConfig.ConnectionData = bytes;

        Debug.Log($"Starting client on port {port2}.");
        var result = CoreManager.NetworkManager.StartClient();

        if (!result)
        {
            Debug.LogError("Failed to start client.");
        }

        if (result)
        {
            LblJoinMessage.text = "Connecting...";
        }

    }

    private int GetValueOrDefault(string text, int defaultValue)
    {
        var result = defaultValue;
        int.TryParse(text, out result);
        return result;
    }

    public void BtnStartHosting_OnClick()
    {
        var port = GetValueOrDefault(TxtHostPort.text, 3334);
        var port2 = Convert.ToUInt16(port);

        UnityTransport.SetConnectionData("127.0.0.1", port2, "0.0.0.0");
        CoreManager.IsNetGame = true;
        CoreManager.IsHost = true;

        var maxPlayers = GetValueOrDefault(TxtHostMaxPlayers.text, 8);
        maxPlayers = Math.Clamp(maxPlayers, 2, 32);
        CoreManager.ServerNetApi.MaxNetPlayers = maxPlayers;

        var passwordHash = ComputeHash(TxtHostPassword.text);
        CoreManager.ServerNetApi.ServerPasswordHash = passwordHash;

        CoreManager.NetworkManager.ConnectionApprovalCallback = CoreManager.ServerNetApi.ConnectionApprovalCallback;
        Debug.Log($"Starting server on port {port}.");
        var result = CoreManager.NetworkManager.StartHost();

        if (!result)
        {
            Debug.LogError("Failed to start host.");
            return;
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
}
