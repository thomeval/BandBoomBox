using System;
using System.Collections.Generic;

public static class NoteUtils
{
    public static float MissCutoff = 0.3f;
    public static float BeatLineCutoff = 1.0f;
    public static int GetNoteLane(NoteType noteType)
    {
        switch (noteType)
        {
            case NoteType.LB:
            case NoteType.LT:
            case NoteType.RB:
            case NoteType.RT:
            case NoteType.AnyT:
                return 0;
            case NoteType.Left:
            case NoteType.Up:
            case NoteType.Right:
            case NoteType.Down:
            case NoteType.AnyD:
                return 1;
            case NoteType.A:
            case NoteType.B:
            case NoteType.X:
            case NoteType.Y:
            case NoteType.AnyB:
                return 2;
            default:
                throw new ArgumentException("Unrecognised note type: " + noteType);
        }
    }

    public static NoteType[] GetNoteTypesInLane(int lane)
    {
        switch (lane)
        {
            case 0:
                return new[] { NoteType.LB, NoteType.LT, NoteType.RB, NoteType.RT, NoteType.AnyT };
            case 1:
                return new[] {NoteType.Left, NoteType.Up, NoteType.Right, NoteType.Down, NoteType.AnyD};
            case 2:
                return new[] {NoteType.A, NoteType.B, NoteType.X, NoteType.Y, NoteType.AnyB};
            default:
                throw new ArgumentException("Invalid lane number: " + lane);
        }

    }

    public static NoteType GetLaneAnyNote(NoteType noteType)
    {
        switch (noteType)
        {
            case NoteType.A:
            case NoteType.B:
            case NoteType.X:
            case NoteType.Y:
            case NoteType.AnyB:
                return NoteType.AnyB;

            case NoteType.Left:
            case NoteType.Up:
            case NoteType.Right:
            case NoteType.Down:
            case NoteType.AnyD:
                return NoteType.AnyD;

            case NoteType.LB:
            case NoteType.LT:
            case NoteType.RB:
            case NoteType.RT:
            case NoteType.AnyT:
                return NoteType.AnyT;

            default:
                throw new ArgumentException("Unrecognised note type: " +  noteType);
        }
    }

    public static NoteType GetLaneAnyNote(int lane)
    {
        switch (lane)
        {
            case 0: return NoteType.AnyT;
            case 1: return NoteType.AnyD;             
            case 2: return NoteType.AnyB;
            default:
                throw new ArgumentException("Invalid lane number: " + lane);
        }
    }

    public static float GetNoteValue(NoteType noteType, NoteClass noteClass)
    {
        if (noteClass == NoteClass.Release)
        {
            return 1.0f;
        }

        switch (noteType)
        {
            case NoteType.Left:
            case NoteType.Up:
            case NoteType.Right:
            case NoteType.Down:
            case NoteType.A:
            case NoteType.B:
            case NoteType.X:
            case NoteType.Y:
            case NoteType.LB:
            case NoteType.LT:
            case NoteType.RB:
            case NoteType.RT:
                return 1.0f;

            case NoteType.AnyD:
            case NoteType.AnyT:
            case NoteType.AnyB:
                return 0.5f;
            default:
                throw new ArgumentException("Unrecognised note type: " + noteType);
        }
    }

    public static NoteType[] GetValidNoteTypesForDifficulty(Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Difficulty.Beginner:
                return new[] { NoteType.AnyB, NoteType.AnyD };
            case Difficulty.Medium:
                return new[] { NoteType.A, NoteType.B, NoteType.Left, NoteType.Down };
            case Difficulty.Hard:
                return  new[] { NoteType.A, NoteType.B, NoteType.X, NoteType.Y, NoteType.Left, NoteType.Down, NoteType.Up, NoteType.Right };
            case Difficulty.Expert:
                return  new[] { NoteType.A, NoteType.B, NoteType.X, NoteType.Y, NoteType.Left, NoteType.Down, NoteType.Up, NoteType.Right, NoteType.LB, NoteType.RB };
            case Difficulty.Master:
                return  new[] { NoteType.A, NoteType.B, NoteType.X, NoteType.Y, NoteType.Left, NoteType.Down, NoteType.Up, NoteType.Right, NoteType.LB, NoteType.RB, NoteType.LT, NoteType.RT };
            default:
                return Array.Empty<NoteType>();
        }

    }

    public static NoteType? GetNoteTypeForInput(InputAction inputAction)
    {
        return GetNoteTypeForInput(inputAction.ToString());
    }

    public static NoteType? GetNoteTypeForInput(string noteType)

    {
        if (Enum.TryParse(noteType, true, out NoteType result))
        {
            return result;
        }
        return null;
    }

    public static void CalculateAbsoluteTimes(IEnumerable<Note> notes, float bpm)
    {
        foreach (var note in notes)
        {
            note.AbsoluteTime = note.Position * 60 / bpm;
        }
    }

    public static void CalculateAbsoluteTimes(IEnumerable<BeatLine> beatline, float bpm)
    {
        foreach (var note in beatline)
        {
            note.AbsoluteTime = note.Position * 60 / bpm;
        }
    }
}

public enum NoteType
{
    A,
    B,
    X,
    Y,
    AnyB,
    Left,
    Up,
    Right,
    Down,
    AnyD,
    LB,
    LT,
    RB,
    RT,
    AnyT  
}

public enum NoteClass
{
    Tap,
    Hold,
    Release,  
    None, 
}

public enum BeatLineType
{
    Beat,
    Phrase,
    Off,
    Finish
}

