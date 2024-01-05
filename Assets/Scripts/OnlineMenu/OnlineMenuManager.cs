using System;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;

public class OnlineMenuManager : ScreenManager
{

    public InputField TxtIpAddress;
    public InputField TxtPort;
    public Text LblMessage;

    private UnityTransport UnityTransport => CoreManager.NetworkManager.GetComponent<UnityTransport>();
    private void Awake()
    {
        FindCoreManager();
    }

    public void BtnHostGame_OnClick()
    {
        var port = Convert.ToUInt16(TxtPort.text);
        UnityTransport.SetConnectionData("127.0.0.1", port, "0.0.0.0");
        CoreManager.IsNetGame = true;
        CoreManager.IsHost = true;

        Debug.Log($"Starting server on port {port}.");
        var result = CoreManager.NetworkManager.StartHost();

        if (!result)
        {
            Debug.LogError("Failed to start host.");
            return;
        }
    }

    public void BtnJoinGame_OnClick()
    {
        var ip = TxtIpAddress.text;
        var port = Convert.ToUInt16(TxtPort.text);
        CoreManager.IsNetGame = true;
        CoreManager.IsHost = false;
        UnityTransport.SetConnectionData(ip, port);
        Debug.Log($"Starting client on port {port}.");
        var result = CoreManager.NetworkManager.StartClient();

        if (!result)
        {
            Debug.LogError("Failed to start client.");
        }

        if (result)
        {
            LblMessage.text = "Connecting...";
        }

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


}
