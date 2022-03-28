using System;

[Serializable]
public class PlayerScore
{
    public string SongId { get; set; }

    public int SongVersion { get; set; }
    public Difficulty Difficulty { get; set; }
    public string ChartGroup { get; set; }

    public int PerfPoints { get; set; }

    public int MaxPerfPoints { get; set; }

    public float PerfPercent
    {
        get { return this.MaxPerfPoints == 0 ? 0 : (1.0f * this.PerfPoints / this.MaxPerfPoints); }
    }

    public int MaxCombo { get; set; }

    public DateTime DateTime { get; set; }

    public override string ToString()
    {
        return string.Format("{0} (v{1}) {2}, {3} {4}/{5}",
            this.SongId,
            this.SongVersion,
            this.Difficulty,
            this.ChartGroup,
            this.PerfPoints,
            this.MaxPerfPoints);
    }

}
