using System;
using UnityEngine;
using UnityEngine.UI;

public class EditorDifficultyListItem : MonoBehaviour
{

    public InputField TxtGroup;
    public InputField TxtDifficultyLevel;
    public Dropdown CmbDifficulty;
    public Button BtnRemove;

    [SerializeField]
    private SongChart _displayedChart;

    public SongChart DisplayedChart
    {
        get { return _displayedChart; }
        set
        {
            _displayedChart = value;
            DisplayCurrentChart();
        }
    }

    private void DisplayCurrentChart()
    {
        TxtDifficultyLevel.text = "" + DisplayedChart.DifficultyLevel;
        TxtGroup.text = DisplayedChart.Group;
        CmbDifficulty.SetSelectedText(DisplayedChart.Difficulty.ToString());
    }

    void Awake()
    {
        BtnRemove.onClick.AddListener(BtnRemove_OnClick);
     //   TxtGroup.onValueChanged.AddListener(arg0 => ApplyChartData());
     //   TxtDifficultyLevel.onValueChanged.AddListener(arg0 => ApplyChartData());
     //   CmbDifficulty.onValueChanged.AddListener(arg0 => ApplyChartData());
    }

    private void BtnRemove_OnClick()
    {
        SendMessageUpwards("ChartListItemRemoved", this);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ApplyChartData()
    {
        DisplayedChart.Group = TxtGroup.text;
        DisplayedChart.DifficultyLevel = Convert.ToInt32(TxtDifficultyLevel.text);
        Enum.TryParse(CmbDifficulty.GetSelectedText(), out DisplayedChart.Difficulty);
    }

    public string ValidateChart()
    {
        if (string.IsNullOrWhiteSpace(DisplayedChart.Group))
        {
            DisplayedChart.Group = "Main";
        }

        var result = "";

        if (DisplayedChart.DifficultyLevel < 1)
        {
            result += "Difficulty Level must be at least 1.;";
        }

        return result;
    }
}
