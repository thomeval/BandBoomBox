using System;
using System.Diagnostics;
using System.Linq;

[Serializable]
[DebuggerDisplay("Notes: {TotalNotes}, Tap: {TapNotes}, Hold: {HoldNotes}, Trimmed Avg NPS: {TrimmedAverageNps}, Max NPS: {MaxNps}")]
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
    public float TrimmedAverageNps;

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
            MaxNps = this.MaxNps,
            TrimmedAverageNps = this.TrimmedAverageNps
        };
    }
}
