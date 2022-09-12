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

    private readonly Note[] _lastSeenHolds = new Note[4];

    private readonly Random _rnd = new Random();


    private NoteType GetNoteType(Difficulty playDifficulty, bool[] filledLanes)
    {

        var available = NoteUtils.GetValidNoteTypesForDifficulty(playDifficulty);
        available = available.Intersect(GetPossibleNoteTypes(filledLanes)).ToArray();
        var result = _rnd.Next(available.Length);

        return available[result];
    }

    public void LoadOrGenerateSongNotes(SongData songData, string group, Difficulty difficulty, NoteManager destination)
    {
        var chart = songData.GetChart(group, difficulty);
       LoadOrGenerateSongNotes(chart, songData.LengthInBeats, destination );
    }

    public void LoadOrGenerateSongNotes(SongChart chart, float lengthInBeats, NoteManager destination)
    {

        if (chart.Notes == null || chart.Notes.Length == 0)
        {
            var notes = GenerateNotes(chart.Difficulty, (int) lengthInBeats);
            ApplyToDestination(chart, destination, notes);
        }
        else
        {
            LoadSongNotes(chart, destination);
        }

    }

    public void LoadSongNotes(SongChart chart, NoteManager destination)
    {
        var notes = new List<Note>();
        LoadNoteArray(chart.Notes, ref notes);
        ApplyToDestination(chart, destination, notes);
    }

    private static void ApplyToDestination(SongChart chart, NoteManager destination, List<Note> notes)
    {
        destination.Chart = chart;
        destination.Notes = notes;
        destination.AttachNotes();
        destination.ApplyNoteSkin();
    }

    public List<Note> GenerateNotes(Difficulty difficulty, int endBeat)
    {
        var result = new List<Note>();
        for (int x = 0; x <= endBeat; x++)
        {
           GenerateNotesAtBeat(difficulty, x, ref result);
        }

        return result;

    }

    public void GenerateTestNotes(int endBeat, ref List<Note> destination)
    {
        for (int x = 0; x <= endBeat; x++)
        {
            GenerateTestNotesAtBeat(x, ref destination);
        }
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
    private void GenerateNotesAtBeat(Difficulty difficulty, int beat, ref List<Note> destination)
    {
        var noteCount = GetNoteCount(beat, difficulty);

        bool[] filledLanes = new bool[3];

        for (int x = 0; x < noteCount; x++)
        {
            var noteType = GetNoteType(difficulty, filledLanes);
            var lane = NoteUtils.GetNoteLane(noteType);
            InstantiateNote(beat, noteType, NoteClass.Tap, ref destination);
            filledLanes[lane] = true;
        }
    }

    private void GenerateTestNotesAtBeat(int beat, ref List<Note> destination)
    {
        InstantiateNote(beat, NoteType.AnyB, NoteClass.Tap, ref destination);
    }

    public Note InstantiateNote(float beat, NoteType noteType, NoteClass noteClass, ref List<Note> destination)
    {
        var prefab = noteClass == NoteClass.Hold ? HoldNotePrefab : TapNotePrefab;
        var note = Instantiate(prefab);

        note.Position = beat;
        note.NoteType = noteType;
        note.NoteClass = noteClass;
        note.Lane = NoteUtils.GetNoteLane(note.NoteType);
        ResolveHoldsWithReleases(note);
        destination.Add(note);
        return note;
    }

    private void ResolveHoldsWithReleases(Note note)
    {
        if (note.NoteClass == NoteClass.Hold)
        {
            _lastSeenHolds[note.Lane] = note;
        }

        else if (note.NoteClass == NoteClass.Release)
        {
            var lastHold = _lastSeenHolds[note.Lane];

            if (lastHold == null)
            {
                Debug.LogWarning($"Note release at beat {note.Position} has no corresponding hold before it!");
                return;
            }

            note.NoteType = lastHold.NoteType;
            lastHold.EndNote = note;
            _lastSeenHolds[note.Lane] = null;
        }
    }

    private int GetNoteCount(int beat, Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Difficulty.Beginner:
                return beat % 2 == 0 ? 1 : 0;
            case Difficulty.Medium:
                return beat % 8 == 7 ?  0 : 1;
            case Difficulty.Hard:
                return beat % 16 == 0 ? 2 : 1;
            case Difficulty.Expert:
                return beat % 4 == 0 ? 2 : 1;
            case Difficulty.Master:
                return beat % 16 == 0 ? 3 : (beat % 4 == 0 ? 2 : 1);
            default:
                return 1;
        }
    }

    public void GenerateBeatLines(BeatLineType beatLineType, float endBeat, int beatsPerMeasure, NoteManager destination)
    {

        if (beatLineType == BeatLineType.Off || beatsPerMeasure <= 0)
        {
            return;
        }

        var beatLines = new List<BeatLine>();
        int beatsPerBeatLine = 0;
        switch (beatLineType)
        {
            case BeatLineType.Beat:
                beatsPerBeatLine = 1;
                break;
            case BeatLineType.Phrase:
                beatsPerBeatLine = beatsPerMeasure;
                break;
        }

        GenerateBeatlines(endBeat, beatsPerBeatLine, beatsPerMeasure, ref beatLines);
        destination.BeatLines = beatLines;

        var endMarker = Instantiate(FinishMarkerPrefab, destination.transform);
        endMarker.Position = endBeat;
        endMarker.transform.parent = destination.transform;
        destination.BeatLines.Add(endMarker);

        destination.AttachBeatLines();
        destination.ApplyNoteSkin();
    }

    private void GenerateBeatlines(float endBeat, int beatsPerBeatline, int beatsPerPhrase, ref List<BeatLine> destination)
    {

        for (int x = 0; x <= endBeat; x += beatsPerBeatline)
        {
            var beatLine = Instantiate(BeatLinePrefab);
            if (beatLine != null)
            {
                beatLine.Position = x;
                beatLine.BeatLineType = x % beatsPerPhrase == 0 ? BeatLineType.Phrase : BeatLineType.Beat;
                destination.Add(beatLine);
            }
        }
    }

    public void LoadNoteArray(string[] noteArray, ref List<Note> destination)
    {
        for (int beat = 0; beat < noteArray.Length; beat++)
        {
            LoadNoteBlock(noteArray[beat], beat, ref destination);
        }

    }

    private const int BLOCK_SIZE = 4;
    private void LoadNoteBlock(string notes, int beat, ref List<Note> destination)
    {
        notes = notes.Trim().Replace(" ","");

        if (string.IsNullOrEmpty(notes))
        {
            return;
        }

        if (notes.Length % 4 != 0)
        {
            throw new ArgumentException($"Invalid Note block at beat {beat}: {notes}");
        }

        List<string> slices = new List<string>();

        for (int x = 0; x < notes.Length; x += BLOCK_SIZE)
        {
            slices.Add(notes.Substring(x,BLOCK_SIZE));
        }

        float beatFraction = 1.0f / slices.Count;
        float currentBeatFraction = 0.0f;
        foreach (var slice in slices)
        {
            LoadNoteSlice(slice, beat + currentBeatFraction,  ref destination);
            currentBeatFraction += beatFraction;
        }
    }

    private void LoadNoteSlice(string slice, float beat,  ref List<Note> destination)
    {
        for (int x = 0; x < BLOCK_SIZE; x++)
        {
            var noteType = SjsonUtils.ResolveNoteType(slice[x], x);
            var noteClass = SjsonUtils.ResolveNoteClass(slice[x]);
            
            if (noteType != null)
            {
                InstantiateNote(beat, noteType.Value, noteClass.Value, ref destination);
            }
        }
    }

}
