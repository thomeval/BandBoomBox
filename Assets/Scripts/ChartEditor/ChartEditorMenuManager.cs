using System;
using System.Linq;
using UnityEngine;

public class ChartEditorMenuManager : MonoBehaviour
{
    private ChartEditorManager _parent;

    [Header("Menus")]
    public Menu MnuMain;
    public GameObject MnuMainContainer;
    public Menu MnuExitConfirm;
    public GameObject MnuExitConfirmContainer;
    public Menu MnuModify;
    public GameObject MnuModifyContainer;
    public Menu MnuOptions;
    public GameObject MnuOptionsContainer;
    public GameObject MnuKeyboardControlsContainer;
    public GameObject MnuControllerControlsContainer;
    public GameObject RootMenuContainer;

    [SerializeField]
    private Menu _activeMenu;
    public Menu ActiveMenu
    {
        get { return _activeMenu; }
        private set { _activeMenu = value; }
    }

    private readonly ChartEditorState[] _handledStates = new[]
    {
        ChartEditorState.MainMenu, ChartEditorState.ModifyMenu, ChartEditorState.KeyboardControls, ChartEditorState.ControllerControls,
        ChartEditorState.OptionsMenu, ChartEditorState.ExitConfirm
    };

    public bool ShouldHandleState(ChartEditorState state)
    {
        return _handledStates.Contains(state);
    }

    public void SetMenuVisibility(ChartEditorState state)
    {
        MnuMainContainer.SetActive(false);
        MnuExitConfirmContainer.SetActive(false);
        MnuModifyContainer.SetActive(false);
        MnuOptionsContainer.SetActive(false);
        MnuKeyboardControlsContainer.SetActive(false);
        MnuControllerControlsContainer.SetActive(false);
        RootMenuContainer.SetActive(true);
        ActiveMenu = null;
        switch (state)
        {
            case ChartEditorState.Edit:
            case ChartEditorState.Playback:
                RootMenuContainer.SetActive(false);
                break;
            case ChartEditorState.MainMenu:
                MnuMainContainer.SetActive(true);
                ActiveMenu = MnuMain;
                break;
            case ChartEditorState.ModifyMenu:
                MnuModifyContainer.SetActive(true);
                ActiveMenu = MnuModify;
                break;
            case ChartEditorState.KeyboardControls:
                RootMenuContainer.SetActive(false);
                MnuKeyboardControlsContainer.SetActive(true);
                ActiveMenu = null;
                break;
            case ChartEditorState.ControllerControls:
                RootMenuContainer.SetActive(false);
                MnuControllerControlsContainer.SetActive(true);
                ActiveMenu = null;
                break;
            case ChartEditorState.OptionsMenu:
                MnuOptionsContainer.SetActive(true);
                _parent.Options.SetOptionsItemText();
                ActiveMenu = MnuOptions;
                break;
            case ChartEditorState.ExitConfirm:
                MnuExitConfirmContainer.SetActive(true);
                ActiveMenu = MnuExitConfirm;
                break;
        }
    }

    void Awake()
    {
        Helpers.AutoAssign(ref _parent);
    }

    public void OnPlayerInputMenus(InputEvent inputEvent, ChartEditorState state)
    {
        // Menus expect gameplay InputEvents, so convert the editor InputEvents to gameplay ones.
        var gameplayEvent = InputEvent.AsGameplayEvent(inputEvent);

        if (gameplayEvent == null)
        {
            return;
        }

        if (state == ChartEditorState.KeyboardControls || state == ChartEditorState.ControllerControls)
        {
            HandleInputControls(gameplayEvent);
            return;
        }

        if (ActiveMenu != null)
        {
            ActiveMenu.HandleInput(gameplayEvent);
        }

    }

    private void HandleInputControls(InputEvent inputEvent)
    {
        if (inputEvent.Action == InputAction.B
            || inputEvent.Action == InputAction.A
            || inputEvent.Action == InputAction.Pause
            || inputEvent.Action == InputAction.Back)
        {
            _parent.PlaySfx(SoundEvent.SelectionCancelled);
            _parent.ChartEditorState = ChartEditorState.MainMenu;
        }
    }

    void MenuItemShifted(MenuEventArgs args)
    {
        switch (_parent.ChartEditorState)
        {
            case ChartEditorState.OptionsMenu:
                MenuItemShiftedOptions(args);
                break;
        }
    }

    private void MenuItemShiftedOptions(MenuEventArgs args)
    {
        var opt = _parent.Options;
        switch (args.SelectedItem)
        {
            case "Note Labels":
                opt.ChangeLabelSkin(args.ShiftAmount);
                _parent.ApplyNoteSkin();
                break;
            case "Auto step forward":
                opt.AutoStepForward = !opt.AutoStepForward;
                break;
            case "Allow All Note Types":
                opt.AllowAllNotes = !opt.AllowAllNotes;
                _parent.ShowNotePalette();
                break;
                case "Auto-save Interval":
                opt.ChangeAutoSaveInterval(args.ShiftAmount);
                _parent.AutoSaver.AutoSaveIntervalMinutes = opt.AutoSaveIntervalMinutes;
                break;

        }

        opt.SetOptionsItemText();
    }

