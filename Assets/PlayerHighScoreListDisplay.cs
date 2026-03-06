using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class PlayerHighScoreListDisplay : MonoBehaviour
{
    public int MaxEntries = 15;

    public GameObject EntryContainer;
    public GameObject HighScoreEntryPrefab;
    public AutoScroller ListAutoScroller;

    private List<PlayerHighScoreListEntry> _entries = new List<PlayerHighScoreListEntry>();

    private ProfileManager _profileManager;
    private PlayerManager _playerManager;

    public void Init()
    {

        Helpers.AutoAssign(ref _profileManager);
        Helpers.AutoAssign(ref _playerManager);

        EntryContainer.ClearChildren();
        _entries.Clear();
        for (int i = 0; i < MaxEntries; i++)
        {
            var entryObj = Instantiate(HighScoreEntryPrefab, EntryContainer.transform);
            var entry = entryObj.GetComponent<PlayerHighScoreListEntry>();
            entry.EntryNumber = i + 1;
            entryObj.SetActive(false);
            _entries.Add(entry);
        }

    }

    public void FetchHighScores(string songId, int version)
    {
        foreach (var entry in _entries)
        {
            entry.gameObject.SetActive(false);
            entry.DisplayedScore = null;
        }

        var scores = _profileManager.GetAllPlayerScores(songId, version);

        var count = Math.Min(scores.Count, MaxEntries);

        for (int x = 0; x < count; x++)
        {
            var entry = _entries[x];
            scores[x].PlayerSlot = _playerManager.GetSlotByProfileId(scores[x].ProfileId);
            entry.DisplayedScore = scores[x];
            entry.gameObject.SetActive(true);
        }

        ListAutoScroller.Reset();

    }
}
