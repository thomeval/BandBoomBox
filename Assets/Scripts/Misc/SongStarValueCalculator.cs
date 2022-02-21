using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class SongStarValueCalculator : MonoBehaviour
{
    public NoteGenerator NoteGenerator;

    public Dictionary<TeamScoreCategory, double[]> Percentages = new Dictionary<TeamScoreCategory, double[]>
    {
        {TeamScoreCategory.Solo, new double[] {0.05, 0.2, 0.35, 0.55, 0.75}},
        {TeamScoreCategory.Duet, new double[] {0.05, 0.2, 0.35, 0.55, 0.70}},
        {TeamScoreCategory.Crowd, new double[] {0.05, 0.2, 0.35, 0.50, 0.65}},
        {TeamScoreCategory.Legion, new double[] {0.05, 0.2, 0.35, 0.50, 0.65}}
    };

    public int StarCount = 5;
    public int NoteBaseValue = 50;

    public SongStarScoreValues CalculateSuggestedScores(List<SongChart> charts, float songEndBeat, float songBpm)
    {
        var category = HighScoreManager.GetCategory(charts.Count);

        var values = new List<long>();
        for (int x = 0; x < StarCount; x++)
        {

            var maxScore = CalculateMaxScore(charts, songEndBeat, songBpm);
            var percent = Percentages[category][x];

            // Target Score = Maximum theoretical score * Percent
            var targetScore = maxScore * percent;
            targetScore = Math.Floor(targetScore);

            values.Add((long)targetScore);
        }

        return new SongStarScoreValues
        {
            ScoreCategory = category,
            Scores = values.ToArray()
        };
    }

    public long CalculateMaxScore(List<SongChart> charts, float songEndBeat, float songBpm)
    {
        double mx = 1.0;
        double maxMx = 1.0;
        long result = 0;
        double lastNoteTime = 0.0;
        
        List<Note> notes = new List<Note>();
        int noteCount = 0;

        foreach (var chart in charts)
        {
            float mxFromHit = HitJudge.JudgeMxValues[JudgeResult.Perfect] * HitJudge.DifficultyMxValues[chart.Difficulty];

            if (chart.Notes.Any())
            {
                NoteGenerator.LoadNoteArray(chart.Notes, ref notes);
            }
            else
            {
                NoteGenerator.GenerateNotes(chart.Difficulty, (int)songEndBeat, notes);
            }
         
            SetNoteMxValue(notes, mxFromHit);
        }
        NoteUtils.CalculateAbsoluteTimes(notes, songBpm);

        foreach (var note in notes.OrderBy(e => e.AbsoluteTime))
        {
            double value = (this.NoteBaseValue * NoteUtils.GetNoteValue(note.NoteType, note.NoteClass));
            value *=  mx;
            result += (long) value;

            // TODO: Consider implementing combo gain rate bonuses, and a "mistake interval".
            mx += note.MxValue;
            maxMx = Math.Max(maxMx, mx);

            // Move to Next note
            var timeDiff = note.AbsoluteTime - lastNoteTime;
            mx = GameplayUtils.DecayMultiplier(mx, timeDiff);

            lastNoteTime = note.AbsoluteTime;
            noteCount++;
        }

        Debug.Log($"CalculateMaxScore result: Charts:{charts.Count}, Notes: {noteCount}, Score: {result}, Ending Mx: {mx:F3}, Max Mx: {maxMx:F3}");
        return result;
    }

    private void SetNoteMxValue(List<Note> notes, float mxFromHit)
    {
        foreach (var note in notes.Where(e => e.MxValue == 0).ToList())
        {
            note.MxValue = mxFromHit;
        }
    }
}
