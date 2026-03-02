using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class NoteCounter
{

    private const int LINE_SIZE = 4;
    private const int MIN_TRIMMED_AVG_INTERVALS = 6;
    public static SongChartNoteCounts CountNotes(IEnumerable<Note> notes, float songLength, float songBpm, float intervalSize)
    {
        var result = new SongChartNoteCounts();
        result.LrrData.IntervalSizeBeats = intervalSize;

        if (notes == null || notes.Count() == 0)
        {
            return result;
        }

        notes = notes.OrderBy(e => e.Position);

        var notesInPhrase = 0;
        var nextPhrase = intervalSize;

        foreach (var note in notes)
        {
            if (note.Position >= nextPhrase)
            {
                nextPhrase += intervalSize;
                var currentNps = notesInPhrase / intervalSize * songBpm / 60;

                result.LrrData.Intervals.Add(currentNps);

                notesInPhrase = 0;
            }

            if (note.Lane < 0 || note.Lane >= LINE_SIZE)
            {
                Debug.LogWarning($"Invalid note lane: {note.Lane}.");
                continue;
            }

            result.LaneNotes[note.Lane]++;

            switch (note.NoteClass)
            {
                case NoteClass.Tap:
                    result.TapNotes++;
                    break;
                case NoteClass.Hold:
                    result.HoldNotes++;
                    break;
            }

            notesInPhrase++;
        }

        if (notesInPhrase > 0)
        {
            var currentNps = notesInPhrase / intervalSize * songBpm / 60;
            result.LrrData.Intervals.Add(currentNps);
        }

        if (songLength > 0.0f)
        {
            result.AverageNps = result.TotalNotes / songLength;
            result.MaxNps = result.LrrData.Intervals.Max();
        }

        CalculateTrimmedAverageNps(ref result);
        return result;

    }

    public static SongChartNoteCounts CountNotes(SongChart chart, float songLength, float songBpm, float intervalSize)
    {
        return CountNotes(chart.Notes, songLength, songBpm, intervalSize);
    }

    public static SongChartNoteCounts CountNotes(string[] notes, float songLength, float songBpm, float intervalSize)
    {
        var result = new SongChartNoteCounts();

        if (notes == null || notes.Length == 0)
        {
            return result;
        }

        var notesInPhrase = 0;
        var nextPhrase = intervalSize;
        var currentInterval = 0;

        for (int x = 0; x < notes.Length; x++)
        {
            if (x >= nextPhrase)
            {
                nextPhrase += intervalSize;
                var currentNps = notesInPhrase / intervalSize * songBpm / 60;
                result.LrrData.Intervals.Add(currentNps);

                notesInPhrase = 0;
                currentInterval++;
            }

            var lines = notes[x].Split(" ");

            foreach (var line in lines)
            {
                notesInPhrase += CountNotesInLine(line, ref result);
            }
        }

        if (notesInPhrase > 0)
        {
            var currentNps = notesInPhrase / intervalSize * songBpm / 60;
            result.LrrData.Intervals.Add(currentNps);
        }

        if (songLength > 0.0f)
        {
            result.AverageNps = result.TotalNotes / songLength;
            result.MaxNps = result.LrrData.Intervals.Count > 0 ? result.LrrData.Intervals.Max() : 0;
        }

        CalculateTrimmedAverageNps(ref result);
        return result;
    }

    private static void CalculateTrimmedAverageNps(ref SongChartNoteCounts result)
    {
        if (result.LrrData.Intervals.Count < MIN_TRIMMED_AVG_INTERVALS)
        {
            result.TrimmedAverageNps = result.AverageNps;
        }
        else
        {
            // Remove the lowest 25% of intervals, and the highest interval, then take the average of the remaining intervals.
            var intervalsToRemove = result.LrrData.Intervals.Count / 4;
            var sortedIntervals = result.LrrData.Intervals.OrderBy(i => i)
                .Skip(intervalsToRemove)
                .OrderByDescending(i => i)
                .Skip(1)
                .ToList();

            result.TrimmedAverageNps = sortedIntervals.Average();
        }
    }

    private static int CountNotesInLine(string line, ref SongChartNoteCounts counts)
    {
        if (line.Length != LINE_SIZE)
        {
            Debug.LogWarning($"Invalid note line length: {line}.");
            return 0;
        }

        var result = 0;
        for (var lane = 0; lane < LINE_SIZE; lane++)
        {
            if (line[lane] == '0')
            {
                continue;
            }

            var noteType = SjsonUtils.ResolveNoteType(line[lane], lane);
            var noteClass = SjsonUtils.ResolveNoteClass(line[lane]);

            if (noteType == null || noteClass == null)
            {
                Debug.LogWarning($"Invalid note type or class in slice: {line}. Unrecognized value: {line[lane]}.");
                continue;
            }

            result++;
            counts.LaneNotes[lane]++;

            switch (noteClass.Value)
            {
                case NoteClass.Tap:
                    counts.TapNotes++;
                    break;
                case NoteClass.Hold:
                    counts.HoldNotes++;
                    break;
            }
        }

        return result;

    }
}