using System.Collections.Generic;

public class BeginnerTwoBeatProvider : NoteAutoFillProviderBase
{
    private readonly NoteType[] _noteTypes = {NoteType.AnyB, NoteType.AnyD};
    private readonly float _stepSize = 2.0f;
    public override string DisplayName => "Beginner Pattern";

    public override List<NoteBase> Generate(double startPosition, double endPosition)
    {
        var result = new List<NoteBase>();
        CurrentPosition = startPosition;

        while (CurrentPosition < endPosition) 
        {
            var type = PickRandomType(_noteTypes);

            var newNote = new NoteBase
            {
               NoteType = type,
               NoteClass = NoteClass.Tap,
               Position = (float) CurrentPosition
            };
            result.Add(newNote);
            CurrentPosition += _stepSize;
        }

        SetNoteLanes(result);
        return result;
    }

}