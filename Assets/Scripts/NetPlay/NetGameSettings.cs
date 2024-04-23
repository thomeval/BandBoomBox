using Unity.Netcode;
using UnityEngine;

public class NetGameSettings : MonoBehaviour, INetworkSerializable
{
    public int MaxNetPlayers = 8;
    public NetSongSelectRules SongSelectRules = NetSongSelectRules.AnyonePicks;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref MaxNetPlayers);
        serializer.SerializeValue(ref SongSelectRules);
    }

    public void CopyFrom(NetGameSettings serverSettings)
    {
        MaxNetPlayers = serverSettings.MaxNetPlayers;
        SongSelectRules = serverSettings.SongSelectRules;
    }
}
