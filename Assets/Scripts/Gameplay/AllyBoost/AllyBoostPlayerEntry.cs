using System;
using UnityEngine;

public class AllyBoostPlayerEntry : MonoBehaviour
{
    [field: SerializeField]
    public int PlayerSlot { get; set; }

    [field: SerializeField]
    public ulong NetId { get; set; }

    [field: SerializeField]
    public AllyBoostMode AllyBoostMode { get; set; }

    [field: SerializeField]
    public int AllyBoostTokens { get; set; }

    [field: SerializeField]
    public int TicksIncreasePerBoost { get; set; }

    [field: SerializeField]
    public int AllyBoostTicks { get; set; }

    [field: SerializeField]
    public int TicksForNextBoost { get; set; }

    [field: SerializeField]
    public int AllyBoostsReceived { get; set; }

    [field: SerializeField]
    public int AllyBoostsProvided { get; set; }

    public bool CanProvideAllyBoosts => AllyBoostMode == AllyBoostMode.ProvideOnly || AllyBoostMode == AllyBoostMode.On;
    public bool CanReceiveAllyBoosts => AllyBoostMode == AllyBoostMode.ReceiveOnly || AllyBoostMode == AllyBoostMode.On;

    public void AddTicks(int amountToAdd)
    {
        AllyBoostTicks += Math.Max(0, amountToAdd);

        while (AllyBoostTicks > TicksForNextBoost) 
        {
            AllyBoostTicks -= TicksForNextBoost;
            TicksForNextBoost += TicksIncreasePerBoost;
            AllyBoostTokens++;       
        }
    }

    public void AssignPlayer(Player player)
    {
        PlayerSlot = player.Slot;
        NetId = player.NetId;
        AllyBoostMode = player.ProfileData.AllyBoostMode;
    }

    public void CopyValues(AllyBoostPlayerStateDto dto)
    {
        PlayerSlot = dto.PlayerSlot;
        NetId = dto.NetId;
        AllyBoostMode = dto.AllyBoostMode;
        AllyBoostTokens = dto.AllyBoostTokens;
        TicksIncreasePerBoost = dto.TicksIncreasePerBoost;
        AllyBoostTicks = dto.AllyBoostTicks;
        TicksForNextBoost = dto.TicksForNextBoost;
        AllyBoostsReceived = dto.AllyBoostsReceived;
        AllyBoostsProvided = dto.AllyBoostsProvided;
    }

    public AllyBoostPlayerStateDto AsDto()
    {
        return new AllyBoostPlayerStateDto
        {
            PlayerSlot = PlayerSlot,     
            NetId = NetId,
            AllyBoostMode = AllyBoostMode,
            AllyBoostTokens = AllyBoostTokens,
            TicksIncreasePerBoost = TicksIncreasePerBoost,
            AllyBoostTicks = AllyBoostTicks,
            TicksForNextBoost = TicksForNextBoost,
            AllyBoostsReceived = AllyBoostsReceived,
            AllyBoostsProvided = AllyBoostsProvided
        };
    }
}