
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;


[Serializable]
public class SongData
{
    [Header("Basics")]
    public string ID;
    public string Title;
    public string Subtitle;
    public string Artist;
    public string ChartAuthor;

    [JsonIgnore] [NonSerialized]
    public string SjsonFilePath;
    public string AudioFile;

    [JsonIgnore] [NonSerialized]
    public string AudioPath;
    public int Version = 1;

    [Header("Timing")]
    public float Bpm;
    public float AudioStart;
    public float Offset;
    public float Length;
    public int BeatsPerMeasure = 4;
    public Dictionary<double, string> Sections = new Dictionary<double, string>();

    [Header("Charts")]
    public List<SongChart> SongCharts;

    public List<SongStarScoreValues> SongStarScoreValues = new List<SongStarScoreValues>();

    [JsonIgnore]
    public float LengthInBeats
    {
        get
        {
            var playableSection = this.Length - this.Offset;
            return playableSection * this.Bpm / 60;
        }
    }


    public SongChart GetChart(string group, Difficulty difficulty)
    {
        return SongCharts.SingleOrDefault(e => e.Group == group && e.Difficulty == difficulty);
    }

    public double GetStarFraction(TeamScoreCategory scoreCategory, long score)
    {
        var scoreValues = SongStarScoreValues.SingleOrDefault(e => e.ScoreCategory == scoreCategory);

        if (scoreValues == null)
        {
            return 0.0;
        }

        return scoreValues.GetStarFraction(score);

    }

    public (int min, int max) GetDifficultyRange(Difficulty diff)
    {
        var charts = SongCharts.Where(e => e.Difficulty == diff).ToList();

        if (!charts.Any())
        {
            return (-1, -1);
        }

        return (charts.Min(e => e.DifficultyLevel), charts.Max(e => e.DifficultyLevel));

    }
}

