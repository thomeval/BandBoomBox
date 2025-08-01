using System;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class PauseMenu : MonoBehaviour
{
    public Menu Menu;
    public SpriteResolver PlayerIcon;

    public MenuSoundEventHandler[] SoundEventHandlers = new MenuSoundEventHandler[4];

    public int Player
    {
        get { return Menu.Player; }
    }

    private GameplayManager _gameplayManager;

    void Awake()
    {
        _gameplayManager = GameObject.FindObjectOfType<GameplayManager>();
    }

    public void Show(int player)
    {
        var label = player > 0 ? "P" + player : "None";
        PlayerIcon.SetCategoryAndLabel("PlayerIdentifiers", label);
        Menu.Player = player;

        var handlerIdx = Math.Clamp(player - 1, 0, SoundEventHandlers.Length - 1);

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
