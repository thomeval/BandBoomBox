using UnityEngine;

public static class ColorLookups
{
    public static readonly Color DefaultMenuColor = FromHex("#7B7EF5");
    public static readonly Color[] PlayerColors = new Color[]
    {
        FromHex("#FF9697"),
        FromHex("#96A5FF"),
        FromHex("#92FF79"),
        FromHex("#FFFC79"),
        FromHex("#AAFFFF"),
        FromHex("#FFAAFF"),
        FromHex("#FFCD96"),
        FromHex("#BFBFBF")
    };

    public static Color GetMenuColor(int playerSlot)
    {
        if (playerSlot == 0 || playerSlot > PlayerColors.Length)
        {
            return DefaultMenuColor;
        }

        return PlayerColors[playerSlot - 1];
    }

    public static Color FromHex(string hex)
    {
        if (ColorUtility.TryParseHtmlString(hex, out var color))
        {
            return color;
        }
        Debug.LogWarning($"Failed to parse color from hex string: {hex}");
        return Color.white;
    }
}