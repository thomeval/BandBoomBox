
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class OptionsManager : ScreenManager
{
    public Menu[] Submenus = new Menu[5];
    public Menu ActiveMenu;
    public Menu MainMenu;
    public CustomBindingDisplay CustomBindingDisplay;

    [Header("Gameplay")]
    public Text TxtAutoSetNoteLabels;
    public Text TxtDefaultKeyboardNoteLabels;
    public Text TxtShowLrr;

    [Header("Audio")]
    public Text TxtAudioLatency;
    public Text TxtMasterVolume;
    public Text TxtGameplaySfxVolume;
    public Text TxtGameplayMusicVolume;
    public Text TxtMistakeVolume;
    public Text TxtMenuSfxVolume;
    public Text TxtMenuMusicVolume;

    [Header("Graphics")]
    public Text TxtScreenResolution;
    public Text TxtFullScreenMode;
    public Text TxtVSync;
    public Text TxtTargetFrameRate;

    [Header("Advanced")]
    public Text TxtSaveDataLocation;
    public Text TxtLogsFolderLocation;

    private readonly int[] _targetFrameRateChoices = { 20, 30, 60, 90, 120, 144, 180, 240, -1 };
    private readonly string[] _resolutionChoices = { "1280x720", "1366x768", "1600x900", "1920x1080", "2560x1440", "3840x2160" };

    private readonly FullScreenMode[] _fullScreenModeChoices =
        {FullScreenMode.Windowed, FullScreenMode.FullScreenWindow, FullScreenMode.ExclusiveFullScreen};
    void Awake()
    {
        FindCoreManager();
    }

    // Start is called before the first frame update
    void Start()
    {
        SetActiveMenu("MainOptions");
        UpdateText();
        DisplayMenuPage();
        UpdateDisplayedKeyBindings();
    }

    private void UpdateDisplayedKeyBindings()
    {
        CustomBindingDisplay.Display(CoreManager.ControlsManager.CustomBindings);
    }

    private void UpdateText()
    {
        var settings = CoreManager.Settings;
        TxtAutoSetNoteLabels.text = settings.AutoSetNoteLabelsFromController ? "On" : "Off";
        TxtDefaultKeyboardNoteLabels.text = settings.DefaultKeyboardNoteLabels;
        TxtShowLrr.text = settings.LrrDisplayEnabled ? "On" : "Off";
        TxtAudioLatency.text = $"{settings.AudioLatency * 1000:F0} ms";
        TxtMasterVolume.text = $"{settings.MasterVolume:P0}";
        TxtGameplaySfxVolume.text = $"{settings.GameplaySfxVolume:P0}";
        TxtGameplayMusicVolume.text = $"{settings.GameplayMusicVolume:P0}";
        TxtMistakeVolume.text = $"{settings.MistakeVolume:P0}";
        TxtMenuSfxVolume.text = $"{settings.MenuSfxVolume:P0}";
        TxtMenuMusicVolume.text = $"{settings.MenuMusicVolume:P0}";
        TxtScreenResolution.text = settings.ScreenResolution;
        TxtFullScreenMode.text = settings.FullScreenMode.ToString();
        TxtTargetFrameRate.text = settings.TargetFrameRate == -1 ? "Unlimited" : "" + settings.TargetFrameRate;
        TxtVSync.text = settings.VSyncEnabled ? "On" : "Off";
        TxtSaveDataLocation.text = Helpers.AppSaveFolder;
        TxtLogsFolderLocation.text = Helpers.AppLogsFolder;

    }

    public override void OnPlayerInput(InputEvent inputEvent)
    {
        ActiveMenu.HandleInput(inputEvent);
        if (ActiveMenu == MainMenu)
        {
            DisplayMenuPage();
        }

        if (inputEvent.Action == InputAction.Left)
        {
            ChangeMenuItemValue(-1);
        }
        else if (inputEvent.Action == InputAction.Right)
        {
            ChangeMenuItemValue(1);
        }
    }

    private const float AUDIO_LATENCY_CHANGE_TICK = 0.01f;
    private const float VOLUME_CHANGE_TICK = 0.05f;

    private void ChangeMenuItemValue(int delta)
    {
        var settings = CoreManager.Settings;
        switch (ActiveMenu.SelectedText)
        {
            case "Auto Set Note Labels":
                settings.AutoSetNoteLabelsFromController = !settings.AutoSetNoteLabelsFromController;
                break;
            case "Default Keyboard Note Labels":
                settings.DefaultKeyboardNoteLabels = Helpers.GetNextValue(Player.LabelSkins, settings.DefaultKeyboardNoteLabels, delta, true);
                break;
                case "Show Long Range Radar":
                    settings.LrrDisplayEnabled = !settings.LrrDisplayEnabled;
                break;
            case "Master Volume":
                AddToOption(ref settings.MasterVolume, VOLUME_CHANGE_TICK * delta, 0.0f, 1.0f);
                break;
            case "Gameplay SFX Volume":
                AddToOption(ref settings.GameplaySfxVolume, VOLUME_CHANGE_TICK * delta, 0.0f, 1.0f);
                break;
            case "Gameplay Music Volume":
                AddToOption(ref settings.GameplayMusicVolume, VOLUME_CHANGE_TICK * delta, 0.0f, 1.0f);
                break;
            case "Mistake Volume":
                AddToOption(ref settings.MistakeVolume, VOLUME_CHANGE_TICK * delta, 0.0f, 1.0f);
                break;
            case "Menu SFX Volume":
                AddToOption(ref settings.MenuSfxVolume, VOLUME_CHANGE_TICK * delta, 0.0f, 1.0f);
                break;
            case "Menu Music Volume":
                AddToOption(ref settings.MenuMusicVolume, VOLUME_CHANGE_TICK * delta, 0.0f, 1.0f);
                break;
            case "Audio Latency":
                AddToOption(ref settings.AudioLatency, AUDIO_LATENCY_CHANGE_TICK * delta, -0.3f, 0.3f);
                break;
            case "VSync":
                settings.VSyncEnabled = !settings.VSyncEnabled;

                // Prevent both Target Frame Rate and Vsync from being enabled at the same time.
                if (settings.VSyncEnabled)
                {
                    CoreManager.Settings.TargetFrameRate = -1;
                }
                break;
            case "Target Frame Rate":
                var newTfr =
                    Helpers.GetNextValue(_targetFrameRateChoices, CoreManager.Settings.TargetFrameRate, delta, false);
                CoreManager.Settings.TargetFrameRate = newTfr;

                // Prevent both Target Frame Rate and Vsync from being enabled at the same time.
                if (newTfr != -1)
                {
                    CoreManager.Settings.VSyncEnabled = false;
                }
                break;
            case "Screen Resolution":
                if (Array.IndexOf(_resolutionChoices, settings.ScreenResolution) == -1)
                {
                    settings.ScreenResolution = _resolutionChoices[0];
                }
                else
                {
                    var newSr =
                        Helpers.GetNextValue(_resolutionChoices, CoreManager.Settings.ScreenResolution, delta, false);
                    CoreManager.Settings.ScreenResolution = newSr;
                }

                break;
            case "Full Screen / Windowed":
                var newFs = Helpers.GetNextValue(_fullScreenModeChoices, settings.FullScreenMode, delta, false);
                CoreManager.Settings.FullScreenMode = newFs;
                break;
        }

        UpdateText();

        if (MainMenu.SelectedText == "Audio")
        {
            CoreManager.SettingsHelper.ApplyAudioSettings();
        }
    }

    private void DisplayMenuPage()
    {
        var menuToShow = "Menu-" + MainMenu.SelectedText;
        foreach (var menu in Submenus)
        {
            menu.gameObject.SetActive(menu.gameObject.name == menuToShow);
        }
    }

    public void MenuItemSelected(MenuEventArgs args)
    {
        switch (args.SelectedItem)
        {
            case "Credits":
                SceneTransition(GameScene.Credits);
                break;
            case "BackToMainOptions":
                SetActiveMenu("MainOptions");
                break;
            case "MainMenu":
                CoreManager.Settings.Save();
                CoreManager.ControlsManager.SaveInputActions();
                CoreManager.SettingsHelper.ApplySettings();
                SceneTransition(GameScene.MainMenu);
                break;
            case "Open Logs Folder":
                TryOpenFolder(Helpers.AppLogsFolder);
                break;
            case "Open Save Data Folder":
                TryOpenFolder(Helpers.AppSaveFolder);
                break;
            case "Calibration":
                SceneTransition(GameScene.Calibration);
                break;
            default:
                SetActiveMenu(args.SelectedItem);
                break;
        }
    }

    private void TryOpenFolder(string folder)
    {
        var result = Helpers.OpenFolderWindow(folder);

        if (!result)
        {
            PlaySfx(SoundEvent.Mistake);
        }
    }

    private void SetActiveMenu(string menuName)
    {
        UnhighlightAllMenus();

        var menuToShow = MainMenu;

        if (menuName != "MainOptions")
        {
            menuToShow = Submenus.Single(e => e.gameObject.name == "Menu-" + menuName);
        }

        menuToShow.SelectionHighlightVisibility = true;
        ActiveMenu = menuToShow;
    }

    private void UnhighlightAllMenus()
    {
        MainMenu.SelectionHighlightVisibility = false;
        foreach (var menu in Submenus)
        {
            menu.SelectionHighlightVisibility = false;
        }
    }

    private void AddToOption(ref float option, float delta, float min, float max)
    {
        option = Mathf.Clamp(option + delta, min, max);
    }
}