    void MenuItemSelected(MenuEventArgs args)
    {
        switch (_parent.ChartEditorState)
        {
            case ChartEditorState.MainMenu:
                MenuItemSelectedMain(args);
                break;
            case ChartEditorState.OptionsMenu:
                MenuItemSelectedOptions(args);
                break;
            case ChartEditorState.ModifyMenu:
                MenuItemSelectedModify(args);
                break;
            case ChartEditorState.ExitConfirm:
                MenuItemSelectedExitConfirm(args);
                break;
        }
    }

    private void MenuItemSelectedOptions(MenuEventArgs args)
    {
        PlayConfirmOrCancelSfx(args.SelectedItem, "Back");
        switch (args.SelectedItem)
        {
            case "Back":
                _parent.ChartEditorState = ChartEditorState.MainMenu;
                break;
        }
    }

    void MenuItemSelectedMain(MenuEventArgs args)
    {
        PlayConfirmOrCancelSfx(args.SelectedItem, "Continue editing");
        switch (args.SelectedItem)
        {
            case "Continue editing":
                _parent.ChartEditorState = ChartEditorState.Edit;
                break;
            case "Play from beginning":
                _parent.PlaybackManager.PlayFromBeginning();
                break;
            case "Options":
                _parent.ChartEditorState = ChartEditorState.OptionsMenu;
                break;
            case "Keyboard Controls":
                _parent.ChartEditorState = ChartEditorState.KeyboardControls;
                break;
            case "Controller Controls":
                _parent.ChartEditorState = ChartEditorState.ControllerControls;
                break;
            case "Modify selected region":
                if (!_parent.RegionSelected)
                {
                    _parent.DisplayMessage("Select a region first, by marking the start and end points with the 'Select Region' key.", true);
                    _parent.PlaySfx(SoundEvent.Mistake);
                    return;
                }
                _parent.ChartEditorState = ChartEditorState.ModifyMenu;
                break;
            case "Paste":
                _parent.ChartEditorState = ChartEditorState.Edit;
                _parent.Clipboard.OnPlayerInput(InputAction.Editor_Paste);
                _parent.RefreshNoteCounts();
                break;
            case "Save":
                _parent.ChartEditorState = ChartEditorState.Edit;
                _parent.SaveChart();
                break;
            case "Exit Editor":
                _parent.ChartEditorState = ChartEditorState.ExitConfirm;
                break;

        }
    }

    private void MenuItemSelectedModify(MenuEventArgs args)
    {
        PlayConfirmOrCancelSfx(args.SelectedItem, "Back");
        switch (args.SelectedItem)
        {
            case "Back":
                _parent.ChartEditorState = ChartEditorState.MainMenu;
                break;
            case "Cut":
                _parent.ChartEditorState = ChartEditorState.Edit;
                _parent.Clipboard.OnPlayerInput(InputAction.Editor_Cut);
                break;
            case "Copy":
                _parent.ChartEditorState = ChartEditorState.Edit;
                _parent.Clipboard.OnPlayerInput(InputAction.Editor_Copy);
                break;
            case "Swap Hands":
                _parent.NoteTransformer.SwapHands();
                _parent.ChartEditorState = ChartEditorState.Edit;
                break;
            case "Invert":
                _parent.NoteTransformer.Invert();
                _parent.ChartEditorState = ChartEditorState.Edit;
                break;
            case "Rotate 90 Degrees":
                _parent.NoteTransformer.Rotate90();
                _parent.ChartEditorState = ChartEditorState.Edit;
                break;
            case "Expand Medium To Hard":
                _parent.NoteTransformer.ExpandMediumToHard();
                _parent.ChartEditorState = ChartEditorState.Edit;
                break;
            case "Expand Hard To Expert":
                _parent.NoteTransformer.ExpandToExpert();
                _parent.ChartEditorState = ChartEditorState.Edit;
                break;
            case "Clear Selected Region":
                _parent.NoteTransformer.ClearRegion();
                _parent.ChartEditorState = ChartEditorState.Edit;
                break;
            case "Clamp Notes to Current Difficulty":
                _parent.NoteTransformer.ClampToDifficulty(_parent.CurrentChart.Difficulty);
                _parent.ChartEditorState = ChartEditorState.Edit;
                break;
            case "Ensure Minimum Spacing":
                _parent.NoteTransformer.EnsureSpacing(1f / _parent.CursorStepSize);
                _parent.ChartEditorState = ChartEditorState.Edit;
                break;
            default:
                throw new Exception("Unknown menu item selected: " + args.SelectedItem);
        }

        _parent.RefreshNoteCounts();
    }

    void MenuItemSelectedExitConfirm(MenuEventArgs args)
    {
        PlayConfirmOrCancelSfx(args.SelectedItem, "No");
        switch (args.SelectedItem)
        {
            case "No":
                _parent.ChartEditorState = ChartEditorState.MainMenu;
                break;
            case "Yes":
                _parent.CloseEditor();
                break;
        }
    }

    private void PlayConfirmOrCancelSfx(string selectedItem, string cancelItem)
    {
        if (selectedItem == cancelItem)
        {
            _parent.PlaySfx(SoundEvent.SelectionCancelled);
            return;
        }
        _parent.PlaySfx(SoundEvent.SelectionConfirmed);
    }
}
