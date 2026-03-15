using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaceDisplay : MonoBehaviour
{
    public Text TxtPb;
    public Text TxtPbPerfPoints;
    public Text TxtRb;
    public Text TxtRbPerfPoints;
    public Text TxtGoal;
    public Text TxtGoalPerfPoints;

    public Color PositiveColor = Color.green;
    public Color NegativeColor = Color.red;
    public Color NeutralColor = Color.black;

    public void Display(int currentPerfPoints, int? pbPerfPoints, int? rivalPerfPoints, int? goalPerfPoints, Grade? goalGrade)
    {
        SetValue(TxtPbPerfPoints, currentPerfPoints, pbPerfPoints);
        SetValue(TxtRbPerfPoints, currentPerfPoints, rivalPerfPoints);
        SetValue(TxtGoalPerfPoints, currentPerfPoints, goalPerfPoints);

        var goalText = goalGrade?.ToString().Replace("Plus", "+") ?? "--";
        TxtGoal.text = goalText;
    }

    private void SetValue(Text target, int currentPoints, int? targetPoints)
    {
        if (!targetPoints.HasValue)
        {
            target.text = "--";
            target.color = NeutralColor;
            return;
        }

        target.text = GetDelta(currentPoints, targetPoints.Value);
        target.color = GetColor(currentPoints, targetPoints.Value);
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
