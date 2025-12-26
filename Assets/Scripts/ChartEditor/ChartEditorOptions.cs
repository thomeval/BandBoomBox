using System;
using UnityEngine;
using UnityEngine.UI;

public class ChartEditorOptions : MonoBehaviour
{
    public string NoteSkin = "Default";
    public string LabelSkin = "WASD";
    public bool AllowAllNotes = false;
    public bool AutoStepForward = false;
    public int AutoSaveIntervalMinutes = 10;
    
    private int[] _autoSaveIntervalOptions = new int[] { 0, 5, 10, 15, 20 };
    [Header("Text")] 
    public Text TxtLabelSkin;
    public Text TxtAllowAllNotes;
    public Text TxtAutoStepForward;
    public Text TxtAutoSaveIntervalMinutes;

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

    public void Load(SettingsManager manager)
    {
        this.AllowAllNotes = manager.EditorAllowAllNotes;
        this.AutoStepForward = manager.EditorAutoStepForward;
        this.LabelSkin = manager.EditorLastUsedNoteLabels;
        this.AutoSaveIntervalMinutes = manager.EditorAutoSaveIntervalMinutes;
        _parent.NoteManager.ScrollSpeed = manager.EditorScrollSpeed;
    }

    public void Save(SettingsManager manager)
    {
        manager.EditorAllowAllNotes = this.AllowAllNotes;
        manager.EditorAutoStepForward = this.AutoStepForward;
        manager.EditorLastUsedNoteLabels = this.LabelSkin;
        manager.EditorAutoSaveIntervalMinutes = this.AutoSaveIntervalMinutes;
        manager.EditorScrollSpeed = _parent.NoteManager.ScrollSpeed;
        manager.Save();
    }
}

