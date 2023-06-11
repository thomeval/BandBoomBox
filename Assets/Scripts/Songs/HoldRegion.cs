public class HoldRegion
{
    public float Start { get; set; }
    public float End { get; set; }
    public int Lane { get; set; }

    public bool IsInsideRegion(Note note)
    {
        return note.Position > this.Start && note.Position < this.End && this.Lane == note.Lane;
    }
}