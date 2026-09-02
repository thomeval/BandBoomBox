using Unity.Netcode;
using UnityEngine;

public class NetGameSettings : MonoBehaviour, INetworkSerializable
{
    public int MaxNetPlayers = 8;
    public NetSongSelectRules SongSelectRules = NetSongSelectRules.AnyonePicks;
    public bool AllowAutoPlay = false;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref MaxNetPlayers);
        serializer.SerializeValue(ref SongSelectRules);
        serializer.SerializeValue(ref AllowAutoPlay);
    }

    public void CopyFrom(NetGameSettings serverSettings)
    {
        MaxNetPlayers = serverSettings.MaxNetPlayers;
        SongSelectRules = serverSettings.SongSelectRules;
        AllowAutoPlay = serverSettings.AllowAutoPlay;
    }
}
