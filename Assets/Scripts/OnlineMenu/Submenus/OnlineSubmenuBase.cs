using System;
using UnityEngine;

public class OnlineSubmenuBase : MonoBehaviour
{
    public Menu Menu;

    public OnlineMenuManager ParentManager;

    public virtual OnlineMenuState OnlineMenuState { get => throw new NotImplementedException(); }

    public virtual void HandleInput(InputEvent inputEvent)
    {
        if (!MenuInputActive())
        {
            return;
        }
        Menu.HandleInput(inputEvent);
    }

    public virtual void LoadFromSettings(SettingsManager settingsManager)
    {
    }

    public virtual void SaveToSettings(SettingsManager settingsManager)
    {
    }

    public virtual void MenuItemSelected(MenuEventArgs args)
    {
    }

    public virtual void MenuItemShifted(MenuEventArgs args)
    {
    }

    public virtual void UpdateDisplayedValues()
    {
    }

    protected virtual bool MenuInputActive()
    {
        return true;
    }

    protected ushort GetValueOrDefault(string text, ushort defaultValue)
    {
        var result = defaultValue;
        ushort.TryParse(text, out result);
        return result;
    }

    protected int GetValueOrDefault(string text, int defaultValue)
    {
        var result = defaultValue;
        int.TryParse(text, out result);
        return result;
    }

    public string ComputeHash(string password)
    {
        if (string.IsNullOrEmpty(password))
        {
            return "";
        }

        var crypt = new System.Security.Cryptography.SHA256Managed();
        var result = new System.Text.StringBuilder();
        var hashBytes = crypt.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password), 0, System.Text.Encoding.UTF8.GetByteCount(password));
        foreach (byte theByte in hashBytes)
        {
            result.Append(theByte.ToString("x2"));
        }

        return result.ToString();
    }

}