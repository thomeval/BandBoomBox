using UnityEngine;
using UnityEngine.InputSystem;
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
    public MenuItem MitUpperLowerCase;

    public int MaxLength = 12;

    private string _letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_-";
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
            case InputAction.B:
                if (Menu.SelectedIndex == 0)
                {
                    MenuItemSelected(new MenuEventArgs { SelectedItem = "Backspace" });
                    Menu.PlaySound("SelectionCancelled");
                }
                return;
            case InputAction.Back:
                MenuItemSelected(new MenuEventArgs { SelectedItem = "Cancel" });

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
            case "Lowercase":
                _letters = _letters.ToLowerInvariant();
                MitUpperLowerCase.SetText("Uppercase");
                ChangeSelectedLetter(0);
                break;
            case "Uppercase":
                _letters = _letters.ToUpperInvariant();
                MitUpperLowerCase.SetText("Lowercase");
                ChangeSelectedLetter(0);
                break;
            case "Space":
                AddLetter(' ');
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
        if (args.SelectedIndex == 0)
        {
            AddLetter(CurrentLetter);
        }
    }

    private void AddLetter(char letter)
    {
        if (EnteredText.Length < MaxLength)
        {
            EnteredText += letter;
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
