using Newtonsoft.Json;
using System.Linq;
using System.Net;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;

public class NetPlayMenuManager : ScreenManager
{
    public NetPlayMainSubmenu MainMenu;
    public NetPlayHostSubmenu HostMenu;
    public NetPlayJoinSubmenu JoinMenu;

    public NetPlaySubmenuBase CurrentSubmenu
    {
        get
        {
            switch (NetPlayMenuState)
            {
                case NetPlayMenuState.MainMenu:
                    return MainMenu;
                case NetPlayMenuState.HostMenu:
                    return HostMenu;
                case NetPlayMenuState.JoinMenu:
                    return JoinMenu;
                default:
                    return MainMenu;
            }
        }
    }

    public Text TxtLocalIps;

    public const ushort DEFAULT_PORT = 3334;

    [SerializeField]
    private NetPlayMenuState _netPlayMenuState = NetPlayMenuState.MainMenu;

    public NetPlayMenuState NetPlayMenuState
    {
        get
        {
            return _netPlayMenuState;
        }
        set
        {
            _netPlayMenuState = value;
            MainMenu.gameObject.SetActive(_netPlayMenuState == NetPlayMenuState.MainMenu);
            HostMenu.gameObject.SetActive(_netPlayMenuState == NetPlayMenuState.HostMenu);
            JoinMenu.gameObject.SetActive(_netPlayMenuState == NetPlayMenuState.JoinMenu);
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
        NetPlayMenuState = NetPlayMenuState.MainMenu;
        GetLocalIps();
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
        CoreManager.NetworkManager.NetworkConfig.ConnectionData = GetJoinParams(passwordHash);

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

    private static byte[] GetJoinParams(string passwordHash)
    {
        var request = new NetGameJoinParams
        {
            PasswordHash = passwordHash,
            ClientGameVersion = Application.version
        };
        var json = JsonConvert.SerializeObject(request);
        var bytes = System.Text.Encoding.UTF8.GetBytes(json);
        return bytes;
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
        CoreManager.NetworkManager.NetworkConfig.ConnectionData = GetJoinParams(passwordHash);

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
        Debug.Log("Connection to host successful. Client ID: " + id);

        CoreManager.PlayerManager.SetNetId();

        foreach (var player in CoreManager.PlayerManager.AsDto())
        {
            CoreManager.ServerNetApi.RegisterNetPlayerServerRpc(player);
        }

        JoinMenu.Message = "Requesting game settings...";

        CoreManager.ClientNetApi.NetSettingsUpdated += ClientNetApi_NetSettingsUpdated;
        CoreManager.ServerNetApi.RequestNetGameSettingsServerRpc();

    }

    private void ClientNetApi_NetSettingsUpdated(object sender, System.EventArgs e)
    {
        CoreManager.ClientNetApi.NetSettingsUpdated -= ClientNetApi_NetSettingsUpdated;
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
        if (inputEvent.Action == InputAction.X && !inputEvent.IsPressed && CurrentSubmenu.MenuInputActive())
        {
            ToggleIpsDisplay();
            PlaySfx(SoundEvent.SelectionShifted);
            return;
        }

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

    
    public void GetLocalIps()
    {
        TxtLocalIps.text = "Your IP's: ";

        try
        {
            var addresses = Dns.GetHostEntry(Dns.GetHostName()).AddressList
                            .Where(e => e.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);

            foreach (var address in addresses)
            {
                TxtLocalIps.text += address.ToString() + ", ";
            }
        }
        catch
        {
            TxtLocalIps.text += "[Unknown]";
        }
    }

    public void ToggleIpsDisplay()
    {
        TxtLocalIps.gameObject.SetActive(!TxtLocalIps.gameObject.activeSelf);
        if (TxtLocalIps.gameObject.activeSelf)
        {
            GetLocalIps();
        }
    }
}
