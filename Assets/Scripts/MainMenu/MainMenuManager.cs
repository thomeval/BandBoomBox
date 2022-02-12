using System;
using System.Collections;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : ScreenManager
{
    public GameObject MenuContainer;
    public Menu Menu;
    public GameObject LblPressStart;
    public MainMenuState State = MainMenuState.PressStart;
    public ErrorMessage ErrorMessage;
    public Text TxtVersion;

    public float StartAnimationDelay = 1.0f;

    [Header("Sounds")]
    public AudioSource SfxStartPressed;
    public AudioSource SfxMistake;

    private TextPulser _pulser;
    void Awake()
    {
        FindCoreManager();
        _pulser = LblPressStart.GetComponent<TextPulser>();
    }

    void Start()
    {
        TxtVersion.text = UnityEngine.Application.version;

        if (CoreManager.TitleScreenShown)
        {
            DisplayMenu();
        }
        else
        {
            CoreManager.MenuMusicManager.PlaySceneMusic(GameScene.MainMenu_Title);
        }
    }

    public override void OnPlayerInput(InputEvent inputEvent)
    {
        switch (State)
        {
            case MainMenuState.PressStart:
                if (inputEvent.Action == "Pause")
                {
                    OnStartPressed();
                }
                break;
            case MainMenuState.MainMenu:
                Menu.HandleInput(inputEvent);
                break;
        }

    }

    private IEnumerator DisplayMenuAfterDelay()
    {
        yield return new WaitForSeconds(StartAnimationDelay);
        DisplayMenu();
    }

    private void DisplayMenu()
    {
        LblPressStart.SetActive(false);
        MenuContainer.SetActive(true);
        State = MainMenuState.MainMenu;
        CoreManager.MenuMusicManager.PlaySceneMusic(GameScene.MainMenu);
    }

    private void OnStartPressed()
    {
        SfxStartPressed.Play();
        CoreManager.MenuMusicManager.StopAll();
        CoreManager.TitleScreenShown = true;
        _pulser.Period = 1 / 10f;
        _pulser.Min = 0.1f;
        StartCoroutine(DisplayMenuAfterDelay());
    }

    public void MenuItemSelected(MenuEventArgs args)
    {
        switch (args.SelectedItem)
        {
            case "Play":
                if (!CoreManager.SongLibrary.Songs.Any())
                {

                    SfxMistake.PlayUnlessNull();
                    this.ErrorMessage.Error = "No playable songs were found in any of the configured song folders. At least one song is required for the game to be playable. Check the log files for details.";
                    var songFolders = CoreManager.Settings.GetResolvedSongFolders().Aggregate((cur, next) => cur +" \r\n" + next);
                    Debug.LogError("No playable songs were found in any of the configured song folders. At least one song is required for the game to be playable. Current song folders: \r\n" + songFolders + "\r\n" +
                                   "Song files must be located in one of the above folders (or a subfolder), be in the .sjson format, and have a .mp3 or .ogg file for audio. ");
                    return;
                }

                this.ErrorMessage.Error = null;

                SceneTransition(GameScene.PlayerJoin);
                break;
            case "Options":
               SceneTransition(GameScene.Options);
                break;
            case "How To Play":
                SceneTransition(GameScene.HowToPlay);
                break;
            case "Editor":
                SceneTransition(GameScene.Editor);
                break;
            case "Exit":
                CoreManager.Settings.Save();
                CoreManager.ControlsManager.SaveInputActions();
                Application.Quit(0);
                break;
        }

    }
}
