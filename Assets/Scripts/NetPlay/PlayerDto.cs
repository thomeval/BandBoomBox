using Unity.Netcode;

public class PlayerDto : INetworkSerializable
{
    public ulong NetId;
    public int Slot;
    public string NoteSkin;
    public string LabelSkin;
    public string Name;
    public string ProfileId;

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
    public FullComboType NetFullComboType;
    public AllyBoostMode AllyBoostMode;
    public int AllyBoosts;
    public int AllyBoostTicks;
    public int TicksForNextBoost;
    public int SectionHits;
    public int SectionPerfPoints;
    public int MaxSectionPerfPoints;
    public SectionJudgeMode SectionDifficulty;
    public LaneOrderType LaneOrderType;
    public int ChartDifficultyLevel;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        // Unity Network Serializer does not support null strings, so we need to ensure that they are not null before serialization.
        ProfileId ??= string.Empty;

        serializer.SerializeValue(ref NetId);
        serializer.SerializeValue(ref Slot);
        serializer.SerializeValue(ref Name);
        serializer.SerializeValue(ref NoteSkin);
        serializer.SerializeValue(ref LabelSkin);
        serializer.SerializeValue(ref ProfileId);
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
        serializer.SerializeValue(ref NetFullComboType);
        serializer.SerializeValue(ref AllyBoostMode);
        serializer.SerializeValue(ref AllyBoosts);
        serializer.SerializeValue(ref AllyBoostTicks);
        serializer.SerializeValue(ref TicksForNextBoost);
        serializer.SerializeValue(ref SectionHits);
        serializer.SerializeValue(ref SectionPerfPoints);
        serializer.SerializeValue(ref MaxSectionPerfPoints);
        serializer.SerializeValue(ref SectionDifficulty);
        serializer.SerializeValue(ref LaneOrderType);
        serializer.SerializeValue(ref ChartDifficultyLevel);

    }

    public static PlayerDto FromPlayer(Player player)
    {
        return new PlayerDto
        {
            NetId = player.NetId,
            Slot = player.Slot,
            Name = player.Name,
            ProfileId = player.ProfileId,
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
            IsParticipating = player.IsParticipating,
            NetFullComboType = player.GetFullComboType(),   
            AllyBoostMode = player.ProfileData.AllyBoostMode,
            AllyBoosts = player.AllyBoosts,
            AllyBoostTicks = player.AllyBoostTicks,
            TicksForNextBoost = player.TicksForNextBoost,
            SectionHits = player.SectionHits,
            SectionPerfPoints = player.SectionPerfPoints,
            MaxSectionPerfPoints = player.MaxSectionPerfPoints,
            SectionDifficulty = player.ProfileData.SectionDifficulty,
            LaneOrderType = player.LaneOrderType,
            ChartDifficultyLevel = player.ChartDifficultyLevel
        };
    }
}
