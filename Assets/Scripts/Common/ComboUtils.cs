using System;
using UnityEngine;

public static class ComboUtils
{

    public static string GetFcCode(FullComboType fullComboType)
    {
        switch (fullComboType)
        {
            case FullComboType.None:
                return "MAX";
            case FullComboType.FullCombo:
                return "FC";
            case FullComboType.PerfectFullCombo:
                return "PFC";
            case FullComboType.SemiFullCombo:
                return "SFC";
            default:
                throw new ArgumentOutOfRangeException("fullComboType", fullComboType, null);
        }
    }


    public static Color GetFcColor(FullComboType fullComboType)
    {
        switch (fullComboType)
        {
            case FullComboType.None:
                return Color.white;
            case FullComboType.FullCombo:
                return Color.green;
            case FullComboType.PerfectFullCombo:
                return Color.cyan;
            case FullComboType.SemiFullCombo:
                return new Color(1.0f, 0.6f, 0.3f);
            default:
                throw new ArgumentOutOfRangeException("fullComboType", fullComboType, null);
        }
    }
}