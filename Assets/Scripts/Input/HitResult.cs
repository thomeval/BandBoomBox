public class HitResult
{
    /// <summary>
    /// Indicates how accurately a note was hit. 
    /// </summary>
    public JudgeResult JudgeResult { get; set; }

    /// <summary>
    /// Indicates whether a note was hit early, late, or not at all.
    /// </summary>
    public DeviationResult DeviationResult { get; set; }

    /// <summary>
    /// Gets or set the amount of Performance Points gained by this hit.
    /// </summary>
    public int PerfPoints { get; set; }

    /// <summary>
    /// Gets or sets the amount of Score Points gained by this hit.
    /// </summary>
    public int ScorePoints { get; set; }

    /// <summary>
    /// Gets or sets the amount of multiplier gained by this hit.
    /// </summary>
    public float MxPoints { get; set; }

    public int Player { get; set; }

    /// <summary>
    /// Gets or sets which lane the note was hit on.
    /// </summary>
    public int Lane { get; set; }

    /// <summary>
    /// Gets or sets how far the hit deviated from perfect accuracy, in decimal seconds. Negative numbers indicate an early hit, positive numbers indicate a late hit.
    /// </summary>
    public float Deviation { get; set; }
}
