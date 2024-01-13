using Unity.Netcode;

public class GameplayStateValuesDto : INetworkSerializable
{
    public long Score;
    public int TeamCombo;
    public int MaxTeamCombo;

    public double Multiplier;
    public double MaxMultiplier;

    public float MxGainRate;

    public double Energy;
    public double MaxEnergy;
    public TeamScoreCategory TeamScoreCategory;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref Score);
        serializer.SerializeValue(ref TeamCombo);
        serializer.SerializeValue(ref MaxTeamCombo);
        serializer.SerializeValue(ref Multiplier);
        serializer.SerializeValue(ref MaxMultiplier);
        serializer.SerializeValue(ref MxGainRate);
        serializer.SerializeValue(ref Energy);
        serializer.SerializeValue(ref MaxEnergy);
        serializer.SerializeValue(ref TeamScoreCategory);
    }
}