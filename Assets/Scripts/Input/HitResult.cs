using Unity.Netcode;

public class HitResult : INetworkSerializable
{
    /// <summary>
    /// Indicates how accurately a note was hit. 
    /// </summary>
    public JudgeResult JudgeResult;

    /// <summary>
    /// Indicates whether a note was hit early, late, or not at all.
    /// </summary>
    public DeviationResult DeviationResult;

    /// <summary>
    /// Gets or set the amount of Performance Points gained by this hit.
    /// </summary>
    public int PerfPoints;

    /// <summary>
    /// Gets or sets the amount of Score Points gained by this hit.
    /// </summary>
    public int ScorePoints;

    /// <summary>
    /// Gets or sets the amount of multiplier gained by this hit.
    /// </summary>
    public float MxPoints;

    /// <summary>
    /// In online games, this gets or sets the Net ID of the player who hit (or missed) this note.
    /// </summary>
    public int NetId;

    /// <summary>
    /// Gets or sets which player hit (or missed) this note.
    /// </summary>
    public int PlayerSlot;

    /// <summary>
    /// Gets or sets which lane the note was hit on.
    /// </summary>
    public int Lane;

    /// <summary>
    /// Gets or sets how far the hit deviated from perfect accuracy, in decimal seconds. Negative numbers indicate an early hit, positive numbers indicate a late hit.
    /// </summary>
    public float Deviation;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref JudgeResult);
        serializer.SerializeValue(ref DeviationResult);
        serializer.SerializeValue(ref PerfPoints); 
        serializer.SerializeValue(ref ScorePoints);
        serializer.SerializeValue(ref MxPoints);
        serializer.SerializeValue(ref NetId);
        serializer.SerializeValue(ref PlayerSlot);
        serializer.SerializeValue(ref Lane);
        serializer.SerializeValue(ref Deviation);
    }
}
