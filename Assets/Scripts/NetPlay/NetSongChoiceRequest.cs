using Unity.Netcode;

public class NetSongChoiceRequest : INetworkSerializable
{
    public string SongId;
    public int SongVersion;
    public string Title;
    public string Artist;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref SongId);
        serializer.SerializeValue(ref SongVersion);
        serializer.SerializeValue(ref Title);
        serializer.SerializeValue(ref Artist);
    }
}