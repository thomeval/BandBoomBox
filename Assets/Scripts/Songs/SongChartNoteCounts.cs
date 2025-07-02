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
    public float AverageNps;
    public float MaxNps;
    public int[] LaneNotes = new int[4];

    [NonSerialized]
    public LrrData LrrData = new();

    public SongChartNoteCounts Clone()
    {
        return new SongChartNoteCounts
        {
            TapNotes = this.TapNotes,
            HoldNotes = this.HoldNotes,
            LaneNotes = this.LaneNotes.ToArray(),
            AverageNps = this.AverageNps,
            MaxNps = this.MaxNps
        };
    }
}
