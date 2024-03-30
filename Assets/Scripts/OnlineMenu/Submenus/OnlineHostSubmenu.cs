using System;
using UnityEngine.UI;

public class OnlineHostSubmenu : OnlineSubmenuBase
{
    public override OnlineMenuState OnlineMenuState => OnlineMenuState.HostMenu;

    public InputField TxtHostPort;
    public InputField TxtHostPassword;
    public Text LblHostMessage;
    public Text LblMaxPlayers;
    public Text LblSongSelectRules;


    public string Message
    {
        get => LblHostMessage.text;
        set => LblHostMessage.text = value;
    }

    public int MaxPlayers { get; set; }

    public ushort HostPort
    {
        get => GetValueOrDefault(TxtHostPort.text, OnlineMenuManager.DEFAULT_PORT);
        set => TxtHostPort.text = value.ToString();
    }

    public string HostPassword
    {
        get => TxtHostPassword.text;
        set => TxtHostPassword.text = value;
    }

    public NetSongSelectRules SongSelectRules { get; set; }

    public override void MenuItemSelected(MenuEventArgs args)
    {
        switch (args.SelectedItem)
        {
            case "Start Hosting":
                var passwordHash = ComputeHash(HostPassword);
                ParentManager.StartHosting(passwordHash);
                break;
            case "Port":
                TxtHostPort.Select();
                break;
            case "Password":
                TxtHostPassword.Select();
                break;
            case "Back":
                ParentManager.OnlineMenuState = OnlineMenuState.MainMenu;
                break;
        }
    }

    protected override bool MenuInputActive()
    {
        return !(TxtHostPort.isFocused || TxtHostPassword.isFocused);
    }

    public override void MenuItemShifted(MenuEventArgs args)
    {
        var delta = args.ShiftAmount;
        switch (args.SelectedItem)
        {

            case "Max Players":
                MaxPlayers = Math.Clamp(MaxPlayers + delta, 2, 32);
                break;
            case "Song Selection":
                SongSelectRules = Helpers.EnumAdd(SongSelectRules, delta, true);
                break;

        }

        UpdateDisplayedValues();
        base.MenuItemShifted(args);
    }

    public override void UpdateDisplayedValues()
    {
        LblMaxPlayers.text = MaxPlayers.ToString();
        LblSongSelectRules.text = SongSelectRules.ToString();
    }

    public override void LoadFromSettings(SettingsManager settingsManager)
    {
        MaxPlayers = settingsManager.NetGameHostMaxPlayers;
        HostPort = settingsManager.NetGameHostPort;
        SongSelectRules = settingsManager.NetGameHostSongSelectRules;
    }

    public override void SaveToSettings(SettingsManager settingsManager)
    {
        settingsManager.NetGameHostMaxPlayers = MaxPlayers;
        settingsManager.NetGameHostPort = HostPort;
        settingsManager.NetGameHostSongSelectRules = SongSelectRules;
    }
}
