public class DifficultyRange
{
    public Difficulty Difficulty { get; set; }
    public int Min { get; set; } = -1;
    public int Max { get; set; } = -1;

    public bool IsEmpty
    {
        get
        {
            return Min == -1 && Max == -1;
        }
    }
}