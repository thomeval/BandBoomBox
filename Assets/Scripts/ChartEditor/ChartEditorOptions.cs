using UnityEngine;
using UnityEngine.UI;

public class ChartEditorOptions : MonoBehaviour
{
    public string NoteSkin = "Default";
    public string LabelSkin = "WASD";
    public bool AllowAllNotes = false;
    public bool AutoStepForward = false;
    public bool NoteClapEnabled = false;
    public float NoteClapLatency = 0.0f;
    public int AutoSaveIntervalMinutes = 10;
    
    private int[] _autoSaveIntervalOptions = new int[] { 0, 5, 10, 15, 20 };
    [Header("Text")] 
    public Text TxtLabelSkin;
    public Text TxtAllowAllNotes;
    public Text TxtAutoStepForward;
    public Text TxtAutoSaveIntervalMinutes;
    public Text TxtNoteClapEnabled;
    public Text TxtNoteClapLatency;

    private ChartEditorManager _parent;
    private void Awake()
    {
        Helpers.AutoAssign(ref _parent);
    }

    public void SetOptionsItemText()
    {
        TxtLabelSkin.text = LabelSkin;
        TxtAllowAllNotes.text = AllowAllNotes ? "On" : "Off";
        TxtAutoStepForward.text = AutoStepForward ? "On" : "Off";
        TxtNoteClapEnabled.text = NoteClapEnabled ? "On" : "Off";
        TxtNoteClapLatency.text = $"{NoteClapLatency*1000:F0}" + " ms";
        TxtAutoSaveIntervalMinutes.text = AutoSaveIntervalMinutes == 0 ? "Off" : AutoSaveIntervalMinutes + " minutes";
        
    }

    public void ChangeLabelSkin(int delta)
    {
        var newValue = Helpers.GetNextValue(Player.LabelSkins, LabelSkin, delta, true);
        LabelSkin = newValue;
        SetOptionsItemText();
    }

    public void ChangeAutoSaveInterval(int delta)
    {      
        var newValue = Helpers.GetNextValue(_autoSaveIntervalOptions, AutoSaveIntervalMinutes, delta, false);
        AutoSaveIntervalMinutes = newValue;
        SetOptionsItemText();
    }

    public void ChangeNoteClapLatency(int delta)
    {
        NoteClapLatency += delta * 0.005f;
        NoteClapLatency = Mathf.Clamp(NoteClapLatency, -0.25f, 0.25f);
        SetOptionsItemText();
    }

    public void Load(SettingsManager manager)
    {
        this.AllowAllNotes = manager.EditorAllowAllNotes;
        this.AutoStepForward = manager.EditorAutoStepForward;
        this.NoteClapEnabled = manager.EditorNoteClapEnabled;
        this.NoteClapLatency = manager.EditorNoteClapLatency;
        this.LabelSkin = manager.EditorLastUsedNoteLabels;
        this.AutoSaveIntervalMinutes = manager.EditorAutoSaveIntervalMinutes;
        _parent.NoteManager.ScrollSpeed = manager.EditorScrollSpeed;
    }

    public void Save(SettingsManager manager)
    {
        manager.EditorAllowAllNotes = this.AllowAllNotes;
        manager.EditorAutoStepForward = this.AutoStepForward;
        manager.EditorNoteClapEnabled = this.NoteClapEnabled;
        manager.EditorNoteClapLatency = this.NoteClapLatency;
        manager.EditorLastUsedNoteLabels = this.LabelSkin;
        manager.EditorAutoSaveIntervalMinutes = this.AutoSaveIntervalMinutes;
        manager.EditorScrollSpeed = _parent.NoteManager.ScrollSpeed;

        manager.Save();
    }
}

