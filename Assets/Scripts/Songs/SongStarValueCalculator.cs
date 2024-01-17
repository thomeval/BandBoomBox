using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class SongStarValueCalculator : MonoBehaviour
{
    private PlayerManager _playerManager;
    private NoteGenerator _noteGenerator;

    public Dictionary<TeamScoreCategory, double[]> Percentages = new()
    {
        //                                1     2    3     4     5     6     7     8     9     10
        {TeamScoreCategory.Solo, new[]   {0.05, 0.2, 0.35, 0.55, 0.75, 1.00}},
        {TeamScoreCategory.Duet, new[]   {0.05, 0.2, 0.35, 0.55, 0.70, 0.90, 1.10}},
        {TeamScoreCategory.Squad, new[]  {0.05, 0.2, 0.35, 0.50, 0.65, 0.85, 1.05, 1.25}},
        {TeamScoreCategory.Crowd, new[]  {0.05, 0.2, 0.35, 0.50, 0.65, 0.85, 1.05, 1.25, 1.45}},
        {TeamScoreCategory.Legion, new[] {0.05, 0.2, 0.35, 0.50, 0.65, 0.85, 1.05, 1.25, 1.45, 1.65}}
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
        foreach (var player in _playerManager.Players.Where(e => IsInGameplay(e.PlayerState)))
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

        foreach (var player in _playerManager.Players.Where(e => IsInGameplay(e.PlayerState)))
        {
            if (_chartCache.Any(e => e.ChartGroup == player.ChartGroup && e.Difficulty == player.Difficulty))
            {
                continue;
            }

            var notes = _noteGenerator.LoadOrGenerateSongNotes(currentSong, player.ChartGroup, player.Difficulty);

            NoteUtils.CalculateAbsoluteTimes(notes, currentSong.Bpm);
            _chartCache.Add(new SongStarCalculatorChart
            {
                ChartGroup = player.ChartGroup,
                Difficulty = player.Difficulty,
                Notes = notes
            });
        }
    }

    private bool IsInGameplay(PlayerState playerState)
    {
        return playerState == PlayerState.Gameplay_ReadyToStart || playerState == PlayerState.Gameplay_Playing || playerState == PlayerState.Gameplay_Loading;
    }

    private void Awake()
    {
        Helpers.AutoAssign(ref _playerManager);
        Helpers.AutoAssign(ref _noteGenerator);
    }

    private SongStarScoreValues CalculateSuggestedScores(List<SongStarCalculatorChart> charts)
    {
        var maxScore = CalculateMaxScore(charts);
        return CalculateSuggestedScores(maxScore, charts.Count);
    }

    public SongStarScoreValues CalculateSuggestedScores(long maxBaseScore, int playerCount)
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
            ActivePlayers = playerCount
        };
    }
    public long CalculateMaxScore(List<SongStarCalculatorChart> charts)
    {

        var mx = 1.0;
        var maxMx = 1.0;
        long result = 0;
        var lastNoteTime = 0.0;

        var notes = charts.SelectMany(e => e.Notes).ToList();
        var noteCount = 0;

        foreach (var note in notes.OrderBy(e => e.AbsoluteTime))
        {
            double value = (this.NoteBaseValue * NoteUtils.GetNoteValue(note.NoteType, note.NoteClass));
            value *= mx;
            result += (long)value;

            // TODO: Consider implementing combo gain rate bonuses, and a "mistake interval".
            mx += note.MxValue;
            maxMx = Math.Max(maxMx, mx);

            // Move to Next note
            var timeDiff = note.AbsoluteTime - lastNoteTime;
            mx = GameplayMultiplierUtils.DecayMultiplier(mx, timeDiff);

            lastNoteTime = note.AbsoluteTime;
            noteCount++;
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
