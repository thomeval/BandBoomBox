﻿using System.Collections;
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
    public SecretCodeHandler SecretCodeHandler;

    public Animator LogoAnimator;
    public float StartAnimationDelay = 1.0f;
    public float MusicStartDelay = 0.5f;
    public float MusicStartDelaySecret = 5.0f;
    private TextPulser _pulser;

    void Awake()
    {
        FindCoreManager();
        _pulser = LblPressStart.GetComponent<TextPulser>();
    }

    void Start()
    {
        TxtVersion.text = Application.version;

        CoreManager.ShutdownNetPlay();
        CoreManager.PlayerManager.ClearParticipation();

        if (CoreManager.TitleScreenShown)
        {
            LogoAnimator.Play("MainMenuFastShown");
            DisplayMenu();
        }
        else
        {
            CoreManager.MenuMusicManager.PlaySceneMusic(GameScene.MainMenu_Title);
            CoreManager.ControlsManager.ApplyCustomBindings();
        }
    }

    public override void OnPlayerInput(InputEvent inputEvent)
    {
        switch (State)
        {
            case MainMenuState.PressStart:
                HandleTitleInput(inputEvent);
                break;
            case MainMenuState.MainMenu:
                Menu.HandleInput(inputEvent);
                break;
        }

    }

    private void HandleTitleInput(InputEvent inputEvent)
    {
        SecretCodeHandler.HandleInput(inputEvent.Action);
        switch (inputEvent.Action)
        {
            case InputAction.Pause:
                OnStartPressed();
                break;
            default:
                PlaySfx(SoundEvent.Mistake);
                break;
        }

    }

    private IEnumerator DisplayMenuAfterDelay()
    {
        LogoAnimator.SetTrigger("ShowMainMenu");
        yield return new WaitForSeconds(StartAnimationDelay);
        DisplayMenu();
    }

    private IEnumerator StartMenuMusicAfterDelay(bool secretUnlocked)
    {
        yield return new WaitForSeconds(secretUnlocked ? MusicStartDelaySecret : MusicStartDelay);
        CoreManager.MenuMusicManager.PlaySceneMusic(GameScene.MainMenu);
    }

    private void DisplayMenu()
    {
        LblPressStart.SetActive(false);
        MenuContainer.SetActive(true);
        State = MainMenuState.MainMenu;
    }

    private void OnStartPressed()
    {
        var secretUnlocked = SecretCodeHandler.ActivatedCode != null;
        if (secretUnlocked)
        {
            PlaySfx(SoundEvent.SecretUnlocked);
            SecretCodeHandler.RunActivatedCode();
            ShowMessage(SecretCodeHandler.Message);
        }
        else
        {
            PlaySfx(SoundEvent.TitleScreen_StartPressed);
        }

        CoreManager.MenuMusicManager.StopAll();
        CoreManager.TitleScreenShown = true;
        _pulser.Period = 1 / 10f;
        _pulser.Min = 0.1f;
        CoreManager.PlayerManager.Players[0].TriggerRumbleForTitleStart();
        StartCoroutine(DisplayMenuAfterDelay());
        StartCoroutine(StartMenuMusicAfterDelay(secretUnlocked));
    }

    public void MenuItemSelected(MenuEventArgs args)
    {
        switch (args.SelectedItem)
        {
            case "Play":
                if (CheckNoSongsLoaded())
                {
                    return;
                }
                this.ErrorMessage.Error = null;

                CoreManager.PlayerManager.AutoSetNoteSkin();
                CoreManager.IsNetGame = false;
                SceneTransition(GameScene.PlayerJoin);
                break;
            case "Network Play":
                if (CheckNoSongsLoaded())
                {
                    return;
                }
                this.ErrorMessage.Error = null;

                CoreManager.PlayerManager.AutoSetNoteSkin();
                SceneTransition(GameScene.NetPlayMenu);
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

    private bool CheckNoSongsLoaded()
    {
        if (!CoreManager.SongLibrary.Songs.Any())
        {
            PlaySfx(SoundEvent.Mistake);
            ShowMessage(  "No playable songs were found in any of the configured song folders. At least one song is required for the game to be playable. Check the log files for details.");
            var songFolders = CoreManager.Settings.GetResolvedSongFolders().Aggregate((cur, next) => cur + " \r\n" + next);
            Debug.LogError("No playable songs were found in any of the configured song folders. At least one song is required for the game to be playable. Current song folders: \r\n" + songFolders + "\r\n" +
                            "Song files must be located in one of the above folders (or a subfolder), be in the .sjson format, and have a .mp3 or .ogg file for audio. ");
            return true;
        }
        return false;
    }

    public void ShowMessage(string message)
    {
        this.ErrorMessage.gameObject.SetActive(!string.IsNullOrEmpty(message));
        this.ErrorMessage.Error = message;
    }
}
