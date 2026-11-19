
using System.Collections.Generic;

public class Mild4thsPatternProvider : NoteAutoFillProviderBase
{
    private readonly NoteType[][] _noteLookups = new NoteType[][]
    {
        new NoteType[] { NoteType.Down, NoteType.Left, NoteType.Right },
        new NoteType[] { NoteType.A, NoteType.B, NoteType.X }
    };

    private readonly float _stepSize = 1.0f;
    public override string DisplayName => "Mild 4ths Pattern";
    private int _count = 0;

    public override List<NoteBase> Generate(double startPosition, double endPosition)
    {
        Reset();

        var result = new List<NoteBase>();
        CurrentPosition = startPosition;

        while (CurrentPosition < endPosition)
        {
            var lane = _count % 2;

            if (_count % 32 >= 16)
            {
                lane = 1 - lane;
            }

            var type = PickRandomType(_noteLookups[lane]);

            var newNote = new NoteBase
            {
                NoteType = type,
                NoteClass = NoteClass.Tap,
                Position = (float)CurrentPosition
            };
            result.Add(newNote);
            CurrentPosition += _stepSize;
            _count++;
        }
        SetNoteLanes(result);
        return result;
    }

    protected override void Reset()
    {
        _count = 0;
    }
}