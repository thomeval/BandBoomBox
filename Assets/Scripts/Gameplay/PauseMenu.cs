using Newtonsoft.Json.Linq;
using System;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class PauseMenu : MonoBehaviour
{
    public Menu Menu;
    public SpriteResolver PlayerIcon;

    public GameObject RestartMenuItem;
    public GameObject ChangeDifficultyMenuItem;
    public GameObject EndSongMenuItem;
    public GameObject AbortSongMenuItem;
    public GameObject DisconnectMenuItem;

    public int Player
    {
        get { return Menu.Player; }
    }

    [SerializeField]
    private bool _isNetGame = false;
    public bool IsNetGame
    {
        get { return _isNetGame; }
        set
        {
            _isNetGame = value;
            SetMenuItemVisibility();

        }
    }

    [SerializeField]
    private bool _isHost = false;
    public bool IsHost
    {
        get { return _isHost; }
        set
        {
            _isHost = value;
            SetMenuItemVisibility();
        }
    }

    private void SetMenuItemVisibility()
    {
        // Only applicable in Local Games
        RestartMenuItem.SetActive(!IsNetGame);
        ChangeDifficultyMenuItem.SetActive(!IsNetGame);
        EndSongMenuItem.SetActive(!IsNetGame);

        // Only applicable in Network Games
        AbortSongMenuItem.SetActive(IsNetGame && IsHost);
        DisconnectMenuItem.SetActive(IsNetGame && !IsHost);
    }

    private GameplayManager _gameplayManager;

    void Awake()
    {
        _gameplayManager = GameObject.FindObjectOfType<GameplayManager>();
    }

    public void Show(int player, bool isNetGame, bool isHost)
    {
        var label = player > 0 ? "P" + player : "None";
        PlayerIcon.SetCategoryAndLabel("PlayerIdentifiers", label);
        Menu.Player = player;
        this.IsNetGame = isNetGame;
        this.IsHost = isHost;

        this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    public void HandleInput(InputEvent inputEvent)
    {
        Menu.HandleInput(inputEvent);
    }

    public void MenuItemSelected(MenuEventArgs args)
    {
        _gameplayManager.SendMessage("PauseMenuItemSelected", args);
    }
}
