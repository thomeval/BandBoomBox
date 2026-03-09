using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaceDisplay : MonoBehaviour
{
    public Text TxtPb;
    public Text TxtPbPerfPoints;
    public Text TxtGoal;
    public Text TxtGoalPerfPoints;

    public Color PositiveColor = Color.green;
    public Color NegativeColor = Color.red;
    public Color NeutralColor = Color.black;

    public void Display(int currentPerfPoints, int? pbPerfPoints, int? goalPerfPoints, Grade? goalGrade)
    {
        if (pbPerfPoints.HasValue)
        {
            TxtPbPerfPoints.text = GetDelta(currentPerfPoints, pbPerfPoints.Value);
            TxtPbPerfPoints.color = GetColor(currentPerfPoints, pbPerfPoints.Value);
        }
        else
        {
            TxtPbPerfPoints.text = "--";
            TxtPbPerfPoints.color = NeutralColor;
        }

        if (goalPerfPoints.HasValue && goalGrade.HasValue)
        {
            var goalText = goalGrade?.ToString().Replace("Plus", "+") ?? "--";
            TxtGoal.text = goalText;
            TxtGoalPerfPoints.text = GetDelta(currentPerfPoints, goalPerfPoints.Value);
            TxtGoalPerfPoints.color = GetColor(currentPerfPoints, goalPerfPoints.Value);
        }
        else
        {
            TxtGoal.text = "";
            TxtGoalPerfPoints.text = "--";
            TxtGoalPerfPoints.color = NeutralColor;
        }
    }

    private Color GetColor(int currentPerfPoints, int targetPerfPoints)
    {
        if (currentPerfPoints > targetPerfPoints)
        {
            return PositiveColor;
        }

        return currentPerfPoints < targetPerfPoints ? NegativeColor : NeutralColor;
    }

    private string GetDelta(int currentPerfPoints, int pbPerfPoints)
    {
        var delta = currentPerfPoints - pbPerfPoints;
        return delta > 0 ? $"+{delta}" : delta.ToString();
    }
}
