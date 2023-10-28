using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine.UIElements;

public static class SjsonUtils
{
    public const int LANE_COUNT = 4;
    public const float FLOAT_TOLERANCE = 0.001f;

    public static readonly Dictionary<char, NoteType>[] NoteTypes =
    {
        new Dictionary<char, NoteType>
        {
            {'1', NoteType.AnyT},
            {'2', NoteType.AnyT},
            {'4', NoteType.LB},
            {'5', NoteType.LB},
            {'7', NoteType.LT},
            {'8', NoteType.LT},
            {'A', NoteType.RB},
            {'B', NoteType.RB},
            {'D', NoteType.RT},
            {'E', NoteType.RT},
            {'G', NoteType.AnyT }
        },
        new Dictionary<char, NoteType>
        {
            {'1', NoteType.AnyD},
            {'2', NoteType.AnyD},
            {'4', NoteType.Left},
            {'5', NoteType.Left},
            {'7', NoteType.Down},
            {'8', NoteType.Down},
            {'A', NoteType.Up},
            {'B', NoteType.Up},
            {'D', NoteType.Right},
            {'E', NoteType.Right},
            {'G', NoteType.AnyD }
        },
        new Dictionary<char, NoteType>
        {
            {'1', NoteType.AnyB},
            {'2', NoteType.AnyB},
            {'4', NoteType.X},
            {'5', NoteType.X},
            {'7', NoteType.A},
            {'8', NoteType.A},
            {'A', NoteType.Y},
            {'B', NoteType.Y},
            {'D', NoteType.B},
            {'E', NoteType.B},
            {'G', NoteType.AnyB }
        },
        new Dictionary<char, NoteType>
        {
            // Lane not used yet
        }
    };

    public static Dictionary<char, NoteClass> NoteClasses = new Dictionary<char, NoteClass>
    {
        {'1', NoteClass.Tap},
        {'2', NoteClass.Hold},
        {'4', NoteClass.Tap},
        {'5', NoteClass.Hold},
        {'7', NoteClass.Tap},
        {'8', NoteClass.Hold},
        {'A', NoteClass.Tap},
        {'B', NoteClass.Hold},
        {'D', NoteClass.Tap},
        {'E', NoteClass.Hold},
        {'G', NoteClass.Release}
    };

    public static string[] ToSjson(List<Note> notes)
    {
        List<string> result = new();
        var blocks = notes.Select(e => new NoteSjsonEntry(e)).OrderBy(e => e.Position).ToList();
        blocks = MergeSjsonEntries(blocks);

        var blocksByBeat = blocks.ToLookup(e => (int)e.Position);

        var lastBeat = 0;

        if (blocks.Any())
        {
            lastBeat = blocks.Select(e => (int)e.Position).Max();
        }

        for (int x = 0; x <= lastBeat; x++)
        {

            if (!blocksByBeat.Contains(x))
            {
                result.Add("0000");
            }
            else
            {
                var block = ParseNoteBlockToSjson(blocksByBeat[x].ToList());
                result.Add(block);
            }

        }

        return result.ToArray();
    }

    private static string ParseNoteBlockToSjson(List<NoteSjsonEntry> notes)
    {

        if (!notes.Any())
        {
            return "0000";
        }

        var stepCount = GetNormalizedStepSize(notes);
        var stepSize = 1.0f / stepCount;

        var result = "";

        var pos = 0.0f;

        while (pos < 1.0)
        {
            var note = notes.SingleOrDefault(e => Math.Abs(e.PositionFraction - pos) < FLOAT_TOLERANCE);

            if (note == null)
            {
                result += "0000 ";
            }
            else
            {
                result += note.Sjson + " ";
            }

            pos += stepSize;
        }
        return result.Trim();

    }

    private static readonly int[] _validBlockCounts = { 1, 2, 3, 4, 6, 8, 12, 24 };
    private static int GetNormalizedStepSize(List<NoteSjsonEntry> notes)
    {
        const float TOLERANCE = 0.001f;
        foreach (var attempt in _validBlockCounts)
        {
            var temp = notes.Select(e => e.PositionFraction * attempt);
            temp = temp.Select(e => e - MathF.Round(e)).ToArray();

            if (temp.All(e => Math.Abs(e) < TOLERANCE))
            {
                return attempt;
            }
        }

        var errStr = notes.Select(e => e.Position.ToString("F3", CultureInfo.InvariantCulture)).Aggregate((cur, next) => $"{cur}, {next}");
        throw new ArgumentException("Unable to determine suitable step size to note block. Note positions: " + errStr);
    }

    private static List<NoteSjsonEntry> MergeSjsonEntries(List<NoteSjsonEntry> entries)
    {

        var result = new List<NoteSjsonEntry>();
        if (!entries.Any())
        {
            return result;
        }

        entries = entries.OrderBy(e => e.Position).ToList();

        const float TOLERANCE = 0.001f;
        var lastEntry = entries.First();

        foreach (var entry in entries)
        {
            if (Math.Abs(entry.Position - lastEntry.Position) < TOLERANCE)
            {
                lastEntry = lastEntry.Merge(entry);
            }
            else
            {
                result.Add(lastEntry);
                lastEntry = entry;
            }
        }

        result.Add(lastEntry);

        return result;

    }

    public static string ToSjson(this Note note)
    {
        var result = "0000".ToArray();
        var noteType = note.NoteType;
        var lane = NoteUtils.GetNoteLane(noteType);

        var c = ResolveCharCode(note.NoteClass, note.NoteType);

        result[lane] = c;

        return result.Aggregate("", (current, c2) => current + c2);
    }

    private static char ResolveCharCode(NoteClass noteClass, NoteType noteType)
    {
        if (noteClass == NoteClass.Release)
        {
            noteType = NoteUtils.GetLaneAnyNote(noteType);
        }

        var lane = NoteUtils.GetNoteLane(noteType);
        var matchingClasses = NoteClasses.Where(e => e.Value == noteClass).Select(e => e.Key).ToList();
        var matchingTypes = NoteTypes[lane].Where(e => e.Value == noteType).Select(e => e.Key).ToList();

        var result = matchingTypes.Intersect(matchingClasses).SingleOrDefault();

        if (result == default)
        {
            throw new Exception($"Unable to Resolve SJSON Char Code for NoteClass {noteClass}, NoteType {noteType}.");
        }
        return result;
    }


    public static string CombineSjson(string[] sjsonNotes)
    {
        var result = "0000".ToCharArray();

        for (int x = 0; x < LANE_COUNT; x++)
        {
            result[x] = sjsonNotes.Select(e => e[x]).Max();
        }
        return result.ToString();
    }

    public static NoteType? ResolveNoteType(char c, int lane)
    {
        if (!NoteTypes[lane].ContainsKey(c))
        {
            return null;
        }

        return NoteTypes[lane][c];
    }

    public static NoteClass? ResolveNoteClass(char c)
    {
        if (!NoteClasses.ContainsKey(c))
        {
            return null;
        }

        return NoteClasses[c];
    }
}

