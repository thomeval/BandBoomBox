using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class SongStarValueCalculator : MonoBehaviour
{
    private PlayerManager _playerManager;
    private NoteGenerator _noteGenerator;

    public Dictionary<TeamScoreCategory, double[]> Percentages = new()
    {
        //                                1     2    3     4     5     6     7     8     9     10
        {TeamScoreCategory.Solo,   new[] {0.05, 0.2, 0.35, 0.50, 0.70, 1.00}},
        {TeamScoreCategory.Duet,   new[] {0.05, 0.2, 0.35, 0.50, 0.60, 0.90, 1.05}},
        {TeamScoreCategory.Squad,  new[] {0.05, 0.2, 0.30, 0.45, 0.60, 0.85, 1.00, 1.10}},
        {TeamScoreCategory.Crowd,  new[] {0.05, 0.2, 0.30, 0.45, 0.60, 0.85, 1.00, 1.10, 1.20}},
        {TeamScoreCategory.Legion, new[] {0.05, 0.2, 0.30, 0.45, 0.60, 0.85, 1.00, 1.10, 1.20, 1.30}}
    };

    public int NoteBaseValue = 50;

    private readonly List<SongStarCalculatorChart> _chartCache = new();

    public SongStarScoreValues CalculateSuggestedScores(SongData currentSong)
    {

        if (currentSong == null)
        {
            Debug.LogWarning("No song loaded, cannot calculate suggested scores.");
            return null;
        }

        LoadCache(currentSong);
        var charts = GetCurrentPlayerCharts();
        return CalculateSuggestedScores(charts);
    }

    private List<SongStarCalculatorChart> GetCurrentPlayerCharts()
    {

        var result = new List<SongStarCalculatorChart>();
        foreach (var player in _playerManager.Players.Where(e => e.IsParticipating))
        {
            var chart = _chartCache.FirstOrDefault(e => e.ChartGroup == player.ChartGroup && e.Difficulty == player.Difficulty);
            if (chart == null)
            {
                Debug.LogWarning($"Could not find chart in cache for {player.ChartGroup} {player.Difficulty}");
                continue;
            }

            result.Add(chart);
        }

        Debug.Log($"GetCurrentPlayerCharts result: {result.Count} charts");
        return result;
    }

    private void LoadCache(SongData currentSong)
    {
        _chartCache.Clear();

        foreach (var player in _playerManager.Players.Where(e => e.IsParticipating))
        {
            if (_chartCache.Any(e => e.ChartGroup == player.ChartGroup && e.Difficulty == player.Difficulty))
            {
                continue;
            }

            var notes = _noteGenerator.LoadOrGenerateSongNotes(currentSong, player.ChartGroup, player.Difficulty);

            NoteUtils.CalculateAbsoluteTimes(notes, currentSong.Bpm);
            var sections = currentSong.Sections.Keys.ToArray();
            _chartCache.Add(new SongStarCalculatorChart
            {
                ChartGroup = player.ChartGroup,
                Difficulty = player.Difficulty,
                Notes = notes,
                Sections = sections
            });
        }
    }

    private void Awake()
    {
        Helpers.AutoAssign(ref _playerManager);
        Helpers.AutoAssign(ref _noteGenerator);
    }

    private SongStarScoreValues CalculateSuggestedScores(List<SongStarCalculatorChart> charts)
    {
        var maxScore = CalculateMaxScore(charts);
        var totalNotes = charts.SelectMany(e => e.Notes).Count();
        return CalculateSuggestedScores(maxScore, charts.Count, totalNotes);
    }

    public SongStarScoreValues CalculateSuggestedScores(long maxBaseScore, int playerCount, int totalNotes)
    {
        var values = new List<long>();
        var category = HighScoreManager.GetScoreCategory(playerCount);
        var starCount = Percentages[category].Length;

        for (var x = 0; x < starCount; x++)
        {
            var percent = Percentages[category][x];
            var targetScore = maxBaseScore * percent;
            targetScore = Math.Floor(targetScore);

            values.Add((long)targetScore);
        }

        return new SongStarScoreValues
        {
            ScoreCategory = category,
            Scores = values.ToArray(),
            MaxPossibleBaseScore = maxBaseScore,
            ActivePlayers = playerCount,
            TotalNotes = totalNotes
        };
    }
    public long CalculateMaxScore(List<SongStarCalculatorChart> charts)
    {
        var mx = 1.0;
        var maxMx = 1.0;
        long result = 0;
        var lastNoteTime = 0.0;
        var sections = charts[0].Sections;
        var mxBonusPerSection = HitJudge.SectionBonusMxValues[SectionJudgeResult.Awesome] * charts.Count;
        var notes = charts.SelectMany(e => e.Notes).ToList();
        var noteCount = 0;
        var currentCombo = 0;
        var mistakes = (charts.Count * 2);
        var mistakeInterval = Math.Max(1, notes.Count / (mistakes + 1));

        // Start at the second section (in other words, apply the bonus at the end of the first section)
        var currentSectionIdx = 1;
        var currentSectionEnd = sections.Length > 1 ? sections[1] : double.MaxValue;

        foreach (var note in notes.OrderBy(e => e.AbsoluteTime))
        {
            // Check for section change
            if (note.Position >= currentSectionEnd)
            {
                currentSectionIdx++;
                mx += mxBonusPerSection;
                currentSectionEnd = (currentSectionIdx >= sections.Length) ? double.MaxValue : sections[currentSectionIdx];
            }

            double value = (this.NoteBaseValue * NoteUtils.GetNoteValue(note.NoteType, note.NoteClass));
            value *= mx;
            result += (long)value;

            var gainRate = GameplayMultiplierUtils.GetMultiplierGainRate(currentCombo, 0);
            mx += note.MxValue * gainRate;
            maxMx = Math.Max(maxMx, mx);

            // Move to Next note
            var timeDiff = note.AbsoluteTime - lastNoteTime;
            mx = GameplayMultiplierUtils.DecayMultiplier(mx, timeDiff);

            lastNoteTime = note.AbsoluteTime;
            noteCount++;
            currentCombo++;

            if (currentCombo > mistakeInterval)
            {
                currentCombo = 0;
            }   
        }

        var category = HighScoreManager.GetScoreCategory(charts.Count);
        var scores = "";

        for (var x = 0; x < Percentages[category].Length; x++)
        {
            scores += $"{x + 1} stars: {Percentages[category][x] * result:F0}, ";
        }
        Debug.Log($"CalculateMaxScore result: Charts:{charts.Count()}, Category: {category}, Notes: {noteCount}, Score: {result}, Ending Mx: {mx:F3}, Max Mx: {maxMx:F3}, {scores}");
        return result;
    }

}
