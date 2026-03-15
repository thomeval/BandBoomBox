using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerJoinRivalSelectFrame : MonoBehaviour
{
    public PlayerJoinFrame Parent;
    public Menu ProfilesMenu;
    public ProfileListItem ProfileListItemPrefab;
    public Text TxtMessage;

    private ProfileManager _profileManager;

    public string DefaultMessage = "Select a rival from the list above, or select [No Rival] to play without one.";
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
            Parent.State = PlayerState.PlayerJoin_Options;
            Parent.RemovePlayer();
            return;
        }
        else
        {
            Parent.TrySetRivalToPlayer(SelectedProfile);
        }
    }

    private void AddSpecialItemsToMenu()
    {
        var guest = _profileManager.GuestProfile;
        AddToMenu(guest);

    }
    public void AddToMenu(ProfileData profile)
    {
        var obj = Instantiate(ProfileListItemPrefab);
        obj.gameObject.name = "Rival: " + profile.Name;
        obj.ProfileData = profile;
        ProfilesMenu.AddItem(obj.gameObject);
    }

    public void HandleInput(InputEvent inputEvent)
    {
        if (inputEvent.Action == InputAction.Y)
        {
            Parent.TrySetRivalToPlayer(_profileManager.GuestProfile);
            return;
        }
        ProfilesMenu.HandleInput(inputEvent);
    }

    public void PopulateProfileList()
    {
        if (_profileManager == null)
        {
            _profileManager = FindObjectOfType<ProfileManager>();
        }
        ProfilesMenu.ClearItems();
        foreach (var profile in _profileManager.Profiles.OrderByDescending(e => e.LastPlayed))
        {
            AddToMenu(profile);
        }

        AddSpecialItemsToMenu();
    }


    public void Refresh()
    {
        ProfilesMenu.FullRefresh();
    }
}
