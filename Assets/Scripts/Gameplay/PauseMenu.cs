using Assets;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class PauseMenu : MonoBehaviour
{
    public Menu Menu;
    public SpriteResolver PlayerIcon;

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
