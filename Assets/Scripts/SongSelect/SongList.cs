using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SongList : MonoBehaviour
{

    private SongSelectManager _songSelectManager;
    private readonly List<SongListItem> _songListItems = new();

    [SerializeField]
    private float _yOffset;

    private const float Y_OFFSET_DECAY_RATE = 0.9f;

    public GameObject InnerContainer;
    public GameObject SongListItemPrefab;
    public GameObject GlowPrefab;

    public int ListItemHeight = 50;
    public int DisplayedAdjacentSongs = 8;

    void Awake()
    {
        _songSelectManager = FindObjectOfType<SongSelectManager>();
        GenerateSongListItems();
    }

    void FixedUpdate()
    {
        if (_yOffset == 0.0f)
        {
            return;

        }

        _yOffset *= Y_OFFSET_DECAY_RATE;
        if (Mathf.Abs(_yOffset) < 1.0f)
        {
            _yOffset = 0;
        }

        InnerContainer.transform.localPosition = new Vector3(0, _yOffset, 0);
    }

    public void MoveSelection(int amount)
    {
        _yOffset -= (ListItemHeight * amount);
        InnerContainer.transform.localPosition = new Vector3(0, _yOffset, 0);
        PopulateSongListItems();
    }

    public void GenerateSongListItems()
    {
        InnerContainer.ClearChildren();
        _songListItems.Clear();
        var itemsCount = (2 * DisplayedAdjacentSongs) + 1;

        for (int x = 0; x < itemsCount; x++)
        {
            var obj = Instantiate(SongListItemPrefab);
            obj.transform.SetParent(InnerContainer.transform, false);
            var listItem = obj.GetComponent<SongListItem>();
            _songListItems.Add(listItem);
        }

        var glowObj = Instantiate(GlowPrefab);
        glowObj.transform.SetParent(InnerContainer.transform, false);
    }

    public void PopulateSongListItems()
    {
        var listIdx = 0;
        var songCount = _songSelectManager.OrderedSongs.Count;
        var songIdx = Helpers.Wrap(_songSelectManager.SelectedSongIndex - DisplayedAdjacentSongs, songCount - 1);

        try
        {
            while (listIdx < _songListItems.Count)
            {
                var listItem = _songListItems[listIdx];
                _songListItems[listIdx].SongData = _songSelectManager.OrderedSongs[songIdx];
                var teamScore = _songSelectManager.GetTeamScore(listItem.SongData);
                listItem.DisplayTeamScore(teamScore);
                listItem.IsSelectable = IsSongSelectable(listItem.SongData.ID);
                listIdx++;
                songIdx = Helpers.Wrap(songIdx + 1, songCount - 1);
            }

            _songListItems[DisplayedAdjacentSongs].IsSelected = true;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

    }

    private bool IsSongSelectable(string songId)
    {
        return !_songSelectManager.UnavailableSongs.Any(e => e == songId);
    }
}
