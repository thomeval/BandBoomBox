using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameplayStateHelper : MonoBehaviour
{
    public GameplayManager ParentManager;
    public GameplayStateValues StateValues;

    private PlayerManager _playerManager;
    private HudManager _hudManager;

    private readonly float[] _turboMxGainRates = { 0.0f, 1.0f, 2.5f, 4.25f, 6.0f, 8.0f, 10.0f, 12.0f, 14.0f, 16.0f };

    /// <summary>
    /// Controls the amount of combo required to gain a bonus to the momentum gain rate.
    /// </summary>
    public readonly float GR_COMBO_FOR_BONUS = 50;

    /// <summary>
    /// Controls the amount of bonus awarded to momentum gain rate if the current team combo is at least GR_COMBO_FOR_BONUS (applied multiple times if appropriate).
    /// </summary>
    public readonly float GR_COMBO_BONUS_AMOUNT = 0.05f;

    public readonly float ENERGY_DRAIN_RATE = 1.0f / 12;

    /// <summary>
    /// Controls the minimum amount of energy required to activate turbo.
    /// </summary>
    public readonly float MIN_ENERGY_FOR_TURBO = 0.15f;

    /// <summary>
    /// Controls the amount of energy gained per note hit perfectly.
    /// </summary>
    public readonly float ENERGY_GAIN_RATE = 0.01f;

    public GameplayScreenState GameplayState
    {
        get { return ParentManager.GameplayState; }
    }

    private void Awake()
    {
        Helpers.AutoAssign(ref _playerManager);
        Helpers.AutoAssign(ref _hudManager);
    }

    public void UpdateGameplayState(double timeDiff)
    {
        UpdateMultiplier(timeDiff);
        UpdatePlayerEnergy(timeDiff);
        UpdateMxGainRate();
    }

    public void UpdateTeamCombo(JudgeResult result)
    {
        var comboBreak = HitJudge.IsComboBreak(result);
        if (!comboBreak.HasValue)
        {
            return;
        }

        if (comboBreak.Value)
        {
            StateValues.TeamCombo = 0;
        }
        else
        {
            StateValues.TeamCombo++;
            StateValues.MaxTeamCombo = Math.Max(StateValues.MaxTeamCombo, StateValues.TeamCombo);
        }

        UpdateMxGainRate();
    }

    public void UpdateMxGainRate()
    {
        var newGainRate = 1.0f;

        var comboGainBonus = ((int)(StateValues.TeamCombo / GR_COMBO_FOR_BONUS)) * GR_COMBO_BONUS_AMOUNT;
        comboGainBonus = Math.Min(comboGainBonus, 1.0f);
        newGainRate += comboGainBonus;

        var playersInTurbo = _playerManager.Players.Count(e => e.TurboActive);
        var turboBonus = _turboMxGainRates[playersInTurbo];
        newGainRate += turboBonus;
        StateValues.MxGainRate = newGainRate;
    }

    public void RecoverMultiplier(double timeDiff)
    {
        if (this.GameplayState == GameplayScreenState.Paused)
        {
            return;
        }

        StateValues.Multiplier = GameplayMultiplierUtils.RecoverMultiplier(StateValues.Multiplier, timeDiff);
    }

    public void DecayMultiplier(double timeDiff)
    {
        if (this.GameplayState == GameplayScreenState.Paused)
        {
            return;
        }

        StateValues.Multiplier = GameplayMultiplierUtils.DecayMultiplier(StateValues.Multiplier, timeDiff);
    }

    public void UpdatePlayerEnergy(double timeDiff)
    {
        if (this.GameplayState == GameplayScreenState.Paused)
        {
            return;
        }

        if (!_playerManager.AnyTurboActive())
        {
            return;
        }
        var amount = (float)(timeDiff * ENERGY_DRAIN_RATE);
        var playersUsingTurbo = _playerManager.Players.Count(e => e.TurboActive);
        amount *= playersUsingTurbo;

        UpdateEnergyAmount(StateValues.Energy - amount);

    }

    public void UpdateEnergyAmount(double amount)
    {
        amount = Math.Clamp(amount, 0.0f, StateValues.MaxEnergy);
        StateValues.Energy = amount;

        if (StateValues.Energy == 0.0f)
        {
            ParentManager.DisableAllTurbos();
        }

        _hudManager.UpdateEnergyMeter(_playerManager.AnyTurboActive());

    }

    public void UpdateMultiplier(double timeDiff)
    {
        if (StateValues.Multiplier > 1.0f)
        {
            DecayMultiplier(timeDiff);
        }
        else
        {
            RecoverMultiplier(timeDiff);
        }
    }

    public void ApplyHitResult(HitResult hitResult)
    {
        var appliedMxGainRate = hitResult.DeviationResult == DeviationResult.NotHit ? 1.0f : StateValues.MxGainRate;

        hitResult.ScorePoints = (int)(StateValues.Multiplier * hitResult.ScorePoints);
        StateValues.Score += hitResult.ScorePoints;
        StateValues.Multiplier += hitResult.MxPoints * appliedMxGainRate;
        StateValues.Multiplier = Math.Max(GameplayMultiplierUtils.MX_MINIMUM, StateValues.Multiplier);
        StateValues.MaxMultiplier = Math.Max(StateValues.Multiplier, StateValues.MaxMultiplier);
        StateValues.Stars = ParentManager.SongStarScoreValues.GetStarFraction(StateValues.Score);
        if (hitResult.JudgeResult <= JudgeResult.Perfect)
        {
            UpdateEnergyAmount(StateValues.Energy + ENERGY_GAIN_RATE);
        }
    }
}
