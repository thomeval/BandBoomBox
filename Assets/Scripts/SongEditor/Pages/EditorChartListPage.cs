using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EditorChartListPage : EditorPageManager
{
    public EditorDifficultyListItem ListItemPrefab;

    public Button BtnBack;
    public Button BtnNext;
    public Button BtnAdd;
    public Text TxtErrorCharts;

    public List<EditorDifficultyListItem> DisplayedCharts = new List<EditorDifficultyListItem>();

    public GameObject ListItemContainer;


    public override EditorPage EditorPage
    {
        get { return EditorPage.ChartList; }
    }
    public SongChart DefaultNewChart
    {
        get
        {
            return new SongChart
            {
                DifficultyLevel = 1,
                Group = "Main",
                Difficulty = Difficulty.Beginner
            };
        }
    }

    void Awake()
    {
        BtnBack.onClick.AddListener(BtnBack_OnClick);
        BtnNext.onClick.AddListener(BtnNext_OnClick);
        BtnAdd.onClick.AddListener(BtnAdd_OnClick);
    }

    private void OnEnable()
    {
        PopulateList();
    }

    #region Event Handlers
    private void BtnBack_OnClick()
    {
        Parent.CurrentPage = EditorPage.Details;
    }
    private void BtnNext_OnClick()
    {
        if (!ApplyAndValidate())
        {
            return;
        }

        SaveCharts();
        Parent.CurrentPage = EditorPage.MainMenu;
    }

    private void SaveCharts()
    {
        var newCharts = DisplayedCharts.Select(e => e.DisplayedChart).ToList();
        Parent.CurrentSong.SongCharts = newCharts;

        Parent.SaveCurrentSong(false);
    }

    private bool ApplyAndValidate()
    {
        foreach (var item in DisplayedCharts)
        {
            item.ApplyChartData();
        }

        return ValidateCharts();
    }

    private bool ValidateCharts()
    {
        var error = "";
        
        if (!DisplayedCharts.Any())
        {
            error += "At least one chart is required for the song to be playable.;";
        }

        foreach (var chart in DisplayedCharts)
        {
            var chartError = chart.ValidateChart();

            if (chartError == "")
            {
                continue;
            }

            error += $"{chartError} ({chart.DisplayedChart.Difficulty});";
        }

        var duplicate = DisplayedCharts.Select(e => e.DisplayedChart)
            .GroupBy(e => e.Group + " " + e.Difficulty).FirstOrDefault(e => e.Count() > 1);

        if (duplicate != null)
        {
            error += $"Multiple charts exist for '{duplicate.Key}'. Duplicates are not allowed.;";
        }

        error = error.TrimEnd(';');
        error = error.Replace(";", "\r\n");
        
        TxtErrorCharts.text = error;
        return error == "";
    }

    private void BtnAdd_OnClick()
    {
        Parent.CurrentSong.SongCharts.Add(DefaultNewChart);
        PopulateList();

    }

    void ChartListItemRemoved(EditorDifficultyListItem item)
    {
        DisplayedCharts.Remove(item);
        Parent.CurrentSong.SongCharts.Remove(item.DisplayedChart);
        Destroy(item.gameObject);
    }

    void ChartListItemEdited(EditorDifficultyListItem item)
    {
        if (!ApplyAndValidate())
        {
            return;
        }

        SaveCharts();

        var args = new Dictionary<string, object>()
        {
            { "SelectedSongData", Parent.CurrentSong }, { "SelectedSongChart", item.DisplayedChart }
        };
        Parent.RaiseSceneTransition(GameScene.ChartEditor, args);
    }

    void ChartListItemCloned(EditorDifficultyListItem item)
    {
        var chart = item.DisplayedChart.Clone();
        Parent.CurrentSong.SongCharts.Add(chart);
        PopulateList();
    }

    #endregion
    public void PopulateList()
    {
        ListItemContainer.gameObject.ClearChildren();
        DisplayedCharts.Clear();

        foreach (var chart in Parent.CurrentSong.SongCharts.OrderBy(e => e.Group).ThenBy(e => e.DifficultyLevel))
        {
            var obj = Instantiate(ListItemPrefab);
            obj.DisplayedChart = chart;
            obj.gameObject.name = $"{chart.Difficulty} - {chart.DifficultyLevel}";
            ListItemContainer.AddChild(obj.gameObject);
            DisplayedCharts.Add(obj);
        }
    }
}

