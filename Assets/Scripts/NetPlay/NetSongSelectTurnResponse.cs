using Unity.Netcode;

public class NetSongSelectTurnResponse : INetworkSerializable
{
    public NetSongSelectRules SongSelectRules;
    public ulong NetId;
    public bool Valid;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref SongSelectRules);
        serializer.SerializeValue(ref NetId);
        serializer.SerializeValue(ref Valid);
    }
}