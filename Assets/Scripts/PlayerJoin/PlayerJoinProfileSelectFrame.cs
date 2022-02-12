using UnityEngine;
using UnityEngine.UI;

public class PlayerJoinProfileSelectFrame : MonoBehaviour
{
    public PlayerJoinFrame Parent;
    public Menu ProfilesMenu;
    public ProfileListItem ProfileListItemPrefab;
    public Text TxtMessage;

    private ProfileManager _profileManager;

    public string DefaultMessage = "Select a profile from the list above, or select [Guest] to play without one.";
    public string Error
    {
        get { return TxtMessage.text == DefaultMessage ? null : TxtMessage.text; }
        set
        {
            var isError = !string.IsNullOrEmpty(value);

            if (isError)
            {
                TxtMessage.text = value;
                TxtMessage.color = Parent.ErrorMessageColor;
            }
            else
            {
                TxtMessage.text = DefaultMessage;
                TxtMessage.color = Color.white;
            }
        }
    }

    void OnEnable()
    {
        ProfilesMenu.RefreshHighlight();
        Error = null;
    }
    public ProfileData SelectedProfile
    {
        get
        {
            var selectObj = ProfilesMenu.SelectedGameObject.GetComponent<ProfileListItem>();
            return selectObj.ProfileData;
        }
    }
    void Awake()
    {
        _profileManager = FindObjectOfType<ProfileManager>();
    }
    void MenuItemSelected(MenuEventArgs args)
    {
        if (args.SelectedItem == "##CANCEL##")
        {
            Parent.State = PlayerJoinState.NotJoined;
            Parent.RemovePlayer();
            Parent.SfxSelectionCancelled.PlayUnlessNull();
            return;
        }

        if (SelectedProfile.ID == "##NEW##")
        {
            Parent.ProfileCreateFrame.EnteredText = "";
            Parent.State = PlayerJoinState.ProfileCreate;
            Parent.SfxSelectionConfirmed.PlayUnlessNull();
        }
        else
        {
            Parent.TrySetProfileToPlayer(SelectedProfile);
        }
    }

    private void AddSpecialItemsToMenu()
    {
        var guest = _profileManager.GuestProfile;
        AddToMenu(guest);

        var newProfile = new ProfileData {ID = "##NEW##"};
        AddToMenu(newProfile);
    }
    public void AddToMenu(ProfileData profile)
    {
        var obj = Instantiate(ProfileListItemPrefab);
        obj.gameObject.name = "Profile: " + profile.Name;
        obj.ProfileData = profile;
        ProfilesMenu.AddItem(obj.gameObject);
    }

    public void HandleInput(InputEvent inputEvent)
    {
        ProfilesMenu.HandleInput(inputEvent);
    }

    public void PopulateProfileList()
    {
        if (_profileManager == null)
        {
            _profileManager = FindObjectOfType<ProfileManager>();
        }
        ProfilesMenu.ClearItems();
        foreach (var profile in _profileManager.Profiles)
        {
            AddToMenu(profile);
        }

        AddSpecialItemsToMenu();
        HighlightProfile(Parent.Player.Name);
    }

    private void HighlightProfile(string playerName)
    {
        ProfilesMenu.HighlightMenuItem("Profile: " + playerName);
    }

    public void Refresh()
    {
        ProfilesMenu.FullRefresh();
    }
}
