using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class EditorSectionListItem : MonoBehaviour
{
    public InputField TxtBeat;
    public InputField TxtSectionName;
    public Button BtnRemove;

    [SerializeField]
    private KeyValuePair<double, string> _displayedSection;

    public KeyValuePair<double, string> DisplayedSection
    {
        get { return _displayedSection; }
        set
        {
            _displayedSection = value;
            DisplayCurrentSection();
        }
    }

    private void DisplayCurrentSection()
    {
        TxtBeat.text = _displayedSection.Key.ToString(CultureInfo.InvariantCulture);
        TxtSectionName.text =   _displayedSection.Value;
    }

    void Awake()
    {
        BtnRemove.onClick.AddListener(BtnRemove_OnClick);
    }

    private void BtnRemove_OnClick()
    {
        SendMessageUpwards("SectionListItemRemoved", this);
    }

}
