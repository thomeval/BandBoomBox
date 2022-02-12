
using System;
using System.Linq;
using UnityEngine.UI;

public static class UIExtensions
{
    public static string GetSelectedText(this Dropdown dropdown)
    {
        return dropdown.options[dropdown.value].text;
    }

    public static void SetSelectedText(this Dropdown dropdown, string value)
    {
        var target = dropdown.options.SingleOrDefault(e => e.text == value);

        if (target == null)
        {
            throw new ArgumentException($"Dropdown does not have an option with {value} as its text.");
        }
        
        dropdown.value = dropdown.options.IndexOf(target);
        dropdown.RefreshShownValue();
    }
}