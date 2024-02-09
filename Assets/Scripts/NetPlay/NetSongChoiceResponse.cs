using Unity.Netcode;

public class NetSongChoiceResponse : INetworkSerializable
{
    public NetSongChoiceResponseType ResponseType;
    public string ResponseMessage;
    public string SongId;
    public int SongVersion;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ResponseType);
        serializer.SerializeValue(ref ResponseMessage);
        serializer.SerializeValue(ref SongId);
        serializer.SerializeValue(ref SongVersion);
    }
}
