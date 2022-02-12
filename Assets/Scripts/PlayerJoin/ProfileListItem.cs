using UnityEngine;
using UnityEngine.UI;

public class ProfileListItem : MonoBehaviour
{

    private Text _txtName;
    private MenuItem _menuItem;

    [SerializeField]
    private ProfileData _profileData;
    public ProfileData ProfileData
    {
        get { return _profileData; }
        set
        {
            _profileData = value;
            DisplayName();
        }
    }

    private void DisplayName()
    {
        string nameText;
        if (ProfileData?.ID == null)
        {
            nameText = "[Guest]";
        }
        else if (ProfileData.ID == "##NEW##")
        {
            nameText = "[Create New]";
        }
        else
        {
            nameText = ProfileData.Name;
        }

        _txtName.text = nameText;
        _menuItem.Text = nameText;
    }

    void Awake()
    {
        _txtName = GetComponent<Text>();
        _menuItem = GetComponent<MenuItem>();
    }

}
