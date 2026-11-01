using System.Collections.Generic;

public abstract class NoteAutoFillProviderBase
{
    public virtual string DisplayName => "Note Provider";
    public double CurrentPosition { get; set; }

    public abstract List<NoteBase> Generate(double startPosition, double endPosition);

    protected void SetNoteLanes(List<NoteBase> notes)
    {
        foreach (var note  in notes) 
        {
            note.Lane = NoteUtils.GetNoteLane(note.NoteType);
        }
    }

    protected NoteType PickRandomType(NoteType[] noteTypes)
    {
        return noteTypes.PickRandom();
    }

}
