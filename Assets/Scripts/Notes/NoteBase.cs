public class NoteBase
{
    public float Position;
    public float AbsoluteTime;
    public NoteType NoteType;
    public NoteClass NoteClass;
    public int Lane;

    public float MxValue = 0;

    public string Description => $"{this.NoteClass}-{this.NoteType}";
    public override string ToString()
    {
        return Description;
    }
}