using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NoteAutoFiller : MonoBehaviour
{
    public Note NotePrefab;
    private ChartEditorManager _parent;

    void Awake()
    {
        Helpers.AutoAssign(ref _parent);
    }

    public List<NoteAutoFillProviderBase> Providers = new()
    {
        new BeginnerTwoBeatProvider()
    };

    public void AutoFill(string providerName)
    {

        var provider = Providers.SingleOrDefault(e => e.DisplayName == providerName);

        if (provider == null)
        {
            throw new ArgumentException("Unrecognised AutoFill Provider name: " + providerName);
        }

        if (!_parent.RegionSelected)
        {
            return;
        }

        _parent.NoteTransformer.ClearRegion();

        var notes = provider.Generate(_parent.SelectedRegionStart.Value, _parent.SelectedRegionEnd.Value);

        foreach (var note in notes)
        {
            var obj = _parent.NoteGenerator.InstantiateNote(note);
            _parent.NoteManager.AttachNote(obj);
        }

        _parent.NoteManager.CalculateAbsoluteTimes(_parent.CurrentSongData.Bpm);
        _parent.RefreshNoteCounts();
    }
}