using System;
using System.Linq;

[Serializable]
public class SongChartNoteCounts
{
    public int TotalNotes
    {
        get
        {
            return LaneNotes.Sum();
        }
    }

    public int TapNotes;
    public int HoldNotes;

    public int[] LaneNotes = new int[4];
}
