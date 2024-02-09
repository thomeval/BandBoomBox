using Unity.Netcode;

public class PlayerDto : INetworkSerializable
{
    public ulong NetId;
    public int Slot;
    public string NoteSkin;
    public string LabelSkin;
    public string Name;

    public int PerfPoints;
    public int MaxPerfPoints;
    public PlayerState PlayerState;
    public int ScrollSpeed;
    public string ChartGroup;
    public Difficulty Difficulty;
    public long Exp;
    public float Goal;
    public int Combo;
    public int MaxCombo;
    public int Momentum;
    public bool TurboActive;
    public bool IsParticipating;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref NetId);
        serializer.SerializeValue(ref Slot);
        serializer.SerializeValue(ref Name);
        serializer.SerializeValue(ref NoteSkin);
        serializer.SerializeValue(ref LabelSkin);
        serializer.SerializeValue(ref PerfPoints);
        serializer.SerializeValue(ref MaxPerfPoints);
        serializer.SerializeValue(ref PlayerState);
        serializer.SerializeValue(ref ScrollSpeed);
        serializer.SerializeValue(ref ChartGroup);
        serializer.SerializeValue(ref Difficulty);
        serializer.SerializeValue(ref Exp);
        serializer.SerializeValue(ref Goal);
        serializer.SerializeValue(ref Combo);
        serializer.SerializeValue(ref MaxCombo);
        serializer.SerializeValue(ref Momentum);
        serializer.SerializeValue(ref TurboActive);
        serializer.SerializeValue(ref IsParticipating);
    }

    public static PlayerDto FromPlayer(Player player)
    {
        return new PlayerDto
        {
            NetId = player.NetId,
            Slot = player.Slot,
            Name = player.Name,
            NoteSkin = player.NoteSkin,
            LabelSkin = player.LabelSkin,
            PlayerState = player.PlayerState,
            PerfPoints = player.PerfPoints,
            MaxPerfPoints = player.MaxPerfPoints,
            ScrollSpeed = player.ScrollSpeed,
            ChartGroup = player.ChartGroup,
            Difficulty = player.Difficulty,
            Goal = player.Goal.GetValueOrDefault(),
            Exp = player.Exp,
            Combo = player.Combo,
            MaxCombo = player.MaxCombo,
            Momentum = player.Momentum,
            TurboActive = player.TurboActive,
            IsParticipating = player.IsParticipating
        };

    }
}
