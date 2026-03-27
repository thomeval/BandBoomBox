using Unity.Netcode;

/// <summary>
/// Contains information about an ally boost event that occurred during a network game. Used to synchronize
/// ally boost state across all clients when the host applies a boost.
/// </summary>
public class AllyBoostDto : INetworkSerializable
{
    /// <summary>
    /// The Net ID of the player providing the ally boost.
    /// </summary>
    public ulong ProviderNetId;

    /// <summary>
    /// The slot of the player providing the ally boost.
    /// </summary>
    public int ProviderSlot;

    /// <summary>
    /// The Net ID of the player receiving the ally boost.
    /// </summary>
    public ulong ReceiverNetId;

    /// <summary>
    /// The slot of the player receiving the ally boost.
    /// </summary>
    public int ReceiverSlot;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ProviderNetId);
        serializer.SerializeValue(ref ProviderSlot);
        serializer.SerializeValue(ref ReceiverNetId);
        serializer.SerializeValue(ref ReceiverSlot);
    }
}
