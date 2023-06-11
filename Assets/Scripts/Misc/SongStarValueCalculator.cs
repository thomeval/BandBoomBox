using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class SongStarValueCalculator : MonoBehaviour
{

    public Dictionary<TeamScoreCategory, double[]> Percentages = new()
    {
        {TeamScoreCategory.Solo, new[]   {0.05, 0.2, 0.35, 0.55, 0.75, 1.00}},
        {TeamScoreCategory.Duet, new[]   {0.05, 0.2, 0.35, 0.55, 0.70, 0.90, 1.10}},
        {TeamScoreCategory.Crowd, new[]  {0.05, 0.2, 0.35, 0.50, 0.65, 0.85, 1.05, 1.25}},
        {TeamScoreCategory.Legion, new[] {0.05, 0.2, 0.35, 0.50, 0.65, 0.85, 1.05, 1.25, 1.45, 1.65}}
    };
    
    public int NoteBaseValue = 50;

    public SongStarScoreValues CalculateSuggestedScores(List<NoteManager> managers)
    {
        managers = managers.Where(e => e.gameObject.activeInHierarchy).ToList();
        var category = HighScoreManager.GetCategory(managers.Count);

        var values = new List<long>();
        var starCount = Percentages[category].Length;
        for (var x = 0; x < starCount; x++)
        {

            var maxScore = CalculateMaxScore(managers);
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

    public long CalculateMaxScore(List<NoteManager> noteManagers)
    {

        var mx = 1.0;
        var maxMx = 1.0;
        long result = 0;
        var lastNoteTime = 0.0;

        var notes = noteManagers.SelectMany(e => e.Notes).ToList();
        var noteCount = 0;
        
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

        Debug.Log($"CalculateMaxScore result: Charts:{noteManagers.Count()}, Notes: {noteCount}, Score: {result}, Ending Mx: {mx:F3}, Max Mx: {maxMx:F3}");
        return result;
    }

}
