using System;
using System.Collections.Generic;
using Unity.Netcode;

public class SongLibraryEntry : INetworkSerializable, IEquatable<SongLibraryEntry>
{
    public string ID;
    public int Version;

    public bool Equals(SongLibraryEntry other)
    {
        if (other == null)
        {
            return false;
        }

        return ID == other.ID && Version == other.Version;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ID, Version);
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as SongLibraryEntry);
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ID);
        serializer.SerializeValue(ref Version);
    }
}
