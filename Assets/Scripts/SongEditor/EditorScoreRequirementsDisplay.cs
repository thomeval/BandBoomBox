using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EditorScoreRequirementsDisplay : MonoBehaviour
{
    public List<SongStarScoreValues> DefaultScoreValues
    {
        get
        {
            return new List<SongStarScoreValues>
            {
                new SongStarScoreValues{ScoreCategory = TeamScoreCategory.Solo, Scores = new long[] {10000,30000,60000,100000, 150000}},
                new SongStarScoreValues{ScoreCategory = TeamScoreCategory.Duet, Scores = new long[] {20000,60000,120000,200000, 300000}},
                new SongStarScoreValues{ScoreCategory = TeamScoreCategory.Crowd, Scores = new long[] {40000,120000,240000,400000, 600000}},
                new SongStarScoreValues{ScoreCategory = TeamScoreCategory.Legion, Scores = new long[] {60000,180000,360000,600000, 900000}}
            };
        }
    }

    public EditorStarScoresColumn[] Columns;
    public SongChartScoreCalculator SongChartScoreCalculator;

    public Text LblCalculating;
    public EditorChartListPage Parent;

    public void Display(SongData songData)
    {
        var values = songData.SongStarScoreValues;
        Display(values);
    }

    public void Display(List<SongStarScoreValues> values)
    {
        var length = Math.Min(Columns.Length, values.Count);
        this.Clear();

        for (int x = 0; x < length; x++)
        {
            this.Columns[x].ScoreValues = values[x];
        }
    }

    public List<SongStarScoreValues> GetSongStarScoreValues()
    {
        return Columns.Select(e => e.ScoreValues).ToList();
    }
    private void Clear()
    {
        foreach (var column in Columns)
        {
            column.Clear();
        }
    }

    public void AutoCalculate()
    {
        StartCoroutine(AutoCalculateCoroutine());
    }

    private IEnumerator AutoCalculateCoroutine()
    {
        // TODO: Coroutines are not Tasks / Threads. This code still runs on the Main thread. Refactor to use multi-threading instead.
        LblCalculating.gameObject.SetActive(true);
        yield return null; // Pause execution until next frame.
        var result = SongChartScoreCalculator.CalculateSuggestedScores(Parent.Parent.CurrentSong);
        Display(result);
        LblCalculating.gameObject.SetActive(false);
        yield return null;
    }

    public string Validate()
    {

        var results = Columns.Select(e => e.Validate()).Where(e => !string.IsNullOrEmpty(e)).ToList();

        if (!results.Any())
        {
            return null;
        }

        var result = results.Aggregate((cur, next) => cur + "\r\n" + next);
        result = result.Trim();

        return result;

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

}
