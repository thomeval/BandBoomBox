using System.Collections.Generic;
using System.Linq;

public static class SjsonUtils
{
    public const int LANE_COUNT = 4;

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

    public static string ToSjson(this Note note)
    {
        var result = "0000".ToArray();
        var noteType = note.NoteType;
        var lane = NoteUtils.GetNoteLane(noteType);

        var c = ResolveCharCode(note.NoteClass, note.NoteType);

        result[lane] = c;
        return result.ToString();
    }

    private static char ResolveCharCode(NoteClass noteClass, NoteType noteType)
    {
        var lane = NoteUtils.GetNoteLane(noteType);
        var matchingClasses = NoteClasses.Where(e => e.Value == noteClass).Select(e => e.Key).ToList();
        var matchingTypes = NoteTypes[lane].Where(e => e.Value == noteType).Select(e => e.Key).ToList();

        var result = matchingTypes.Intersect(matchingClasses).SingleOrDefault();

        return result;
    }

    public static string[] ToSjson(Note[] notes)
    {
        var length = (int) notes.Max(e => e.Position);

        var result = new string[length];

        for (int x = 0; x < length; x++)
        {
            var block = notes.Where(e => (int) e.Position == x).Select(e => e.ToSjson()).ToArray();
            result[x] = CombineSjson(block);
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

