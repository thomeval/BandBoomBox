using Unity.Netcode;

public class AllyBoostPlayerStateDto : INetworkSerializable
{
    public int PlayerSlot;
    public ulong NetId;
    public AllyBoostMode AllyBoostMode;
    public int AllyBoostTokens;
    public int TicksIncreasePerBoost;
    public int AllyBoostTicks;
    public int TicksForNextBoost;
    public int AllyBoostsReceived;
    public int AllyBoostsProvided;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref PlayerSlot);
        serializer.SerializeValue(ref NetId);
        serializer.SerializeValue(ref AllyBoostMode);
        serializer.SerializeValue(ref AllyBoostTokens);
        serializer.SerializeValue(ref TicksIncreasePerBoost);
        serializer.SerializeValue(ref AllyBoostTicks);
        serializer.SerializeValue(ref TicksForNextBoost);
        serializer.SerializeValue(ref AllyBoostsReceived);
        serializer.SerializeValue(ref AllyBoostsProvided);
    }
}