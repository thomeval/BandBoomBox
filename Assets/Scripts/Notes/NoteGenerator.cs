using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class NoteGenerator : MonoBehaviour
{
    public Note TapNotePrefab;
    public Note HoldNotePrefab;
    public BeatLine BeatLinePrefab;
    public BeatLine FinishMarkerPrefab;
    public BeatLine SectionLinePrefab;

    private readonly Random _rnd = new();

    private NoteType GetNoteType(Difficulty playDifficulty, bool[] filledLanes)
    {

        var available = NoteUtils.GetValidNoteTypesForDifficulty(playDifficulty);
        available = available.Intersect(GetPossibleNoteTypes(filledLanes)).ToArray();
        var result = _rnd.Next(available.Length);

        return available[result];
    }

    public List<NoteBase> LoadOrGenerateSongNotes(SongData songData, string group, Difficulty difficulty)
    {
        var chart = songData.GetChart(group, difficulty);
        return LoadOrGenerateSongNotes(chart, songData.LengthInBeats);
    }

    public void LoadOrGenerateSongNotes(SongData songData, string group, Difficulty difficulty, NoteManager destination)
    {
        var chart = songData.GetChart(group, difficulty);
        LoadOrGenerateSongNotes(chart, songData.LengthInBeats, destination);
    }

    public void LoadOrGenerateSongNotes(SongChart chart, float lengthInBeats, NoteManager destination)
    {
        var noteBases = LoadOrGenerateSongNotes(chart, lengthInBeats);
        var notes = InstantiateNotes(noteBases);
        ResolveHoldsWithReleases(notes);
        ApplyToDestination(chart, destination, notes);
    }

    public List<NoteBase> LoadOrGenerateSongNotes(SongChart chart, float lengthInBeats)
    {
        var result = new List<NoteBase>();
        if (chart.Notes == null || chart.Notes.Length == 0)
        {
            result = GenerateNotes(chart.Difficulty, (int)lengthInBeats);

        }
        else
        {
            result = LoadNoteArray(chart.Notes);
        }

        SetNoteMxValue(result, chart.Difficulty);
        return result;
    }

    private void SetNoteMxValue(List<NoteBase> result, Difficulty difficulty)
    {
        var mxValue = HitJudge.JudgeMxValues[JudgeResult.Perfect] * HitJudge.DifficultyMxValues[difficulty];

        foreach (var note in result)
        {
            note.MxValue = mxValue;
        }
    }

    private static void ApplyToDestination(SongChart chart, NoteManager destination, List<Note> notes)
    {
        destination.Chart = chart;
        destination.Notes = notes;
        destination.AttachNotes();
        destination.ApplyNoteSkin();
    }

    public List<NoteBase> GenerateNotes(Difficulty difficulty, int endBeat)
    {
        var result = new List<NoteBase>();
        for (int x = 0; x <= endBeat; x++)
        {
            GenerateNotesAtBeat(difficulty, x, ref result);
        }

        return result;
    }

    public List<Note> GenerateTestNotes(int endBeat)
    {
        var result = new List<Note>();
        for (int x = 0; x <= endBeat; x++)
        {
            result.Add(GenerateTestNotesAtBeat(x));
        }

        return result;
    }

    public NoteType[] GetPossibleNoteTypes(bool[] filledLanes)
    {
        var result = new List<NoteType>();

        for (int x = 0; x < filledLanes.Length; x++)
        {
            if (!filledLanes[x])
            {
                result.AddRange(NoteUtils.GetNoteTypesInLane(x));
            }
        }

        return result.ToArray();
    }

    private void GenerateNotesAtBeat(Difficulty difficulty, int beat, ref List<NoteBase> destination)
    {
        var noteCount = GetNoteCount(beat, difficulty);

        var filledLanes = new bool[3];

        for (var x = 0; x < noteCount; x++)
        {
            var noteType = GetNoteType(difficulty, filledLanes);
            var lane = NoteUtils.GetNoteLane(noteType);
            var note = new NoteBase
            {
                Position = beat,
                NoteType = noteType,
                NoteClass = NoteClass.Tap,
                Lane = lane
            };
            destination.Add(note);
            filledLanes[lane] = true;
        }
    }

    private Note GenerateTestNotesAtBeat(int beat)
    {
        var result = InstantiateNote(beat, NoteType.AnyB, NoteClass.Tap);
        return result;
    }

    public Note InstantiateNote(float beat, NoteType noteType, NoteClass noteClass)
    {
        var noteBase = new NoteBase
        {
            Position = beat,
            NoteType = noteType,
            NoteClass = noteClass,
            Lane = NoteUtils.GetNoteLane(noteType)
        };

        return InstantiateNote(noteBase);

    }

    private Note InstantiateNote(NoteBase noteBase)
    {
        var prefab = noteBase.NoteClass == NoteClass.Hold ? HoldNotePrefab : TapNotePrefab;
        var note = Instantiate(prefab);
        note.NoteBase = noteBase;
        note.name = note.Description;
        return note;
    }

    private void ResolveHoldsWithReleases(List<Note> notes)
    {
        for (int x = 0; x < LINE_SIZE; x++)
        {
            var notesInLane = notes.Where(n => n.Lane == x).OrderBy(n => n.Position).ToList();

            Note lastSeenNote = null;
            foreach (var note in notesInLane)
            {

                switch (note.NoteClass)
                {
                    case NoteClass.Release:
                        if (!ValidateReleaseNote(lastSeenNote, note))
                        {
                            continue;
                        }
                        note.NoteType = lastSeenNote.NoteType;
                        lastSeenNote.EndNote = note;
                        break;
                }

                lastSeenNote = note;
            }
        }
    }

    private bool ValidateReleaseNote(Note lastSeenNote, Note note)
    {
        var lastSeenNoteClass = lastSeenNote?.NoteClass.ToString() ?? "Nothing at all";
        if (lastSeenNote == null || lastSeenNote.NoteClass != NoteClass.Hold)
        {
            Debug.LogWarning($"Invalid Note in chart: Release note at beat {note.Position}, Lane {note.Lane} should be preceded by a Hold note, but is actually preceded by a {lastSeenNoteClass} note.");
            return false;
        }

        return true;
    }

    private int GetNoteCount(int beat, Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Difficulty.Beginner:
                return beat % 2 == 0 ? 1 : 0;
            case Difficulty.Medium:
                return beat % 8 == 7 ? 0 : 1;
            case Difficulty.Hard:
                return beat % 16 == 0 ? 2 : 1;
            case Difficulty.Expert:
                return beat % 4 == 0 ? 2 : 1;
            case Difficulty.Nerf:
                return beat % 16 == 0 ? 3 : (beat % 4 == 0 ? 2 : 1);
            default:
                return 1;
        }
    }

    public void GenerateBeatLines(SongData song, NoteManager destination)
    {
        GenerateBeatLines(song.LengthInBeats, song.BeatsPerMeasure, song.Sections, destination);
    }

    public void GenerateBeatLines(float endBeat, int beatsPerMeasure, Dictionary<double, string> sections, NoteManager destination)
    {

        if (beatsPerMeasure <= 0)
        {
            return;
        }

        var beatLines = new List<BeatLine>();
        sections ??= new Dictionary<double, string>();
        var sectionTimes = sections.Keys.ToList();
        GenerateBeatLines(endBeat, beatsPerMeasure, sectionTimes, ref beatLines);
        GenerateSectionLines(sectionTimes, ref beatLines);
        destination.BeatLines = beatLines;

        var endMarker = Instantiate(FinishMarkerPrefab, destination.transform);
        endMarker.Position = endBeat;
        endMarker.transform.parent = destination.transform;
        destination.BeatLines.Add(endMarker);

        destination.AttachBeatLines();
        destination.ApplyNoteSkin();
    }

    private void GenerateBeatLines(float endBeat, int beatsPerMeasure, List<double> sections, ref List<BeatLine> destination)
    {

        for (var x = 0; x <= endBeat; x += beatsPerMeasure)
        {
            if (sections.Contains(x))
            {
                continue;
            }

            var beatLine = InstantiateBeatLine(BeatLinePrefab, x, BeatLineType.Phrase);
            if (beatLine == null)
            {
                continue;
            }

            destination.Add(beatLine);
        }
    }

    public void GenerateSectionLines(List<double> sections, ref List<BeatLine> destination)
    {

        foreach (var section in sections)
        {
            var beatLine = InstantiateBeatLine(SectionLinePrefab, (float)section, BeatLineType.Section);

            destination.Add(beatLine);
        }
    }

    public List<Note> InstantiateNotes(List<NoteBase> noteBases)
    {
        var result = new List<Note>();
        foreach (var noteBase in noteBases)
        {
            result.Add(InstantiateNote(noteBase));
        }

        return result;
    }

    private BeatLine InstantiateBeatLine(BeatLine prefab, float xPos, BeatLineType beatLineType)
    {
        var result = Instantiate(prefab);
        Debug.Assert(result != null, $"Prefab instantiate failed for beatline of type {beatLineType}");
        result!.Position = xPos;
        result.BeatLineType = beatLineType;
        result.gameObject.name = $"{beatLineType} Beatline ({xPos:F1})";
        return result;
    }

    public List<NoteBase> LoadSongNotes(SongChart chart)
    {
        var result = LoadNoteArray(chart.Notes);
        return result;
    }

    public List<NoteBase> LoadNoteArray(string[] noteArray)
    {
        var result = new List<NoteBase>();
        if (noteArray == null)
        {
            return result;
        }

        for (int beat = 0; beat < noteArray.Length; beat++)
        {
            result.AddRange(LoadNoteBlock(noteArray[beat], beat));
        }

        return result;
    }

    private const int LINE_SIZE = 4;
    private List<NoteBase> LoadNoteBlock(string notes, int beat)
    {
        var result = new List<NoteBase>();
        notes = notes.Trim().Replace(" ", "");

        if (string.IsNullOrEmpty(notes))
        {
            return result;
        }

        if (notes.Length % 4 != 0)
        {
            throw new ArgumentException($"Invalid Note block at beat {beat}: {notes}");
        }

        var lines = new List<string>();

        for (int x = 0; x < notes.Length; x += LINE_SIZE)
        {
            lines.Add(notes.Substring(x, LINE_SIZE));
        }

        float beatFraction = 1.0f / lines.Count;
        float currentBeatFraction = 0.0f;
        foreach (var line in lines)
        {
            var lineNotes = LoadNoteLine(line, beat + currentBeatFraction);
            result.AddRange(lineNotes);

            currentBeatFraction += beatFraction;
        }

        return result;
    }

    private List<NoteBase> LoadNoteLine(string line, float beat)
    {
        var result = new List<NoteBase>();
        if (line.Length != LINE_SIZE)
        {

            Debug.LogWarning($"Invalid note line length: {line}.");
            return result;
        }

        for (int lane = 0; lane < LINE_SIZE; lane++)
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

            result.Add(new NoteBase { Position = beat, NoteType = noteType.Value, NoteClass = noteClass.Value, Lane = lane });
        }

        return result;
    }

}
