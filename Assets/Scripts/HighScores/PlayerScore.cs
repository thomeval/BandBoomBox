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


}
