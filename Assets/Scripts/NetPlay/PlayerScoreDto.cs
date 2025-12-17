using Unity.Netcode;

public class PlayerScoreDto : INetworkSerializable
{
    public ulong NetId;
    public int Slot;

    public int PerfPoints;
    public int MaxPerfPoints;
    public PlayerState PlayerState;
    public int Combo;
    public int MaxCombo;
    public bool TurboActive;
    public FullComboType FullComboType;
    public int SectionHits;
    public int SectionPerfPoints;
    public int MaxSectionPerfPoints;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref NetId);
        serializer.SerializeValue(ref Slot);
        serializer.SerializeValue(ref PerfPoints);
        serializer.SerializeValue(ref MaxPerfPoints);
        serializer.SerializeValue(ref PlayerState);
        serializer.SerializeValue(ref Combo);
        serializer.SerializeValue(ref MaxCombo);
        serializer.SerializeValue(ref TurboActive);
        serializer.SerializeValue(ref FullComboType);
        serializer.SerializeValue(ref SectionHits);
        serializer.SerializeValue(ref SectionPerfPoints);
        serializer.SerializeValue(ref MaxSectionPerfPoints);
    }

    public static PlayerScoreDto FromPlayer(Player player)
    {
        return new PlayerScoreDto
        {
            NetId = player.NetId,
            Slot = player.Slot,
            PlayerState = player.PlayerState,
            PerfPoints = player.PerfPoints,
            MaxPerfPoints = player.MaxPerfPoints,
            Combo = player.Combo,
            MaxCombo = player.MaxCombo,
            TurboActive = player.TurboActive,
            FullComboType = player.GetFullComboType(),
            SectionHits = player.SectionHits,
            SectionPerfPoints = player.SectionPerfPoints,
            MaxSectionPerfPoints = player.MaxSectionPerfPoints
        };

    }
}