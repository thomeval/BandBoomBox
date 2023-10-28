
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
    public string Issues;
    public string Url;

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

    public DifficultyRange GetDifficultyRange(Difficulty diff)
    {
        var result = new DifficultyRange
        {
            Difficulty = diff
        };

        var charts = SongCharts.Where(e => e.Difficulty == diff).ToList();

        if (!charts.Any())
        {
            return result;
        }

        result.Min = charts.Min(e => e.DifficultyLevel);
        result.Max = charts.Max(e => e.DifficultyLevel);
        return result;

    }

    public List<DifficultyRange> GetDifficultyRanges()
    {
        var result = new List<DifficultyRange>();

        foreach (var diff in Enum.GetValues(typeof(Difficulty)).Cast<Difficulty>())
        {
            var range = GetDifficultyRange(diff);
            if (range.IsEmpty)
            {
                continue;
            }

            result.Add(range);
        }

        result = result.OrderBy(e => e.Min).ThenBy(e => e.Max).ThenBy(e => e.Difficulty).ToList();
        return result;
    }

    public override string ToString()
    {
        return $"{Artist} - {Title} {Subtitle}";
    }

    public KeyValuePair<double, string> GetSection(double position)
    {
        if (!this.Sections.Any(e => e.Key <= position))
        {
            return default;
        }

        var currentSection = this.Sections.Last(e => e.Key <= position);
        return currentSection;   
    }

    public string GetSectionName(double position)
    {
        return GetSection(position).Value;
    }
}