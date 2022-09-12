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
    public GameObject RootMenuContainer;

    [SerializeField]
    private Menu _activeMenu;
    public Menu ActiveMenu
    {
        get { return _activeMenu; }
        private set {  _activeMenu = value; }
    }

    private ChartEditorState[] _handledStates = new[]
    {
        ChartEditorState.MainMenu, ChartEditorState.ModifyMenu, ChartEditorState.ExitConfirm
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

        if (ActiveMenu != null)
        {
            ActiveMenu.HandleInput(gameplayEvent);
        }

    }

    void MenuItemSelected(MenuEventArgs args)
    {
        switch (_parent.ChartEditorState)
        {
            case ChartEditorState.MainMenu:
                MenuItemSelectedMain(args);
                break;
            case ChartEditorState.ModifyMenu:
                MenuItemSelectedModify(args);
                break;
            case ChartEditorState.ExitConfirm:
                MenuItemSelectedExitConfirm(args);
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
                _parent.PlayFromBeginning();
                break;
            case "Modify selected region":
                _parent.ChartEditorState = ChartEditorState.ModifyMenu;
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
        }
    }

    private void PlayConfirmOrCancelSfx(string selectedItem, string cancelItem)
    {
        if (selectedItem == cancelItem)
        {
            _parent.PlaySfx("SelectionCancelled");
            return;
        }
        _parent.PlaySfx("SelectionConfirmed");
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

}
