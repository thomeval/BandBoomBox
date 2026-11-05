using System;
using System.Collections.Generic;

public class MediumPatternProvider : NoteAutoFillProviderBase
{

    private string[] _patterns = { "ABABCDC ", "BABADCD ", "AABBCCD ", "BBAADDC " };

    private readonly Dictionary<char, NoteType[]> _noteLookups = new()
    {
         { 'A', new NoteType[] { NoteType.Down, NoteType.Left } },
         { 'B', new NoteType[] { NoteType.A, NoteType.B } },
         { 'C', new NoteType[] { NoteType.Down, NoteType.Left } },
         { 'D', new NoteType[] { NoteType.A, NoteType.B } }
    };

    private Dictionary<char, NoteType> _selectedNotes = new();

    private readonly float _stepSize = 1.0f;
    private string _currentPattern = null;
    private int _currentPatternIndex = 0;

    public override string DisplayName => "Medium Pattern";

    public override List<NoteBase> Generate(double startPosition, double endPosition)
    {
        var result = new List<NoteBase>();
        CurrentPosition = startPosition;
        while (CurrentPosition < endPosition)
        {
            var nextNoteType = GetNextNoteType();

            if (nextNoteType.HasValue)
            {
                var newNote = new NoteBase
                {
                    NoteType = nextNoteType.Value,
                    NoteClass = NoteClass.Tap,
                    Position = (float)CurrentPosition
                };
                result.Add(newNote);
            }

            CurrentPosition += _stepSize;
        }
        SetNoteLanes(result);
        return result;
    }

    private NoteType? GetNextNoteType()
    {
        if (_currentPattern == null || _currentPatternIndex >= _currentPattern.Length)
        {
            _currentPattern = _patterns.PickRandom();
            _currentPatternIndex = 0;
            _selectedNotes = GetNotePalette();
        }

        var nextChar = _currentPattern[_currentPatternIndex];
        _currentPatternIndex++;

        if (nextChar == ' ')
        {
            return null;
        }

        var result = _selectedNotes[nextChar];
        return result;
    }

    private Dictionary<char, NoteType> GetNotePalette()
    {
        return new Dictionary<char, NoteType>
        {
            { 'A', _noteLookups['A'].PickRandom() },
            { 'B', _noteLookups['B'].PickRandom() },
            { 'C', _noteLookups['C'].PickRandom() },
            { 'D', _noteLookups['D'].PickRandom() }
        };
    }
}