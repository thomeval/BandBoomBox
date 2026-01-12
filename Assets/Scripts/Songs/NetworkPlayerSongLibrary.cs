using System;
using System.Diagnostics;
using Unity.Netcode;

[DebuggerDisplay("NetId: {NetId}, Songs:{Songs.Length}")]
public class NetworkPlayerSongLibrary : INetworkSerializable
{
    public ulong NetId;
    public SongLibraryEntry[] Songs = new SongLibraryEntry[0];

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref NetId);
        serializer.SerializeValue(ref Songs);
    }

    public bool Any()
    {
        return Songs.Length > 0;
    }
}