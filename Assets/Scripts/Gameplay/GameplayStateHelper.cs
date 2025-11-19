using System;
using System.Linq;
using UnityEngine;

public class GameplayStateHelper : MonoBehaviour
{
    public GameplayManager ParentManager;
    public GameplayStateValues StateValues;

    private PlayerManager _playerManager;
    private HudManager _hudManager;

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
        var playersInTurbo = _playerManager.Players.Count(e => e.TurboActive);
        var newGainRate = GameplayMultiplierUtils.GetMultiplierGainRate(StateValues.TeamCombo, playersInTurbo);
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
        StateValues.Multiplier = Math.Clamp(StateValues.Multiplier, GameplayMultiplierUtils.MX_MINIMUM, GameplayMultiplierUtils.MX_MAXIMUM);
        StateValues.MaxMultiplier = Math.Max(StateValues.Multiplier, StateValues.MaxMultiplier);
        StateValues.Stars = ParentManager.SongStarScoreValues.GetStarFraction(StateValues.Score);
        if (hitResult.JudgeResult <= JudgeResult.Perfect)
        {
            UpdateEnergyAmount(StateValues.Energy + ENERGY_GAIN_RATE);
        }
    }

    public void ApplySectionResult(SectionJudgeResult result)
    {
        var sectionBonus = HitJudge.SectionBonusMxValues[result];
        StateValues.Multiplier += sectionBonus;
        StateValues.Multiplier = Math.Clamp(StateValues.Multiplier, GameplayMultiplierUtils.MX_MINIMUM, GameplayMultiplierUtils.MX_MAXIMUM);
    }
}
