using Assets;
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
        for (int x = 0; x < DisplayItems.Length; x++)
        {
            var diff = (Difficulty) x;
            (int min, int max) diffRange = SongData.GetDifficultyRange(diff);
            DisplayItems[x].DisplayDifficulty(diff, diffRange.min, diffRange.max);
        }
        
    }

}
