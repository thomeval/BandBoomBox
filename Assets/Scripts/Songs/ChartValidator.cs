using System.Collections.Generic;
using System.Linq;

public static class ChartValidator
{
    private static readonly NoteClass[] _longNoteClasses = { NoteClass.Hold };

    public static string ValidateNotes(IList<Note> notes)
    {
        var result = CheckForStackedNotes(notes);
        result ??= CheckForNotesInsideHolds(notes);
        result ??= CheckForUnterminatedHolds(notes);
        return result;
    }

    private static string CheckForUnterminatedHolds(IList<Note> notes)
    {
        var longNotes = notes.Where(e => _longNoteClasses.Contains(e.NoteClass)).ToList();
        var unterminatedNote = longNotes.FirstOrDefault(e => e.EndNote == null);

        if (unterminatedNote != null)
        {
            return
                $"Error: Hold Note {unterminatedNote.NoteType} at position {unterminatedNote.Position}, on lane {unterminatedNote.Lane} has no corresponding Release Note. This is not allowed.";
        }

        return null;
    }

    public static List<Note> GetNotesInsideHolds(IList<Note> notes)
    {
        var result = new List<Note>();
        var holdRegions = GetHoldRegions(notes);
        result.AddRange(notes.Where(e => holdRegions.Any(r => r.IsInsideRegion(e))));
        return result;
    }

    public static string CheckForNotesInsideHolds(IList<Note> notes)
    {
        var notesInsideHolds = GetNotesInsideHolds(notes);
        var firstResult = notesInsideHolds.FirstOrDefault();
        if (firstResult == null)
        {
            return null;
        }

        return $"Error: Note {firstResult.NoteType} at position {firstResult.Position}, on lane {firstResult.Lane} is inside a hold note. This is not allowed.";
    }

    private static List<HoldRegion> GetHoldRegions(IList<Note> notes)
    {
        var holdRegions = notes.Where(e => e.EndNote != null).Select(e => new HoldRegion
        {
            Start = e.Position,
            End = e.EndNote.Position,
            Lane = e.Lane
        }).ToList();
        return holdRegions;
    }

    private static string CheckForStackedNotes(IList<Note> notes)
    {
        var groups = notes.GroupBy(e => (e.Lane, e.Position));
        var stackedNotes = groups.FirstOrDefault(e => e.Count() > 1);

        if (stackedNotes != null)
        {
            return $"Error: Multiple notes are located at position {stackedNotes.Key.Position}, on lane {stackedNotes.Key.Lane}. This is not allowed.";
        }

        return null;
    }
}