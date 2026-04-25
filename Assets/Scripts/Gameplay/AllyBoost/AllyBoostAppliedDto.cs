using Unity.Netcode;

public class AllyBoostAppliedDto : INetworkSerializable
{
    /// <summary>
    /// The Network ID of the player that provided the Ally Boost. Used to identify which machine the provider player is playing on.
    /// </summary>
    public ulong ProviderNetId;

    /// <summary>
    /// The player slot of the the player that provided the Ally Boost. Used to identify which player provided the boost in the case that multiple players are playing on the same machine.
    /// </summary>
    public int ProviderPlayerSlot;

    /// <summary>
    /// The Network ID of the player that received the Ally Boost. Used to identify which machine the receiver player is playing on.
    /// </summary>
    public ulong ReceiverNetId;

    /// <summary>
    /// The player slot of the the player that received the Ally Boost. Used to identify which player received the boost in the case that multiple players are playing on the same machine.
    /// </summary>
    public int ReceiverPlayerSlot;

    /// <summary>
    /// Indicates the lane on which the Ally Boost was applied. Used to display the visual effect of Ally Boosting on the correct lane for the receiver player.
    /// </summary>
    public int Lane;

    /// <summary>
    /// Indicates whether the Ally Boost was applied on an early hit or a late hit. Used to keep track of hit totals for the receiver player, which are tracked separately for early and late hits.
    /// </summary>
    public DeviationResult DeviationResult;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
            serializer.SerializeValue(ref ProviderNetId);
            serializer.SerializeValue(ref ProviderPlayerSlot);
            serializer.SerializeValue(ref ReceiverNetId);
            serializer.SerializeValue(ref ReceiverPlayerSlot);
            serializer.SerializeValue(ref Lane);
            serializer.SerializeValue(ref DeviationResult);
    }
}