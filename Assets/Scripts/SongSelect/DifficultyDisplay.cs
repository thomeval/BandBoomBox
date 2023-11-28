using System.Linq;
using UnityEngine;

public class DifficultyDisplay : MonoBehaviour
{
    private SettingsManager _settingsManager;

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
        Helpers.AutoAssign(ref _settingsManager);

        HideAll();

        var x = 0;

        var ranges = SongData.GetDifficultyRanges();
        foreach (var range in ranges.Where(e => _settingsManager.IsDifficultyVisible(e.Difficulty)))
        {
            DisplayItems[x].DisplayDifficulty(range);
            x++;
        }
    }

}
