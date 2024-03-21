using System;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class SongChart
{
    public string Group;
    public Difficulty Difficulty;
    public int DifficultyLevel;
    public SongChartNoteCounts NoteCounts = new();

    [HideInInspector]
    public string[] Notes;

    [JsonIgnore]
    public bool IsBlank
    {
        get
        {
            return Notes == null || Notes.Length == 0 || Notes.All(e => e == "0000");
        }
    }
    public override string ToString()
    {
        return string.Format("{0} - {1}({2})", Group, Difficulty, DifficultyLevel);
    }

    public SongChart Clone()
    {

        var result = new SongChart
        {
            Difficulty = this.Difficulty,
            Group = this.Group,
            DifficultyLevel = this.DifficultyLevel,
        };

        if (Notes != null)
        {
            var newNotes = new string[Notes.Length];

            for (int x = 0; x < Notes.Length; x++)
            {
                newNotes[x] = this.Notes[x];
            }

            result.Notes = newNotes;
        }

        return result;
    }
}
