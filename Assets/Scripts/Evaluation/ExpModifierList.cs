using System.Collections.Generic;
using UnityEngine;

public class ExpModifierList : MonoBehaviour
{
    public List<ExpModifierEntry> Entries = new();

    public ExpModifierEntry EntryPrefab;

    public float TotalExpModifier
    {
        get
        {
            var result = 1.0f;

            foreach (var entry in Entries)
            {
                result *= entry.Value;
            }
            return result;
        }
    }

    public int GetTotalExpGain(Player player)
    {
        return (int) (player.GetBaseExpGain() * TotalExpModifier);
    }

    private readonly Dictionary<int, float> _starsExpModifiers = new()
    {
        { 0, 1.0f },
        { 1, 1.0f },
        { 2, 1.0f },
        { 3, 1.0f },
        { 4, 1.05f },
        { 5, 1.15f },
        { 6, 1.25f },
        { 7, 1.35f },
        { 8, 1.5f },
        { 9, 1.5f },
        { 10, 1.5f }
    };

    private readonly Dictionary<int, float> _numPlayersExpModifiers = new()
    {
        { 0, 1.0f },
        { 1, 1.0f },
        { 2, 1.1f },
        { 3, 1.2f },
        { 4, 1.3f },
    };

    public void DisplayExpModifier(Player player, double stars, int numPlayers)
    {
        this.Entries.Clear();
        this.gameObject.ClearChildren();

        AddDifficultyResult(player);
        AddGoalResult(player);
        AddFullComboResult(player);
        AddStarsResult(stars);
        AddNumPlayersResult(numPlayers);
    }

    private void AddDifficultyResult(Player player)
    {
        var label = $"{player.Difficulty} Diff";
        var value = HitJudge.DifficultyExpValues[player.Difficulty];
        Add(label, value);
    }

    public void AddGoalResult(Player player)
    {
        if (player.Goal == null)
        {
            return;
        }

        var goalMet = player.PerfPoints  >= player.GoalPerfPoints.GetValueOrDefault();
        var label = goalMet ? "Goal Passed!" : "Goal Failed!";
        var value = goalMet ? HitJudge.GoalExpValues[player.GetGoalGrade().GetValueOrDefault()] : 0.5f;

        Add(label,value);
    }

    private void AddStarsResult(double stars)
    {
        var wholeStars = (int)stars;
        var label = $"{wholeStars} Stars Bonus";
        var value = _starsExpModifiers[wholeStars];

        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (value == 1.0f)
        {
            return;
        }
        
        Add(label, value);
    }

    private void AddNumPlayersResult(int numPlayers)
    {
        var label = $"{numPlayers} Players Bonus";
        var value = _numPlayersExpModifiers[numPlayers];

        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (value == 1.0f)
        {
            return;
        }
        
        Add(label, value);
    }

    private void AddFullComboResult(Player player)
    {
        var fullComboType = player.GetFullComboType();
        if (fullComboType == FullComboType.None)
        {
            return;
        }

        var fcExpValue = HitJudge.FullComboExpValues[fullComboType];
        var fcLabel = GetFullComboLabel(fullComboType);
        Add(fcLabel, fcExpValue);
    }

    private string GetFullComboLabel(FullComboType fullComboType)
    {
        switch (fullComboType)
        {
            case FullComboType.SemiFullCombo:
                return "Semi Full Combo!";
            case FullComboType.FullCombo:
                return "Full Combo!";
            case FullComboType.PerfectFullCombo:
                return "Perfect FC!";
            default:
                return "";
        }
    }

    public void Add(string label, float value)
    {
        var newEntry = Instantiate(EntryPrefab);
        newEntry.Display(label, value);
        newEntry.transform.parent = this.transform;
        this.Entries.Add(newEntry);
    }
}
