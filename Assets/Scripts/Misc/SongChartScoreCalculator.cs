using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class SongChartScoreCalculator : MonoBehaviour
{
    public NoteGenerator NoteGenerator;

    public Dictionary<TeamScoreCategory, double[]> Percentages = new Dictionary<TeamScoreCategory, double[]>
    {
        {TeamScoreCategory.Solo, new double[] {0.5, 0.45, 0.7, 0.8, 0.85}},
        {TeamScoreCategory.Duet, new double[] {0.45, 0.4, 0.65, 0.75, 0.8}},
        {TeamScoreCategory.Crowd, new double[] {0.4, 0.35, 0.6, 0.7, 0.75}},
        {TeamScoreCategory.Legion, new double[] {0.4, 0.35, 0.6, 0.7, 0.75}}
    };

    public Difficulty[] StarDifficulties = new[]
    {
        Difficulty.Beginner, Difficulty.Medium, Difficulty.Medium, Difficulty.Hard, Difficulty.Expert
    };

    public Dictionary<TeamScoreCategory, int> CategoryPlayerCounts = new Dictionary<TeamScoreCategory, int>()
    {
        {TeamScoreCategory.Solo, 1},
        {TeamScoreCategory.Duet, 2},
        {TeamScoreCategory.Crowd, 4},
        {TeamScoreCategory.Legion, 6}
    };

    public int StarCount = 5;
    public int NoteBaseValue = 50;


    public List<SongStarScoreValues> CalculateSuggestedScores(SongData songData)
    {
        var result = new List<SongStarScoreValues>();

        foreach (var key in Percentages.Keys)
        {
            result.Add(CalculateSuggestedScores(songData, key));
        }
        return result;
    }

    public SongStarScoreValues CalculateSuggestedScores(SongData songData, TeamScoreCategory category)
    {
        var values = new List<long>();
        for (int x = 0; x < StarCount; x++)
        {
            
            var chart = GetSongChart(songData, StarDifficulties[x]);
            var players = CategoryPlayerCounts[category];
            var maxScore = CalculateMaxScore(chart, songData.LengthInBeats, songData.Bpm, players);
            var percent = Percentages[category][x];

            // Target Score = Maximum theoretical score * Percent * number of players, rounded down to the nearest thousand.
            var targetScore = maxScore * percent;
         

            // Round down to nearest 1000
            targetScore /= 1000;
            targetScore = Math.Floor(targetScore);
            targetScore *= 1000;

            values.Add((long) targetScore);
        }

        return new SongStarScoreValues
        {
            ScoreCategory = category,
            Scores = values.ToArray()
        };
    }

    public long CalculateMaxScore(SongChart chart, float songEndBeat, float songBpm, int players)
    {
        double mx = 1.0;
        double maxMx = 1.0;
        long result = 0;
        double lastNoteTime = 0.0;
        float mxPerNote = HitJudge.JudgeMxValues[JudgeResult.Perfect];
        List<Note> notes = new List<Note>();
        int noteCount = 0;

        if (chart.Notes.Any())
        {
            NoteGenerator.LoadNoteArray(chart.Notes, ref notes);
        }
        else
        {
            NoteGenerator.GenerateNotes(chart.Difficulty, (int) songEndBeat,notes);
        }

        NoteUtils.CalculateAbsoluteTimes(notes, songBpm);

        foreach (var note in notes.OrderBy(e => e.AbsoluteTime))
        {
            double value = (this.NoteBaseValue * NoteUtils.GetNoteValue(note.NoteType, note.NoteClass));
            value *=  mx;
            value *= players;
            result += (long) value;

            mx += mxPerNote * players;
            maxMx = Math.Max(maxMx, mx);

            // Move to Next note
            var timeDiff = note.AbsoluteTime - lastNoteTime;
            mx = GameplayUtils.DecayMultiplier(mx, timeDiff);

            lastNoteTime = note.AbsoluteTime;
            noteCount++;
        }

        Debug.Log($"CalculateMaxScore result: Chart: {chart.Difficulty}, Notes: {noteCount}, Players: {players}, Score: {result}, Ending Mx: {mx:F3}, Max Mx: {maxMx:F3}");
        return result;
    }

    private SongChart GetSongChart(SongData songData, Difficulty diff)
    {
        var mainChart = songData.SongCharts.SingleOrDefault(e => e.Group == "Main" && e.Difficulty == diff);
        var firstChart = songData.SongCharts.FirstOrDefault(e => e.Difficulty == diff);

        return mainChart ?? firstChart;
    }
}
