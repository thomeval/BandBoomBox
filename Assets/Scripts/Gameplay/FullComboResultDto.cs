using Unity.Netcode;

public class FullComboResultDto : INetworkSerializable
{
    public ulong NetId;
    public int PlayerSlot;
    public FullComboType FullComboType;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref NetId);
        serializer.SerializeValue(ref PlayerSlot);
        serializer.SerializeValue(ref FullComboType);
    }
}

public class FullComboResultSetDto : INetworkSerializable
{
    public FullComboResultDto[] FullComboResults;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref FullComboResults);
    }
}
