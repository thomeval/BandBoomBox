using System.Collections.Generic;
using UnityEngine;

public class HowToPlayManager : ScreenManager
{
    private readonly List<GameObject> _pages = new List<GameObject>();
    public GameObject PageContainer;
    public Menu MainMenu;

    public GameObject ActivePage
    {
        get { return _pages[MainMenu.SelectedIndex]; }
    }

    void Awake()
    {
        FindCoreManager();

        foreach (var obj in PageContainer.GetChildren())
        {
            _pages.Add(obj);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        DisplayMenuPage();
    }

    public override void OnPlayerInput(InputEvent inputEvent)
    {
        MainMenu.HandleInput(inputEvent);
        DisplayMenuPage();

        switch (inputEvent.Action)
        {
            case "B":
            case "Back":
                SceneTransition(GameScene.MainMenu);
                break;
        }
    }
    private void DisplayMenuPage()
    {
        var idx = MainMenu.SelectedIndex;

        for (int x = 0; x < _pages.Count; x++)
        {
            _pages[x].SetActive(x == idx);
        }
    }

    public void MenuItemSelected(MenuEventArgs args)
    {
        // Intentionally left blank
    }
}
