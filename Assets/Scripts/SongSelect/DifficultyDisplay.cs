using Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DifficultyDisplay : MonoBehaviour
{

    public DifficultyDisplayItem[] DisplayItems = new DifficultyDisplayItem[5];

    [SerializeField]
    private SongData _songData;

    public SongData SongData 
    {
        get { return _songData; }
        set
        {
            _songData = value;
            ShowDifficulties();
        }
    }

    public void HideAll()
    {
        foreach (var displayItem in DisplayItems)
        {
            displayItem.Hide();
        }
    }
    private void ShowDifficulties()
    {
        HideAll();

        var x = 0;

        var ranges = SongData.GetDifficultyRanges();
        foreach (var range in ranges)
        {
            DisplayItems[x].DisplayDifficulty(range);
            x++;
        }
    }

}
