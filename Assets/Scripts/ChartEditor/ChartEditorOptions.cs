using UnityEngine;
using UnityEngine.UI;

public class ChartEditorOptions : MonoBehaviour
{
    public string NoteSkin = "Default";
    public string LabelSkin = "WASD";
    public bool AllowAllNotes = false;
    public bool AutoStepForward = false;

    [Header("Text")] 
    public Text TxtLabelSkin;
    public Text TxtAllowAllNotes;
    public Text TxtAutoStepForward;

    public void SetOptionsItemText()
    {
        TxtLabelSkin.text = LabelSkin;
        TxtAllowAllNotes.text = AllowAllNotes ? "On" : "Off";
        TxtAutoStepForward.text = AutoStepForward ? "On" : "Off";
    }

    public void ChangeLabelSkin(int delta)
    {
        var newValue = Helpers.GetNextValue(Player.LabelSkins, LabelSkin, delta, true);
        LabelSkin = newValue;
        SetOptionsItemText();
    }
}

