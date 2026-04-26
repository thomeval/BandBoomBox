using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AllyBoostManager : MonoBehaviour
{
    public List<AllyBoostPlayerEntry> AllyBoostPlayerEntries = new List<AllyBoostPlayerEntry>();
    public GameObject EntryContainer;

    public AllyBoostPlayerEntry AllyBoostPlayerEntryPrefab;

    public const int TICKS_FOR_FIRST_ALLY_BOOST = 200;
    public const int TICKS_INC_PER_BOOST = 100;

    private CoreManager _coreManager;
    private ClientNetApi _clientNetApi;

    void Awake()
    {
        Helpers.AutoAssign(ref _coreManager);
        Helpers.AutoAssign(ref _clientNetApi);
    }

    public void Init(PlayerManager playerManager)
    {
        foreach (var entry in AllyBoostPlayerEntries.ToArray())
        {
            Destroy(entry.gameObject);
            AllyBoostPlayerEntries.Remove(entry);
        }

        foreach (var player in playerManager.Players)
        {
            var entry = Instantiate(AllyBoostPlayerEntryPrefab);
            entry.name = $"Player {player.DisplayNetId}-{player.Slot}";
            entry.transform.SetParent(EntryContainer.transform);
            entry.AssignPlayer(player);
            entry.TicksForNextBoost = TICKS_FOR_FIRST_ALLY_BOOST;
            entry.TicksIncreasePerBoost = TICKS_INC_PER_BOOST;

#if ENABLE_CHEATS
            entry.AllyBoostTokens = 10;
#endif

            AllyBoostPlayerEntries.Add(entry);
        }
    }

    public AllyBoostPlayerEntry GetPlayer(ulong netId, int playerSlot)
    {
        var result = AllyBoostPlayerEntries.FirstOrDefault(e => e.NetId == netId && e.PlayerSlot == playerSlot);
        return result;
    }

    public AllyBoostPlayerEntry GetPlayer(int playerSlot)
    {
        var netId = _coreManager.NetId;
        return GetPlayer(netId, playerSlot);
    }

    public void AddTicksFromHitResult(HitResult hitResult)
    {
        var player = GetPlayer(hitResult.NetId, hitResult.PlayerSlot);

        if (player == null)
        {
            Debug.LogError($"Missing player in AllyBoostManager: {hitResult.NetId}-{hitResult.PlayerSlot}");
            return;
        }

        if (!player.CanProvideAllyBoosts)
        {
            return;
        }

        var amountToAdd = HitJudge.JudgeAllyBoostTickValues[hitResult.JudgeResult];

        if (amountToAdd != 0)
        {
            player.AddTicks(amountToAdd);
            SendStateChangedUpdate(player);
        }
    }

    private void SendStateChangedUpdate(AllyBoostPlayerEntry player)
    {
        if (!_coreManager.IsNetGame || !_coreManager.IsHost)
        {
            return;
        }

        var dto = player.AsDto();
        _clientNetApi.ReceiveAllyBoostPlayerStateChangedClientRpc(dto);
    }

    public AllyBoostPlayerEntry GetProviderForAllyBoost(int receivingPlayerSlot, ulong receivingPlayerNetId)
    {
        var provider = AllyBoostPlayerEntries.OrderByDescending(e => e.AllyBoostTokens)
            .FirstOrDefault(e => !(e.NetId == receivingPlayerNetId && e.PlayerSlot == receivingPlayerSlot)
                && e.CanProvideAllyBoosts
                && e.AllyBoostTokens > 0);
        return provider;
    }


    public AllyBoostAppliedDto TryBoostAlly(HitResult hitResult)
    {
        var provider = GetProviderForAllyBoost(hitResult.PlayerSlot, hitResult.NetId);
        var receiver = GetPlayer(hitResult.NetId, hitResult.PlayerSlot);

        if (provider == null || receiver == null || !receiver.CanReceiveAllyBoosts)
        {
            return null;
        }

        if (hitResult.JudgeResult != JudgeResult.Cool)
        {
            Debug.LogWarning("Attempted to boost ally on non-Cool hit result. Boost not applied.");
            return null;
        }

        hitResult.JudgeResult = JudgeResult.CoolWithBoost;
        hitResult.ScorePoints = HitJudge.JudgeScoreValues[JudgeResult.CoolWithBoost];
        hitResult.PerfPoints = HitJudge.JudgePerfPointValues[JudgeResult.CoolWithBoost];

        provider.AllyBoostTokens--;
        provider.AllyBoostsProvided++;
        receiver.AllyBoostsReceived++;

        SendStateChangedUpdate(receiver);
        SendStateChangedUpdate(provider);

        
        Debug.Log($"Ally Boost applied: Player {provider.NetId}-{provider.PlayerSlot} to {receiver.NetId}-{receiver.PlayerSlot}");

        var dto = new AllyBoostAppliedDto
        {
            ProviderNetId = provider.NetId,
            ProviderPlayerSlot = provider.PlayerSlot,
            ReceiverNetId = receiver.NetId,
            ReceiverPlayerSlot = receiver.PlayerSlot,
            Lane = hitResult.Lane,
            DeviationResult = hitResult.DeviationResult
        };

        if (_coreManager.IsNetGame && _coreManager.IsHost)
        {
            _coreManager.ClientNetApi.ReceiveAllyBoostAppliedClientRpc(dto);
        }

        return dto;
    }

    public void CopyValues(AllyBoostPlayerStateDto dto)
    {
        var entry = GetPlayer(dto.NetId, dto.PlayerSlot);

        if (entry == null)
        {
            Debug.LogError($"Missing player in AllyBoostManager: {dto.NetId}-{dto.PlayerSlot}");
            return;
        }

        entry.CopyValues(dto);
    }

}

