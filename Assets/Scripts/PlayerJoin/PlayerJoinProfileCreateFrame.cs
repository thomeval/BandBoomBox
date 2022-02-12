using UnityEngine;
using UnityEngine.UI;

public class PlayerJoinProfileCreateFrame : MonoBehaviour
{
    public PlayerJoinFrame Parent;

    public string EnteredText = "";

    public int SelectedLetterIndex = 0;
    public Menu Menu;
    public Text TxtEnteredText;
    public Text TxtCurrentLetter;
    public Text TxtMessage;

    public int MaxLength = 12;

    private readonly string _letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_?!";
    private ProfileManager _profileManager;

    public char CurrentLetter
    {
        get { return _letters[SelectedLetterIndex]; }
    }


    public string DefaultMessage = "Enter a name for the new profile. It can be up to 12 characters long.";
    public string Error
    {
        get { return TxtMessage.text == DefaultMessage ? null : TxtMessage.text; }
        set
        {
            var isError = !string.IsNullOrEmpty(value);

            if (isError)
            {
                TxtMessage.text = value;
                TxtMessage.color = TxtMessage.color = Parent.ErrorMessageColor; ;
            }
            else
            {
                TxtMessage.text = DefaultMessage;
                TxtMessage.color = Color.white;
            }
        }
    }


    void Awake()
    {
        _profileManager = FindObjectOfType<ProfileManager>();
    }

    void OnEnable()
    {
        this.Error = null;
        this.Refresh();
    }

    public void Reset()
    {
        EnteredText = "";
        SelectedLetterIndex = 0;
        Refresh();
    }
    public void HandleInput(InputEvent inputEvent)
    {
        switch (inputEvent.Action)
        {
            case "B":
                if (Menu.SelectedIndex == 0)
                {
                    MenuItemSelected(new MenuEventArgs {SelectedItem = "Backspace"});
                    Menu.PlaySound("SelectionCancelled");
                }
                return;
            case "Back":
                MenuItemSelected(new MenuEventArgs {SelectedItem = "Cancel"});

                break;
        }
        Menu.HandleInput(inputEvent);
    }

    void MenuItemShifted(MenuEventArgs args)
    {
        if (args.SelectedIndex == 0)
        {
            ChangeSelectedLetter(args.ShiftAmount);
        }
    }

    private void ChangeSelectedLetter(int delta)
    {
        SelectedLetterIndex = Helpers.Wrap(SelectedLetterIndex + delta, _letters.Length - 1);
        Refresh();
    }

    private void Refresh()
    {
        TxtEnteredText.text = EnteredText + "_";
        TxtCurrentLetter.text = "" + CurrentLetter;
    }

    void MenuItemSelected(MenuEventArgs args)
    {
        switch (args.SelectedItem)
        {
            case "Backspace":
                if (EnteredText.Length > 0)
                {
                    EnteredText = EnteredText.Remove(EnteredText.Length - 1);
                    Refresh();
                }
                break;
            case "Cancel":
                Parent.SfxSelectionCancelled.PlayUnlessNull();
                Parent.State = PlayerJoinState.ProfileSelect;
                break;
            case "Confirm":
                CreateProfile();
                break;
            default:
                AddLetter(args);
                break;
        }
    }

    private void AddLetter(MenuEventArgs args)
    {
        if (args.SelectedIndex == 0 && EnteredText.Length < MaxLength)
        {
            EnteredText += CurrentLetter;
            Refresh();
        }
    }

    private void CreateProfile()
    {
        if (string.IsNullOrWhiteSpace(EnteredText))
        {
            this.Error = "The profile name cannot be blank.";
            Parent.SfxMistake.PlayUnlessNull();
            return;
        }
        if (_profileManager.ContainsName(EnteredText))
        {
            this.Error = "This profile name is invalid, or already exists.";
            Parent.SfxMistake.PlayUnlessNull();
            return;
        }

        this.Error = null;
        Parent.SfxSelectionConfirmed.PlayUnlessNull();
        var newProfile = _profileManager.Create(EnteredText);
        _profileManager.Save(newProfile);
        Parent.ProfileSelectFrame.PopulateProfileList();
        Parent.State = PlayerJoinState.ProfileSelect;
    }
}
