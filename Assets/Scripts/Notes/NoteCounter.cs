using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class NoteCounter
{

    private const int LINE_SIZE = 4;

    public static SongChartNoteCounts CountNotes(IEnumerable<Note> notes)
    {
        var result = new SongChartNoteCounts();

        if (notes == null || notes.Count() == 0)
        {
            return result;
        }

        foreach (var note in notes)
        {
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
        }

        return result;

    }

    public static SongChartNoteCounts CountNotes(SongChart chart)
    {
        return CountNotes(chart.Notes);
    }

    public static SongChartNoteCounts CountNotes(string[] notes)
    {
        var result = new SongChartNoteCounts();

        if (notes == null || notes.Length == 0)
        {
            return result;
        }

        var lines = notes.SelectMany(e => e.Split(" ")).ToArray();

        foreach (var line in lines)
        {
            CountNotesInLine(line, ref result);
        }
        return result;
    }

    private static SongChartNoteCounts CountNotesInLine(string line, ref SongChartNoteCounts result)
    {
        if (line.Length != LINE_SIZE)
        {
            Debug.LogWarning($"Invalid note line length: {line}.");
            return result;
        }

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

            result.LaneNotes[lane]++;

            switch (noteClass.Value)
            {
                case NoteClass.Tap:
                    result.TapNotes++;
                    break;
                case NoteClass.Hold:
                    result.HoldNotes++;
                    break;
            }
        }

        return result;

    }
}