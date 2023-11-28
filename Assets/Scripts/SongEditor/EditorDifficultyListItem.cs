using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorDifficultyListItem : MonoBehaviour
{

    public InputField TxtGroup;
    public InputField TxtDifficultyLevel;
    public Dropdown CmbDifficulty;
    public Text TxtIsBlank;
    public Button BtnRemove;
    public Button BtnEdit;
    public Button BtnClone;

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
        CmbDifficulty.SetSelectedText(DisplayedChart.Difficulty.GetDisplayName());
        TxtIsBlank.text = DisplayedChart.IsBlank ? "B" : "";
    }

    void Awake()
    {
        BtnRemove.onClick.AddListener(BtnRemove_OnClick);
        BtnEdit.onClick.AddListener(BtnEdit_OnClick);
        BtnClone.onClick.AddListener(BtnClone_OnClick);
        //   TxtGroup.onValueChanged.AddListener(arg0 => ApplyChartData());
        //   TxtDifficultyLevel.onValueChanged.AddListener(arg0 => ApplyChartData());
        //   CmbDifficulty.onValueChanged.AddListener(arg0 => ApplyChartData());
    }

    private void BtnRemove_OnClick()
    {
        SendMessageUpwards("ChartListItemRemoved", this);
    }

    private void BtnEdit_OnClick()
    {
        SendMessageUpwards("ChartListItemEdited", this);
    }

    private void BtnClone_OnClick()
    {
        SendMessageUpwards("ChartListItemCloned", this);
    }

    public void ApplyChartData()
    {
        DisplayedChart.Group = TxtGroup.text;
        DisplayedChart.DifficultyLevel = Convert.ToInt32(TxtDifficultyLevel.text);
        DisplayedChart.Difficulty = Helpers.GetDifficultyByDisplayName(CmbDifficulty.GetSelectedText());
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
